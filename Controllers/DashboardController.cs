using System.Security.Claims;
using System.Text.Json;
using HerbalMedicalCare.Data;
using HerbalMedicalCare.DTOs;
using HerbalMedicalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbalMedicalCare.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly HerbalCareDbContext _context;

        public DashboardController(HerbalCareDbContext context)
        {
            _context = context;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("nameid")?.Value
                              ?? User.FindFirst("sub")?.Value;

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private static List<string> ParseJsonArray(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string GetFirstImage(string? imagesJson)
        {
            var images = ParseJsonArray(imagesJson);
            return images.FirstOrDefault() ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var monthAgo = now.AddDays(-30);

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId.Value)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var notes = await _context.Notes
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var plants = await _context.Plants.ToListAsync();

            // ---------- TOP STATS ----------
            var favoritesThisWeek = favorites.Count(f => f.CreatedAt >= weekAgo);
            var notesThisWeek = notes.Count(n => n.CreatedAt >= weekAgo);

            var topPlant = favorites
                .Where(f => f.ItemType == "plant")
                .GroupBy(f => f.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";

            // ---------- JOURNEY ----------
            var plantIdsFromFavorites = favorites
                .Where(f => f.ItemType == "plant")
                .Select(f => f.ItemId)
                .Distinct()
                .ToList();

            var plantNamesFromNotes = notes
                .Where(n => !string.IsNullOrWhiteSpace(n.Plant))
                .Select(n => n.Plant.Trim().ToLower())
                .Distinct()
                .ToList();

            var plantIdsFromNotes = plants
                .Where(p => plantNamesFromNotes.Contains(p.Name.ToLower()))
                .Select(p => p.Id)
                .Distinct()
                .ToList();

            var plantsExplored = plantIdsFromFavorites
                .Union(plantIdsFromNotes)
                .Distinct()
                .Count();

            var monthlyFavoritePlantIds = favorites
                .Where(f => f.ItemType == "plant" && f.CreatedAt >= monthAgo)
                .Select(f => f.ItemId)
                .Distinct()
                .ToList();

            var monthlyNotePlantNames = notes
                .Where(n => n.CreatedAt >= monthAgo && !string.IsNullOrWhiteSpace(n.Plant))
                .Select(n => n.Plant.Trim().ToLower())
                .Distinct()
                .ToList();

            var monthlyNotePlantIds = plants
                .Where(p => monthlyNotePlantNames.Contains(p.Name.ToLower()))
                .Select(p => p.Id)
                .Distinct()
                .ToList();

            var plantsExploredThisMonth = monthlyFavoritePlantIds
                .Union(monthlyNotePlantIds)
                .Distinct()
                .Count();

            var daysActive = favorites
                .Select(f => f.CreatedAt.Date)
                .Union(notes.Select(n => n.CreatedAt.Date))
                .Distinct()
                .Count();

            var daysActiveThisWeek = favorites
                .Where(f => f.CreatedAt >= weekAgo)
                .Select(f => f.CreatedAt.Date)
                .Union(
                    notes.Where(n => n.CreatedAt >= weekAgo)
                         .Select(n => n.CreatedAt.Date)
                )
                .Distinct()
                .Count();

            // ---------- FAVORITE PREVIEW ----------
            var favoritePreview = favorites
                .Take(3)
                .Select(f => new DashboardFavoritePreviewDTO
                {
                    Id = f.Id,
                    ItemType = f.ItemType,
                    ItemId = f.ItemId,
                    Name = f.Name,
                    ImageUrl = f.ImageUrl
                })
                .ToList();

            // ---------- NOTES PREVIEW ----------
            var notesPreview = notes
                .Take(3)
                .Select(n =>
                {
                    var matchingPlant = plants.FirstOrDefault(p =>
                        !string.IsNullOrWhiteSpace(n.Plant) &&
                        p.Name.ToLower() == n.Plant.Trim().ToLower());

                    return new DashboardNotePreviewDTO
                    {
                        Id = n.Id,
                        Title = string.IsNullOrWhiteSpace(n.Plant) ? "My Note" : n.Plant,
                        Description = n.Text.Length > 80 ? n.Text.Substring(0, 80) + "..." : n.Text,
                        ImageUrl = matchingPlant != null ? GetFirstImage(matchingPlant.Images) : string.Empty,
                        CreatedAt = n.CreatedAt
                    };
                })
                .ToList();

            // ---------- BAR CHART ----------
            var favoriteGrouped = favorites
                .Where(f => !string.IsNullOrWhiteSpace(f.Name))
                .GroupBy(f => f.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Favorites = g.Count()
                })
                .ToList();

            var noteGrouped = notes
                .Where(n => !string.IsNullOrWhiteSpace(n.Plant))
                .GroupBy(n => n.Plant.Trim())
                .Select(g => new
                {
                    Name = g.Key,
                    Notes = g.Count()
                })
                .ToList();

            var chartNames = favoriteGrouped.Select(x => x.Name)
                .Union(noteGrouped.Select(x => x.Name))
                .Take(6)
                .ToList();

            var barData = chartNames.Select(name => new DashboardBarChartDTO
            {
                Name = name,
                Favorites = favoriteGrouped.FirstOrDefault(x => x.Name == name)?.Favorites ?? 0,
                Notes = noteGrouped.FirstOrDefault(x => x.Name == name)?.Notes ?? 0
            }).ToList();

            // ---------- PIE CHART ----------
            var pieData = favorites
                .GroupBy(f => f.ItemType)
                .Select(g => new DashboardPieChartDTO
                {
                    Name = g.Key,
                    Value = g.Count()
                })
                .ToList();

            if (!pieData.Any())
            {
                pieData.Add(new DashboardPieChartDTO
                {
                    Name = "plant",
                    Value = 0
                });
            }

            // ---------- LINE CHART ----------
            var days = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            var activityMap = days.ToDictionary(d => d, d => 0);

            foreach (var date in favorites.Select(f => f.CreatedAt).Concat(notes.Select(n => n.CreatedAt)))
            {
                var day = days[(int)date.DayOfWeek];
                activityMap[day]++;
            }

            var lineData = days.Select(day => new DashboardLineChartDTO
            {
                Day = day,
                Value = activityMap[day]
            }).ToList();

            // ---------- SUGGESTED ----------
            string favoritePlantName = favorites
                .Where(f => f.ItemType == "plant")
                .GroupBy(f => f.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? string.Empty;

            string notePlantName = notes
                .Where(n => !string.IsNullOrWhiteSpace(n.Plant))
                .GroupBy(n => n.Plant.Trim())
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? string.Empty;

            var excludePlantNames = new HashSet<string>(
                favorites.Where(f => f.ItemType == "plant")
                         .Select(f => f.Name.Trim().ToLower())
            );

            Plant? suggestedPlant = null;

            if (!string.IsNullOrWhiteSpace(favoritePlantName))
            {
                var favPlant = plants.FirstOrDefault(p =>
                    p.Name.ToLower() == favoritePlantName.Trim().ToLower());

                if (favPlant != null)
                {
                    var favProperties = ParseJsonArray(favPlant.Properties);

                    suggestedPlant = plants.FirstOrDefault(p =>
                        p.Id != favPlant.Id &&
                        !excludePlantNames.Contains(p.Name.ToLower()) &&
                        ParseJsonArray(p.Properties).Any(prop => favProperties.Contains(prop)));
                }
            }

            if (suggestedPlant == null && !string.IsNullOrWhiteSpace(notePlantName))
            {
                suggestedPlant = plants.FirstOrDefault(p =>
                    p.Name.ToLower() != notePlantName.Trim().ToLower() &&
                    !excludePlantNames.Contains(p.Name.ToLower()));
            }

            if (suggestedPlant == null)
            {
                suggestedPlant = plants.FirstOrDefault(p =>
                    !excludePlantNames.Contains(p.Name.ToLower()));
            }

            var suggested = suggestedPlant == null
                ? new DashboardSuggestionDTO
                {
                    Id = 0,
                    ItemType = "plant",
                    Name = "No suggestion yet",
                    Description = "Start exploring herbal plants to get personalized suggestions.",
                    ImageUrl = string.Empty
                }
                : new DashboardSuggestionDTO
                {
                    Id = suggestedPlant.Id,
                    ItemType = "plant",
                    Name = suggestedPlant.Name,
                    Description = suggestedPlant.Description.Length > 100
                        ? suggestedPlant.Description.Substring(0, 100) + "..."
                        : suggestedPlant.Description,
                    ImageUrl = GetFirstImage(suggestedPlant.Images)
                };

            var response = new DashboardDTO
            {
                TopStats = new DashboardTopStatsDTO
                {
                    Favorites = favorites.Count,
                    FavoritesThisWeek = favoritesThisWeek,
                    Notes = notes.Count,
                    NotesThisWeek = notesThisWeek,
                    TopPlant = topPlant
                },
                Journey = new DashboardJourneyDTO
                {
                    PlantsExplored = plantsExplored,
                    PlantsExploredThisMonth = plantsExploredThisMonth,
                    DaysActive = daysActive,
                    DaysActiveThisWeek = daysActiveThisWeek,
                    NotesCreated = notes.Count,
                    NotesCreatedThisWeek = notesThisWeek,
                    Quote = "The nature has provided us with everything to remain happy and healthy.",
                    QuoteAuthor = "Unknown"
                },
                Suggested = suggested,
                FavoritePreview = favoritePreview,
                NotesPreview = notesPreview,
                Charts = new DashboardChartsDTO
                {
                    BarData = barData,
                    PieData = pieData,
                    LineData = lineData
                }
            };

            return Ok(response);
        }
    }
}
using System.Security.Claims;
using HerbalMedicalCare.Data;
using HerbalMedicalCare.DTOs;
using HerbalMedicalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbalMedicalCare.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class FavoritesController : ControllerBase
    {
        private readonly HerbalCareDbContext _context;

        public FavoritesController(HerbalCareDbContext context)
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

        [HttpGet]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId.Value)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FavoriteResponseDTO
                {
                    Id = f.Id,
                    ItemType = f.ItemType,
                    ItemId = f.ItemId,
                    Name = f.Name,
                    Description = f.Description,
                    ImageUrl = f.ImageUrl,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            dto.ItemType = dto.ItemType.Trim().ToLower();

            if (dto.ItemType != "plant" && dto.ItemType != "disease")
                return BadRequest("ItemType must be 'plant' or 'disease'.");

            var exists = await _context.Favorites.AnyAsync(f =>
                f.UserId == userId.Value &&
                f.ItemType == dto.ItemType &&
                f.ItemId == dto.ItemId);

            if (exists)
                return BadRequest("This item is already in favorites.");

            var favorite = new Favorite
            {
                UserId = userId.Value,
                ItemType = dto.ItemType,
                ItemId = dto.ItemId,
                Name = dto.Name?.Trim() ?? string.Empty,
                Description = dto.Description?.Trim() ?? string.Empty,
                ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            var response = new FavoriteResponseDTO
            {
                Id = favorite.Id,
                ItemType = favorite.ItemType,
                ItemId = favorite.ItemId,
                Name = favorite.Name,
                Description = favorite.Description,
                ImageUrl = favorite.ImageUrl,
                CreatedAt = favorite.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId.Value);

            if (favorite == null)
                return NotFound("Favorite not found.");

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorite removed successfully." });
        }

        [HttpDelete("item/{itemType}/{itemId}")]
        public async Task<IActionResult> DeleteFavoriteByItem(string itemType, int itemId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            itemType = itemType.Trim().ToLower();

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId.Value &&
                    f.ItemType == itemType &&
                    f.ItemId == itemId);

            if (favorite == null)
                return NotFound("Favorite not found.");

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorite removed successfully." });
        }
    }
}
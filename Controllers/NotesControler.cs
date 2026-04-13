using System.Security.Claims;
using HerbalMedicalCare.Data;
using HerbalMedicalCare.DTOs;
using HerbalMedicalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbalMedicalCare.Controllers
{
    [Route("api/notes")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class NotesController : ControllerBase
    {
        private readonly HerbalCareDbContext _context;

        public NotesController(HerbalCareDbContext context)
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
        public async Task<IActionResult> GetMyNotes()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var notes = await _context.Notes
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.Pinned)
                .ThenByDescending(n => n.CreatedAt)
                .Select(n => new NoteResponseDTO
                {
                    Id = n.Id,
                    Text = n.Text,
                    Category = n.Category,
                    Plant = n.Plant,
                    Pinned = n.Pinned,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var note = new Note
            {
                UserId = userId.Value,
                Text = dto.Text?.Trim() ?? string.Empty,
                Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim(),
                Plant = dto.Plant?.Trim() ?? string.Empty,
                Pinned = dto.Pinned,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return Ok(new NoteResponseDTO
            {
                Id = note.Id,
                Text = note.Text,
                Category = note.Category,
                Plant = note.Plant,
                Pinned = note.Pinned,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.Value);

            if (note == null)
                return NotFound("Note not found.");

            note.Text = dto.Text?.Trim() ?? string.Empty;
            note.Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim();
            note.Plant = dto.Plant?.Trim() ?? string.Empty;
            note.Pinned = dto.Pinned;
            note.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new NoteResponseDTO
            {
                Id = note.Id,
                Text = note.Text,
                Category = note.Category,
                Plant = note.Plant,
                Pinned = note.Pinned,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            });
        }

        [HttpPatch("{id}/pin")]
        public async Task<IActionResult> TogglePin(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.Value);

            if (note == null)
                return NotFound("Note not found.");

            note.Pinned = !note.Pinned;
            note.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Pin status updated successfully.",
                note.Id,
                note.Pinned
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Invalid user token.");

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.Value);

            if (note == null)
                return NotFound("Note not found.");

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Note deleted successfully." });
        }
    }
}
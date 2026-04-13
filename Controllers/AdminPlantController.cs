using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HerbalMedicalCare.Data;
using HerbalMedicalCare.Models;
using HerbalMedicalCare.DTOs;
using System.Text.Json;

namespace HerbalMedicalCare.Controllers
{
    [ApiController]
    [Route("api/admin/plants")]
    [Authorize(Roles = "Admin")]
    public class AdminPlantController : ControllerBase
    {
        private readonly HerbalCareDbContext _context;

        public AdminPlantController(HerbalCareDbContext context)
        {
            _context = context;
        }

        // CREATE SINGLE
        [HttpPost]
        public IActionResult CreatePlant([FromBody] PlantDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Plant data is required" });

            var plant = MapToPlant(dto);

            _context.Plants.Add(plant);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Plant created successfully",
                data = ToResponse(plant)
            });
        }

        // CREATE BULK
        [HttpPost("bulk")]
        public IActionResult CreatePlantsBulk([FromBody] List<PlantDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "Plant list is required" });

            var plants = dtos.Select(MapToPlant).ToList();

            _context.Plants.AddRange(plants);
            _context.SaveChanges();

            return Ok(new
            {
                message = $"{plants.Count} plants created successfully",
                count = plants.Count,
                data = plants.Select(ToResponse).ToList()
            });
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetPlants()
        {
            var plants = _context.Plants
                .ToList()
                .Select(ToResponse)
                .ToList();

            return Ok(plants);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetPlantById(int id)
        {
            var plant = _context.Plants.Find(id);

            if (plant == null)
                return NotFound(new { message = "Plant not found" });

            return Ok(ToResponse(plant));
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult UpdatePlant(int id, [FromBody] PlantDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Plant data is required" });

            var plant = _context.Plants.Find(id);

            if (plant == null)
                return NotFound(new { message = "Plant not found" });

            plant.Name = dto.Name;
            plant.Scientific = dto.Scientific;
            plant.Description = dto.Description;
            plant.Images = JsonSerializer.Serialize(dto.Images ?? new List<string>());
            plant.Properties = JsonSerializer.Serialize(dto.Properties ?? new List<string>());
            plant.Helps = JsonSerializer.Serialize(dto.Helps ?? new List<HelpDto>());

            _context.SaveChanges();

            return Ok(new
            {
                message = "Plant updated successfully",
                data = ToResponse(plant)
            });
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult DeletePlant(int id)
        {
            var plant = _context.Plants.Find(id);

            if (plant == null)
                return NotFound(new { message = "Plant not found" });

            _context.Plants.Remove(plant);
            _context.SaveChanges();

            return Ok(new { message = "Plant deleted successfully" });
        }

        // ---------- HELPERS ----------

        private static Plant MapToPlant(PlantDto dto)
        {
            return new Plant
            {
                Name = dto.Name,
                Scientific = dto.Scientific,
                Description = dto.Description,
                Images = JsonSerializer.Serialize(dto.Images ?? new List<string>()),
                Properties = JsonSerializer.Serialize(dto.Properties ?? new List<string>()),
                Helps = JsonSerializer.Serialize(dto.Helps ?? new List<HelpDto>())
            };
        }

        private static object ToResponse(Plant plant)
        {
            return new
            {
                plant.Id,
                plant.Name,
                plant.Scientific,
                plant.Description,
                Images = SafeDeserializeList<string>(plant.Images),
                Properties = SafeDeserializeList<string>(plant.Properties),
                Helps = SafeDeserializeList<HelpDto>(plant.Helps)
            };
        }

        private static List<T> SafeDeserializeList<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }
    }
}
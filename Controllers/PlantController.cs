using Microsoft.AspNetCore.Mvc;
using HerbalMedicalCare.Data;
using System.Text.Json;

namespace HerbalMedicalCare.Controllers
{
    [ApiController]
    [Route("api/plants")]
    public class PlantController : ControllerBase
    {
        private readonly HerbalCareDbContext _context;

        public PlantController(HerbalCareDbContext context)
        {
            _context = context;
        }

        // 🟢 PUBLIC GET ALL
        [HttpGet]
        public IActionResult GetPlants()
        {
            var plants = _context.Plants.ToList().Select(p => new
            {
                p.Id,
                p.Name,
                p.Scientific,
                p.Description,
                Images = JsonSerializer.Deserialize<List<string>>(p.Images),
                Properties = JsonSerializer.Deserialize<List<string>>(p.Properties),
                Helps = JsonSerializer.Deserialize<List<object>>(p.Helps)
            });

            return Ok(plants);
        }

        // 🟢 GET BY ID (optional but useful)
        [HttpGet("{id}")]
        public IActionResult GetPlant(int id)
        {
            var p = _context.Plants.Find(id);

            if (p == null) return NotFound();

            return Ok(new
            {
                p.Id,
                p.Name,
                p.Scientific,
                p.Description,
                Images = JsonSerializer.Deserialize<List<string>>(p.Images),
                Properties = JsonSerializer.Deserialize<List<string>>(p.Properties),
                Helps = JsonSerializer.Deserialize<List<object>>(p.Helps)
            });
        }
    }
}
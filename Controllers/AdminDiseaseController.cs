using HerbalMedicalCare.Data;
using HerbalMedicalCare.DTOs;
using HerbalMedicalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Authorize]
public class AdminDiseaseController : ControllerBase
{
    private readonly HerbalCareDbContext _context;

    public AdminDiseaseController(HerbalCareDbContext context)
    {
        _context = context;
    }

    // =========================
    // USER + ADMIN : GET ALL
    // =========================
    [HttpGet("api/user/diseases")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAllForUser()
    {
        var data = await _context.Diseases
            .Include(d => d.Remedies)
            .Include(d => d.Variants)
            .Include(d => d.Precautions)
            .Include(d => d.Whys)
            .Include(d => d.RelatedDiseases)
            .ToListAsync();

        var result = data.Select(d => new DiseaseDTO
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            CardImageUrl = d.CardImageUrl,
            BannerImageUrl = d.BannerImageUrl,

            Remedies = d.Remedies != null
                ? d.Remedies.Select(r => new RemedyDTO
                {
                    Title = r.Title,
                    Description = r.Description
                }).ToList()
                : new List<RemedyDTO>(),

            Variants = d.Variants != null
                ? d.Variants.Select(v => new VariantDTO
                {
                    Severity = v.Severity,
                    Recovery = v.Recovery,
                    BestRemedyTitle = v.BestRemedyTitle,
                    BestRemedyDesc = v.BestRemedyDesc
                }).ToList()
                : new List<VariantDTO>(),

            Precautions = d.Precautions != null
                ? d.Precautions.Select(p => p.Text).ToList()
                : new List<string>(),

            WhyItWorks = d.Whys != null
                ? d.Whys.Select(w => w.Text).ToList()
                : new List<string>(),

            Related = d.RelatedDiseases != null
                ? d.RelatedDiseases.Select(r => new RelatedDTO
                {
                    Name = r.Name,
                    RelatedDiseaseId = r.RelatedToId
                }).ToList()
                : new List<RelatedDTO>()
        }).ToList();

        return Ok(result);
    }

    // =========================
    // USER + ADMIN : GET BY ID
    // =========================
    [HttpGet("api/user/diseases/{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdForUser(int id)
    {
        var d = await _context.Diseases
            .Include(x => x.Remedies)
            .Include(x => x.Variants)
            .Include(x => x.Precautions)
            .Include(x => x.Whys)
            .Include(x => x.RelatedDiseases)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (d == null)
            return NotFound(new { message = "Disease not found" });

        var result = new DiseaseDTO
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            CardImageUrl = d.CardImageUrl,
            BannerImageUrl = d.BannerImageUrl,

            Remedies = d.Remedies != null
                ? d.Remedies.Select(r => new RemedyDTO
                {
                    Title = r.Title,
                    Description = r.Description
                }).ToList()
                : new List<RemedyDTO>(),

            Variants = d.Variants != null
                ? d.Variants.Select(v => new VariantDTO
                {
                    Severity = v.Severity,
                    Recovery = v.Recovery,
                    BestRemedyTitle = v.BestRemedyTitle,
                    BestRemedyDesc = v.BestRemedyDesc
                }).ToList()
                : new List<VariantDTO>(),

            Precautions = d.Precautions != null
                ? d.Precautions.Select(p => p.Text).ToList()
                : new List<string>(),

            WhyItWorks = d.Whys != null
                ? d.Whys.Select(w => w.Text).ToList()
                : new List<string>(),

            Related = d.RelatedDiseases != null
                ? d.RelatedDiseases.Select(r => new RelatedDTO
                {
                    Name = r.Name,
                    RelatedDiseaseId = r.RelatedToId
                }).ToList()
                : new List<RelatedDTO>()
        };

        return Ok(result);
    }

    // =========================
    // ADMIN : GET ALL
    // =========================
    [HttpGet("api/admin/diseases")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Diseases
            .Include(d => d.Remedies)
            .Include(d => d.Variants)
            .Include(d => d.Precautions)
            .Include(d => d.Whys)
            .Include(d => d.RelatedDiseases)
            .ToListAsync();

        var result = data.Select(d => new DiseaseDTO
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            CardImageUrl = d.CardImageUrl,
            BannerImageUrl = d.BannerImageUrl,

            Remedies = d.Remedies != null
                ? d.Remedies.Select(r => new RemedyDTO
                {
                    Title = r.Title,
                    Description = r.Description
                }).ToList()
                : new List<RemedyDTO>(),

            Variants = d.Variants != null
                ? d.Variants.Select(v => new VariantDTO
                {
                    Severity = v.Severity,
                    Recovery = v.Recovery,
                    BestRemedyTitle = v.BestRemedyTitle,
                    BestRemedyDesc = v.BestRemedyDesc
                }).ToList()
                : new List<VariantDTO>(),

            Precautions = d.Precautions != null
                ? d.Precautions.Select(p => p.Text).ToList()
                : new List<string>(),

            WhyItWorks = d.Whys != null
                ? d.Whys.Select(w => w.Text).ToList()
                : new List<string>(),

            Related = d.RelatedDiseases != null
                ? d.RelatedDiseases.Select(r => new RelatedDTO
                {
                    Name = r.Name,
                    RelatedDiseaseId = r.RelatedToId
                }).ToList()
                : new List<RelatedDTO>()
        }).ToList();

        return Ok(result);
    }

    // =========================
    // ADMIN : CREATE
    // =========================
    [HttpPost("api/admin/diseases")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] DiseaseDTO dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        var disease = new Disease
        {
            Name = dto.Name ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            CardImageUrl = dto.CardImageUrl ?? string.Empty,
            BannerImageUrl = dto.BannerImageUrl ?? string.Empty
        };

        _context.Diseases.Add(disease);
        await _context.SaveChangesAsync();

        if (dto.Remedies != null && dto.Remedies.Any())
        {
            var remedies = dto.Remedies.Select(r => new Remedy
            {
                Title = r.Title ?? string.Empty,
                Description = r.Description ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList();

            _context.Remedies.AddRange(remedies);
        }

        if (dto.Variants != null && dto.Variants.Any())
        {
            var variants = dto.Variants.Select(v => new Variant
            {
                Severity = v.Severity,
                Recovery = v.Recovery ?? string.Empty,
                BestRemedyTitle = v.BestRemedyTitle ?? string.Empty,
                BestRemedyDesc = v.BestRemedyDesc ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList();

            _context.Variants.AddRange(variants);
        }

        if (dto.Precautions != null && dto.Precautions.Any())
        {
            var precautions = dto.Precautions.Select(p => new Precaution
            {
                Text = p ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList();

            _context.Precautions.AddRange(precautions);
        }

        if (dto.WhyItWorks != null && dto.WhyItWorks.Any())
        {
            var whys = dto.WhyItWorks.Select(w => new Why
            {
                Text = w ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList();

            _context.Whys.AddRange(whys);
        }

        if (dto.Related != null && dto.Related.Any())
        {
            var related = dto.Related.Select(r => new RelatedDisease
            {
                Name = r.Name ?? string.Empty,
                DiseaseId = disease.Id,
                RelatedToId = r.RelatedDiseaseId
            }).ToList();

            _context.RelatedDiseases.AddRange(related);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Disease created successfully",
            disease.Id,
            disease.Name,
            disease.Description,
            disease.CardImageUrl,
            disease.BannerImageUrl
        });
    }

    // =========================
    // ADMIN : UPDATE
    // =========================
    [HttpPut("api/admin/diseases/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] DiseaseDTO dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        var disease = await _context.Diseases
            .Include(d => d.Remedies)
            .Include(d => d.Variants)
            .Include(d => d.Precautions)
            .Include(d => d.Whys)
            .Include(d => d.RelatedDiseases)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (disease == null)
            return NotFound("Disease not found");

        disease.Name = dto.Name ?? string.Empty;
        disease.Description = dto.Description ?? string.Empty;
        disease.CardImageUrl = dto.CardImageUrl ?? string.Empty;
        disease.BannerImageUrl = dto.BannerImageUrl ?? string.Empty;

        if (disease.Remedies != null && disease.Remedies.Any())
            _context.Remedies.RemoveRange(disease.Remedies);

        if (disease.Variants != null && disease.Variants.Any())
            _context.Variants.RemoveRange(disease.Variants);

        if (disease.Precautions != null && disease.Precautions.Any())
            _context.Precautions.RemoveRange(disease.Precautions);

        if (disease.Whys != null && disease.Whys.Any())
            _context.Whys.RemoveRange(disease.Whys);

        if (disease.RelatedDiseases != null && disease.RelatedDiseases.Any())
            _context.RelatedDiseases.RemoveRange(disease.RelatedDiseases);

        disease.Remedies = dto.Remedies != null
            ? dto.Remedies.Select(r => new Remedy
            {
                Title = r.Title ?? string.Empty,
                Description = r.Description ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList()
            : new List<Remedy>();

        disease.Variants = dto.Variants != null
            ? dto.Variants.Select(v => new Variant
            {
                Severity = v.Severity,
                Recovery = v.Recovery ?? string.Empty,
                BestRemedyTitle = v.BestRemedyTitle ?? string.Empty,
                BestRemedyDesc = v.BestRemedyDesc ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList()
            : new List<Variant>();

        disease.Precautions = dto.Precautions != null
            ? dto.Precautions.Select(p => new Precaution
            {
                Text = p ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList()
            : new List<Precaution>();

        disease.Whys = dto.WhyItWorks != null
            ? dto.WhyItWorks.Select(w => new Why
            {
                Text = w ?? string.Empty,
                DiseaseId = disease.Id
            }).ToList()
            : new List<Why>();

        disease.RelatedDiseases = dto.Related != null
            ? dto.Related.Select(r => new RelatedDisease
            {
                Name = r.Name ?? string.Empty,
                DiseaseId = disease.Id,
                RelatedToId = r.RelatedDiseaseId
            }).ToList()
            : new List<RelatedDisease>();

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Disease updated successfully",
            disease.Id,
            disease.Name,
            disease.Description,
            disease.CardImageUrl,
            disease.BannerImageUrl
        });
    }

    // =========================
    // ADMIN : DELETE
    // =========================
    [HttpDelete("api/admin/diseases/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var disease = await _context.Diseases
            .Include(d => d.Remedies)
            .Include(d => d.Variants)
            .Include(d => d.Precautions)
            .Include(d => d.Whys)
            .Include(d => d.RelatedDiseases)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (disease == null)
            return NotFound("Disease not found");

        if (disease.Remedies != null && disease.Remedies.Any())
            _context.Remedies.RemoveRange(disease.Remedies);

        if (disease.Variants != null && disease.Variants.Any())
            _context.Variants.RemoveRange(disease.Variants);

        if (disease.Precautions != null && disease.Precautions.Any())
            _context.Precautions.RemoveRange(disease.Precautions);

        if (disease.Whys != null && disease.Whys.Any())
            _context.Whys.RemoveRange(disease.Whys);

        if (disease.RelatedDiseases != null && disease.RelatedDiseases.Any())
            _context.RelatedDiseases.RemoveRange(disease.RelatedDiseases);

        _context.Diseases.Remove(disease);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Deleted successfully" });
    }
}
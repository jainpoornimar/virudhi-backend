using HerbalMedicalCare.Models;

namespace HerbalMedicalCare.DTOs
{
    public class DiseaseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string CardImageUrl { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;

        public List<RemedyDTO> Remedies { get; set; } = new();
        public List<VariantDTO> Variants { get; set; } = new();

        public List<string> Precautions { get; set; } = new();
        public List<string> WhyItWorks { get; set; } = new();

        public List<RelatedDTO> Related { get; set; } = new();
    }

    public class RemedyDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class VariantDTO
    {
        public SeverityType Severity { get; set; }
        public string Recovery { get; set; } = string.Empty;
        public string BestRemedyTitle { get; set; } = string.Empty;
        public string BestRemedyDesc { get; set; } = string.Empty;
    }

    public class RelatedDTO
    {
        public string Name { get; set; } = string.Empty;
        public int RelatedDiseaseId { get; set; }
    }
}
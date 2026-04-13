namespace HerbalMedicalCare.Models
{
    public class Disease
    {
        public int Id { get; set; }   // ✅ INT PRIMARY KEY
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string CardImageUrl { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;

        public ICollection<Remedy>? Remedies { get; set; }
        public ICollection<Variant>? Variants { get; set; }

        public ICollection<Precaution>? Precautions { get; set; }
        public ICollection<Why>? Whys { get; set; }
        public ICollection<RelatedDisease>? RelatedDiseases { get; set; }
    }
}
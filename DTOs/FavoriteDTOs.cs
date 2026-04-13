using System.ComponentModel.DataAnnotations;

namespace HerbalMedicalCare.DTOs
{
    public class FavoriteCreateDTO
    {
        [Required]
        public string ItemType { get; set; } = string.Empty;

        [Required]
        public int ItemId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class FavoriteResponseDTO
    {
        public int Id { get; set; }

        public string ItemType { get; set; } = string.Empty;

        public int ItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
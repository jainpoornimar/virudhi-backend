using System.ComponentModel.DataAnnotations;

namespace HerbalMedicalCare.Models
{
    public class Note
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = "General";

        [MaxLength(200)]
        public string Plant { get; set; } = string.Empty;

        public bool Pinned { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
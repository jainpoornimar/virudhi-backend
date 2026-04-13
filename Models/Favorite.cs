using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerbalMedicalCare.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ItemType { get; set; } = string.Empty;

        public int ItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
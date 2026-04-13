using System.ComponentModel.DataAnnotations;

namespace HerbalMedicalCare.DTOs
{
    public class NoteCreateDTO
    {
        [Required]
        public string Text { get; set; } = string.Empty;

        public string Category { get; set; } = "General";

        public string Plant { get; set; } = string.Empty;

        public bool Pinned { get; set; } = false;
    }

    public class NoteUpdateDTO
    {
        [Required]
        public string Text { get; set; } = string.Empty;

        public string Category { get; set; } = "General";

        public string Plant { get; set; } = string.Empty;

        public bool Pinned { get; set; } = false;
    }

    public class NoteResponseDTO
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string Category { get; set; } = "General";

        public string Plant { get; set; } = string.Empty;

        public bool Pinned { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
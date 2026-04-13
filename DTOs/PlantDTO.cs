namespace HerbalMedicalCare.DTOs
{
    public class PlantDto
    {
        public string Name { get; set; } = string.Empty;
        public string Scientific { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<string> Images { get; set; } = new();
        public List<string> Properties { get; set; } = new();

        public List<HelpDto> Helps { get; set; } = new();
    }
}
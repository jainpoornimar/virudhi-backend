namespace HerbalMedicalCare.DTOs
{
    public class HelpDto
    {
        public string Problem { get; set; } = string.Empty;
        public string Remedy { get; set; } = string.Empty;
        public List<string> Tips { get; set; } = new();
    }
}
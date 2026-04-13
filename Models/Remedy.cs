using HerbalMedicalCare.Models;

public class Remedy
{
    public int Id { get; set; }

    public int DiseaseId { get; set; }   // ✅ FIXED INT
    public Disease? Disease { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
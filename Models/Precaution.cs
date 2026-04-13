using HerbalMedicalCare.Models;

public class Precaution
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int DiseaseId { get; set; }   // ✅ FIXED
    public Disease? Disease { get; set; }
}
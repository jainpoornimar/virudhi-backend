using HerbalMedicalCare.Models;

public class Variant
{
    public int Id { get; set; }

    public int DiseaseId { get; set; }   // ✅ FIXED INT
    public Disease? Disease { get; set; }

    public SeverityType Severity { get; set; }
    public string Recovery { get; set; } = string.Empty;

    public string BestRemedyTitle { get; set; } = string.Empty;
    public string BestRemedyDesc { get; set; } = string.Empty;
}
using HerbalMedicalCare.Models;

public class RelatedDisease
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DiseaseId { get; set; }      // parent
    public Disease? Disease { get; set; }

    public int RelatedToId { get; set; }    // target disease
}
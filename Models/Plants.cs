public class Plant
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Scientific { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Images { get; set; } = "[]";
    public string Properties { get; set; } = "[]";
    public string Helps { get; set; } = "[]";
}
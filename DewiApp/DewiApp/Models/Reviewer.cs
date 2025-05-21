namespace DewiApp.Models;

public class Reviewer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Review { get; set; }
    public float Rating { get; set; }
    public string? ImagePath { get; set; }
    public int PositionId { get; set; }
    public Position Position { get; set; }
}

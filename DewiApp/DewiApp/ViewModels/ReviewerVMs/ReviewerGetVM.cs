using DewiApp.ViewModels.PositionVMs;

namespace DewiApp.ViewModels.ReviewerVMs;

public class ReviewerGetVM
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Review { get; set; }
    public float Rating { get; set; }
    public string? ImagePath { get; set; }
    public PositionGetVM Position { get; set; }
}

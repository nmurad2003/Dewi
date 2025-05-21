namespace DewiApp.ViewModels.ReviewerVMs;

public class ReviewerCreateVM
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Review { get; set; }
    public float Rating { get; set; }
    public IFormFile? Image { get; set; }
    public int PositionId { get; set; }
}

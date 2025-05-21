namespace DewiApp.Models;

public class Position : BaseEntity
{
    public string Name { get; set; }
    IEnumerable<Reviewer> Reviewers { get; set; }
}

using DewiApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DewiApp.Contexts;

public class DewiDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Position> Positions { get; set; }
    public DbSet<Reviewer> Reviewers { get; set; }

    public DewiDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

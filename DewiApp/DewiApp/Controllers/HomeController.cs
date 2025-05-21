using DewiApp.Contexts;
using DewiApp.Models;
using DewiApp.ViewModels.PositionVMs;
using DewiApp.ViewModels.ReviewerVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DewiApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DewiDbContext _context;

    public HomeController(ILogger<HomeController> logger, DewiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<ReviewerGetVM> vms = await _context.Reviewers.Select(r => new ReviewerGetVM()
        {
            Id = r.Id,
            ImagePath = r.ImagePath,
            FirstName = r.FirstName,
            LastName = r.LastName,
            Position = new PositionGetVM() { Id = r.PositionId, Name = r.Position.Name },
            Review = r.Review,
            Rating = r.Rating,
        }).ToListAsync();

        return View(vms);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

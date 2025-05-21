using DewiApp.Contexts;
using DewiApp.Models;
using DewiApp.ViewModels.PositionVMs;
using DewiApp.ViewModels.ReviewerVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DewiApp.Areas.Admin.Controllers;

public class ReviewerController(DewiDbContext _context, IWebHostEnvironment _env) : AdminBaseController
{
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

    public async Task<IActionResult> Create()
    {
        await FillPositionsToViewBagAsync();
        return View();
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(ReviewerCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            await FillPositionsToViewBagAsync();
            return View(model);
        }

        string? imagePath = null;

        if (model.Image != null)
        {
            if (model.Image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Image", "File size cannot exceed 2 MBs!");
                await FillPositionsToViewBagAsync();
                return View(model);
            }
            if (!model.Image.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("Image", "Only image files are accepted!");
                await FillPositionsToViewBagAsync();
                return View(model);
            }

            imagePath = await CopyToNewImagePathAsync(model.Image);
        }

        var entity = new Reviewer()
        {
            ImagePath = imagePath,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PositionId = model.PositionId,
            Review = model.Review,
            Rating = model.Rating,
        };

        await _context.Reviewers.AddAsync(entity);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        Reviewer? entity = await _context.Reviewers.FindAsync(id);
        if (entity == null)
            return NotFound();

        var model = new ReviewerUpdateVM()
        {
            Id = entity.Id,
            ImagePath = entity.ImagePath,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PositionId = entity.PositionId,
            Review = entity.Review,
            Rating = entity.Rating,
        };

        await FillPositionsToViewBagAsync();

        return View(model);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(ReviewerUpdateVM model)
    {
        if (!ModelState.IsValid)
        {
            await FillPositionsToViewBagAsync();
            return View(model);
        }

        if (await _context.Positions.FindAsync(model.PositionId) == null)
        {
            ModelState.AddModelError("PositionId", "Position was not found!");
            await FillPositionsToViewBagAsync();
            return View(model);
        }

        Reviewer? entity = await _context.Reviewers.FindAsync(model.Id);
        if (entity == null)
            return NotFound();

        string? imagePath = entity.ImagePath;

        if (model.Image != null)
        {
            if (model.Image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Image", "File size cannot exceed 2 MBs!");
                await FillPositionsToViewBagAsync();
                return View(model);
            }
            if (!model.Image.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("Image", "Only image files are accepted!");
                await FillPositionsToViewBagAsync();
                return View(model);
            }

            if (entity.ImagePath != null)
                await CopyToExistingImagePathAsync(model.Image, entity.ImagePath);
            else
                imagePath = await CopyToNewImagePathAsync(model.Image);
        }

        entity.ImagePath = imagePath;
        entity.FirstName = model.FirstName;
        entity.LastName = model.LastName;
        entity.PositionId = model.PositionId;
        entity.Review = model.Review;
        entity.Rating = model.Rating;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Reviewer? entity = await _context.Reviewers.FindAsync(id);
        if (entity == null)
            return NotFound();

        if (entity.ImagePath != null)
            System.IO.File.Delete(_env.WebRootPath + entity.ImagePath);

        _context.Reviewers.Remove(entity);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    #region Utility Methods
    public async Task FillPositionsToViewBagAsync()
    {
        ViewBag.Positions = await _context.Positions.Select(p => new PositionGetVM()
        {
            Id = p.Id,
            Name = p.Name,
        }).ToListAsync();
    }

    public async Task<string> CopyToNewImagePathAsync(IFormFile image)
    {
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        string fullPath = Path.Combine(_env.WebRootPath, "uploads", fileName);

        using var fs = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(fs);

        return "/uploads/" + fileName;
    }

    public async Task CopyToExistingImagePathAsync(IFormFile image, string imagePath)
    {
        string fullPath = _env.WebRootPath + imagePath;
        using var fs = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(fs);
    }
    #endregion
}

using DewiApp.Models;
using DewiApp.ViewModels.AccountVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DewiApp.Controllers;

public class AccountController(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager, SignInManager<AppUser> _signInManager) : Controller
{
    #region Initializing roles and admins
    //public async Task<IActionResult> CreateRoles()
    //{
    //    List<string> roleNames = ["admin", "member"];

    //    foreach (string roleName in roleNames)
    //        await _roleManager.CreateAsync(new IdentityRole() { Name = roleName });

    //    return Ok("Roles Created!");
    //}

    //public async Task<IActionResult> CreateAdmins()
    //{
    //    var admin = new AppUser()
    //    {
    //        FirstName = "admin",
    //        LastName = "admin",
    //        UserName = "admin@code.edu.az",
    //        Email = "admin@code.edu.az"
    //    };

    //    await _userManager.CreateAsync(admin, "admin1234");
    //    await _userManager.AddToRoleAsync(admin, "admin");

    //    return Ok("Admins Created!");
    //}
    #endregion

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new AppUser()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, "member");

        return RedirectToAction(nameof(Login));
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        AppUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password!");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid email or password!");
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

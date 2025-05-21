using Microsoft.AspNetCore.Mvc;

namespace DewiApp.Areas.Admin.Controllers;

public class DashboardController : AdminBaseController
{
    public IActionResult Index()
    {
        return View();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Triapka.Api.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult About()
    {
        return View();
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace CareerAdvisorApp.Controllers;
public class DashboardController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    public DashboardController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    [Authorize]
    public IActionResult Index()
    {
        var user = _userManager.GetUserAsync(User).Result;
        return View(user);
    }
}

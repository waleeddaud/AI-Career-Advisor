using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace CareerAdvisorApp.Controllers;
public class DashboardController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(UserManager<IdentityUser> userManager, ILogger<DashboardController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    
    [Authorize(Policy="UserOnly")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found in dashboard");
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard for user");
            TempData["ErrorMessage"] = "An error occurred while loading the dashboard. Please try again.";
            return RedirectToAction("Index", "Home");
        }
    }
}

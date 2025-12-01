using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Google.GenAI;
using Google.GenAI.Types;

namespace CareerAdvisorApp.Controllers;
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    public AdminController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    [Authorize(Policy="AdminOnly")]
    public IActionResult Index()
    {
        return View();
    }
}

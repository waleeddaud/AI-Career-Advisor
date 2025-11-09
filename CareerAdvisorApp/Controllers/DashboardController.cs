using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace CareerAdvisorApp.Controllers;
public class DashboardController : Controller
{   
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}

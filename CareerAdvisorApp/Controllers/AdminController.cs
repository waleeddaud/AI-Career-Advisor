using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using CareerAdvisorApp.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CareerAdvisorApp.Controllers;

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(UserManager<IdentityUser> userManager, IAdminService adminService, ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy="AdminOnly")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var users = await _adminService.GetAllUsersWithRolesAsync();
            Console.WriteLine("Fetched " + users.Count + " users for admin panel.");
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching users: {ex.Message}");
            TempData["ErrorMessage"] = "Failed to load users.";
            return View(new List<UserViewModel>());
        }
    }

    [Authorize(Policy="AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            var currentUserId = _userManager.GetUserId(User);
            
            // Prevent admin from deleting themselves
            if (userId == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("Index");
            }

            var result = await _adminService.DeleteUserAsync(userId);
            
            if (result)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting user: {ex.Message}");
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
        }

        return RedirectToAction("Index");
    }
}
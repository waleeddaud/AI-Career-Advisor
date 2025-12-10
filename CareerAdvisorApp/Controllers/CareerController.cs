using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CareerAdvisorApp.Models.Interfaces;
using Google.GenAI;
using Google.GenAI.Types;
using Markdig;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using CareerAdvisorApp.Hubs;

namespace CareerAdvisorApp.Controllers;
public class CareerController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<HomeController> _logger;
    private readonly ICareerService _careerService;
    private readonly IHubContext<ChatHub> _hubContext;

    public CareerController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ICareerService careerService, IHubContext<ChatHub> hubContext)
    {
        _logger = logger; 
        _userManager = userManager;
        _careerService = careerService;
        _hubContext = hubContext;
    }
    [Authorize(Policy = "UserOnly")]
    public IActionResult DetailsForm()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading details form");
            TempData["ErrorMessage"] = "An error occurred while loading the form. Please try again.";
            return RedirectToAction("Index", "Home");
        }
    }
    [Authorize(Policy = "UserOnly")]
    public async Task<IActionResult> GeneratePlan(CareerDetails careerDetails)
    {
        try
        {
            string? plan = await _careerService.GenerateCareerPlanAsync(careerDetails);
            string? userId = _userManager.GetUserId(User);
            int career_id = _careerService.SaveCareerPlan(plan, userId);
            return RedirectToAction("ViewPlan", new {career_id = career_id} );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating career plan");
            TempData["ErrorMessage"] = "Failed to generate career plan. Please try again.";
            return RedirectToAction("DetailsForm");
        }
    }

    [Authorize(Policy = "UserOnly")]
    [HttpGet("Career/ViewPlan/{career_id}")]
    public async Task<IActionResult> ViewPlan(int career_id)
    {
        try
        {
            string? userId = _userManager.GetUserId(User);

            _logger.LogInformation("Viewing plan with ID: {CareerPlanId}", career_id);
            CareerPlan? plan = _careerService.GetCareerPlanById(career_id);
            string TextToDisplay = plan?.PlanDetails ?? "No plans found against this Career Id for this user.";
            if(plan == null || plan.UserId != userId)
            {
                TextToDisplay = "No plans found against this Career Id for this user.";
            }

            var pipeline = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .Build();

            string htmlContent = Markdown.ToHtml(TextToDisplay ?? "", pipeline);
            ViewBag.HtmlPlan = htmlContent;
            ViewBag.ErrorMessage = null;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying career plan with ID: {CareerPlanId}", career_id);
            ViewBag.HtmlPlan = null;
            ViewBag.ErrorMessage = "An error occurred while displaying your career plan. Please try again later.";
            return View();
        }
    }
    [Authorize(Policy = "UserOnly")]
    public IActionResult Chat()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading chat page");
            TempData["ErrorMessage"] = "An error occurred while loading the chat. Please try again.";
            return RedirectToAction("Index", "Home");
        }
    }
    [Authorize(Policy = "UserOnly")]
    public IActionResult SavedPlans()   
    {
        try
        {
            string? userId = _userManager.GetUserId(User);
            List<CareerPlan> plans = _careerService.GetAllCareerPlansByUserId(userId);
            return View(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading saved plans");
            TempData["ErrorMessage"] = "An error occurred while loading your saved plans. Please try again.";
            return View(new List<CareerPlan>());
        }
    }

    [HttpPost]
    [Authorize(Policy = "UserOnly")]
    public async Task<IActionResult> SendChatMessage([FromBody] ChatMessageRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            if (string.IsNullOrWhiteSpace(request.ConnectionId))
            {
                return BadRequest(new { error = "Connection ID is required" });
            }

            await _careerService.StreamCareerChatResponseAsync(
                request.Message, 
                request.ConnectionId, 
                _hubContext
            );

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            return StatusCode(500, new { error = "Failed to process message" });
        }
    }
}
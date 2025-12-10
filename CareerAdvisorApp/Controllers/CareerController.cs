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
        return View();
    }
    [Authorize(Policy = "UserOnly")]
    public async Task<IActionResult> GeneratePlan(CareerDetails careerDetails)
    {   
        string? plan = await _careerService.GenerateCareerPlanAsync(careerDetails);
        string? userId = _userManager.GetUserId(User);
        int career_id = _careerService.SaveCareerPlan(plan, userId);
        return RedirectToAction("ViewPlan", new {career_id = career_id} );
    }

    [Authorize(Policy = "UserOnly")]
    [HttpGet("Career/ViewPlan/{career_id}")]
    public async Task<IActionResult> ViewPlan(int career_id)
    {
        string? userId = _userManager.GetUserId(User);

        Console.WriteLine("Viewing plan with ID: " + career_id);
        CareerPlan? plan = _careerService.GetCareerPlanById(career_id);
        string TextToDisplay = plan?.PlanDetails ?? "No plans found against this Career Id for this user.";
        if(plan == null || plan.UserId != userId)
        {
            TextToDisplay = "No plans found against this Career Id for this user.";
        }
        try
        {
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
            Console.WriteLine($"Error displaying plan: {ex.Message}");
            ViewBag.HtmlPlan = null;
            ViewBag.ErrorMessage = "An error occurred while displaying your career plan. Please try again later.";
            return View();
        }
    }
    [Authorize(Policy = "UserOnly")]
    public IActionResult Chat()
    {
        return View();
    }
    [Authorize(Policy = "UserOnly")]
    public IActionResult SavedPlans()   
    {
        string? userId = _userManager.GetUserId(User);
        List<CareerPlan> plans = _careerService.GetAllCareerPlansByUserId(userId);
        return View(plans);
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

            // Stream response to client via SignalR
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

    public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}
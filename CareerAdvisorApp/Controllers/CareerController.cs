using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CareerAdvisorApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Google.GenAI;
using Google.GenAI.Types;
using Markdig;

namespace CareerAdvisorApp.Controllers;
public class CareerController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    public CareerController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    [Authorize]
    public IActionResult DetailsForm()
    {
        Console.WriteLine("Reached Career Details Form");
        return View();
    }

    [Authorize]
    public async Task<IActionResult> ViewPlan(string userPrompt = "Develop career path for software developer interested in AI")
    {
        try
        {
            // var client = new Client(apiKey: "AIzaSyAH2Xs9_-f9mlxDvKKcA8DjPtOL1YCIry4");
            var client = new Client(apiKey: "AIzaSyC7E2AYIsmOPXx6eRhQ5OVel8V8aq5AZVw");
            
            // Retry logic for overloaded API
            int maxRetries = 3;
            int retryDelay = 2000; // 2 seconds
            Exception? lastException = null;
            
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var result = await client.Models.GenerateContentAsync(
                        model: "gemini-2.5-flash",
                        contents: userPrompt
                    );
                    
                    var aiResponse = result?.Candidates?[0].Content?.Parts?[0].Text ?? "No response from AI";
                    Console.WriteLine("AI Response: " + aiResponse);
                    
                    var pipeline = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .Build();

                    string htmlContent = Markdown.ToHtml(aiResponse, pipeline);
                    ViewBag.HtmlPlan = htmlContent;
                    ViewBag.ErrorMessage = null;

                    return View();
                }
                catch (Exception ex) when (ex.Message.Contains("overloaded") && attempt < maxRetries - 1)
                {
                    lastException = ex;
                    Console.WriteLine($"ex.Message: {ex.Message}");
                    Console.WriteLine($"API overloaded, retrying in {retryDelay}ms... (Attempt {attempt + 1}/{maxRetries})");
                    await Task.Delay(retryDelay);
                    retryDelay *= 2;  
                }
            }
            
            throw lastException ?? new Exception("API request failed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating plan: {ex.Message}");
            ViewBag.HtmlPlan = null;
            ViewBag.ErrorMessage = ex.Message.Contains("overloaded") 
                ? "The AI service is currently experiencing high demand. Please try again in a few moments."
                : "An error occurred while generating your career plan. Please try again later.";
            return View();
        }
    }
    [Authorize]
    public IActionResult Chat()
    {
        return View();
    }
    [Authorize]
    public IActionResult SavedPlans()   
    {
        return View();
    }
}

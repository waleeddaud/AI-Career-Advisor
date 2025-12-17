using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CareerAdvisorApp.Models;
using CareerAdvisorApp.Models.Interfaces;
using System.Security.Claims;

namespace CareerAdvisorApp.ViewComponents;

public class UserStatsViewComponent : ViewComponent
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICareerService _careerService;

    public UserStatsViewComponent(UserManager<IdentityUser> userManager, ICareerService careerService)
    {
        _userManager = userManager;
        _careerService = careerService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = _userManager.GetUserId((ClaimsPrincipal)User);
        var allPlans = _careerService.GetAllCareerPlansByUserId(userId);
        
        var firstPlan = allPlans.OrderBy(p => p.CreatedAt).FirstOrDefault();
        var lastPlan = allPlans.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
        
        var daysActive = firstPlan?.CreatedAt != null 
            ? (DateTime.Now - firstPlan.CreatedAt.Value).Days 
            : 0;

        var plansThisMonth = allPlans
            .Count(p => p.CreatedAt?.Month == DateTime.Now.Month && 
                       p.CreatedAt?.Year == DateTime.Now.Year);

        var stats = new UserStatsViewModel
        {
            TotalPlans = allPlans.Count,
            LastPlanDate = lastPlan?.CreatedAt,
            FirstPlanDate = firstPlan?.CreatedAt,
            DaysActive = daysActive,
            MostRecentPlanPreview = lastPlan?.PlanDetails?.Length > 100 
                ? lastPlan.PlanDetails.Substring(0, 100) + "..." 
                : lastPlan?.PlanDetails ?? "No plans yet",
            PlansThisMonth = plansThisMonth
        };

        return View(stats);
    }
}
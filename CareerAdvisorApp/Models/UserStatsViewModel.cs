namespace CareerAdvisorApp.Models;

public class UserStatsViewModel
{
    public int TotalPlans { get; set; }
    public DateTime? LastPlanDate { get; set; }
    public DateTime? FirstPlanDate { get; set; }
    public int DaysActive { get; set; }
    public string MostRecentPlanPreview { get; set; } = string.Empty;
    public int PlansThisMonth { get; set; }
}
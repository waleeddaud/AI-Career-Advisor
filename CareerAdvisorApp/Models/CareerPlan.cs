namespace CareerAdvisorApp.Models;
public class CareerPlan
{
    public string? UserId { get; set; }
    public string? PlanDetails { get; set; }    
    //I will think about add proposed topic for this plan later, maybe another api call to identify topic would suffice
}
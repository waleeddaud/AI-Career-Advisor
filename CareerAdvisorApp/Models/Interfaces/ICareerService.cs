namespace CareerAdvisorApp.Models.Interfaces;
public interface ICareerService
{
    public Task<string> GenerateCareerPlanAsync(CareerDetails careerDetails);   
    public int SaveCareerPlan(string? careerPlan, string? userId);
    public string GetCareerPlanById(int careerPlanId);
}
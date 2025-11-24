namespace CareerAdvisorApp.Models.Interfaces;
public interface ICareerService
{
    public Task<string> GenerateCareerPlanAsync(CareerDetails careerDetails);   
    public void SaveCareerPlan(string careerPlan, int userId);
}
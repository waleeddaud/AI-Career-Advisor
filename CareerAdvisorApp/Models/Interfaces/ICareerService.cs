using Microsoft.AspNetCore.SignalR;
using CareerAdvisorApp.Hubs;

namespace CareerAdvisorApp.Models.Interfaces;
public interface ICareerService
{
    public Task<string> GenerateCareerPlanAsync(CareerDetails careerDetails);   
    public int SaveCareerPlan(string? careerPlan, string? userId);
    public CareerPlan? GetCareerPlanById(int careerPlanId);
    public List<CareerPlan> GetAllCareerPlansByUserId(string? userId);
    public Task StreamCareerChatResponseAsync(string userMessage, string connectionId, IHubContext<ChatHub> hubContext);
}
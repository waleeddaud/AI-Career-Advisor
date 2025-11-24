using CareerAdvisorApp.Models;
using CareerAdvisorApp.Models.Interfaces;
using Microsoft.Data.SqlClient;
using Google.GenAI;
using Google.GenAI.Types;
using Dapper;
using System.Configuration;
namespace CareerAdvisorApp.Models.Services;
public class CareerService : ICareerService
{
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;
    public CareerService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    public string createPrompt(CareerDetails careerDetails)
    {
        return $"Create a detailed career development plan for an individual with the following details: Education Level: {careerDetails.EducationLevel}, Skills: {careerDetails.Skills}, Interests: {careerDetails.Interests}, Career Goals: {careerDetails.CareerGoals}, Experience: {careerDetails.Experience}, Industry: {careerDetails.Industry}, Work Style: {careerDetails.WorkStyle}, Salary Expectation: {careerDetails.Salary}, Timeline: {careerDetails.Timeline}. The plan should include recommended steps, resources, and a timeline to achieve their career goals.";
    }
    public async Task<string> GenerateCareerPlanAsync(CareerDetails careerDetails)
    {
        string userPrompt = createPrompt(careerDetails);
        string _apiKey = _configuration.GetSection("GeminiSettings")["apikey1"] ?? _configuration.GetSection("GeminiSettings")["apikey2"] ?? throw new Exception("API Key not found in configuration");
        var client = new Client(apiKey: _apiKey);
        
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
                return aiResponse;
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
    public int SaveCareerPlan(string? careerPlan, string? userId)
    {   
        using var connection = new SqlConnection(_connectionString);
        var sql = "INSERT INTO CareerPlan (UserId, PlanDetails) Output Inserted.Id VALUES (@UserId, @PlanDetails)";
        int r = connection.ExecuteScalar<int>(sql, new { UserId = userId, PlanDetails = careerPlan });
        return r;
    }
    public string GetCareerPlanById(int careerPlanId)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        string sql = "SELECT PlanDetails FROM CareerPlan WHERE Id = @Id";
        string? planDetails = conn.QuerySingleOrDefault<string>(sql, new { Id = careerPlanId });
        return planDetails ?? "No plan found."; 
    }
}
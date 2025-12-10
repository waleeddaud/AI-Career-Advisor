using Xunit;
using Moq;
using CareerAdvisorApp.Models;
using CareerAdvisorApp.Models.Services;
using Microsoft.Extensions.Configuration;

namespace CareerAdvisorApp.Tests
{
    public class CareerServiceTests
    {
        [Fact]
        public void CreatePrompt_WithValidCareerDetails_ReturnsFormattedPrompt()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockConnectionSection = new Mock<IConfigurationSection>();
            
            // Mock GetSection("ConnectionStrings")
            mockConnectionSection.Setup(x => x["DefaultConnection"])
                .Returns("test-connection-string");
            
            mockConfiguration.Setup(x => x.GetSection("ConnectionStrings"))
                .Returns(mockConnectionSection.Object);
            
            var careerService = new CareerService(mockConfiguration.Object);
            
            var careerDetails = new CareerDetails
            {
                EducationLevel = "Bachelor's Degree",
                Skills = "C#, ASP.NET Core, SQL",
                Interests = "Web Development, AI",
                CareerGoals = "Become a Senior Software Engineer",
                Experience = "3 years",
                Industry = "Technology",
                WorkStyle = "Remote",
                Salary = "$80,000",
                Timeline = "2 years"
            };

            // Act
            var result = careerService.createPrompt(careerDetails);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Bachelor's Degree", result);
            Assert.Contains("C#, ASP.NET Core, SQL", result);
            Assert.Contains("Web Development, AI", result);
            Assert.Contains("Become a Senior Software Engineer", result);
            Assert.Contains("3 years", result);
            Assert.Contains("Technology", result);
            Assert.Contains("Remote", result);
            Assert.Contains("$80,000", result);
            Assert.Contains("2 years", result);
            Assert.Contains("Create a detailed career development plan", result);
        }
    }
}

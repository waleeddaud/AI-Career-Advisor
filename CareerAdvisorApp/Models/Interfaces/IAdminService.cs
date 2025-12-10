using CareerAdvisorApp.Models;

namespace CareerAdvisorApp.Models.Interfaces;
public interface IAdminService
{
        Task<List<UserViewModel>> GetAllUsersWithRolesAsync();
        Task<bool> DeleteUserAsync(string userId);
}
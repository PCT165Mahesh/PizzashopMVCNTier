using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface IUserRepository
{
    public Task<User> GetUserByEmail(string email);

    public Task<bool> UpdateUserPassword(User user, string password);
    public Task<bool> UpdateUserProfileData(User user, ProfileDataViewModel model);
}

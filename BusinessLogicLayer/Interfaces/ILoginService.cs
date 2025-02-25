using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Interfaces;

public interface ILoginService
{
    public Task<string> LoginUser(string email, string password);
}

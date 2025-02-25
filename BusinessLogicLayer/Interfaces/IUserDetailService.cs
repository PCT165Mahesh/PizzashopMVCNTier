using System.Text.Json.Nodes;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogicLayer.Interfaces;

public interface IUserDetailService
{
    public JsonResult UserDetails(string token);

    public string UserName(string token);
    public string ImgUrl(string token);
    public string Email(string token);

    // public JsonResult GetUserDetails(int pageNo, int pageSize, string search);

    public Task<ProfileDataViewModel> GetProfileData(string email);

    public Task<bool> UpdateUserProfileData(ProfileDataViewModel model, string email);
}

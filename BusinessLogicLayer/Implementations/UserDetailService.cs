using System.Text.Json.Nodes;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogicLayer.Implementations;

public class UserDetailService : IUserDetailService
{
    private readonly JwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICountryDetailRepository _countryDetailRepository;


    public UserDetailService(JwtService jwtService, IGetUserRecordsRepository userRecordsRepositor, 
                IUserRepository userRepository, IRoleRepository roleRepository, ICountryDetailRepository countryDetailRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _countryDetailRepository = countryDetailRepository;
    }

    public string Email(string token)
    {
        return _jwtService.GetClaimValue(token, "email");
    }

    public string ImgUrl(string token)
    {
        return _jwtService.GetClaimValue(token, "imgUrl");
    }

    public string UserName(string token)
    {
        return _jwtService.GetClaimValue(token, "userName");
    }

    public JsonResult UserDetails(string token)
    {
        var email = _jwtService.GetClaimValue(token, "email");
        var userName = _jwtService.GetClaimValue(token, "userName");
        var imgUrl = _jwtService.GetClaimValue(token, "imgUrl");

        return new JsonResult(new { Email = email, UserName = userName, ImgUrl = imgUrl });
    }

    public async Task<ProfileDataViewModel> GetProfileData(string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            return new ProfileDataViewModel();
        }

        var roleObj = await _roleRepository.GetRoleById(user.Roleid);

        ProfileDataViewModel model = new ProfileDataViewModel();
        model.firstName = user.Firstname;
        model.lastName = user.Lastname;
        model.userName = user.Username;
        model.Role = roleObj.Rolename;
        model.phoneNo = user.Phone;
        model.zipcode = user.Zipcode;
        model.address = user.Address;
        model.Imgurl = user.Imgurl;
        model.CountryId = user.Countryid;
        model.CityId = user.Cityid;
        model.StateId = user.Stateid;
        model.Countries = _countryDetailRepository.GetCountry();
        model.States = _countryDetailRepository.GetState(user.Countryid);
        model.Cities = _countryDetailRepository.GetCity(user.Stateid);

        return model;
    }

    public async Task<bool> UpdateUserProfileData(ProfileDataViewModel model, string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if(user == null)
        {
            return false;
        }

        return await _userRepository.UpdateUserProfileData(user, model);
    }

    // public JsonResult GetUserDetails(int pageNo, int pageSize, string search)
    // {
    //     var (users, totalRecords) = _userRecordsRepository.GetAllUserRecords(pageNo, pageSize, search);

    //     return new JsonResult({users, totalRecords});
    // }


}

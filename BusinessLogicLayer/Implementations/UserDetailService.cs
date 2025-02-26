using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogicLayer.Implementations;

public class UserDetailService : IUserDetailService
{
    private readonly JwtService _jwtService;
    private readonly IGetUserRecordsRepository _userRecordsRepository;

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICountryDetailRepository _countryDetailRepository;


    public UserDetailService(JwtService jwtService, IGetUserRecordsRepository userRecordsRepository, 
                IUserRepository userRepository, IRoleRepository roleRepository, ICountryDetailRepository countryDetailRepository)
    {
        _jwtService = jwtService;
        _userRecordsRepository = userRecordsRepository;

        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _countryDetailRepository = countryDetailRepository;
    }

    /*-------------------------------------------------------------------------------------------------------------Get User Detials Service Implementation
    -----------------------------------------------------------------------------------------------------------------------------------------*/

    #region Loged In User Details Service
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

    #endregion



    /*-------------------------------------------------------------------------------------------------------------Get User Profile Data Service Implementation
    -----------------------------------------------------------------------------------------------------------------------------------------*/

    #region Profile Data Service
    public async Task<ProfileDataViewModel> GetProfileData(string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            return new ProfileDataViewModel();
        }

        var roleObj = await _roleRepository.GetRoleById(user.Roleid);

        return new ProfileDataViewModel()
        {
            firstName = user.Firstname,
            lastName = user.Lastname,
            userName = user.Username,
            Role = roleObj.Rolename,
            phoneNo = user.Phone,
            zipcode = user.Zipcode,
            address = user.Address,
            Imgurl = user.Imgurl,
            CountryId = user.Countryid,
            CityId = user.Cityid,
            StateId = user.Stateid,
            Countries = _countryDetailRepository.GetCountry(),
            States = _countryDetailRepository.GetState(user.Countryid),
            Cities = _countryDetailRepository.GetCity(user.Stateid)
        };
    }

    public async Task<JsonResult> GetUserDetails(int pageNo, int pageSize, string search)
    {
        PaginationViewModel model = await _userRecordsRepository.GetAllUserRecordsAsync(pageNo, pageSize, search);

        return new JsonResult(new { model.UserList, model.TotalRecords });
    }

    public async Task<long> GetUserIdByUserNameAsync(string userName)
    {
        var user = await _userRepository.GetUserByUserName(userName);
        if(user != null){
            return user.Id;
        }
        return -1;
    }
    #endregion


    /*-------------------------------------------------------------------------------------------------------------Update User Profile Service Implementation
    -----------------------------------------------------------------------------------------------------------------------------------------*/

    #region Update Profile Data Service
    public async Task<bool> UpdateUserProfileData(ProfileDataViewModel model, string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if(user == null)
        {
            return false;
        }

        return await _userRepository.UpdateUserProfileData(user, model);
    }

    #endregion
}

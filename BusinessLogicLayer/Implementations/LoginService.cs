using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BusinessLogicLayer.Implementations;

public class LoginService : ILoginService
{
    private readonly JwtService _jwtService;
    private readonly EncryptionService _encryptSercive;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;


    public LoginService(JwtService jwtService, EncryptionService encryptService, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _jwtService = jwtService;
        _encryptSercive = encryptService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }
    public async Task<string> LoginUser(string email, string password)
    {
        //Get the user from ther user repository
        var user = await _userRepository.GetUserByEmail(email);
        if(user == null){
            return null;
        }

        //Fetch the role 
        var roleObj = await _roleRepository.GetRoleById(user.Roleid);
        // Check for the Hashed Password

        if(user.Password == _encryptSercive.EncryptPassword(password)){

            // generate the Jwt token
            var token = _jwtService.GenerateJwtToken(email, roleObj.Rolename, user.Username, user.Imgurl);
            return token;
        }
        return null;
    }

}

using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class UserRepository : IUserRepository
{
    private readonly PizzaShopDbContext _context;


    public UserRepository(PizzaShopDbContext context)
    {
        _context = context;

    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.Where(u => u.Email == email && !u.Isdeleted).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateUserPassword(User user, string password)
    {
        try
        {
            user.Password = password;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("error message from Reset Password: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateUserProfileData(User user, ProfileDataViewModel model)
    {
        try
        {
            user.Firstname = model.firstName;
            user.Lastname = model.lastName;
            user.Username = model.userName;
            user.Phone = model.phoneNo;
            user.Zipcode = model.zipcode;
            user.Address = model.address;
            user.Countryid = model.CountryId;
            user.Stateid = model.StateId;
            user.Cityid = model.CityId;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id;


            // Handle Image Upload
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = $"{Guid.NewGuid()}_{model.ProfileImage.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                user.Imgurl = $"/uploads/{fileName}"; // Store relative path in DB
            }
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("error message from Reset Password: " + ex.Message);
            return false;
        }
    }

}

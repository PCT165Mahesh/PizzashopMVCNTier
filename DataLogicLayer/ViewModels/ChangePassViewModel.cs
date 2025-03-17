using System.ComponentModel.DataAnnotations;

namespace DataLogicLayer.ViewModels;

public class ChangePassViewModel
{
    
    [Required(ErrorMessage = "Current Password is required.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New Password is required.")]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, contain at least one letter, one number, and one special character.")]

    public string NewPassword { get; set; }
    [Required(ErrorMessage = "Confirm Password is required.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Password Do Not Match")]
    public string ConfirmNewPassword { get; set; }
}

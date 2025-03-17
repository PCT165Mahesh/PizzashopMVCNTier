using System.ComponentModel.DataAnnotations;

namespace DataLogicLayer.ViewModels;

public class ForgotPassViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Enter Valid Email")]
    public string Email { get; set; }
}

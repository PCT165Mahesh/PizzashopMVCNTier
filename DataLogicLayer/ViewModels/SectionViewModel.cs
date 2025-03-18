using System.ComponentModel.DataAnnotations;

namespace DataLogicLayer.ViewModels;

public class SectionViewModel
{
    public long SectionID { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    public string? Description { get; set; }

}

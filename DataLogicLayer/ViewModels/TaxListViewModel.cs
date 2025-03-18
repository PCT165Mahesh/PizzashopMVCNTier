using System.ComponentModel.DataAnnotations;

namespace DataLogicLayer.ViewModels;

public class TaxListViewModel
{

    public long? TaxId { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string TaxName { get; set; }

    [Required(ErrorMessage = "Tax Type is requied")]
    public bool TaxType { get; set;}
    public bool Isenabled { get; set;}
    public bool Default { get; set;}

    [Required(ErrorMessage = "Amount is requied")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Tax Amount must be Positive")]
    public Decimal TaxValue { get; set;}
}

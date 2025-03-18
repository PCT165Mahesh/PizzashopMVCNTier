namespace DataLogicLayer.ViewModels;

public class TaxViewModel
{
    public List<TaxListViewModel> TaxList { get; set; } = new List<TaxListViewModel>();
    public PaginationViewModel? Page{ get; set; }
}

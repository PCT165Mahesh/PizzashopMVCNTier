namespace DataLogicLayer.ViewModels;

public class ItemListViewModel
{
    public List<ItemsViewModel> ItemList { get; set; } = new List<ItemsViewModel>();

    public int TotalRecords { get; set; }
}

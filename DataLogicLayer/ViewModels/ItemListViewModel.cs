namespace DataLogicLayer.ViewModels;

public class ItemListViewModel
{
    public IEnumerable<ItemsViewModel>? ItemList { get; set; }

    public PaginationViewModel? Page { get; set; }
}

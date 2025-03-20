using System.ComponentModel.DataAnnotations;
using DataLogicLayer.Models;

namespace DataLogicLayer.ViewModels;

public class TableViewModel
{

    public long SectionId { get; set; }
    public long? TableId { get; set; }
    
    [Required(ErrorMessage = "Table Name is required")]
    public string? TableName { get; set; }

    [Required(ErrorMessage = "Capacity is requied")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
    public int Capacity { get; set; } = 0;
    public bool IsOccupied { get; set; }
    public IEnumerable<SectionViewModel>? SectionList { get; set; }
}

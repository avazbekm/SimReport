using System.Windows.Controls;

namespace SimReport.Windows;

public class ItemReturn
{
    public int Id { get; set; }
    public long SeriaNumber { get; set; } 
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public bool IsChecked {  get; set; }
}

using System;

namespace SimReport.Windows.Reports.PartnerSales;

public class ItemPartnerSale
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string PartnerFullName { get; set; } = string.Empty;
    public string ConnectedPersonFullName { get; set; } = string.Empty;
    public string TariffPlan { get; set; } = string.Empty;
    public string ConnectedPhoneNumber { get;set; } = string.Empty;
    public string SeriaNumber { get; set;} = string.Empty;
    public DateTime SaleDate { get; set; }
}

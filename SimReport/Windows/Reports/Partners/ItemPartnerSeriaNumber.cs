
using System;

namespace SimReport.Windows.Reports.Partners;

public class ItemPartnerSeriaNumber
{
    public int Id { get; set; }
    public string PartnerFullName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string SeriaNumber { get; set; } = string.Empty;
    public DateTime ComeDate { get; set; }
}

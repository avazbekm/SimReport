using SimReport.Entities.Users;
using SimReport.Entities.Companies;

namespace SimReport.Entities.Cards;

public class Card : Auditable
{
    public int CompanyId { get; set; }
    public Company Company { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public long CardNumber { get; set; }
    public string? Comment { get; set; }
    public bool IsSold { get; set; }
    public bool IsReturn {  get; set; }
}

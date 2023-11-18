using System.ComponentModel.DataAnnotations;

namespace SimReport.Entities.Users;

public class Company : Auditable
{
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public int AssetId { get; set; }
}

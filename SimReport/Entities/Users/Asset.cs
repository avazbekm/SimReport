using System.ComponentModel.DataAnnotations;

namespace SimReport.Entities.Users;

public class Asset : Auditable
{
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; }
}
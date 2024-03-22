using SimReport.Entities.Cards;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimReport.Entities.Users;

public class User : Auditable
{
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;
    public ICollection<Card> Cards { get; set; }
}

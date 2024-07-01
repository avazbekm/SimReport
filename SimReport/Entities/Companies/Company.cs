using SimReport.Entities.Cards;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimReport.Entities.Companies;

public class Company : Auditable
{
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Card> Cards { get; set; }
}

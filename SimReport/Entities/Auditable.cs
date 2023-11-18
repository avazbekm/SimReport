using System;

namespace SimReport.Entities;

public class Auditable : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public bool IsSold { get; set; }
}

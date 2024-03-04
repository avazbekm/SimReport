using System;

namespace SimReport.Entities;

public class Auditable
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public bool IsSold { get; set; }
}

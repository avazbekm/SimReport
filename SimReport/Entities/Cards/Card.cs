﻿using SimReport.Entities.Companies;

namespace SimReport.Entities.Cards;

public class Card : Auditable
{
    public int CompanyId { get; set; }
    public Company Company { get; set; }

    public long CardNumber { get; set; }
    public int UserId { get; set; }
    public bool IsSold { get; set; }
}

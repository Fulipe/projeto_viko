using System;
using System.Collections.Generic;

namespace viko_api.Models.Entities;

public partial class Student
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long EntityId { get; set; }

    public virtual Entity Entity { get; set; } = null!;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual User User { get; set; } = null!;
}

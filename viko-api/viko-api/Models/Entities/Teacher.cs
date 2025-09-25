using System;
using System.Collections.Generic;

namespace viko_api.Models.Entities;

public partial class Teacher
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long EntityId { get; set; }

    public virtual Entity Entity { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual User User { get; set; } = null!;
}

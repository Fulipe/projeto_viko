using System;
using System.Collections.Generic;

namespace viko_api.Models.Entities;

public partial class EventStatus
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}

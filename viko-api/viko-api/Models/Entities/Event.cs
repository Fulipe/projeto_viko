using System;
using System.Collections.Generic;

namespace viko_api.Models.Entities;

public partial class Event
{
    public long Id { get; set; }

    public long TeacherId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime FinishDate { get; set; }

    public DateTime RegistrationDeadline { get; set; }

    public string Category { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public long EventStatusId { get; set; }

    public long EntityId { get; set; }

    public virtual Entity Entity { get; set; } = null!;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual EventStatus EventStatus { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}

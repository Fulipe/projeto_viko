using System;
using System.Collections.Generic;

namespace viko_api.Models.Entities;

public partial class EventRegistration
{
    public long Id { get; set; }

    public long StudentId { get; set; }
    public User Student { get; set; } = null!;

    public long EventId { get; set; }
    public Event Event { get; set; } = null!;

    public DateTime? RegistrationDate { get; set; }
}

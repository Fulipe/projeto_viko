using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class EventRegistrationDto
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long EventId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual EventDto Event { get; set; } = null!;

    public virtual StudentDto Student { get; set; } = null!;
}

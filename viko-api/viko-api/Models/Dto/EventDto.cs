using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class EventDto
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

    public virtual EntityDto Entity { get; set; } = null!;

    public virtual ICollection<EventRegistrationDto> EventRegistrations { get; set; } = new List<EventRegistrationDto>();

    public virtual EventStatusDto EventStatus { get; set; } = null!;

    public virtual TeacherDto Teacher { get; set; } = null!;
}

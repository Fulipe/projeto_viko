using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class TeacherDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long EntityId { get; set; }

    public virtual EntityDto Entity { get; set; } = null!;

    public virtual ICollection<EventDto> Events { get; set; } = new List<EventDto>();

    public virtual UserDto User { get; set; } = null!;
}

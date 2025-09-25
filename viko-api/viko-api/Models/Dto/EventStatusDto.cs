using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class EventStatusDto
{
    public long Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<EventDto> Events { get; set; } = new List<EventDto>();
}

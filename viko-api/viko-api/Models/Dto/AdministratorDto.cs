using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class AdministratorDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long EntityId { get; set; }

    public virtual EntityDto Entity { get; set; } = null!;

    public virtual UserDto User { get; set; } = null!;
}

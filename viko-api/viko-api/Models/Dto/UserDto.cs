using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; }
    public DateOnly Birthdate { get; set; }
    public long EntityId { get; set; }
}

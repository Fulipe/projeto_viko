using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Role {  get; set; }
}

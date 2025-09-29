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
    public virtual ICollection<AdministratorDto> Administrators { get; set; } = new List<AdministratorDto>();
    public virtual EntityDto Entity { get; set; } = null!;
    public virtual ICollection<StudentDto> Students { get; set; } = new List<StudentDto>();
    public virtual ICollection<TeacherDto> Teachers { get; set; } = new List<TeacherDto>();
}

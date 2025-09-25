using System;
using System.Collections.Generic;

namespace viko_api.Models.Dto;

public class EntityDto
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public string Languages { get; set; } = null!;

    public virtual ICollection<AdministratorDto> Administrators { get; set; } = new List<AdministratorDto>();

    public virtual ICollection<EventDto> Events { get; set; } = new List<EventDto>();

    public virtual ICollection<StudentDto> Students { get; set; } = new List<StudentDto>();

    public virtual ICollection<TeacherDto> Teachers { get; set; } = new List<TeacherDto>();

    public virtual ICollection<UserDto> Users { get; set; } = new List<UserDto>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace viko_api.Models.Entities;

public partial class Entity
{
    public long Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    [Required]
    public string Languages { get; set; } = null!;

    public virtual ICollection<Administrator> Administrators { get; set; } = new List<Administrator>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

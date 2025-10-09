using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace viko_api.Models.Entities;

public partial class User
{
    public long Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required, EmailAddress(ErrorMessage = "This email is already in use")]
    public string Email { get; set; } = null!;
    
    [Required, Phone]
    public string Phone { get; set; } = null!;

    [Required, DataType(DataType.Date)]
    public DateOnly Birthdate { get; set; }

    public long EntityId { get; set; }

    public virtual ICollection<Administrator> Administrators { get; set; } = new List<Administrator>();

    public virtual Entity Entity { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}

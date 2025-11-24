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
    public string PasswordHash { get; set; } = null!;

    [Required, EmailAddress(ErrorMessage = "This email is already in use")]
    public string Email { get; set; } = null!;
    
    [Required, Phone]
    public string Phone { get; set; } = null!;

    [Required, DataType(DataType.Date)]
    public DateOnly Birthdate { get; set; }

    [Required]
    public int RoleId { get; set; }
    public Role Role { get; set; }  

    public long EntityId { get; set; }

    [Required]
    public Guid UserGuid { get; set; }

    public virtual Entity Entity { get; set; } = null!;
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

}

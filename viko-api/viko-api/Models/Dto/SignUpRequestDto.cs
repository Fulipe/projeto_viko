using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class SignUpRequestDto
    {
        [Required]
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; }
        [Required]
        [JsonPropertyName("LastName")]
        public string LastName { get; set; }
        [JsonPropertyName("Languages")]
        public string Languages { get; set; }
        [Required]
        [JsonPropertyName("Username")]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [Required, Phone]
        [JsonPropertyName("Phone")]
        public string Phone { get; set; }

        [Required, MinLength(8, ErrorMessage = "Password must consist of at least 8 characters.")]
        [JsonPropertyName("Password")]
        public string Password { get; set; }
        [Required]
        [JsonPropertyName("ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required, DataType(DataType.Date)]
        [JsonPropertyName("BirthDate")]
        public DateOnly BirthDate { get; set; }
    }
}

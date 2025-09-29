using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class SignUpRequestDto
    {
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        public string Languages { get; set; }
        [Required]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }
        [Required, MinLength(8, ErrorMessage = "Password must have at least 8 ")]
        public string Password { get; set; }

        [Required, DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
    }
}

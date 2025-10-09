using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class UserInfoDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Language { get; set; }
        public DateOnly Birthdate { get; set; }
        public string Phone { get; set; }
    }
}

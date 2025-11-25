using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class PasswordChangeDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword {  get; set; }
        public string ConfirmPassword { get; set; }
    }
}

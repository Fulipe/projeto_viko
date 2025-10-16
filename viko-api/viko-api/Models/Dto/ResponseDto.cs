using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class ResponseDto
    {
        public bool status {  get; set; }
        public string msg{ get; set; }
        public int value { get; set; }
    }
}

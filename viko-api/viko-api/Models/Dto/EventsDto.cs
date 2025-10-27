using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class EventsDto
    {
        public string Title { get; set; }
        public string Image {  get; set; }
        public string Teacher { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int EventStatus { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public Guid guid { get; set; }

    }
}

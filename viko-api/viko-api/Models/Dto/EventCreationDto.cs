using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace viko_api.Models.Dto
{
    public class EventCreationDto
    {
        [Required]
        //[JsonPropertyName("Title")]
        public string Title { get; set; }

        //[JsonPropertyName("Image")]
        public string Image { get; set; }

        [Required]
        //[JsonPropertyName("Language")]
        public string Language { get; set; }

        [Required]
        //[JsonPropertyName("Teacher")]
        public string Teacher { get; set; }

        //[JsonPropertyName("Description")]
        public string Description { get; set; }

        [Required]
        //[JsonPropertyName("Category")]
        public string Category { get; set; }

        [Required]
        //[JsonPropertyName("Location")]
        public string Location { get; set; }

        [Required]
        //[JsonPropertyName("City")]
        public string City { get; set; }

        [Required]
        //[JsonPropertyName("StartDate")]
        public DateTime StartDate { get; set; }
        
        [Required]
        //[JsonPropertyName("EndDate")]
        public DateTime EndDate { get; set; }

        //[Required]
        //[JsonPropertyName("RegistrationDeadline")]
        public DateTime RegistrationDeadline { get; set; }

    }
}

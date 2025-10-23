using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using viko_api.Models;
using viko_api.Models.Dto;

namespace viko_api.Services
{
    public interface IEventsService
    {
        Task<(ResponseDto, List<EventsDto>)> GetUserEvents(long userid);

        public class EventService : IEventsService
        {
            private readonly VikoDbContext _dbContext;
            public EventService(VikoDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<(ResponseDto, List<EventsDto>)> GetUserEvents(long userid)
            {
                var eventfetched = await _dbContext.EventRegistrations
                        .Where(u => u.StudentId == userid)
                        .Join(_dbContext.Events,
                            regId => regId.EventId,
                            ev => ev.Id,
                            (regId , ev) => new {regId, ev})

                        .Join(_dbContext.Entities,
                            events => events.ev.EntityId,
                            eId => eId.Id,
                            (events, eId) => new { events, eId })

                        .Select(join => new EventsDto 
                            {
                                Title = join.eId.Name,
                                Image = join.eId.Image,
                                Description = join.events.ev.Description,
                                Category = join.events.ev.Category,
                                Location = join.events.ev.Location,
                                StartDate = join.events.ev.StartDate,
                                EndDate = join.events.ev.FinishDate,
                                RegistrationDeadline = join.events.ev.RegistrationDeadline,
                                EventStatus = join.events.ev.EventStatusId   
                            }
                        ).ToListAsync();
                
                if (eventfetched == null || !eventfetched.Any())
                {
                    return (new ResponseDto
                    {
                        status = false,
                        msg = "Event not found!"
                    }, new List<EventsDto>());
                }

                return (new ResponseDto
                {
                    status = true,
                    msg = "Event fetched successfully"
                }, eventfetched);
            }
        }
    }
}

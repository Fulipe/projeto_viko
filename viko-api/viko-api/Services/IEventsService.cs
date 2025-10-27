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
        Task<(ResponseDto, List<EventsDto>)> GetAllPublicEvents();
        Task<(ResponseDto, List<EventsDto>)> GetStudentEvents(long userid);

        public class EventService : IEventsService
        {
            private readonly VikoDbContext _dbContext;
            public EventService(VikoDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<(ResponseDto, List<EventsDto>)> GetAllPublicEvents()
            {
                var eventfetched = await _dbContext.Events
                    .Join( _dbContext.Entities,
                        ev => ev.EntityId,
                        e => e.Id,

                        (ev, e) => new {ev, e}
                    )
                    .Select( 
                        join => new EventsDto
                        {
                            Title = join.e.Name,
                            Image = join.e.Image,
                            Language = join.e.Languages,
                            Description = join.ev.Description,
                            Category = join.ev.Category,
                            Location = join.ev.Location,
                            StartDate = join.ev.StartDate,
                            EndDate = join.ev.FinishDate,
                            RegistrationDeadline = join.ev.RegistrationDeadline,
                            EventStatus = join.ev.EventStatusId
                        }
                    )
                    .ToListAsync();

                if (eventfetched == null || eventfetched.Count < 1) 
                {
                    return (new ResponseDto
                    {
                        status = false,
                        msg = "No events were found"
                    }, new List<EventsDto>());
                }

                return (new ResponseDto
                {
                    status = true,
                    msg = "Events were found"
                }, eventfetched);
            }
            public async Task<(ResponseDto, List<EventsDto>)> GetStudentEvents(long userid)
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
                                Language = join.eId.Languages,
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
                        msg = "No events were found!"
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

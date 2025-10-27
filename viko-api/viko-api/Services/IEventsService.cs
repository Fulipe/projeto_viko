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
        Task<(ResponseDto, List<EventsDto>)> GetTeacherEvents(long userid);
        Task<(ResponseDto, EventsDto)> CreateEvent(long userid);

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
                    .Join(_dbContext.Users, //Gets the User registration for the responsible teacher 
                        t => t.ev.TeacherId,
                        teacher => teacher.Id,
                        (t, teacher) => new { t, teacher })

                    .Join(_dbContext.Entities, //Gets Entiity from teachers user registration, to get name
                        tn => tn.teacher.EntityId,
                        teacherName => teacherName.Id,
                        (tn, teacherName) => new { tn, teacherName })
                    .Select( 
                        join => new EventsDto
                        {
                            Title = join.tn.t.e.Name,
                            Image = join.tn.t.e.Image,
                            Language = join.tn.t.e.Languages,
                            Teacher = join.teacherName.Name,
                            Description = join.tn.t.ev.Description,
                            Category = join.tn.t.ev.Category,
                            Location = join.tn.t.ev.Location,
                            StartDate = join.tn.t.ev.StartDate,
                            EndDate = join.tn.t.ev.FinishDate,
                            RegistrationDeadline = join.tn.t.ev.RegistrationDeadline,
                            EventStatus = join.tn.t.ev.EventStatusId,
                            guid = join.tn.t.ev.EventGuid,
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

                        .Join(_dbContext.Users, //Gets the User registration for the responsible teacher 
                            t => t.ev.TeacherId,
                            teacher => teacher.Id,
                            (t, teacher) => new {t, teacher})

                        .Join(_dbContext.Entities, //Gets Entiity from teachers user registration, to get name
                            tn => tn.teacher.EntityId,
                            teacherName => teacherName.Id,
                            (tn, teacherName) => new { tn, teacherName })

                        .Join(_dbContext.Entities,
                            events => events.tn.t.ev.EntityId,
                            eId => eId.Id,
                            (events, eId) => new { events, eId })

                        .Select(join => new EventsDto 
                            {
                                Title = join.eId.Name,
                                Image = join.eId.Image,
                                Language = join.eId.Languages,
                                Teacher = join.events.teacherName.Name,
                                Description = join.events.tn.t.ev.Description,
                                Category = join.events.tn.t.ev.Category,
                                Location = join.events.tn.t.ev.Location,
                                StartDate = join.events.tn.t.ev.StartDate,
                                EndDate = join.events.tn.t.ev.FinishDate,
                                RegistrationDeadline = join.events.tn.t.ev.RegistrationDeadline,
                                EventStatus = join.events.tn.t.ev.EventStatusId,
                                guid = join.events.tn.t.ev.EventGuid
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
            public async Task<(ResponseDto, List<EventsDto>)> GetTeacherEvents(long userid)
            {
                var eventsFetched = await _dbContext.Events
                    .Where(t => t.TeacherId == userid)
                    .Join( _dbContext.Entities,
                        events => events.EntityId,
                        e => e.Id,
                        (events, e) => new {events, e})

                    .Join(_dbContext.Users, //Gets the User registration for the responsible teacher 
                        t => userid, //userid is from the user id of the teacher
                        teacher => teacher.Id,
                        (t, teacher) => new { t, teacher })

                    .Join(_dbContext.Entities, //Gets Entiity from teachers user registration, to get name
                        tn => tn.teacher.EntityId,
                        teacherName => teacherName.Id,
                        (tn, teacherName) => new { tn, teacherName })
                    .Select(
                        teacherEvents => new EventsDto
                        {
                            Title = teacherEvents.tn.t.e.Name,
                            Image = teacherEvents.tn.t.e.Image,
                            Language = teacherEvents.tn.t.e.Languages,
                            Teacher = teacherEvents.teacherName.Name,
                            Description = teacherEvents.tn.t.events.Description,
                            Category = teacherEvents.tn.t.events.Category,
                            Location = teacherEvents.tn.t.events.Location,
                            StartDate = teacherEvents.tn.t.events.StartDate,
                            EndDate = teacherEvents.tn.t.events.FinishDate,
                            RegistrationDeadline = teacherEvents.tn.t.events.RegistrationDeadline,
                            EventStatus = teacherEvents.tn.t.events.EventStatusId,
                            guid = teacherEvents.tn.t.events.EventGuid
                        }).ToListAsync();

                if (eventsFetched == null || !eventsFetched.Any())
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
                    msg = "Events fetched successfully"
                }, eventsFetched);
            }
            
            public async Task<(ResponseDto, EventsDto)> CreateEvent(long userid)
            {
                var e
            }

        }
    }
}

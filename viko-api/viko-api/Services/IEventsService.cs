using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using viko_api.Functions;
using viko_api.Models;
using viko_api.Models.Dto;
using viko_api.Models.Entities;

namespace viko_api.Services
{
    public interface IEventsService
    {
        Task<(ResponseDto, List<EventsDto>)> GetAllPublicEvents();
        Task<(ResponseDto, List<EventsDto>)> GetStudentEvents(long userid);
        Task<(ResponseDto, List<EventsDto>)> GetTeacherEvents(long userid);
        Task<ResponseDto> AdminCreateEvent(AdminEventCreationDto eventCreated);
        Task<ResponseDto> TeacherCreateEvent(long teacherid, TeacherEventCreationDto eventCreated);
        Task<(ResponseDto, EventsDto?)> GetEvent(string guid);
        Task<(ResponseDto, List<EventsDto?>)> GetEventOfUser(string guid);
        Task<ResponseDto> EventRegistration(long studentid, string guid);
        Task<ResponseDto> CancelEventRegistration(long studentid, string guid);
        Task<(ResponseDto, List<UserInfoDto>?)> RegistrationsList(string guid);
        Task<ResponseDto> EditEvent(string guid, EventEditDto eventUpdate);

        // Method to update event status
        Task<ResponseDto> UpdateEventStatus(string guid, int newStatus);
        Task<ResponseDto> DeleteEvent(string guid);
        Task<ResponseDto> RepublishEvent(string guid);
        Task<ResponseDto> EraseEvent(string guid);


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
                    //.Where(e => e.isViewed == true)
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
                            TeacherPhoto = join.teacherName.Image,
                            Description = join.tn.t.ev.Description,
                            Category = join.tn.t.ev.Category,
                            Location = join.tn.t.ev.Location,
                            City = join.tn.t.ev.City,
                            StartDate = join.tn.t.ev.StartDate,
                            EndDate = join.tn.t.ev.FinishDate,
                            RegistrationDeadline = join.tn.t.ev.RegistrationDeadline,
                            EventStatus = join.tn.t.ev.EventStatusId,
                            guid = join.tn.t.ev.EventGuid,
                            HasPendingStatusChange = join.tn.t.ev.HasPendingStatusChange,
                            isViewed = join.tn.t.ev.isViewed
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
                        .Where(u => u.StudentId == userid && u.Event.EventStatusId != 3 && u.Event.isViewed == true) //Searches for the events of the student and are not classified as 'finished'
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
                                City = join.events.tn.t.ev.City,
                                StartDate = join.events.tn.t.ev.StartDate,
                                EndDate = join.events.tn.t.ev.FinishDate,
                                RegistrationDeadline = join.events.tn.t.ev.RegistrationDeadline,
                                EventStatus = join.events.tn.t.ev.EventStatusId,
                                guid = join.events.tn.t.ev.EventGuid,
                                HasPendingStatusChange = join.events.tn.t.ev.HasPendingStatusChange,
                                isViewed = join.events.tn.t.ev.isViewed
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
                    .Where(t => t.TeacherId == userid && t.isViewed == true)
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
                            City = teacherEvents.tn.t.events.City,
                            StartDate = teacherEvents.tn.t.events.StartDate,
                            EndDate = teacherEvents.tn.t.events.FinishDate,
                            RegistrationDeadline = teacherEvents.tn.t.events.RegistrationDeadline,
                            EventStatus = teacherEvents.tn.t.events.EventStatusId,
                            guid = teacherEvents.tn.t.events.EventGuid,
                            HasPendingStatusChange = teacherEvents.tn.t.events.HasPendingStatusChange,
                            isViewed = teacherEvents.tn.t.events.HasPendingStatusChange
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
            public async Task<ResponseDto> AdminCreateEvent(AdminEventCreationDto eventCreated)
            {

                //Checks if title sent, is already in use
                bool checkTitle = await _dbContext.Events
                                .Join(_dbContext.Entities,
                                    ev => ev.EntityId,
                                    e => e.Id,

                                    (ev, e) => new { ev, e }
                                ).AnyAsync(r => r.e.Name == eventCreated.Title);

                if (checkTitle) return new ResponseDto { status = false, msg = "Event not created. Title selected is already in use" };

                
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var newEntity = new Entity
                    {
                        Name = eventCreated.Title,
                        Image = eventCreated.Image,
                        Languages = eventCreated.Language,
                    };

                    _dbContext.Entities.Add(newEntity);
                    await _dbContext.SaveChangesAsync();

                    var newEvent = new Event
                    {
                        EntityId = newEntity.Id,
                        TeacherId = eventCreated.Teacher,
                        Description = eventCreated.Description,
                        Category = eventCreated.Category,
                        Location = eventCreated.Location,
                        City = eventCreated.City,
                        StartDate = eventCreated.StartDate,
                        FinishDate = eventCreated.EndDate,
                        RegistrationDeadline = eventCreated.RegistrationDeadline,
                        EventStatusId = 1,
                        EventGuid = Guid.NewGuid(),
                        isViewed = true,
                    };

                    _dbContext.Add(newEvent);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var res = new ResponseDto
                    {
                        status = true,
                        msg = "Event was successfully created."
                    };
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event Creation failed - " + ex.Message
                    };
                }
            }
            public async Task<ResponseDto> TeacherCreateEvent(long teacherid, TeacherEventCreationDto eventCreated)
            {

                //Checks if title sent, is already in use
                bool checkTitle = await _dbContext.Events
                                .Join(_dbContext.Entities,
                                    ev => ev.EntityId,
                                    e => e.Id,

                                    (ev, e) => new {ev, e}
                                ).AnyAsync(r => r.e.Name == eventCreated.Title);

                if (checkTitle) return new ResponseDto { status = false, msg = "Event not created. Title selected is already in use" };

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var newEntity = new Entity
                    {
                        Name = eventCreated.Title,
                        Image = eventCreated.Image,
                        Languages = eventCreated.Language,
                    };

                    _dbContext.Entities.Add(newEntity);
                    await _dbContext.SaveChangesAsync();

                    var newEvent = new Event
                    {
                        EntityId = newEntity.Id,
                        TeacherId = teacherid,
                        Description = eventCreated.Description,
                        Category = eventCreated.Category,
                        Location = eventCreated.Location,
                        City = eventCreated.City,
                        StartDate = eventCreated.StartDate,
                        FinishDate = eventCreated.EndDate,
                        RegistrationDeadline = eventCreated.RegistrationDeadline,
                        EventStatusId = 1,
                        EventGuid = Guid.NewGuid(),
                        isViewed = true,
                    };

                    _dbContext.Add(newEvent);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var res = new ResponseDto
                    {
                        status = true,
                        msg = "Event was successfully created."
                    };
                    return res;
                }
                catch (Exception ex) {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event Creation failed - " + ex.Message
                    };
                }
            }
            public async Task<(ResponseDto, EventsDto?)>GetEvent(string guid)
            {
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return (new ResponseDto { status = false, msg = "GUID is Invalid" }, null);

                var eventFetched = await _dbContext.Events
                    .Where(ev => ev.EventGuid == parsedGuid)
                    .Join(_dbContext.Entities,
                        ev => ev.EntityId, //EntityID field in Events table
                        e => e.Id,         //Id field in Entities table (Refering to Event, to get Title, Languages, and Image)
                        (ev, e) => new {ev, e})

                    .Join(_dbContext.Users,
                        ev => ev.ev.TeacherId, //Teacher ID field in Events table
                        teacher => teacher.Id, //Id field in Users table

                        (ev, teacher) => new {ev, teacher})
                    .Join(_dbContext.Entities,
                        tn => tn.teacher.EntityId, //EntityID field in Users Table
                        teacherName => teacherName.Id, //Id field in Entities table (Refering to User, to get Teacher name)

                        (tn, teacherName) => new {tn, teacherName})
                    

                    .Select(join => new
                    {
                        EventFetched = new EventsDto
                        {
                            Title = join.tn.ev.e.Name,
                            Image = join.tn.ev.e.Image,
                            Language = join.tn.ev.e.Languages,
                            Teacher = join.teacherName.Name,
                            TeacherPhoto = join.teacherName.Image,
                            Description = join.tn.ev.ev.Description,
                            Category = join.tn.ev.ev.Category,
                            Location = join.tn.ev.ev.Location,
                            City = join.tn.ev.ev.City,
                            EventStatus = join.tn.ev.ev.EventStatusId,
                            StartDate = join.tn.ev.ev.StartDate,
                            EndDate = join.tn.ev.ev.FinishDate,
                            guid = join.tn.ev.ev.EventGuid,
                            RegistrationDeadline = join.tn.ev.ev.RegistrationDeadline,
                            isViewed = join.tn.ev.ev.isViewed
                            
                        }
                    })
                    .FirstOrDefaultAsync();

                if (eventFetched == null) 
                    return (new ResponseDto { status = false, msg = "Event not found" }, null);

                return (
                    new ResponseDto {
                        status = true, 
                        msg = "Event found" 
                    
                    }, eventFetched.EventFetched);
            }
            public async Task<(ResponseDto, List<EventsDto>?)> GetEventOfUser(string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return (new ResponseDto { status = false, msg = "GUID is Invalid" }, null);

                //Get user through guid in the params
                var getUser = await _dbContext.Users
                    .Where(e => e.UserGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getUser == null)
                    return (new ResponseDto { status = false, msg = "No event was found with the given GUID" }, null);

                //Get User Id of User provided by the GUID
                var userId = getUser.Id;

                //Get User Role of User provided by the GUID
                var userRole = getUser.RoleId;

                if (userRole == 1)
                {
                    var events = await this.GetStudentEvents(userId);
                    return (events.Item1, events.Item2);
                }
                else 
                {
                    var events = await this.GetTeacherEvents(userId);
                    return (events.Item1, events.Item2);
                }

            }
            public async Task<ResponseDto> EventRegistration(long studentid, string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return (new ResponseDto { status = false, msg = "GUID is Invalid" });

                //Get events to register through guid in the params
                var getGuidEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getGuidEvent == null) 
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };

                //Checks if Event is Opened, if not, doesn't allow registration
                if (getGuidEvent.EventStatusId != 1)
                    return new ResponseDto { status = false, msg = "Registration failed. This event is closed or finished" };

                //Get EventId of Event provided by the GUID
                var eventId = getGuidEvent.Id;

                //Checks if Student is already registered in the event
                bool checkRegistration = await _dbContext.EventRegistrations
                    .AnyAsync(r => r.EventId == eventId && r.StudentId == studentid);

                if (checkRegistration)
                    return new ResponseDto { status = false, msg = "Registration failed. Student is already registered in this event" };

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try {
                    //Create a new EventRegistration using that EventId and studentid in params
                    var newRegistration = new EventRegistration
                    {
                        EventId = eventId,
                        StudentId = studentid,
                        RegistrationDate = DateTime.UtcNow,
                    };

                    _dbContext.Add(newRegistration);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var res = new ResponseDto
                    {
                        status = true,
                        msg = "Student successfully registered"
                    };
                    return res;

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            public async Task<ResponseDto> CancelEventRegistration(long studentid, string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return (new ResponseDto { status = false, msg = "GUID is Invalid" });

                //Get events to register through guid in the params
                var getGuidEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getGuidEvent == null)
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };

                //Checks if Event is Opened, if not, doesn't allow registration
                if (getGuidEvent.EventStatusId != 1)
                    return new ResponseDto { status = false, msg = "Cancellation of Registration failed. This event is closed or finished" };

                //Get EventId of Event provided by the GUID
                var eventId = getGuidEvent.Id;

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    //Checks if Student is already registered in the event
                    var registration = await _dbContext.EventRegistrations
                        .Where(er => er.EventId == eventId && er.StudentId == studentid)
                        .FirstOrDefaultAsync();

                    if (registration == null)
                        return new ResponseDto { status = false, msg = "Cancellation of Registration failed. Student is not registered in this event" };


                    _dbContext.Remove(registration);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var res = new ResponseDto
                    {
                        status = true,
                        msg = "Student registration cancelled successfully"
                    };
                    return res;

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            public async Task<(ResponseDto, List<UserInfoDto>?)> RegistrationsList(string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return (new ResponseDto { status = false, msg = "GUID is Invalid" }, null);

                //Get events to register through guid in the params
                var getGuidEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getGuidEvent == null)
                    return (new ResponseDto { status = false, msg = "No event was found with the given GUID" }, null);

                //Get EventId of Event provided by the GUID
                var eventId = getGuidEvent.Id;

                var registrations = await _dbContext.EventRegistrations
                                    .Where(r => r.EventId == eventId)
                                    .Join(_dbContext.Users,
                                        er => er.StudentId,
                                        u => u.Id,

                                        (er, u) => new {er, u})
                                    .Join(_dbContext.Entities,
                                        u => u.u.EntityId,
                                        e => e.Id,

                                        (u, e) => new {u, e})
                                    .Join(_dbContext.Roles,
                                        u => u.u.u.RoleId,
                                        role => role.Id,
                                        
                                        (u, role) => new {u, role})
                                    .Select( userRegistrations => new UserInfoDto
                                    {
                                        Name = userRegistrations.u.e.Name,
                                        Email = userRegistrations.u.u.u.Email,
                                        Username = userRegistrations.u.u.u.Username,
                                        Role = userRegistrations.role.Name,
                                        Language = userRegistrations.u.e.Languages,
                                        Birthdate = userRegistrations.u.u.u.Birthdate,
                                        Phone = userRegistrations.u.u.u.Phone,
                                        Photo = userRegistrations.u.e.Image,
                                        UserGuid = userRegistrations.u.u.u.UserGuid,

                                    }).ToListAsync();

                return (new ResponseDto { status = true, msg = "Registrations fetched successfully!" }, registrations);

            }
            public async Task<ResponseDto> EditEvent(string guid, EventEditDto eventUpdate)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return new ResponseDto { status = false, msg = "GUID is Invalid" };

                //Get events to register through guid in the params
                var getGuidEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getGuidEvent == null)
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };

                //Get EventId of Event provided by the GUID
                var eventId = getGuidEvent.Id;

                var eventToUpdate = await _dbContext.Events
                    .Where(ev => ev.Id == eventId)
                    .Join(_dbContext.Entities,
                        ev => ev.EntityId,
                        e => e.Id,

                        (ev, e) => new {ev, e})
                    .FirstOrDefaultAsync();

                if (eventToUpdate == null)
                    return new ResponseDto{ status = false, msg = "Event not found."} ;

                // Gets Teachers UserId to save it on the Event table
                var getTeacherId = await _dbContext.Entities
                    .Where(e => e.Name == eventUpdate.Teacher)
                    .Join(_dbContext.Users,
                        u => u.Id,
                        ue => ue.EntityId,

                        (u, ue) => new {u, ue})
                    .FirstOrDefaultAsync();

                if (getTeacherId == null)
                    return new ResponseDto { status = false, msg = "Teacher not found." };

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    if (!string.IsNullOrEmpty(eventUpdate.Title))
                        eventToUpdate.e.Name = eventUpdate.Title;

                    if (!string.IsNullOrEmpty(eventUpdate.Image))
                        eventToUpdate.e.Image = eventUpdate.Image;

                    if (!string.IsNullOrEmpty(eventUpdate.Language))
                        eventToUpdate.e.Languages = eventUpdate.Language;
                    
                    if (!string.IsNullOrEmpty(eventUpdate.Teacher))
                        eventToUpdate.ev.TeacherId = getTeacherId.ue.Id;

                    if (!string.IsNullOrEmpty(eventUpdate.Description))
                        eventToUpdate.ev.Description = eventUpdate.Description;

                    if (!string.IsNullOrEmpty(eventUpdate.Category))
                        eventToUpdate.ev.Category = eventUpdate.Category;

                    if (!string.IsNullOrEmpty(eventUpdate.City))
                        eventToUpdate.ev.City = eventUpdate.City;

                    if (!string.IsNullOrEmpty(eventUpdate.Description))
                        eventToUpdate.ev.Description = eventUpdate.Description;

                    if (!string.IsNullOrEmpty(eventUpdate.Location))
                        eventToUpdate.ev.Location = eventUpdate.Location;

                    if (eventUpdate.StartDate != default)
                        eventToUpdate.ev.StartDate = eventUpdate.StartDate;

                    if (eventUpdate.EndDate != default)
                        eventToUpdate.ev.FinishDate = eventUpdate.EndDate;

                    //TO DO: Do when crontab for status monitoring is online
                    if (eventUpdate.RegistrationDeadline != default)
                            eventToUpdate.ev.RegistrationDeadline = eventUpdate.RegistrationDeadline;

                    _dbContext.Events.Update(eventToUpdate.ev);
                    _dbContext.Entities.Update(eventToUpdate.e);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseDto
                    {
                        status = true,
                        msg = "Event updated!"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event not updated - " + ex.Message
                    };
                }

            }
            public async Task<ResponseDto> UpdateEventStatus(string guid, int newStatus)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return new ResponseDto { status = false, msg = "GUID is Invalid" };

                //Get events to register through guid in the params
                var getGuidEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getGuidEvent == null)
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };

                //Get EventId of Event provided by the GUID
                var eventId = getGuidEvent.Id;
                var eventToUpdate = await _dbContext.Events
                    .Where(ev => ev.Id == eventId)
                    .FirstOrDefaultAsync();

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                { 
                    eventToUpdate.EventStatusId = newStatus;


                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseDto
                    {
                        status = true,
                        msg = "Event Status updated!"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event Status not updated - " + ex.Message
                    };
                }
            }
            public async Task<ResponseDto> DeleteEvent (string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return new ResponseDto { status = false, msg = "GUID is Invalid" };

                //Get events to register through guid in the params
                var getEvent= await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getEvent == null)
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };


                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    getEvent.isViewed = false;
                    getEvent.EventStatusId = 3;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseDto
                    {
                        status = true,
                        msg = "Event Deleted!"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event not deleted - " + ex.Message
                    };
                }

            }
            public async Task<ResponseDto> RepublishEvent (string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return new ResponseDto { status = false, msg = "GUID is Invalid" };

                //Get events to register through guid in the params
                var getEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getEvent == null)
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };


                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    getEvent.isViewed = true;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseDto
                    {
                        status = true,
                        msg = "Event Republished!"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event was not republished - " + ex.Message
                    };
                }

            }
            public async Task<ResponseDto> EraseEvent (string guid)
            {
                //Parses incoming string as GUID
                if (!Guid.TryParse(guid, out Guid parsedGuid))
                    return new ResponseDto { status = false, msg = "GUID is Invalid" };

                //Get events to register through guid in the params
                var getEvent = await _dbContext.Events
                    .Where(e => e.EventGuid == parsedGuid)
                    .FirstOrDefaultAsync();

                //Checks if GUID provided exists.
                if (getEvent == null)
                    return new ResponseDto { status = false, msg = "No event was found with the given GUID" };

                var registrationsToDelete = await _dbContext.EventRegistrations.Where(e => e.EventId == getEvent.Id).ToListAsync();
                
                //Checks if registration exists.
                if (registrationsToDelete == null)
                    return new ResponseDto { status = false, msg = "No registration was found " };

                var entityToDelete = await _dbContext.Entities.Where(e => e.Id == getEvent.EntityId).FirstOrDefaultAsync();

                //Checks if entity exists.
                if (entityToDelete == null)
                    return new ResponseDto { status = false, msg = "No entity was found " };

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    _dbContext.Events.Remove(getEvent);
                    _dbContext.EventRegistrations.RemoveRange(registrationsToDelete);
                    _dbContext.Entities.Remove(entityToDelete);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseDto
                    {
                        status = true,
                        msg = "Event Erased permanantly!"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Event not Erased - " + ex.Message
                    };
                }
            }
        }
    }
}

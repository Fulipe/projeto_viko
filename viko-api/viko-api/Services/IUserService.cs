using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using viko_api.Models;
using viko_api.Models.Dto;
using viko_api.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace viko_api.Services
{
    public interface IUserService
    {
        Task<(UserDto?, ResponseDto)> Authenticate(string username, string password);
        Task<ResponseDto> RegisterUser(SignUpRequestDto request);
        Task<(ResponseDto, UserInfoDto)> GetUserById(int id);
        Task<ResponseDto> UpdateUser (int id, UserInfoDto userinfo);

        public class UserService : IUserService
        {
            //Instanciates services
            private readonly VikoDbContext _dbContext;
            public UserService(VikoDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<(UserDto?, ResponseDto)> Authenticate(string username, string password)
            {
                var user = await _dbContext.Users
                    .Where (u => u.Username == username && u.PasswordHash == password)
                    .Select(u => new
                    {
                        UserDto = new UserDto
                        {
                            Id = u.Id,
                            Username = u.Username,
                        },
                    }).FirstOrDefaultAsync();

                if (user == null)
                {
                    return (null, 
                        new ResponseDto {
                            status = false,
                            msg = "Authentication failed. Please check you credentials."
                        }
                    );
                }

                return (user.UserDto, 
                    new ResponseDto {
                        status = true,
                        msg = "Authentication successful!"
                    }
                );
            }
            public async Task<ResponseDto> RegisterUser(SignUpRequestDto request)
            {
                //Username Validation
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Username == request.Username))
                {
                    var res = new ResponseDto
                    {
                        status = false,
                        msg = "Username alrerady in use. Please choose a different username"
                    };
                    return res;
                }

                //Email Validation
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == request.Email))
                {
                    var res = new ResponseDto
                    {
                        status = false,
                        msg = "Email alrerady in use. Please choose a different email"
                    };
                    return res;
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var newEntity = new Entity
                    {
                        Name = request.FirstName + ' ' + request.LastName,
                        Image = request.Photo,
                        Languages = request.Languages,
                    };

                    _dbContext.Entities.Add(newEntity);
                    await _dbContext.SaveChangesAsync();

                    var newUser = new User
                    {
                        EntityId = newEntity.Id,
                        Username = request.Username,
                        Email = request.Email,
                        Birthdate = request.BirthDate,
                        PasswordHash = request.Password,
                        Phone = request.Phone,
                        RoleId = 1,
                    };

                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var res = new ResponseDto
                    {
                        status = true,
                        msg = "User was successfully created."
                    };
                    return res;
                }
                catch (Exception ex) {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            public async Task<(ResponseDto, UserInfoDto?)> GetUserById(int id)
            {
                var user = await _dbContext.Users
                    .Where(user => user.Id == id)
                    .Join(_dbContext.Entities,
                        u => u.EntityId,
                        e => e.Id,
                        (u, e) => new { u, e })
                    .Join(_dbContext.Roles,
                        u => u.u.RoleId,
                        r => r.Id,
                        (u, r) => new { u, r })
                    .Select(join => new
                    {
                        UserInfoDto = new UserInfoDto
                        {
                            Name = join.u.e.Name,
                            Language = join.u.e.Languages,
                            Username = join.u.u.Username,
                            Email = join.u.u.Email,
                            Birthdate = join.u.u.Birthdate,
                            Phone = join.u.u.Phone,
                            Photo = join.u.e.Image,
                            Role = join.r.Name

                        },
                    }).FirstOrDefaultAsync();

                if (user == null)
                {
                    return (new ResponseDto
                    {
                        status = false,
                        msg = "User was not found"

                    }, null);
                }

                return (new ResponseDto 
                {
                    status = true,
                    msg = "User was found and sent" 
                }, user.UserInfoDto);
            }
            public async Task<ResponseDto> UpdateUser(int id, UserInfoDto userinfo)
            {
                var user = await _dbContext.Users
                    .Where(user => user.Id == id)
                    .Join(_dbContext.Entities,
                        u => u.EntityId,
                        e => e.Id,
                        (u, e) => new { u, e })
                    .FirstOrDefaultAsync();

                if(user == null)
                {
                    return new ResponseDto
                    {
                        status = false,
                        msg = "User not found.",
                    };
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    if (!string.IsNullOrEmpty(userinfo.Username))
                        user.u.Username = userinfo.Username;

                    if (!string.IsNullOrEmpty(userinfo.Email))
                        user.u.Email = userinfo.Email;

                    if (!string.IsNullOrEmpty(userinfo.Phone))
                        user.u.Phone = userinfo.Phone;
                    
                    if (userinfo.Birthdate != default)
                        user.u.Birthdate = userinfo.Birthdate;

                    if (!string.IsNullOrEmpty(userinfo.Name))
                        user.e.Name = userinfo.Name;
                    
                    if (!string.IsNullOrEmpty(userinfo.Language))
                        user.e.Languages = userinfo.Language;
                    
                    if (!string.IsNullOrEmpty(userinfo.Photo))
                        user.e.Image = userinfo.Photo; // base64 string

                    _dbContext.Users.Update(user.u);
                    _dbContext.Entities.Update(user.e);
                    await _dbContext.SaveChangesAsync();
    
                    await transaction.CommitAsync();

                    return new ResponseDto
                    {
                        status = true,
                        msg = "User updated!"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseDto
                    {
                        status = false,
                        msg = "User not update - " + ex.Message 
                    };
                    throw;
                }

            }
        }
    }
}

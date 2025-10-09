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
namespace viko_api.Services
{
    public interface IUserService
    {
        Task<UserDto> Authenticate(string username, string password);
        Task<SignUpResponseDto> RegisterUser(SignUpRequestDto request);
        Task<UserInfoDto> GetUserById(int id);

        public class UserService : IUserService
        {
            //Instanciates services
            private readonly VikoDbContext _dbContext;
            public UserService(VikoDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<UserDto> Authenticate(string username, string password)
            {
                var user = await _dbContext.Users
                    .Where (u => u.Username == username && u.Password == password)
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
                    return (null);
                }
                return (user.UserDto);
            }
            public async Task<SignUpResponseDto> RegisterUser(SignUpRequestDto request)
            {
                //Username Validation
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Username == request.Username))
                {
                    var res = new SignUpResponseDto
                    {
                        status = false,
                        msg = "Username alrerady in use. Please choose a different username"
                    };
                    return res;
                }

                //Email Validation
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == request.Email))
                {
                    var res = new SignUpResponseDto
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
                        Name = request.firstName + ' ' + request.lastName,
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
                        Password = request.Password,
                        Phone = request.Phone,
                    };

                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var res = new SignUpResponseDto
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
            public async Task<UserInfoDto> GetUserById(int id)
            {
                var user = await _dbContext.Users
                    .Where(user => user.Id == id)
                    .Join(_dbContext.Entities,
                        u => u.EntityId,
                        e => e.Id,
                        (u, e) => new { u, e })
                    .Select(join => new
                    {
                        UserInfoDto = new UserInfoDto
                        {
                            Name = join.e.Name,
                            Language = join.e.Languages,
                            Username = join.u.Username,
                            Email = join.u.Email,
                            Birthdate = join.u.Birthdate,
                            Phone = join.u.Phone

                        },
                    }).FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                return user.UserInfoDto;
            }
        }
    }
}

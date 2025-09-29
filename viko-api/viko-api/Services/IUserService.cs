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
        Task<bool> RegisterUser(SignUpRequestDto request);

        public class UserService : IUserService
        {
            //Instanciates services
            private readonly VikoDbContext _dbContext;
            public UserService(VikoDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<UserDto> Authenticate(
                string username, string password)
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
            public async Task<bool> RegisterUser(SignUpRequestDto request)
            {
                //Username Validation
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Username == request.Username)) return false;
                
                //Email Validation
                if (await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == request.Email)) return false;

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
                    return true;
                }
                catch (Exception ex) {
                    await transaction.RollbackAsync();
                    throw;
                }
            }


        }
    }
}

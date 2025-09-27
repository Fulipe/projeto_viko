using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using viko_api.Models;
using viko_api.Models.Dto;
namespace viko_api.Services
{
    public interface IUserService
    {
        Task<UserDto> Authenticate(string username, string password);

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
        }
    }
}

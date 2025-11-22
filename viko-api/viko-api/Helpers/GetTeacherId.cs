using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using viko_api.Models;
using viko_api.Models.Dto;

namespace viko_api.Helpers
{
    public class GetTeacherId
    {
        private readonly VikoDbContext _dbContext;

        public GetTeacherId(VikoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TeacherShortInfoDto> GetTeacherByUsername(string teacherUsername)
        {
            var teacher = await _dbContext.Users
                .Where(t => t.Username == teacherUsername)
                .Join(_dbContext.Entities,
                    u => u.EntityId,
                    e => e.Id,

                    (u, e) => new {u, e}
                )
                .Select( teacher => new
                { 
                    Teacher = new TeacherShortInfoDto{
                        Id = teacher.u.Id,
                        Username = teacher.u.Username,
                        Name = teacher.e.Name,
                    }
                }).FirstOrDefaultAsync();

            return teacher.Teacher;
        }
    }
}

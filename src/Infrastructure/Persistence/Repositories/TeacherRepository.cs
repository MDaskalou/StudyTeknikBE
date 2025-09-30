using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using Domain.Abstractions.Enum;
using Application.Mapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _db;
        public TeacherRepository(AppDbContext db) => _db = db;

        
     

        public async Task<GetTeacherByIdDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
           var teacherUser = await _db.Users
               .AsNoTracking()
               .Where(u => u.Id == id && u.Role == UserRole.Teacher)
               .Select (u => new
               {
                   u.Id,
                     u.FirstName,
                     u.LastName,
                     u.Email,
               })
               .FirstOrDefaultAsync(ct);

           if (teacherUser is null)
           {
               return null;
           }
           
           var teacherTaughtClasses = await _db.Classes
               .AsNoTracking()
               .Where(c => c.TeacherId == teacherUser.Id)
               .Select(c => new TeacherTaughtClassDto(c.Id, c.ClassName))
               .ToListAsync(ct);
           
           var resultDto = new GetTeacherByIdDto(
               teacherUser.Id,
               teacherUser.FirstName,
               teacherUser.LastName,
               teacherUser.Email,
               teacherTaughtClasses);
           
           return resultDto;

        }
        
        public async Task<IReadOnlyList<GetAllTeachersDto>> GetAllAsync(CancellationToken ct)
        {
            var teacher  = await _db.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Teacher)
                .OrderBy(u => u.FirstName).ThenBy(u=>u.LastName)
                .ToListAsync(ct);
            
            var teacherDto = teacher
                .Select(TeacherMapper.ToDetailsDto)
                .ToList();
            return teacherDto;
        }

        public void Add(UserEntity user)
        {
            _db.Users.Add(user);
        }
    }
}
using Application.Common.Results;
using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using Domain.Abstractions.Enum;
using Application.Mapper;
using Domain.Entities;
using Domain.Common;
using Microsoft.Data.SqlClient;
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
               .Where(u => u.Id == id && u.Role == Role.Teacher)
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
                .Where(u => u.Role == Role.Teacher)
                .OrderBy(u => u.FirstName).ThenBy(u=>u.LastName)
                .ToListAsync(ct);
            
            var teacherDto = teacher
                .Select(TeacherMapper.ToDetailsDto)
                .ToList();
            return teacherDto;
        }
        
        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        {
            return await _db.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, ct);
        }

        public async Task<OperationResult> AddAsync(UserEntity user, CancellationToken ct)
        {
            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);
                return OperationResult.Success();

            }
            catch (DbUpdateException ex)
            {
                    if(ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                    {
                        return OperationResult.Failure(Error.Conflict("Teacher.EmailAlreadyExists", "En användare med denna e-postadress finns redan."));
                    }
                    
                    return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett fel uppstod vid lagring av läraren i databasen."));
            }
        }

        public async Task<OperationResult> UpdateAsync(UserEntity user, CancellationToken ct)
        {
            try
            {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
            return OperationResult.Success();
                
            }
            catch (DbUpdateException ex)
            {
                return OperationResult.Failure(
                    Error.InternalServiceError("Database.Error", "Ett fel uppstod vid uppdatering av läraren i databasen."));
            }
        }
        
        //använder inte AsNoTracking här eftersom vi vill spåra entiteten för uppdatering
        //EntityFramwork kommer att spåra ändringar på den här entiteten
        //Jag lägger den här metoden i repositoryt eftersom det är repositoryts ansvar att hantera databasoperationer
        //och det är bättre att hålla databaslogiken inkapslad i repository än att sprida den över hela applikationen
        
        public async Task<UserEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct)
        {
            
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == Role.Teacher, ct);
        }
        
        public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            var rowsAffected = await _db.Users
                .Where (u => u.Id == id && u.Role == Role.Teacher)
                .ExecuteDeleteAsync(ct);

            if (rowsAffected == 0)
            {
                return OperationResult.Failure(Error.NotFound("Teacher.NotFound", $"Lärare med ID {id} kunde inte hittas."));
            }
            return OperationResult.Success();
        }
    }
}   
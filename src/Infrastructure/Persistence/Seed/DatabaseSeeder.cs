using Domain.Abstractions.Enum;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed
{
    public static class DatabaseSeeder
    {
        // GILTIGA (hex) statiska GUIDs – lätta att känna igen
        public static readonly Guid AdminId   = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid TeacherId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid MentorId  = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid StudentId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        public static async Task SeedAsync(AppDbContext db, ILogger? logger = null, CancellationToken ct = default)
        {
            // Idempotent: om Users finns redan, hoppa
            if (await db.Users.AnyAsync(ct))
            {
                logger?.LogInformation("Seed: Users finns redan – hoppar över.");
                return;
            }

            var now = DateTime.UtcNow;

            // OBS: Role är int i UserEntity.
            // Din enum-order är: Student=0, Teacher=1, Admin=2, Mentor=3
            var admin = new UserEntity
            {
                Id = AdminId,
                FirstName = "Alice",
                LastName = "Admin",
                Email = "admin@demo.local",
                SecurityNumber = "19700101-0000",   
                Role = UserRole.Admin,
                ExternalProvider = "dev",
                ExternalSubject = "admin",
                ConsentGiven = false,                
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            var teacher = new UserEntity
            {
                Id = TeacherId,
                FirstName = "Tom",
                LastName = "Teacher",
                Email = "teacher@demo.local",
                SecurityNumber = "19700101-0001",   
                Role = UserRole.Teacher,
                ExternalProvider = "dev",
                ExternalSubject = "teacher",
                ConsentGiven = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            var mentor = new UserEntity
            {
                Id = MentorId,
                FirstName = "Mira",
                LastName = "Mentor",
                Email = "mentor@demo.local",
                SecurityNumber = "19700101-0002",   
                Role = UserRole.Mentor,
                ExternalProvider = "dev",
                ExternalSubject = "mentor",
                ConsentGiven = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            var student = new UserEntity
            {
                Id = StudentId,
                FirstName = "Sam",
                LastName = "Student",
                Email = "student@demo.local",
                SecurityNumber = "19700101-0003",   
                Role = UserRole.Student,
                ExternalProvider = "dev",
                ExternalSubject = "student",
                ConsentGiven = false,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await db.Users.AddRangeAsync(admin, teacher, mentor, student);
            await db.SaveChangesAsync(ct);

            // Classes med TeacherId
            var c1 = new ClassEntity
            {
                Id = Guid.NewGuid(), SchoolName = "Bräckeskolan", Year = 2025, ClassName = "9B",
                TeacherId = TeacherId, CreatedAtUtc = now, UpdatedAtUtc = now
            };
            var c2 = new ClassEntity
            {
                Id = Guid.NewGuid(), SchoolName = "Bräckeskolan", Year = 2025, ClassName = "9C",
                TeacherId = TeacherId, CreatedAtUtc = now, UpdatedAtUtc = now
            };
            await db.Classes.AddRangeAsync(c1, c2);
            await db.SaveChangesAsync(ct);

            // Enrollment (student -> klass)
            var enrollment = new EnrollmentEntity
            {
                Id = Guid.NewGuid(), StudentId = StudentId, ClassId = c1.Id,
                CreatedAtUtc = now, UpdatedAtUtc = now
            };
            await db.Enrollments.AddAsync(enrollment, ct);

            // MentorAssignment (mentor -> student)
            var ma = new MentorAssigmentEntity
            {
                Id = Guid.NewGuid(), MentorId = MentorId, StudentId = StudentId,
                CreatedAtUtc = now, UpdatedAtUtc = now
            };
            await db.MentorAssignments.AddAsync(ma, ct);
            await db.SaveChangesAsync(ct);

            // Diary entries (student)
            var d1 = new DiaryEntity
            {
                Id = Guid.NewGuid(), StudentId = StudentId,
                EntryDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1)),
                Text = "Första anteckningen.", CreatedAtUtc = now, UpdatedAtUtc = now
            };
            var d2 = new DiaryEntity
            {
                Id = Guid.NewGuid(), StudentId = StudentId,
                EntryDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                Text = "Andra anteckningen.", CreatedAtUtc = now, UpdatedAtUtc = now
            };
            await db.Diaries.AddRangeAsync(d1, d2);
            await db.SaveChangesAsync(ct);

            logger?.LogInformation("Seed klar: admin/teacher/mentor/student + klasser, enrollment, mentorassignment, diary entries.");
        }
    }
}

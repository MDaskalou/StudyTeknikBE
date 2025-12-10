using Domain.Entities;
using Domain.Models.StudentProfiles;
// Där din Entity (DB-klass) ligger

// Där din Domän-modell ligger

namespace Infrastructure.Persistence.Mapper
{
    public static class StudentProfileMapper
    {
        // 1. Från Domän -> Till Databas (Används vid Spara/Add/Update)
        public static StudentProfileEntity ToEntity(StudentProfile domain)
        {
            if (domain == null) return null;

            return new StudentProfileEntity
            {
                Id = domain.Id,
                // Här mappar vi domain.StudentId till databasens UserId
                StudentId = domain.StudentId, 
                
                PlanningHorizonWeeks = domain.PlanningHorizonWeeks,
                WakeUpTime = domain.WakeUpTime,
                BedTime = domain.BedTime,
                
                // Om du har audit-fält i domänen kan du mappa dem, 
                // annars sätter DbContext ofta CreatedAtUtc automatiskt vid Add.
                CreatedAtUtc = domain.CreatedAtUtc,
                UpdatedAtUtc = domain.UpdatedAtUtc
            };
        }

        // 2. Från Databas -> Till Domän (Används vid Get/Find)
        public static StudentProfile ToDomain(StudentProfileEntity entity)
        {
            if (entity == null) return null;

            // Här använder vi din statiska Load-metod eftersom konstruktorn är private!
            return StudentProfile.Load(
                entity.Id,
                entity.StudentId, 
                entity.PlanningHorizonWeeks,
                entity.WakeUpTime,
                entity.BedTime,
                entity.CreatedAtUtc,
                entity.UpdatedAtUtc,
                
                // Om du vill ladda kurserna också, behöver du en loop här,
                // eller skicka in null/tom lista om du inte Include:at dem.
                new List<Course>() 
            );
        }
    }
}
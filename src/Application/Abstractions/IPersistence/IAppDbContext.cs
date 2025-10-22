using System.Threading;
using Domain.Audit;
using Domain.Entities;
using Domain.Models.Classes;
using Domain.Models.Diary;
using Domain.Models.Mentors;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;


namespace Application.Abstractions.IPersistence
{
    //TODO: implementera AppDbContext i infrastucture (DbContext)
    //TODO: Lägg till DbSets för entiteter
    //Syftet med den här interfacen är att definera en kontrakt för databasoperationer
    
    //databasoperationer syftar på de olika åtgärder som kan utföras på en databas,
    //såsom att skapa, läsa, uppdatera och ta bort data.
    public interface IAppDbContext
    {
        //DbSet
        
        DbSet<UserEntity> Users { get; }
        DbSet<DiaryEntity> Diaries { get; }
        DbSet<WeeklySummaryEntity> WeeklySummaries { get; }
        DbSet<ClassEntity> Classes { get; }
        DbSet<EnrollmentEntity> Enrollments { get; }
        DbSet<MentorAssigmentEntity> MentorAssignments { get; }
        DbSet<AuditLogEntity> AuditLogs { get; }
        DbSet<DeckEntity> Decks { get; }
        DbSet<FlashCardEntity> FlashCards { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.IPersistence; // IAppDbContext
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    //Todo: Implementering av AppDBContext
    //Todo: db-provider är sql server (ef core)
    //Den här klassen ska ärva från DbContext och representera databaskontexten för applikationen.
    //Den ska innehålla DbSet-egenskaper för varje entitet i applikationen.
    //AppDBContext används för att interagera med databasen och utföra CRUD-operationer.
    //Den ska konfigureras i Web.Program.cs för att använda rätt databasleverantör och anslutningssträng.
    public class AppDbContext :DbContext, IAppDbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<DiaryEntryEntity> DiaryEntries => Set<DiaryEntryEntity>();
        public DbSet<WeeklySummaryEntity> WeeklySummaries => Set<WeeklySummaryEntity>();
        public DbSet<ClassEntity> Classes => Set<ClassEntity>();
        public DbSet<EnrollmentEntity> Enrollments => Set<EnrollmentEntity>();
        public DbSet<MentorAssigmentEntity> MentorAssignments => Set<MentorAssigmentEntity>(); // se notis nedan
        public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
        
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Plockar upp alla *Configuration.cs i Infrastructure.Persistence.Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

        }
        
        // Stämplar CreatedAtUtc/UpdatedAtUtc om fälten finns
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var e in ChangeTracker.Entries()
                         .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (e.Metadata.FindProperty("UpdatedAtUtc") != null)
                    e.Property("UpdatedAtUtc").CurrentValue = now;

                if (e.State == EntityState.Added
                    && e.Metadata.FindProperty("CreatedAtUtc") != null
                    && e.Property("CreatedAtUtc").CurrentValue == null)
                {
                    e.Property("CreatedAtUtc").CurrentValue = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data
{
    public class AssessmentConfigContext : DbContextBase
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public AssessmentConfigContext(DbContextOptions<AssessmentConfigContext> options) : base(options)
        {
        }
        public DbSet<ActivityEntity> Activities { get; set; }
        public DbSet<LtActivityEntity> LtActivities { get; set; }
        public DbSet<ActivityStatusTypeEntity> ActivityStatusTypes { get; set; }
        public DbSet<AlternativeEntity> Alternatives { get; set; }
        public DbSet<LtSurveyEntity> LtSurveys { get; set; }
        public DbSet<PeriodEntity> Periods { get; set; }
        public DbSet<ScaleEntity> Scales { get; set; }
        public DbSet<SurveyEntity> Surveys { get; set; }
        public DbSet<StatusTypeEntity> StatusTypes { get; set; }
        public DbSet<LtStatusTypeEntity> LtStatusTypes { get; set; }
        public DbSet<LtAlternativeEntity> LtAlternatives { get; set; }
        public DbSet<StatusTypeMappingEntity> StatusTypeMappingEntities { get; set; }

        /// <summary>
        /// TODO: Some entities do not belong to this domain, we will move to correct places
        /// </summary>
        public DbSet<LanguageEntity> LanguageEntities { get; set; }
        public DbSet<LevelLimitEntity> LevelLimitEntities { get; set; }
        public DbSet<LevelGroupEntity> LevelGroupEntitis { get; set; }
        public DbSet<LtLevelGroupEntity> LtLevelGroupEntities { get; set; }
        public DbSet<LtLevelLimitEntity> LtLevelLimitEntities { get; set; }
        public DbSet<LtOwnerColorEntity> LtOwnerColorEntities { get; set; }
        public DbSet<OwnerColorEntity> OwnerColorEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ActivityMap());
            modelBuilder.ApplyConfiguration(new ActivityStatusTypeMap());
            modelBuilder.ApplyConfiguration(new ActivityMap());
            modelBuilder.ApplyConfiguration(new ActivityStatusTypeMap());

            modelBuilder.ApplyConfiguration(new AlternativeMap());
            modelBuilder.ApplyConfiguration(new LanguageMap());
            modelBuilder.ApplyConfiguration(new LevelGroupEntityMap());
            modelBuilder.ApplyConfiguration(new LevelLimitEntityMap());

            modelBuilder.ApplyConfiguration(new LtActivityMap());
            modelBuilder.ApplyConfiguration(new LtAlternativeMap());
            modelBuilder.ApplyConfiguration(new LtLevelGroupEntityMap());
            modelBuilder.ApplyConfiguration(new LtLevelLimitEntityMap());

            modelBuilder.ApplyConfiguration(new LtOwnerColorMap());
            modelBuilder.ApplyConfiguration(new LtStatusTypeMap());
            modelBuilder.ApplyConfiguration(new LtSurveyMap());
            modelBuilder.ApplyConfiguration(new OwnerColorMap());

            modelBuilder.ApplyConfiguration(new PeriodMap());
            modelBuilder.ApplyConfiguration(new ScaleMap());
            modelBuilder.ApplyConfiguration(new StatusTypeMap());
            modelBuilder.ApplyConfiguration(new StatusTypeMappingMap());

            modelBuilder.ApplyConfiguration(new SurveyMap());
        }
    }
}

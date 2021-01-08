using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.OutboxPattern;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using CommentEntity = Microservice.StandaloneSurvey.Domain.Entities.Comment;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public class StandaloneSurveyDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public StandaloneSurveyDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public DbSet<Domain.Entities.StandaloneSurvey> Forms { get; set; }

        public DbSet<SurveyAnswer> FormAnswers { get; set; }

        public DbSet<SurveyQuestion> FormQuestions { get; set; }

        public DbSet<SurveyQuestionAnswer> FormQuestionAnswers { get; set; }

        public DbSet<SurveySection> FormSections { get; set; }

        public DbSet<SurveyQuestionAttachment> FormQuestionAttachments { get; set; }

        public DbSet<VersionTracking> VersionTrackings { get; set; }

        public DbSet<CommentEntity> Comments { get; set; }

        // Synced from csl
        public DbSet<SyncedCslCommunity> SyncedCslCommunities { get; set; }

        public DbSet<SyncedCslCommunityMember> SyncedCslCommunityMembers { get; set; }

        // Access control tables
        public DbSet<SyncedUser> Users { get; set; }

        public DbSet<HierarchyDepartment> HierarchyDepartments { get; set; }

        public DbSet<DepartmentTypeDepartment> DepartmentTypeDepartments { get; set; }

        public DbSet<DepartmentType> DepartmentTypes { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<AccessRight> AccessRights { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<SurveyParticipant> FormParticipant { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DbConnection != null)
            {
                optionsBuilder.UseSqlServer(DbConnection);
            }
            else
            {
                optionsBuilder.UseSqlServer(ChooseConnectionString());
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto apply configuration by convention.
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        private string ChooseConnectionString()
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            // We want to explicit code here so please do not use ? :
            if (string.IsNullOrEmpty(ConnectionString))
            {
                return _connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs());
            }

            return ConnectionString;
        }
    }
}

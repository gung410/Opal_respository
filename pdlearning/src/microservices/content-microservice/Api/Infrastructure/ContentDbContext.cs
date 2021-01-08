using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.InboxPattern.Entities;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Infrastructure
{
    // TODO: rename to ContentDbContext
    public class ContentDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public ContentDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public DbSet<DigitalContent> DigitalContents { get; set; }

        public DbSet<AttributionElement> AttributionElements { get; set; }

        public DbSet<VersionTracking> VersionTrackings { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<LearningTracking> LearningTrackings { get; set; }

        // Access control tables
        public DbSet<UserEntity> Users { get; set; }

        public DbSet<HierarchyDepartment> HierarchyDepartments { get; set; }

        public DbSet<DepartmentTypeDepartment> DepartmentTypeDepartments { get; set; }

        public DbSet<DepartmentType> DepartmentTypes { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<AccessRight> AccessRights { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<Chapter> Chapters { get; set; }

        public DbSet<ChapterAttachment> ChapterAttachments { get; set; }

        public DbSet<VideoComment> VideoComments { get; set; }

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

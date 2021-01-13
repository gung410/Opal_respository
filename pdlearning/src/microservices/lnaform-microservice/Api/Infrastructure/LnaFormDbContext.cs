using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.OutboxPattern;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using CommentEntity = Microservice.LnaForm.Domain.Entities.Comment;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Infrastructure
{
    public class LnaFormDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public LnaFormDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public DbSet<FormEntity> Forms { get; set; }

        public DbSet<FormAnswer> FormAnswers { get; set; }

        public DbSet<FormQuestion> FormQuestions { get; set; }

        public DbSet<FormQuestionAnswer> FormQuestionAnswers { get; set; }

        public DbSet<FormSection> FormSections { get; set; }

        public DbSet<FormQuestionAttachment> FormQuestionAttachments { get; set; }

        public DbSet<VersionTracking> VersionTrackings { get; set; }

        public DbSet<CommentEntity> Comments { get; set; }

        // Access control tables
        public DbSet<UserEntity> Users { get; set; }

        public DbSet<HierarchyDepartment> HierarchyDepartments { get; set; }

        public DbSet<DepartmentTypeDepartment> DepartmentTypeDepartments { get; set; }

        public DbSet<DepartmentType> DepartmentTypes { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<AccessRight> AccessRights { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<FormParticipant> FormParticipant { get; set; }

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
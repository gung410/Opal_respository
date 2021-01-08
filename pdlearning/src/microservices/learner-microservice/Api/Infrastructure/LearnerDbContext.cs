using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.InboxPattern.Entities;
using Conexus.Opal.OutboxPattern;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure
{
    public class LearnerDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public LearnerDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public DbSet<MyCourse> MyCourses { get; set; }

        public DbSet<LectureInMyCourse> LecturesInMyCourse { get; set; }

        public DbSet<CourseReview> CourseReviews { get; set; }

        public DbSet<UserBookmark> UserBookmarks { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<UserReview> UserReviews { get; set; }

        public DbSet<MyLearningPackage> MyLearningPackages { get; set; }

        public DbSet<MyAssignment> MyAssignments { get; set; }

        public DbSet<LearnerLearningPath> LearnerLearningPaths { get; set; }

        public DbSet<LearnerLearningPathCourse> LearnerLearningPathCourses { get; set; }

        public DbSet<LearnerUser> Users { get; set; }

        public DbSet<HierarchyDepartment> HierarchyDepartments { get; set; }

        public DbSet<DepartmentTypeDepartment> DepartmentTypeDepartments { get; set; }

        public DbSet<DepartmentType> DepartmentTypes { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<LearningTracking> LearningTrackings { get; set; }

        public DbSet<InboxMessage> InboxMessages { get; set; }

        public DbSet<DigitalContent> DigitalContents { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<MyOutstandingTask> MyOutstandingTasks { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<Form> Forms { get; set; }

        public DbSet<FormParticipant> FormParticipants { get; set; }

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

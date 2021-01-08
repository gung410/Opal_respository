using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure
{
    public class CourseDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public CourseDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public DbSet<CourseEntity> Course { get; set; }

        public DbSet<Section> Section { get; set; }

        public DbSet<Lecture> Lecture { get; set; }

        public DbSet<LectureContent> LectureContent { get; set; }

        public DbSet<Assignment> Assignment { get; set; }

        public DbSet<ParticipantAssignmentTrack> ParticipantAssignmentTrack { get; set; }

        public DbSet<ClassRun> ClassRun { get; set; }

        public DbSet<Session> Session { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<CommentTrack> CommentTrack { get; set; }

        public DbSet<AttendanceTracking> AttendanceTracking { get; set; }

        public DbSet<ClassRunInternalValue> ClassRunInternalValue { get; set; }

        public DbSet<CourseInternalValue> CourseInternalValue { get; set; }

        public DbSet<ECertificateTemplate> ECertificateTemplate { get; set; }

        public DbSet<LearningPath> LearningPath { get; set; }

        public DbSet<LearningPathCourse> LearningPathCourse { get; set; }

        public DbSet<Registration> Registration { get; set; }

        public DbSet<MetadataTag> MetadataTags { get; set; }

        public DbSet<CoursePlanningCycle> CoursePlanningCycle { get; set; }

        public DbSet<QuizAssignmentForm> QuizAssignmentForm { get; set; }

        public DbSet<QuizAssignmentFormQuestion> QuizAssignmentFormQuestion { get; set; }

        public DbSet<ParticipantAssignmentTrackQuizAnswer> ParticipantAssignmentTrackQuizAnswer { get; set; }

        public DbSet<ParticipantAssignmentTrackQuizQuestionAnswer> ParticipantAssignmentTrackQuizQuestionAnswer { get; set; }

        public DbSet<UserMetadata> UserMetadata { get; set; }

        public DbSet<UserSystemRole> UserSystemRole { get; set; }

        public DbSet<CourseCriteria> CourseCriteria { get; set; }

        public DbSet<DepartmentUnitTypeDepartment> DepartmentUnitTypeDepartment { get; set; }

        public DbSet<Announcement> Announcement { get; set; }

        public DbSet<AnnouncementTemplate> AnnouncementTemplate { get; set; }

        public DbSet<CommentViewPermission> CommentViewPermission { get; set; }

        public DbSet<RegistrationECertificate> RegistrationECertificate { get; set; }

        // Access control tables
        public DbSet<CourseUser> Users { get; set; }

        public DbSet<HierarchyDepartment> HierarchyDepartments { get; set; }

        public DbSet<DepartmentTypeDepartment> DepartmentTypeDepartments { get; set; }

        public DbSet<DepartmentType> DepartmentTypes { get; set; }

        public DbSet<CourseDepartment> Departments { get; set; }

        // Outbox pattern
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

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

            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto apply configuration by convention.
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

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

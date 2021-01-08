using Conexus.Opal.InboxPattern.Entities;
using Conexus.Opal.OutboxPattern;
using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure
{
    public class CalendarDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public CalendarDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        // Events tables
        public DbSet<EventEntity> Events { get; set; }

        // User-Personal event
        public DbSet<UserPersonalEvent> UserPersonalEvents { get; set; }

        // Community tables
        public DbSet<Community> Communities { get; set; }

        public DbSet<CommunityMembership> CommunityMemberships { get; set; }

        // Access sharing.
        public DbSet<TeamAccessSharing> TeamAccessSharings { get; set; }

        // Users
        public DbSet<CalendarUser> Users { get; set; }

        // CAM tables
        public DbSet<Course> Courses { get; set; }

        public DbSet<ClassRun> ClassRuns { get; set; }

        // Inbox pattern tables
        public DbSet<InboxMessage> InboxMessages { get; set; }

        // Outbox pattern tables
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

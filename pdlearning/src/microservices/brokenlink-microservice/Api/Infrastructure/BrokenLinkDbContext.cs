using Conexus.Opal.OutboxPattern;
using Microservice.BrokenLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.BrokenLink.Infrastructure
{
    public class BrokenLinkDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public BrokenLinkDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public DbSet<BrokenLinkReport> BrokenLinkReports { get; set; }

        public DbSet<ExtractedUrl> ExtractedUrls { get; set; }

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

using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data
{
    public class AssessmentContext : DbContextBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AssessmentContext(DbContextOptions<AssessmentContext> options) : base(options)
        {
        }
        /// <summary>
        /// Answers
        /// </summary>
        public DbSet<AnswerEntity> Answers { get; set; }
        public DbSet<ResultEntity> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AnswerMap());
            modelBuilder.ApplyConfiguration(new ResultMap());
        }
    }
}

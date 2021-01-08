using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserTypesConfiguration : BaseEntityTypeConfiguration<SAM_UserTypes>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserTypes> builder)
        {
            builder.HasKey(e => e.UserTypeId);

            builder.ToTable("sam_UserTypes", "staging");

            builder.Property(e => e.UserTypeId).HasMaxLength(64);

            builder.Property(e => e.ArchetypeId).HasMaxLength(64);

            builder.Property(e => e.ExtId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.MasterId).HasMaxLength(64);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.ParentId).HasMaxLength(64);
        }
    }
}

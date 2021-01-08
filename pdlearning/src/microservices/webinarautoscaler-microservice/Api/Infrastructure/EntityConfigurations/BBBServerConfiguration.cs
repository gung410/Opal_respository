using Microservice.WebinarAutoscaler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.WebinarAutoscaler.Infrastructure.EntityConfigurations
{
    public class BBBServerConfiguration : BaseEntityTypeConfiguration<BBBServer>
    {
        public override void Configure(EntityTypeBuilder<BBBServer> builder)
        {
            builder
                .Property(e => e.PrivateIp)
                .HasMaxLength(20);

            builder
                .Property(e => e.InstanceId)
                .HasMaxLength(100);
            builder
                .Property(e => e.TargetGroupArn)
                .HasMaxLength(255);

            builder
                .Property(e => e.RuleArn)
                .HasMaxLength(500);

            builder
                .HasIndex(e => e.InstanceId)
                .IsUnique(true);

            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.PrivateIp);
            builder.Property(e => e.Status).ConfigureForEnum();
        }
    }
}

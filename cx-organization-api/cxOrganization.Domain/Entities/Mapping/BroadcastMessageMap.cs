using System;
using System.Collections.Generic;
using System.Text.Json;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Entities.BroadcastMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities.Mapping
{
    public class BroadcastMessageMap : IEntityTypeConfiguration<BroadcastMessageEntity>
    {
        public void Configure(EntityTypeBuilder<BroadcastMessageEntity> builder)
        {
            builder.ToTable("BroadcastMessage", "dbo");
            builder.HasKey(t => new { t.BroadcastMessageId });
            builder.OwnsOne(t => t.Recipient)
                .Property(p => p.DepartmentIds)
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, default),
                    v => JsonSerializer.Deserialize<IEnumerable<int>>(v, default));
            builder.OwnsOne(t => t.Recipient)
                .Property(p => p.RoleIds)
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, default),
                    v => JsonSerializer.Deserialize<IEnumerable<int>>(v, default));
            builder.OwnsOne(t => t.Recipient)
                .Property(p => p.UserIds)
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, default),
                    v => JsonSerializer.Deserialize<IEnumerable<Guid>>(v, default));
            builder.OwnsOne(t => t.Recipient)
                .Property(p => p.GroupIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, default),
                    v => JsonSerializer.Deserialize<IEnumerable<int>>(v, default));
            builder.Property(t => t.Title).IsRequired(true);
            builder.Property(t => t.BroadcastContent).IsRequired(true);
            builder.Property(t => t.ValidFromDate).IsRequired(true);
            builder.Property(t => t.ValidToDate).IsRequired(true);
            builder.Property(t => t.Status).IsRequired(true);
            builder.Property(t => t.OwnerId).IsRequired(true);
            builder.Property(t => t.CreatedDate).IsRequired(true);
            builder.Property(t => t.TargetUserType).IsRequired(true);
            builder.Property(t => t.SendMode).IsRequired(true);
            builder.Property(t => t.MonthRepetition).IsRequired(true);
            builder.Property(t => t.DayRepetitions).HasConversion(
                                            v => JsonSerializer.Serialize(v, default),
                                            v => JsonSerializer.Deserialize<List<DayRepetition>>(v, default));
        }
    }
}

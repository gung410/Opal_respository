using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    public class UTUMap : IEntityTypeConfiguration<UTUEntity>
    {
        public void Configure(EntityTypeBuilder<UTUEntity> builder)
        {
            builder.HasKey(t => new { t.UserId, t.UserTypeId});
            builder.ToTable("UT_U", "org");
            builder.Property(t => t.UserId).HasColumnName("UserID");
            builder.Property(t => t.UserTypeId).HasColumnName("UserTypeID");

            builder.HasOne(t => t.UserType)
                    .WithMany(t => t.UT_Us)
                    .HasForeignKey(t => t.UserTypeId);

            builder.HasOne(t => t.User)
                    .WithMany(t => t.UT_Us)
                    .HasForeignKey(t => t.UserId);
        }
    }
}

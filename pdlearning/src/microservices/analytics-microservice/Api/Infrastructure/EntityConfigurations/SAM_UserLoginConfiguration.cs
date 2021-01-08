using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserLoginConfiguration : BaseEntityTypeConfiguration<SAM_UserLogin>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserLogin> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("sam_User_Logins", "staging");

            builder.Property(e => e.Id).HasColumnName("UserloginID").ValueGeneratedOnAdd();

            builder.Property(e => e.SessionId).HasColumnName("SessionID");

            builder.Property(e => e.UserId).HasColumnName("UserID");

            builder.Property(e => e.DepartmentId).HasColumnName("Departmentid");

            builder.HasOne(e => e.Sam_UserHistory)
                .WithMany(e => e.SamUserLogins)
                .HasForeignKey(e => e.UserHistoryId);
        }
    }
}

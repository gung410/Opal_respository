using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_DepartmentConfiguration : BaseEntityTypeConfiguration<SAM_Department>
    {
        public override void Configure(EntityTypeBuilder<SAM_Department> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("sam_Departments", "staging");

            builder.Property(e => e.Id).HasColumnName("DepartmentId").HasMaxLength(64);

            builder.Property(e => e.Adress).HasMaxLength(512);

            builder.Property(e => e.ArchetypeId).HasMaxLength(64);

            builder.Property(e => e.Hdid).HasColumnName("HDID");

            builder.Property(e => e.Name).HasMaxLength(256);

            builder.Property(e => e.ParentId)
                .HasColumnName("ParentID")
                .HasMaxLength(64);

            builder.Property(e => e.ParentIdD).HasColumnName("ParentID_D");

            builder.Property(e => e.Path).HasMaxLength(1024);

            builder.Property(e => e.PathNameHD)
                .IsRequired()
                .HasColumnName("PathName_H_D")
                .HasMaxLength(4000);

            builder.Property(e => e.TypeOfOrganizationUnits).HasMaxLength(64);

            builder.HasOne(d => d.Archetype)
                .WithMany(p => p.SamDepartments)
                .HasForeignKey(d => d.ArchetypeId)
                .HasConstraintName("FK__sam_Depar__Arche__62A57E71");

            builder.HasOne(d => d.EntityStatus)
                .WithMany(p => p.SamDepartments)
                .HasForeignKey(d => d.EntityStatusId)
                .HasConstraintName("FK__sam_Depar__Entit__648DC6E3");

            builder.HasOne(d => d.EntityStatusReason)
                .WithMany(p => p.SamDepartments)
                .HasForeignKey(d => d.EntityStatusReasonId)
                .HasConstraintName("FK__sam_Depar__Entit__6581EB1C");

            builder.HasOne(d => d.Parent)
                .WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__sam_Depar__Paren__6399A2AA");
        }
    }
}

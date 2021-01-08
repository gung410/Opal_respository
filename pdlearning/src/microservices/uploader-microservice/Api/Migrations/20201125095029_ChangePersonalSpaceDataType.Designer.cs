﻿// <auto-generated />
using System;
using Microservice.Uploader.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Uploader.Migrations
{
    [DbContext(typeof(UploaderDbContext))]
    [Migration("20201125095029_ChangePersonalSpaceDataType")]
    partial class ChangePersonalSpaceDataType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.DepartmentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentTypeID");

                    b.Property<string>("ExtId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtID");

                    b.HasKey("Id");

                    b.ToTable("DepartmentTypes");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.DepartmentTypeDepartment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentTypeID");

                    b.HasKey("Id");

                    b.ToTable("DepartmentTypeDepartments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.HierarchyDepartment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<int>("HierarchyDepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("HDID");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("ParentID");

                    b.Property<string>("Path")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.ToTable("HierarchyDepartments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OriginalUserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microservice.Uploader.Domain.Entities.PersonalFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileExtension")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("FileLocation")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<double>("FileSize")
                        .HasColumnType("float");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<Guid>("PersonalSpaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("PersonalFiles");
                });

            modelBuilder.Entity("Microservice.Uploader.Domain.Entities.PersonalSpace", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("TotalSpace")
                        .HasColumnType("float");

                    b.Property<double>("TotalUsed")
                        .HasColumnType("float");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("PersonalSpaces");
                });
#pragma warning restore 612, 618
        }
    }
}

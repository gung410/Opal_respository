using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Entities.BroadcastMessage;
using cxOrganization.Domain.Entities.Mapping;
using cxOrganization.Domain.Extensions;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain
{
    public class OrganizationDbContext : DbContextBase
    {
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options) : base(options)
        {

        }
        /// <summary>
        /// Gets or sets the customers.
        /// </summary>
        /// <value>The customers.</value>
        public DbSet<CustomerEntity> Customers { get; set; }
        /// <summary>
        /// Gets or sets the departments.
        /// </summary>
        /// <value>The departments.</value>
        public DbSet<DepartmentEntity> Departments { get; set; }
        /// <summary>
        /// Gets or sets the department groups.
        /// </summary>
        /// <value>The department groups.</value>
        public DbSet<DepartmentGroupEntity> DepartmentGroups { get; set; }
        /// <summary>
        /// Gets or sets the department types.
        /// </summary>
        /// <value>The department types.</value>
        public DbSet<DepartmentTypeEntity> DepartmentTypes { get; set; }
        /// <summary>
        /// Gets or sets the h_ d.
        /// </summary>
        /// <value>The h_ d.</value>
        public DbSet<HierarchyDepartmentEntity> H_D { get; set; }
        /// <summary>
        /// Gets or sets the hierarchies.
        /// </summary>
        /// <value>The hierarchies.</value>
        public DbSet<HierarchyEntity> Hierarchies { get; set; }
        /// <summary>
        /// Gets or sets the l t_ department group.
        /// </summary>
        /// <value>The l t_ department group.</value>
        public DbSet<LtDepartmentGroup> LtDepartmentGroup { get; set; }
        /// <summary>
        /// Gets or sets the type of the l t_ department.
        /// </summary>
        /// <value>The type of the l t_ department.</value>
        public DbSet<LtDepartmentTypeEntity> LtDepartmentType { get; set; }
        /// <summary>
        /// Gets or sets the type of the l t_ user.
        /// </summary>
        /// <value>The type of the l t_ user.</value>
        public DbSet<LtUserTypeEntity> LtUserType { get; set; }
        /// <summary>
        /// Gets or sets the owners.
        /// </summary>
        /// <value>The owners.</value>
        public DbSet<OwnerEntity> Owners { get; set; }
        /// <summary>
        /// Gets or sets the u_ d.
        /// </summary>
        /// <value>The u_ d.</value>
        public DbSet<UserDepartmentEntity> UD { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserGroupEntity> UserGroups { get; set; }
        public DbSet<UGMemberEntity> UserGroupUsers { get; set; }
        public DbSet<UserTypeEntity> UserTypes { get; set; }
        public DbSet<MemberRoleEntity> MemberRoles { get; set; }
        public DbSet<LtMemberRoleEntity> LT_MemberRoles { get; set; }
        public DbSet<LoginServiceEntity> LoginServices { get; set; }
        public DbSet<LtLoginService> LT_LoginService { get; set; }
        public DbSet<LoginServiceUserEntity> LoginService_User { get; set; }
        public DbSet<PropValueEntity> PropValues { get; set; }
        public DbSet<PropOptionEntity> PropOptionEntities { get; set; }
        public DbSet<PropPageEntity> PropPageEntities { get; set; }
        public DbSet<PropertyEntity> PropertyEntities { get; set; }
        public DbSet<LtPropOptionEntity> LtPropOptionEntities { get; set; }
        public DbSet<LtPropertyEntity> LtPropertyEntities { get; set; }
        public DbSet<LanguageEntity> LanguageEntities { get; set; }
        public DbSet<SiteParameterEntity> SiteParameters { get; set; }
        public DbSet<PeriodEntity> PeriodEntities { get; set; }
        public DbSet<LtPeriodEntity> LtPeriodEntities { get; set; }
        public DbSet<DGDEntity> DGDEntities { get; set; }
        public DbSet<BroadcastMessageEntity> BroadcastMessageEntities { get; set; }
        public DbSet<FileInfoEntity> FileInfoEntities { get; set; }
        public DbSet<DTDEntity> DTDEntities { get; set; }
        public DbSet<DTUGEntity> DTUGEntities { get; set; }
        public DbSet<UDUTEntity> UDUTEntities { get; set; }
        public DbSet<UTUEntity> UTUEntities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new DepartmentGroupMap());
            modelBuilder.ApplyConfiguration(new DepartmentTypeMap());
            modelBuilder.ApplyConfiguration(new DGDMap());

            modelBuilder.ApplyConfiguration(new DTDMap());
            modelBuilder.ApplyConfiguration(new DTUGMap());
            modelBuilder.ApplyConfiguration(new HierarchyDepartmentMap());
            modelBuilder.ApplyConfiguration(new HierarchyMap());

            modelBuilder.ApplyConfiguration(new LanguageMap());
            modelBuilder.ApplyConfiguration(new LoginServiceMap());
            modelBuilder.ApplyConfiguration(new LoginServiceUserMap());
            modelBuilder.ApplyConfiguration(new LtDepartmentGroupMap());

            modelBuilder.ApplyConfiguration(new LtDepartmentTypeMap());
            modelBuilder.ApplyConfiguration(new LtLoginServicesMap());
            modelBuilder.ApplyConfiguration(new LtMemberRoleMap());
            modelBuilder.ApplyConfiguration(new LtPeriodMap());

            modelBuilder.ApplyConfiguration(new LtUserGroupTypeMap());
            modelBuilder.ApplyConfiguration(new LtUserTypeMap());
            modelBuilder.ApplyConfiguration(new LtPropertyMap());
            modelBuilder.ApplyConfiguration(new LtPropOptionMap());

            modelBuilder.ApplyConfiguration(new MemberRoleMap());
            modelBuilder.ApplyConfiguration(new OwnerMap());
            modelBuilder.ApplyConfiguration(new PeriodMap());
            modelBuilder.ApplyConfiguration(new PropertyMap());

            modelBuilder.ApplyConfiguration(new PropOptionMap());
            modelBuilder.ApplyConfiguration(new PropPageMap());
            modelBuilder.ApplyConfiguration(new PropValueMap());
            modelBuilder.ApplyConfiguration(new SiteParameterMap());

            modelBuilder.ApplyConfiguration(new UDUTMap());
            modelBuilder.ApplyConfiguration(new UGMemberMap());
            modelBuilder.ApplyConfiguration(new UserDepartmentMap());
            modelBuilder.ApplyConfiguration(new UserGroupMap());

            modelBuilder.ApplyConfiguration(new UserGroupTypeMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new UserTypeMap());
            modelBuilder.ApplyConfiguration(new UTUMap());

            modelBuilder.ApplyConfiguration(new DepartmentMap());
            modelBuilder.ApplyConfiguration(new BroadcastMessageMap());
            modelBuilder.ApplyConfiguration(new FileInfoMap());

            modelBuilder.AddJsonValue();
            modelBuilder.AddJsonQuery();

        }
    }
}

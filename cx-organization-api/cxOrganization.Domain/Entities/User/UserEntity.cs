using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using cxPlatform.Core.Entities;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class User.
    /// </summary>
    [Serializable]
    public partial class UserEntity : EntityBase, IDynamicAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserEntity"/> class.
        /// </summary>
        public UserEntity()
        {
            U_D = new List<UserDepartmentEntity>();
            UT_Us = new List<UTUEntity>();
            UGMembers = new List<UGMemberEntity>();
        }

        /// <summary>
        /// Gets or sets the user Identifier.
        /// </summary>
        /// <value>The user Identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the department Identifier.
        /// </summary>
        /// <value>The department Identifier.</value>
        public int DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the role Identifier.
        /// </summary>
        /// <value>The role Identifier.</value>
        public int? RoleId { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        /// <value>The mobile.</value>
        public string Mobile { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the SSN.
        /// </summary>
        /// <value>The SSN.</value>
        public string SSN { get; set; }
        /// <summary>
        /// Gets or sets the SSN hash.
        /// </summary>
        /// <value>The SSN hash.</value>
        public string SSNHash { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the locked.
        /// </summary>
        /// <value>The locked.</value>
        public short Locked { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [change password].
        /// </summary>
        /// <value><c>true</c> if [change password]; otherwise, <c>false</c>.</value>
        public bool ChangePassword { get; set; }
        /// <summary>
        /// Gets or sets the hash password.
        /// </summary>
        /// <value>The hash password.</value>
        public string HashPassword { get; set; }
        /// <summary>
        /// Gets or sets the salt password.
        /// </summary>
        /// <value>The salt password.</value>
        public string SaltPassword { get; set; }
        /// <summary>
        /// Gets or sets the one time password.
        /// </summary>
        /// <value>The one time password.</value>
        public string OneTimePassword { get; set; }
        /// <summary>
        /// Gets or sets the otp expire time.
        /// </summary>
        /// <value>The otp expire time.</value>
        public DateTime? OTPExpireTime { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>The gender.</value>
        public short? Gender { get; set; }
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public int? CountryCode { get; set; }
        /// <summary>
        /// Gets or sets the Force User Login Again.
        /// </summary>
        /// <value>
        /// The Force User Login Again.
        /// </value>
        public bool ForceUserLoginAgain { get; set; }

        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// Gets or sets last updated by.
        /// </summary>
        /// <value>The last updated by.</value>
        public int? LastUpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the last synchronized.
        /// </summary>
        /// <value>The last synchronized.</value>
        public DateTime LastSynchronized { get; set; }
        /// <summary>
        /// Gets or sets the archetype Identifier.
        /// </summary>
        /// <value>The archetype Identifier.</value>
        public int? ArchetypeId { get; set; }
        /// <summary>
        /// Get or set the dymanic attributes in json format
        /// </summary>
        public string DynamicAttributes { get; set; }
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>The department.</value>
        public virtual DepartmentEntity Department { get; set; }
        /// <summary>
        /// Gets or sets the otp expire time.
        /// </summary>
        /// <value>The otp expire time.</value>
        public DateTime? EntityExpirationDate { get; set; }
        /// <summary>
        /// Gets or sets the entity active date.
        /// </summary>
        /// <value>The otp expire time.</value>
        public DateTime? EntityActiveDate { get; set; }
        /// <summary>
        /// Gets or sets the u_ d.
        /// </summary>
        /// <value>The u_ d.</value>
        public ICollection<UserDepartmentEntity> U_D { get; set; }
        /// <summary>
        /// Gets or sets the user types.
        /// </summary>
        /// <value>The user types.</value>
        //public virtual ICollection<UserTypeEntity> UserTypes { get; set; }
        /// <summary>
        /// Get or set the User group users.
        /// </summary>
        public virtual ICollection<UGMemberEntity> UGMembers { get; set; }

        /// <summary>
        /// Get or set the login service
        /// </summary>
        public virtual ICollection<LoginServiceUserEntity> LoginServiceUsers { get; set; }

        /// <summary>
        /// Gets or sets the user groups.
        /// </summary>
        /// <value>The user groups.</value>
        public ICollection<UserGroupEntity> UserGroups { get; set; }
        public ICollection<UTUEntity> UT_Us { get; set; }

        public bool IsInHdPath(string path)
        {
            if (Department == null || Department.H_D == null)
                return false;
            return Department.H_D.Any(x => x.Path.Contains($"\\{path}\\"));
        }


        public bool IsActiveFromNow()
        {
            if (EntityActiveDate == null)
                return true;
            return EntityActiveDate <= DateTime.Now;
        }


    }
}

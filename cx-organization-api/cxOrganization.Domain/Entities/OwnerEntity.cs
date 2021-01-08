using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class Owner.
    /// </summary>
    [Serializable]
    public class OwnerEntity
    {
        /// <summary>
        /// Gets or sets the owner Identifier.
        /// </summary>
        /// <value>The owner Identifier.</value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the report server.
        /// </summary>
        /// <value>The report server.</value>
        public string ReportServer { get; set; }
        /// <summary>
        /// Gets or sets the report database.
        /// </summary>
        /// <value>The report database.</value>
        public string ReportDB { get; set; }
        /// <summary>
        /// Gets or sets the main hierarchy Identifier.
        /// </summary>
        /// <value>The main hierarchy Identifier.</value>
        public int? MainHierarchyId { get; set; }
        /// <summary>
        /// Gets or sets the olap server.
        /// </summary>
        /// <value>The olap server.</value>
        public string OLAPServer { get; set; }
        /// <summary>
        /// Gets or sets the olapdb.
        /// </summary>
        /// <value>The olapdb.</value>
        public string OLAPDB { get; set; }
        /// <summary>
        /// Gets or sets the CSS.
        /// </summary>
        /// <value>The CSS.</value>
        public string Css { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the type of the login.
        /// </summary>
        /// <value>The type of the login.</value>
        public int LoginType { get; set; }
        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        /// <value>The logging.</value>
        public short Logging { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the OTP_LENGTH.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public int OTPLength { get; set; }
        /// <summary>
        /// Gets or sets the OTP_CHARACTERS.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public string OTPCharacters { get; set; }
        /// <summary>
        /// Gets or sets the OTP_LOWERCASE.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public bool OTPAllowLowercase { get; set; }
        /// <summary>
        /// Gets or sets the OTP_UPPERCASE.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public bool OTPAllowUppercase { get; set; }
        /// <summary>
        /// Gets or sets the Use OTP with Case Sensitive
        /// </summary>
        public bool UseOTPCaseSensitive { get; set; }
        /// <summary>
        /// Gets or sets the use hash password.
        /// </summary>
        /// <value>The use hash password.</value>
        public short UseHashPassword { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [use otp].
        /// </summary>
        /// <value><c>true</c> if [use otp]; otherwise, <c>false</c>.</value>
        public bool UseOTP { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [use otp].
        /// </summary>
        /// <value><c>true</c> if [use otp]; otherwise, <c>false</c>.</value>
        public int OTPDuration { get; set; }
        /// <summary>
        /// Gets or sets the default hash method.
        /// </summary>
        /// <value>
        /// The default hash method.
        /// </value>
        public int DefaultHashMethod { get; set; }
    }
}

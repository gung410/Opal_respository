namespace Thunder.Platform.Core.Context
{
    public static class CommonUserContextKeys
    {
        public const string TenantId = "TenantId";
        public const string RequestId = "RequestId";
        public const string OriginIp = "OriginIp";
        public const string Username = "Username";
        public const string UserFullName = "UserFullName";
        public const string UserId = "UserId";
        public const string ValidThunderSecretKey = "HasThunderSecretKey";
        public const string Environment = "Environment";
        public const string ProcessId = "ProcessId";
        public const string UserRoles = "UserRoles";
        public const string UserPermissions = "UserPermissions";

        /// <summary>
        /// Key to get Dictionary(string, ModulePermission) from userContext. Key is Permission Action.
        /// </summary>
        public const string UserPermissionsDic = "UserPermissionsDic";
    }
}

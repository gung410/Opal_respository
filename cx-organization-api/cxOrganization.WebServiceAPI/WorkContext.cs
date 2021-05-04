using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Extensions;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.Domain
{
    public class WorkContext : IAdvancedWorkContext
    {
        private readonly IHttpContextAccessor _httpContext;
        private string _clientId;
        private string _sub;
        private string _sourceIp;
        public WorkContext(IOptions<AppSettings> options, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            var setting = options?.Value;
            CurrentHdId = setting?.HDId != null ? setting.HDId : default(int);
        }


        public int CurrentUserId { get; set; }
        public int CurrentDepartmentId { get; set; }
        public int CurrentCustomerId { get; set; }
        public int CurrentOwnerId { get; set; }
        public int CurrentHdId { set; get; }
        public int CurrentLanguageId { set; get; }
        public bool IsEnableFiltercxToken { get; set; }
        public string CurrentLocale { get; set; }
        public string CorrelationId { get; set; }
        public string RequestId { get; set; }

        public string ClientId
        {
            get { return _clientId = _clientId ?? _httpContext?.HttpContext?.GetClientId(); }
            private set { _clientId = value; }
        }

        public string SourceIp
        {
            get { return _sourceIp = _sourceIp ?? _httpContext?.HttpContext?.GetClientRequestIP(); }
            private set { _sourceIp = value; }
        }
        public Locale FallBackLanguage { get; set; }
        public IList<Locale> CurrentLocales { get; set; }

        public string Sub
        {
            get
            {
                return _sub = _sub ??
                              _httpContext?.HttpContext.GetSub()
                              ?? UserIdCXID;
            }

            private set { _sub = value; }
        }

        public bool isInternalRequest
        {
            get
            {
                return string.IsNullOrEmpty(this.Sub);
            }
        }

        public string UserIdCXID { get; set; }
        public object CurrentHd { get; set; }
        public object CurrentUser { get; set; }
        public IList<UserRole> CurrentUserRoles { get; set; }
        public bool IsSelfAccess { get; set; }
        public IEnumerable<string> CurrentUserPermissionKeys { get; set; }
        public string OriginalTokenString => _httpContext.HttpContext?.Request.GetXOriginalAuth();

        

        public static WorkContext CopyFrom(IWorkContext sourceWorkContext)
        {
            return new WorkContext(null, null)
            {
                CurrentUserId = sourceWorkContext.CurrentUserId,
                CurrentDepartmentId = sourceWorkContext.CurrentDepartmentId,
                CurrentCustomerId = sourceWorkContext.CurrentCustomerId,
                CurrentOwnerId = sourceWorkContext.CurrentOwnerId,
                CurrentHdId = sourceWorkContext.CurrentHdId,
                CurrentLanguageId = sourceWorkContext.CurrentLanguageId,
                IsEnableFiltercxToken = sourceWorkContext.IsEnableFiltercxToken,
                CurrentLocale = sourceWorkContext.CurrentLocale,
                CorrelationId = sourceWorkContext.CorrelationId,
                RequestId = sourceWorkContext.RequestId,
                ClientId = sourceWorkContext.ClientId,
                SourceIp = sourceWorkContext.SourceIp,
                FallBackLanguage = sourceWorkContext.FallBackLanguage,
                CurrentLocales = sourceWorkContext.CurrentLocales == null
                    ? null
                    : new List<Locale>(sourceWorkContext.CurrentLocales),
                Sub = sourceWorkContext.Sub,
                UserIdCXID = sourceWorkContext.UserIdCXID,
                CurrentHd = sourceWorkContext.CurrentHd,
                CurrentUser = sourceWorkContext.CurrentUser,
                CurrentUserRoles = sourceWorkContext.CurrentUserRoles,
                IsSelfAccess = sourceWorkContext.IsSelfAccess,
            };

        }

        public bool HasPermission(ICollection<string> permissionKeys, LogicalOperator logicalOperator = LogicalOperator.OR)
        {
            if (CurrentUserPermissionKeys == null || permissionKeys == null) return false;

            if (logicalOperator == LogicalOperator.OR)
            {
                return CurrentUserPermissionKeys.Any(p => permissionKeys.Contains(p));
            }

            return CurrentUserPermissionKeys.Count(p => permissionKeys.Contains(p)) == permissionKeys.Count;
        }

        public bool HasPermission(string permissionKey)
        {
            return CurrentUserPermissionKeys is object
                && !string.IsNullOrEmpty(permissionKey)
                && CurrentUserPermissionKeys.Contains(permissionKey);
        }

       
    }
}

using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Extensions;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace cxOrganization.Domain
{
    public class WorkContext : IWorkContext
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
            get { return _sub = _sub ??
                                _httpContext?.HttpContext.GetSub() 
                                ?? UserIdCXID; }

            private set { _sub = value; }
        }

        public string UserIdCXID { get; set; }
        public object CurrentHd { get; set; }
        public object CurrentUser { get; set; }
        public IList<UserRole> CurrentUserRoles { get; set; }
        public bool IsSelfAccess { get; set; }

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
    }
}

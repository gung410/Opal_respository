using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services.Reports;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services.ExportService
{
    public class UserEventLogExportService : UserExportServiceBase<UserEventLogInfo>, IExportService<UserEventLogInfo>
    {
        public UserEventLogExportService(
            ILogger<UserManagementExportService> logger,
            IWorkContext workContext,
            IOptions<AppSettings> appSettingOptions,
            IDepartmentService departmentService,
            IDepartmentTypeRepository departmentTypeRepository,
            IUserRepository userRepository) : base(logger, workContext, appSettingOptions, userRepository,
            departmentService, departmentTypeRepository)
        {
            _logger = logger;
        }

        protected override void AddDataToExportDataTable(DataTable dataTableUser, IEnumerable<UserEventLogInfo> source, ExportOption exportOption)
        {
            var userPropertyInfos = typeof(UserGenericDto).GetProperties();
            var userEventLogInfoPropertyInfos = typeof(UserEventLogInfo).GetProperties();
            var userPropertyPrefix = $"{nameof(UserEventLogInfo.CreatedByUser)}.";

            var specialFieldValueMappings = GetSpecialFieldValueOnUserMappingFunctions();
            foreach (UserEventLogInfo item in source)
            {
                var row = dataTableUser.NewRow();
                var rowNumber = dataTableUser.Rows.Count + 1;
                for (int i = 0; i < dataTableUser.Columns.Count; i++)
                {
                    var column = dataTableUser.Columns[i];
                    var fieldName = column.ColumnName;

                    object fieldValue;
                    if (fieldName.StartsWith(userPropertyPrefix))
                    {
                        var userFieldName = fieldName.Remove(0, userPropertyPrefix.Length);
                        fieldValue = GetFieldValueFromUser(rowNumber, userFieldName, item.CreatedByUser, specialFieldValueMappings, userPropertyInfos, column.GetFormat(), exportOption);

                    }
                    else
                    {
                        fieldValue = GetFieldValueFromUserEventLogInfo(rowNumber, fieldName, item, userEventLogInfoPropertyInfos, column.GetFormat(),exportOption);

                    }

                    row[i] = fieldValue;
                }
                dataTableUser.Rows.Add(row);
            }
        }

        protected override string GetDefaultExportTitle()
        {
            return "User Audit Events";
        }

        protected object GetFieldValueFromUserEventLogInfo(int rowNumber, string fieldName, UserEventLogInfo item,
            PropertyInfo[] userEventLogPropertyInfos, string fieldFormat, ExportOption exportOption)
        {

            if (item == null) return null;
            if (fieldName == RecordTypeFieldName)
                return "User Account";

            if (fieldName == RowNumberFieldName)
                return rowNumber;

            var property = userEventLogPropertyInfos.FirstOrDefault(p =>
                string.Equals(fieldName, p.Name, StringComparison.CurrentCultureIgnoreCase));
            if (property != null)
            {
                return FormatObjectValue(property.GetValue(item), fieldFormat, exportOption);
            }


            return null;
        }


    }
}

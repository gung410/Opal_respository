using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services.Reports;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services.ExportService
{
    public class UserAccountDetailsExportService : UserExportServiceBase<UserAccountDetailsInfo>, IExportService<UserAccountDetailsInfo>
    {
        public UserAccountDetailsExportService(
            ILogger<UserManagementExportService> logger,
            IAdvancedWorkContext workContext,
            IOptions<AppSettings> appSettingOptions,
            IDepartmentService departmentService,
            IDepartmentTypeRepository departmentTypeRepository,
            IUserRepository userRepository) : base(logger, workContext, appSettingOptions, userRepository,
            departmentService, departmentTypeRepository)
        {
            _logger = logger;
        }


        protected override void AddDataToExportDataTable(DataTable dataTableUser, IEnumerable<UserAccountDetailsInfo> source, ExportOption exportOption)
        {
            var propertyInfos = typeof(UserAccountDetailsInfo).GetProperties();
            foreach (UserAccountDetailsInfo item in source)
            {
                var row = dataTableUser.NewRow();
                var rowNumber = dataTableUser.Rows.Count + 1;
                for (int i = 0; i < dataTableUser.Columns.Count; i++)
                {
                    var column = dataTableUser.Columns[i];
                    var fieldName = column.ColumnName;
                    var fieldValue = GetFieldValueFromUserAccountDetailsInfo(rowNumber, fieldName, item,  propertyInfos, column.GetFormat(), exportOption);

                    row[i] = fieldValue;
                }
                dataTableUser.Rows.Add(row);
            }
        }
        protected object GetFieldValueFromUserAccountDetailsInfo(int rowNumber, string fieldName, UserAccountDetailsInfo item,
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
        protected override string GetDefaultExportTitle()
        {
            return "User Account Details";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services.ExportService
{
    public class UserManagementExportService : UserExportServiceBase<UserGenericDto>, IExportService<UserGenericDto>
    {
        public UserManagementExportService(
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

        protected override void AddDataToExportDataTable(DataTable dataTableUser, IEnumerable<UserGenericDto> source, ExportOption exportOption)
        {
            var propertyInfos = typeof(UserGenericDto).GetProperties();
            var specialFieldValueMappings = GetSpecialFieldValueOnUserMappingFunctions();
            foreach (UserGenericDto item in source)
            {
                var row = dataTableUser.NewRow();
                var rowNumber = dataTableUser.Rows.Count + 1;
                for (int i = 0; i < dataTableUser.Columns.Count; i++)
                {
                    var column = dataTableUser.Columns[i];
                    var fieldName = column.ColumnName;
                    var fieldValue = GetFieldValueFromUser(rowNumber, fieldName, item, specialFieldValueMappings, propertyInfos, column.GetFormat(), exportOption);

                    row[i] = fieldValue;
                }
                dataTableUser.Rows.Add(row);
            }
        }

        protected override string GetDefaultExportTitle()
        {
            return "User Accounts";
        }
    }
}

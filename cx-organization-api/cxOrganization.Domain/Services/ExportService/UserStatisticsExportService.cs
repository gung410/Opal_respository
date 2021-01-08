using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services.Reports;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services.ExportService
{
    public class UserStatisticsExportService : UserExportServiceBase<UserStatisticsDto>, IExportService<UserStatisticsDto>
    {
        public UserStatisticsExportService(
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

        public override byte[] ExportDataToBytes(IList<UserStatisticsDto> source, ExportOption exportOption,
            IWorkContext currentWorkContext = null)
        {
            throw new NotSupportedException(
                $"Not support export collection of {typeof(UserStatisticsDto).Name} to bytes");
        }
        public override byte[] ExportDataToBytes(UserStatisticsDto source, ExportOption exportOption, IWorkContext currentWorkContext = null)
        {
            if (currentWorkContext != null)
                _workContext = currentWorkContext;

            exportOption = SetDefaultExportOptionValue(exportOption);

            var exportDataTables = ExportToDataTables(source, exportOption);
            if (exportDataTables == null) return null;

            if (exportOption.ExportType == ExportType.Excel)
                return ExcelExporter.ExportFromDataTables(exportOption, exportDataTables);
            return CsvExporter.ExportFromDataTables(exportOption, exportDataTables);

        }

        private List<DataTable> ExportToDataTables(UserStatisticsDto source, ExportOption exportOption)
        {
            var exportDataTables = new List<DataTable>();

            var dataTableInfos = CreateInfoRecordDataTable(exportOption);
            if (dataTableInfos != null)
            {
                exportDataTables.Add(dataTableInfos);
            }


            var dataTableExport = InitExportDataTable(exportOption);

            var columnStatistics = new DataColumn("Statistics", typeof(object)) {Caption = ""};
            var columnInfo = new DataColumn("Info", typeof(object)) { Caption = "" };
            dataTableExport.Columns.Add(columnStatistics);
            dataTableExport.Columns.Add(columnInfo);
            var hasRecordTypeColumn = dataTableExport.Columns.Contains(RecordTypeFieldName);
            var columnStatisticsOrdinal = hasRecordTypeColumn ? 1 : 0;
            columnStatistics.SetOrdinal(columnStatisticsOrdinal);
            columnInfo.SetOrdinal(columnStatisticsOrdinal + 1);
            foreach (var exportOptionVerticalExportField in exportOption.VerticalExportFields)
            {
                ExportFieldInfo fieldInfo = ExportHelper.ConvertToExportFieldInfo(exportOptionVerticalExportField.Value);
                var verticalCaption = "";
                var displayFormat = "";
                var isGroup = false;
                if (fieldInfo != null)
                {
                    verticalCaption = fieldInfo.Caption;
                    displayFormat = fieldInfo.DisplayFormat;
                    isGroup = fieldInfo.IsGroupField;
                }

                if (string.IsNullOrEmpty(verticalCaption))
                    verticalCaption = exportOptionVerticalExportField.Key;

                var verticalFieldName = exportOptionVerticalExportField.Key;

                var statisticsValuesInfoGroup = GetStatisticsValues(source, verticalFieldName);

                foreach (var statisticsValuesInfo in statisticsValuesInfoGroup)
                {

                    var dataRow = dataTableExport.NewRow();
                    var rowNumber = dataTableExport.Rows.Count + 1;
                    for (int i = 0; i < dataTableExport.Columns.Count; i++)
                    {
                        var column = dataTableExport.Columns[i];
                        var fieldName = column.ColumnName;
                        object fieldValue;
                        if (column.ColumnName == columnStatistics.ColumnName)
                        {
                            fieldValue = CellInfo.Create(verticalCaption, isGroup: isGroup);
                        }
                        else if(column.ColumnName == columnInfo.ColumnName)
                        {
                            fieldValue = CellInfo.Create(statisticsValuesInfo.Info);
                        }
                        else
                        {
                            fieldValue = column.ColumnName == columnStatistics.ColumnName
                                ? CellInfo.Create(verticalCaption, isGroup: isGroup)
                                : GetFieldValue(rowNumber, fieldName, statisticsValuesInfo.StatisticsValues,
                                    column.GetFormat(), exportOption);
                        }

                        dataRow[i] = fieldValue;
                    }

                    dataTableExport.Rows.Add(dataRow);
                }
            }

            exportDataTables.Add(dataTableExport);

            return exportDataTables;
        }

        private static List<(string Info, Dictionary<AccountType, int> StatisticsValues)> GetStatisticsValues(
            UserStatisticsDto source,
            string verticalFieldName)
        {
            var accountStatisticsPrefix = $"{nameof(UserStatisticsDto.AccountStatistics)}.";
            var eventStatisticsPrefix = $"{nameof(UserStatisticsDto.EventStatistics)}.";
            var onBoardingStatisticsPrefix = $"{nameof(UserStatisticsDto.OnBoardingStatistics)}.";
            if (verticalFieldName.StartsWith(accountStatisticsPrefix, StringComparison.CurrentCultureIgnoreCase))
            {
                if (source.AccountStatistics != null)
                {
                    var entityStatusName = verticalFieldName.Remove(0, accountStatisticsPrefix.Length);
                    if (Enum.TryParse(typeof(EntityStatusEnum), entityStatusName, true, out object entityStatusObj))
                    {
                        var entityStatus = (EntityStatusEnum) entityStatusObj;
                        var accountStatisticsOfStatus = source.AccountStatistics
                            .Where(a => a.Key.EntityStatus == entityStatus)
                            .Select(a => (a.Key.OrganizationTypeName, a.Value))
                            .ToList();
                        return accountStatisticsOfStatus;
                    }
                }

                return new List<(string Info, Dictionary<AccountType, int> StatisticsValues)> {("", null)};
            }

            Dictionary<AccountType, int> statisticsValues = null;

            if (verticalFieldName.StartsWith(eventStatisticsPrefix, StringComparison.CurrentCultureIgnoreCase))
            {
                if (source.EventStatistics != null)
                {
                    var eventStatisticsInfos = verticalFieldName.Remove(0, eventStatisticsPrefix.Length).Split('.');
                    var eventTypeName = eventStatisticsInfos[0];
                    if (Enum.TryParse(typeof(UserEventType), eventTypeName, true, out object entityStatusObj))
                    {
                        var eventType = (UserEventType) entityStatusObj;
                        if (source.EventStatistics.ContainsKey(eventType))
                        {
                            statisticsValues = new Dictionary<AccountType, int>();

                            var eventStatisticsValues = source.EventStatistics[eventType];
                            EventValueType eventValueType;

                            if (eventStatisticsInfos.Length > 1 && Enum.TryParse(typeof(EventValueType),
                                    eventStatisticsInfos[1], true, out object eventValueTypeObj))
                            {
                                eventValueType = (EventValueType) eventValueTypeObj;
                            }
                            else
                            {
                                eventValueType = EventValueType.NumberOfUniqueUsers;
                            }

                            var accountTypes = eventStatisticsValues.Keys.ToList();

                            foreach (var accountType in accountTypes)
                            {
                                eventStatisticsValues[accountType].TryGetValue(eventValueType, out var value);
                                statisticsValues.Add(accountType, value);
                            }
                        }
                    }
                }
            }
            else if (verticalFieldName.StartsWith(onBoardingStatisticsPrefix,
                StringComparison.CurrentCultureIgnoreCase))
            {
                if (source.OnBoardingStatistics != null)
                {
                    var onBoardingStatisticsPropertyName =
                        verticalFieldName.Remove(0, onBoardingStatisticsPrefix.Length).ToLower();

                    if (string.Equals(onBoardingStatisticsPropertyName, nameof(UserOnBoardingStatisticsDto.NotStarted),
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        statisticsValues = source.OnBoardingStatistics.NotStarted;
                    }
                    else if (string.Equals(onBoardingStatisticsPropertyName,
                        nameof(UserOnBoardingStatisticsDto.Started),
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        statisticsValues = source.OnBoardingStatistics.Started;
                    }
                    else if (string.Equals(onBoardingStatisticsPropertyName,
                        nameof(UserOnBoardingStatisticsDto.Completed),
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        statisticsValues = source.OnBoardingStatistics.Completed;
                    }
                }
            }

            return new List<(string Info, Dictionary<AccountType, int> StatisticsValues)>
            {
                ("", statisticsValues)
            };
        }

        protected object GetFieldValue(int rowNumber, string fieldName, Dictionary<AccountType, int> statisticsValues, string fieldFormat, ExportOption exportOption)
        {

            if (fieldName == RecordTypeFieldName)
                return null;

            if (fieldName == RowNumberFieldName)
                return rowNumber;

            if (statisticsValues == null) return null;

            if (Enum.TryParse(typeof(AccountType), fieldName,true, out object accountTypeObj))
            {
                if (statisticsValues.IsNullOrEmpty()) return null;
               
                var accountType = (AccountType) accountTypeObj;
                if (statisticsValues.ContainsKey(accountType))
                    return FormatObjectValue(statisticsValues[accountType], fieldFormat, exportOption);
            }

           


            return null;
        }
        protected override void AddDataToExportDataTable(DataTable dataTableUser, IEnumerable<UserStatisticsDto> source, ExportOption exportOption)
        {
            //DO NOTHING HERE
        }

        protected override string GetDefaultExportTitle()
        {
            return "User Statistics";
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using cxOrganization.Client.ConexusBase;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services.ExportService
{
    public abstract class UserExportServiceBase<T>
    {
        protected ILogger _logger;
        protected IAdvancedWorkContext _workContext;
        protected readonly IUserRepository _userRepository;
        protected readonly IDepartmentService _departmentService;
        protected readonly IDepartmentTypeRepository _departmentTypeRepository;
        protected readonly AppSettings _appSettings;

        protected List<HierachyDepartmentIdentityDto> _currentUserDepartmentHierarchy;
        protected Dictionary<int, string> _departmentNameHierarchyCache;
        protected Dictionary<int, string> _departmentTypesCache;

        protected const string RecordTypeFieldName = "@RecordType";
        protected const string RowNumberFieldName = "@RowNumber";


        protected UserExportServiceBase(ILogger logger, 
            IAdvancedWorkContext workContext,
            IOptions<AppSettings> appSettingOptions,
            IUserRepository userRepository,
            IDepartmentService departmentService,
            IDepartmentTypeRepository departmentTypeRepository)
        {
            _logger = logger;
            _workContext = workContext;
            _appSettings = appSettingOptions.Value;
            _userRepository = userRepository;
            _departmentService = departmentService;
            _departmentTypeRepository = departmentTypeRepository;

        }

        public virtual byte[] ExportDataToBytes(IList<T> source, ExportOption exportOption, IAdvancedWorkContext currentWorkContext = null)
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
        public virtual byte[] ExportDataToBytes(IDictionary<string, List<T>> source, ExportOption exportOption, IAdvancedWorkContext currentWorkContext = null)
        {
            if (currentWorkContext != null)
                _workContext = currentWorkContext;

            exportOption = SetDefaultExportOptionValue(exportOption);

            var exportToDataSets = ExportToDataSets(source, exportOption);
            if (exportToDataSets == null) return null;

            if (exportOption.ExportType == ExportType.Excel)
                return ExcelExporter.ExportFromDataSets(exportOption, exportToDataSets);
            return CsvExporter.ExportFromDataSets(exportOption, exportToDataSets);
        }
        public virtual byte[] ExportDataToBytes(T source, ExportOption exportOption, IAdvancedWorkContext currentWorkContext = null)
        {
            return ExportDataToBytes(new List<T> {source}, exportOption, currentWorkContext);
        }
        protected abstract void AddDataToExportDataTable(DataTable dataTableUser, IEnumerable<T> source, ExportOption exportOption);
        protected abstract string GetDefaultExportTitle();

        protected object GetFieldValueFromUser<TUser>( int rowNumber, string fieldName, TUser item, Dictionary<string, Func<UserDtoBase, object>> specialFieldValueMappings, PropertyInfo[] userGenericDtoPropertyInfos, string fieldFormat, ExportOption exportOption) where TUser : UserDtoBase
        {

            if (item == null) return null;
            
            if (fieldName == RecordTypeFieldName)
                return "User Account";

            if (fieldName == RowNumberFieldName)
                return rowNumber;

            if (specialFieldValueMappings.TryGetValue(fieldName, out var funcGetFieldValue))
            {
                return FormatObjectValue(funcGetFieldValue(item), fieldFormat, exportOption);
            }

            var property = userGenericDtoPropertyInfos.FirstOrDefault(p => string.Equals(fieldName, p.Name, StringComparison.CurrentCultureIgnoreCase));
            if (property != null)
            {
                return FormatObjectValue(property.GetValue(item), fieldFormat, exportOption);
            }

            if (item.CustomData != null && item.CustomData.TryGetValue(fieldName, out object customValue))
            {
                return FormatObjectValue(customValue, fieldFormat, exportOption);
            }

            if (item.JsonDynamicAttributes != null && item.JsonDynamicAttributes.TryGetValue(fieldName, out var dynamicValue))
            {
                return FormatObjectValue(dynamicValue, fieldFormat, exportOption);
            }

            return null;
        }

        protected CellInfo FormatObjectValue(object value, string fieldFormat, ExportOption exportOption)
        {
            if (value is ICollection collection)
            {
                var localizeDtos = collection.OfType<ILocalizedDto>().ToList();
                if (localizeDtos.Count > 0)
                {
                    return CellInfo.Create(GetLocalizedText(localizeDtos, "Name"));
                }

                return CellInfo.Create(string.Join(", ", collection.Cast<object>()));
            }

            if (value is DateTime)
            {
                var dateTimeFormat = !string.IsNullOrEmpty(fieldFormat) ? fieldFormat :
                    (!string.IsNullOrEmpty(exportOption.DateTimeFormat) ? exportOption.DateTimeFormat : _appSettings.DateTimeDisplayFormat);

                var dateTimeValue = (DateTime)value;
                var timeZoneOffset = (exportOption.TimeZoneOffset ?? _appSettings.TimeZoneOffset) ?? 0;
                dateTimeValue = dateTimeValue.ToUniversalTime().AddHours(timeZoneOffset);

                return CellInfo.Create(dateTimeValue.ToString(dateTimeFormat), dateTimeFormat);
            }

            return CellInfo.Create(value, fieldFormat);

        }
        protected ExportOption SetDefaultExportOptionValue(ExportOption exportOption)
        {
            if (exportOption == null)
            {
                exportOption = new ExportOption
                {
                    CsvDelimiter = ","
                };
            }

            if (exportOption.ExportFields == null || exportOption.ExportFields.Count == 0)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_CUSTOM,
                    "ExportFields is required when exporting");
            }

            if (string.IsNullOrEmpty(exportOption.DateTimeFormat))
            {
                if (!string.IsNullOrEmpty(_appSettings.DateTimeDisplayFormat))
                {
                    exportOption.DateTimeFormat = _appSettings.DateTimeDisplayFormat;
                }
                else
                {
                    var locale = _workContext.CurrentLocale;
                    if (string.IsNullOrEmpty(locale)) locale = _appSettings.FallBackLanguageCode;
                    var culture = CultureInfo.GetCultureInfo(locale);
                    exportOption.DateTimeFormat = $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}";
                }

            }

            if (string.IsNullOrEmpty(exportOption.ExportTitle))
            {
                exportOption.ExportTitle = GetDefaultExportTitle();
            }
            return exportOption;
        }
        private UserEntity GetCurrentUser()
        {
            UserEntity currentUser = null;
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {
                currentUser = _userRepository.GetUsers(_workContext.CurrentOwnerId,
                    customerIds: new List<int> { _workContext.CurrentCustomerId },
                    extIds: new List<string> { _workContext.Sub }).Items.FirstOrDefault();
                if (currentUser == null)
                {
                    _logger.LogError(
                        $"Unable to get user from login service claim with sub '{_workContext.CurrentUserId}'");
                }
            }
            else if (_workContext.CurrentUserId > 0)
            {
                currentUser = _userRepository.GetById(_workContext.CurrentUserId);
                if (currentUser == null)
                {
                    _logger.LogError($"Unable to get user with id {_workContext.CurrentUserId} from work context");
                }
            }
            else
            {
                _logger.LogError($"Unable to get user from work context since missing identifier information");
            }

            return currentUser;
        }
        private  DataTable CreateSummaryCountTotal(ExportOption exportOption, DataTable dataTableUser)
        {
            var dataTableCountTotal = ExportHelper.CreateSummaryTable(null);
            dataTableCountTotal.SetFlagToHideHeader();
           
            if (exportOption.ShowRowNumber == true)
            {
                AddRowNumberColumn(dataTableCountTotal, exportOption.RowNumberColumnCaption, true);
            }
            if (exportOption.ShowRecordType == true)
            {
                AddRecordTypeColumn(dataTableCountTotal);
            }

            dataTableCountTotal.Columns.Add("DisplayText");
            dataTableCountTotal.Columns.Add("CountValue", typeof(double));

            var totalRow = dataTableCountTotal.NewRow();
            if (exportOption.ShowRecordType == true)
            {
                totalRow[RecordTypeFieldName] = "Count total";
            }

            var displayText = exportOption.SummaryOption.CountTotalDisplayText;
            if (!string.IsNullOrEmpty(displayText))
                displayText = "Total number of accounts";

            totalRow["DisplayText"] = displayText;
            totalRow["CountValue"] = dataTableUser.Rows.Count;

            dataTableCountTotal.Rows.Add(totalRow);
            return dataTableCountTotal;
        }

        private DataColumn AddRecordTypeColumn(DataTable dataTable)
        {
            var column = dataTable.Columns.Add(RecordTypeFieldName);
            column.Caption = string.Empty;
            return column;
        }

        private DataColumn AddRowNumberColumn(DataTable dataTable, string caption, bool hideCaption = false)
        {
            var column = dataTable.Columns.Add(RowNumberFieldName, typeof(int));
            column.Caption = hideCaption ? string.Empty : (string.IsNullOrEmpty(caption) ? "S/N" : caption);
            return column;
        }


        private string GetLocalizedText<TData>(IEnumerable<TData> localizedDtos, string fieldName) where TData : ILocalizedDto
        {
            var text = string.Join(", ", localizedDtos.Where(t => t.LocalizedData != null && t.LocalizedData.Any())
                .Select(t => t.LocalizedData.GetLocalizedText(fieldName, _workContext.CurrentLocale)).Distinct());
            return text;
        }
        private string GetTextValue(Dictionary<string, object> source, string key, string fieldName)
        {
            if (source.TryGetValue(key, out var usertypes))
            {

                if (usertypes is IEnumerable<ILocalizedDto>)
                {
                    var userTypeList = (IEnumerable<ILocalizedDto>)usertypes;
                    var persionalGroupArray = userTypeList.Where(t => t.LocalizedData != null && t.LocalizedData.Any())
                        .Select(t => t.LocalizedData.GetLocalizedText(fieldName, _workContext.CurrentLocale))
                        .ToArray();

                    return string.Join(", ", persionalGroupArray);
                }
            }
            return string.Empty;
        }
        private List<DataTable> CreateUserAccountDetails(ExportOption exportOption, DataTable dataTableUser)
        {

            var summaryTables = new List<DataTable>();
            var dataTableCountTotal = exportOption.SummaryOption.CountTotal
                ? CreateSummaryCountTotal(exportOption, dataTableUser)
                : null;

            var dataTableCountByField = exportOption.SummaryOption.ShouldCountByField()
                ? CreateSummaryCountByField(
                    exportOption, dataTableUser)
                : null;
            if (dataTableCountByField != null) summaryTables.Add(dataTableCountByField);

            if (dataTableCountTotal != null)
            {
                if (exportOption.SummaryOption.ShowTotalBeforeCountByField)
                {
                    summaryTables.Insert(0, dataTableCountTotal);
                }
                else
                {
                    summaryTables.Add(dataTableCountTotal);
                }
            }

            return summaryTables;
        }

        private DataTable CreateSummaryCountByField(ExportOption exportOption, DataTable dataTableUser)
        {
            var couldByFieldName = exportOption.SummaryOption.CountByField;
            var couldByFieldNameCaption = GetExportFieldInfo(couldByFieldName, exportOption.ExportFields)?.Caption;
            if (string.IsNullOrEmpty(couldByFieldNameCaption)) couldByFieldNameCaption = couldByFieldName;

            var countByFieldValueCaption = exportOption.SummaryOption.CountByFieldValueCaption;
            if (string.IsNullOrEmpty(countByFieldValueCaption))
                countByFieldValueCaption = $"Total number accounts (each {couldByFieldNameCaption})";

            var dataTableCountByField = ExportHelper.CreateSummaryTable($"Count User Accounts By {couldByFieldName}");

            var columnRecordTypeIndex = -1;

            if (exportOption.ShowRowNumber == true)
            {
                AddRowNumberColumn(dataTableCountByField, exportOption.RowNumberColumnCaption, true);
            }
            if (exportOption.ShowRecordType == true)
            {
                var recordTypeColumn = AddRecordTypeColumn(dataTableCountByField);
                columnRecordTypeIndex = recordTypeColumn.Ordinal;
            }
          
            dataTableCountByField.Columns.AddRange(new[]
            {
                new DataColumn(couldByFieldName) {Caption = couldByFieldNameCaption},
                new DataColumn("CountValue", typeof(double)) {Caption = countByFieldValueCaption},
            });

            var columnCountByFieldIndex = dataTableCountByField.Columns[couldByFieldName];
            var columnCountByFieldValueIndex = dataTableCountByField.Columns["CountValue"];

            var groupsByCountFields = dataTableUser.Rows.OfType<DataRow>()
                .GroupBy(r => r[couldByFieldName].ToString())
                .OrderBy(r => r.Key);
            foreach (var groupsByCountField in groupsByCountFields)
            {
                var summaryRow = dataTableCountByField.NewRow();
                if (columnRecordTypeIndex >= 0)
                {
                    summaryRow[columnRecordTypeIndex] = $"Count by {couldByFieldNameCaption}";
                }

                summaryRow[columnCountByFieldIndex] = groupsByCountField.Key;
                summaryRow[columnCountByFieldValueIndex] = groupsByCountField.Count();

                dataTableCountByField.Rows.Add(summaryRow);
            }

            return dataTableCountByField;
        }
        private static ExportFieldInfo GetExportFieldInfo(string fieldName, Dictionary<string, dynamic> exportFields)
        {

            var value = exportFields[fieldName];
            return ExportHelper.ConvertToExportFieldInfo(value);
        }

        private List<DataTable> ExportToDataTables(IEnumerable<T> source, ExportOption exportOption)
        {

            //If this method is call , we need to pass original work context to this

            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return null;
            }

            if (exportOption.ExportFields.ContainsKey(ExportHelper.OrganizationHierarchyFieldName))
            {
                _currentUserDepartmentHierarchy = _departmentService.GetDepartmentHierachyDepartmentIdentities(
                    departmentId: currentUser.DepartmentId,
                    customerIds: new List<int> {_workContext.CurrentCustomerId},
                    ownerId: _workContext.CurrentOwnerId,
                    includeChildrenHDs: true);

                if (_currentUserDepartmentHierarchy == null)
                {
                    _logger.LogWarning(
                        $"Unable to find current department {currentUser.DepartmentId} of the user with EXTID {_workContext.Sub}");
                    return null;
                }
            }

            var exportDataTables = new List<DataTable>();


            var dataTableInfos = CreateInfoRecordDataTable(exportOption);
            if (dataTableInfos != null)
            {
                exportDataTables.Add(dataTableInfos);
            }

            var dataTableUser = InitExportDataTable(exportOption);

            AddDataToExportDataTable(dataTableUser, source, exportOption);

            exportDataTables.Add(dataTableUser);

            if (exportOption.SummaryPosition != SummaryPosition.None && exportOption.SummaryOption != null)
            {
                var summaryTables = CreateUserAccountDetails(exportOption, dataTableUser);
                exportDataTables.AddRange(summaryTables);

            }

            return exportDataTables;
        }
        private List<DataSet> ExportToDataSets(IDictionary<string, List<T>> source, ExportOption exportOption)
        {

            //If this method is call , we need to pass original work context to this

            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return null;
            }

            if (exportOption.ExportFields.ContainsKey(ExportHelper.OrganizationHierarchyFieldName))
            {
                _currentUserDepartmentHierarchy = _departmentService.GetDepartmentHierachyDepartmentIdentities(
                    departmentId: currentUser.DepartmentId,
                    customerIds: new List<int> {_workContext.CurrentCustomerId},
                    ownerId: _workContext.CurrentOwnerId,
                    includeChildrenHDs: true);

                if (_currentUserDepartmentHierarchy == null)
                {
                    _logger.LogWarning(
                        $"Unable to find current department {currentUser.DepartmentId} of the user with EXTID {_workContext.Sub}");
                    return null;
                }
            }

            var exportDataSets = new List<DataSet>();

            var dataTableInfoTemplate = CreateInfoRecordDataTable(exportOption);
            var dataTableUserTemplate = InitExportDataTable(exportOption);

            foreach (var itemSource in source)
            {
                var dataSet = new DataSet(itemSource.Key);

                if (dataTableInfoTemplate != null)
                {
                    dataSet.Tables.Add(dataTableInfoTemplate.Copy());
                }

                var dataTableUser = dataTableUserTemplate.Clone();
                dataTableUser.TableName = itemSource.Key;
                AddDataToExportDataTable(dataTableUser, itemSource.Value, exportOption);
                dataSet.Tables.Add(dataTableUser);

                if (exportOption.SummaryPosition != SummaryPosition.None && exportOption.SummaryOption != null)
                {
                    var summaryTables = CreateUserAccountDetails(exportOption, dataTableUser);
                    dataSet.Tables.AddRange(summaryTables.ToArray());

                }

                exportDataSets.Add(dataSet);
            }

            return exportDataSets;
        }

        protected DataTable CreateInfoRecordDataTable(ExportOption exportOption)
        {
            if (!exportOption.InfoRecords.IsNullOrEmpty())
            {
                var dataTableInfos = new DataTable();
                dataTableInfos.Columns.Add();
                dataTableInfos.Columns.Add("infoValue", typeof(object));
                dataTableInfos.SetFlagToHideHeader();

                foreach (var info in exportOption.InfoRecords)
                {
                    var value = FormatObjectValue(info.Value, info.Format, exportOption);
                    dataTableInfos.Rows.Add(info.Caption, value);
                }

                dataTableInfos.Rows.Add();
                return dataTableInfos;
            }
            return null;
        }

        protected DataTable InitExportDataTable(ExportOption exportOption)
        {
            var fieldsMappings = exportOption.ExportFields;

            var shouldCountByField = exportOption.SummaryOption != null && exportOption.SummaryOption.ShouldCountByField();

            if (shouldCountByField && !fieldsMappings.ContainsKey(exportOption.SummaryOption.CountByField))
            {
                //If the field for counting is not given, we add here for to get data
                fieldsMappings.Add(exportOption.SummaryOption.CountByField, null);

            }

            var dataTableExport = new DataTable(exportOption.ExportTitle);
            if (exportOption.ShowRowNumber == true)
            {
                AddRowNumberColumn(dataTableExport, exportOption.RowNumberColumnCaption);
            }

            if (exportOption.ShowRecordType == true)
            {
                AddRecordTypeColumn(dataTableExport);
            }
          
            foreach (var fieldMapping in fieldsMappings)
            {
                ExportFieldInfo fieldInfo = ExportHelper.ConvertToExportFieldInfo(fieldMapping.Value);
                var caption = "";
                var displayFormat = "";
                if (fieldInfo != null)
                {
                    caption = fieldInfo.Caption;
                    displayFormat = fieldInfo.DisplayFormat;
                }

                if (string.IsNullOrEmpty(caption))
                    caption = fieldMapping.Key;

                var column = new DataColumn(fieldMapping.Key, typeof(object)) {Caption = caption};

                if (!string.IsNullOrEmpty(displayFormat))
                {
                    column.SetFormat(displayFormat);
                }

                dataTableExport.Columns.Add(column);
            }

            return dataTableExport;
        }

        protected Dictionary<string, Func<UserDtoBase, object>> GetSpecialFieldValueOnUserMappingFunctions()
        {
            return new Dictionary<string, Func<UserDtoBase, object>>(StringComparer.CurrentCultureIgnoreCase)
            {
                {"StatusId", GetStatusId},
                {ExportHelper.OrganizationHierarchyFieldName, GetDepartmentHierarchy},
                {ExportHelper.OrganizationTypesFieldName, GetDepartmentType},
                {nameof(UserDtoBase.Gender), GetGender},
                {nameof(UserDtoBase.EntityStatus.ExpirationDate), GetExpirationDate}
            };

        }

        private object GetStatusId(UserDtoBase user)
        {
            return user.EntityStatus.StatusId;
        }
        private object GetExpirationDate(UserDtoBase user)
        {
            return user.EntityStatus.ExpirationDate; 
        }

        private object GetGender(UserDtoBase user)
        {
            return user.Gender.HasValue ? (Gender)user.Gender.Value : Gender.Unknown;
        }

        private string GetDepartmentType(UserDtoBase user)
        {
            if (_departmentTypesCache == null)
                _departmentTypesCache = new Dictionary<int, string>();
            var departmentId = user.GetParentDepartmentId();

            if (_departmentTypesCache.TryGetValue(departmentId, out var value))
                return value;

            var departmentTypes = _departmentTypeRepository.GetDepartmentTypes(ownerId: _workContext.CurrentOwnerId, departmentIds: new List<int> { departmentId });
            var departmentTypenames = GetLtDepartmentTypeText(departmentTypes, _workContext.CurrentLanguageId);
            string typeNames = String.Join(", ", departmentTypenames.ToArray());
            _departmentTypesCache.Add(departmentId, typeNames);
            return typeNames;
        }

        private IList<string> GetLtDepartmentTypeText(List<DepartmentTypeEntity> departmentTypes, int currentLanguageId)
        {
            if (departmentTypes == null && !departmentTypes.Any())
                return Array.Empty<string>();

            IList<string> texts = new List<string>();
            foreach (var departmenttype in departmentTypes)
            {
                if (departmenttype.LT_DepartmentType != null && departmenttype.LT_DepartmentType.Any())
                {
                    var ltText = departmenttype.LT_DepartmentType.FirstOrDefault(t => t.LanguageId == currentLanguageId);
                    if (ltText != null)
                        texts.Add(ltText.Name);
                    continue;
                }
                else
                    continue;
            }
            return texts;
        }

        private string GetDepartmentHierarchy(UserDtoBase user)
        {
            if (_departmentNameHierarchyCache == null)
                _departmentNameHierarchyCache = new Dictionary<int, string>();

            var departmentId = user.GetParentDepartmentId();

            if (_departmentNameHierarchyCache.TryGetValue(departmentId, out var value))
                return value;
            var departmentNames = new List<string>();
            var finishBuildingPath = false;
            var departmentIdTemp = departmentId;

            while (!finishBuildingPath)
            {
                if (departmentIdTemp <= 0)
                {
                    finishBuildingPath = true;
                    continue;
                }
                var target = _currentUserDepartmentHierarchy.FirstOrDefault(t => t.Identity.Id == departmentIdTemp);
                if (target == null)
                {
                    finishBuildingPath = true;
                    continue;
                }

                departmentNames.Insert(0, target.DepartmentName);
                departmentIdTemp = target.ParentDepartmentId;
            }

            var path = string.Join("/", departmentNames.ToArray());
            _departmentNameHierarchyCache.Add(departmentId, path);

            return path;
        }


    }
}

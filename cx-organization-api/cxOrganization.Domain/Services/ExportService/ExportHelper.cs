using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Services.Reports;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using Newtonsoft.Json.Linq;

namespace cxOrganization.Domain.Services.ExportService
{
    public static class ExportHelper
    {

        public static readonly string PersonnelGroupFieldName =
            $"{ArchetypeEnum.PersonnelGroup.ToString().ToLowerFirstLetter()}s";

        public static readonly string SystemRoleFieldName =
            $"{ArchetypeEnum.SystemRole.ToString().ToLowerFirstLetter()}s";

        public static readonly string CareerPathFieldName =
            $"{ArchetypeEnum.CareerPath.ToString().ToLowerFirstLetter()}s";

        public static readonly string OrganizationHierarchyFieldName = "organizationHierarchy";
        public static readonly string OrganizationTypesFieldName = "organizationtypes";

        public static string[] NeedDepartmentFields = new[]
        {
            nameof(UserGenericDto.DepartmentName),
            nameof(UserGenericDto.DepartmentAddress),
            $"{nameof(UserEventLogInfo.CreatedByUser)}.{nameof(UserGenericDto.DepartmentName)}",
            $"{nameof(UserEventLogInfo.CreatedByUser)}.{nameof(UserGenericDto.DepartmentAddress)}",
        };

        public static string[] NeedRoleFields = new[]
        {
            nameof(UserGenericDto.Roles),
            ExportHelper.CareerPathFieldName,
            ExportHelper.PersonnelGroupFieldName,
            ExportHelper.SystemRoleFieldName,
            $"{nameof(UserEventLogInfo.CreatedByUser)}.{nameof(UserGenericDto.Roles)}",
            $"{nameof(UserEventLogInfo.CreatedByUser)}.{ExportHelper.CareerPathFieldName}",
            $"{nameof(UserEventLogInfo.CreatedByUser)}.{ExportHelper.PersonnelGroupFieldName}",
            $"{nameof(UserEventLogInfo.CreatedByUser)}.{ExportHelper.SystemRoleFieldName}"
        };


        public static Dictionary<string, dynamic> DefaultExportUserFieldMappings =
            new Dictionary<string, dynamic>
            {
                {nameof(UserGenericDto.FirstName), null},
                {nameof(UserGenericDto.LastName), null},
                {nameof(UserGenericDto.Roles), null},
                {"StatusId", null},
                {nameof(UserGenericDto.Gender), null},
                {nameof(UserGenericDto.EmailAddress), null},
                {nameof(UserGenericDto.MobileCountryCode), null},
                {nameof(UserGenericDto.MobileNumber), null},
                {ExportHelper.PersonnelGroupFieldName, null},
                {ExportHelper.SystemRoleFieldName, null},
                {ExportHelper.CareerPathFieldName, null},
                {"designation", null},
                {ExportHelper.OrganizationHierarchyFieldName, null},
                {nameof(UserGenericDto.Created), null},
                {nameof(UserGenericDto.EntityStatus.ExpirationDate), null},
                {ExportHelper.OrganizationTypesFieldName, null},
                {UserJsonDynamicAttributeName.FirstLoginDate, null},
                {UserJsonDynamicAttributeName.LastLoginDate, null},

            };

        public static Dictionary<string, dynamic> DefaultExportUserEventInfoFieldMappings =
            new Dictionary<string, dynamic>
            {
                {nameof(UserEventLogInfo.Level), null},
                {$"{nameof(UserEventLogInfo.CreatedByUser)}.{nameof(UserGenericDto.FirstName)}", null},
                {$"{nameof(UserEventLogInfo.CreatedByUser)}.{nameof(UserGenericDto.EmailAddress)}", null},
                {$"{nameof(UserEventLogInfo.CreatedByUser)}.{nameof(UserGenericDto.DepartmentName)}", null},
                {$"{nameof(UserEventLogInfo.CreatedByUser)}.{SystemRoleFieldName}", null},
                {nameof(UserEventLogInfo.Type), null},
                {nameof(UserEventLogInfo.Created), null},
                {nameof(UserEventLogInfo.SourceIp), null},
                {nameof(UserEventLogInfo.EventInfo), null}


            };

        public static Dictionary<string, dynamic> DefaultExportUserStatisticsFieldMappings =
            new Dictionary<string, dynamic>
            {
                {nameof(AccountType.ExternalMastered), null},
                {nameof(AccountType.NonExternalMastered), null},
                {nameof(AccountType.All), null}

            };

        public static Dictionary<string, dynamic> DefaultExportUserStatisticsVerticalFieldMappings =
            new Dictionary<string, dynamic>
            {
                {"AccountStatistics", "Account Statistics"},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.All}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.New}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.Active}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.Inactive}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.Deactive}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.IdentityServerLocked}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.PendingApproval1st}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.PendingApproval2nd}", null},
                {$"{nameof(UserStatisticsDto.AccountStatistics)}.{EntityStatusEnum.PendingApproval3rd}", null},
                {"LoginStatistics", "Login Statistics"},
                {$"{nameof(UserStatisticsDto.EventStatistics)}.{UserEventType.LoginSuccess}", null},
                {$"{nameof(UserStatisticsDto.EventStatistics)}.{UserEventType.LoginFail}", null},
                {"OnBoardingStatistics", "OnBoarding statistics"},
                {
                    $"{nameof(UserStatisticsDto.OnBoardingStatistics)}.{nameof(UserOnBoardingStatisticsDto.NotStarted)}",
                    null
                },
                {
                    $"{nameof(UserStatisticsDto.OnBoardingStatistics)}.{nameof(UserOnBoardingStatisticsDto.Started)}",
                    null
                },
                {
                    $"{nameof(UserStatisticsDto.OnBoardingStatistics)}.{nameof(UserOnBoardingStatisticsDto.Completed)}",
                    null
                }

            };

        public static Dictionary<string, dynamic> DefaultApprovingOfficerFieldMappings =
            new Dictionary<string, dynamic>
            {
                {nameof(ApprovingOfficerInfo.FirstName), null},
                {nameof(ApprovingOfficerInfo.EmailAddress), null},
                {nameof(ApprovingOfficerInfo.Created), null},
                {nameof(ApprovingOfficerInfo.DepartmentName), null},
                {nameof(ApprovingOfficerInfo.PrimaryApprovalMemberCount), null},
                {nameof(ApprovingOfficerInfo.AlternativeApprovalMemberCount), null}

            };

        public static Dictionary<string, dynamic> DefaultUserAccountDetailsFieldMappings =
            new Dictionary<string, dynamic>
            {
                {nameof(UserAccountDetailsInfo.TypeOfOrganization), "Type of Organisation"},
                {nameof(UserAccountDetailsInfo.DepartmentName), "Name of Organisation"},
                {nameof(UserAccountDetailsInfo.FullName), "Full Name"},
                {nameof(UserAccountDetailsInfo.ServiceScheme), "Service Scheme"},
                {nameof(UserAccountDetailsInfo.Designation), "Designation"},
                {nameof(UserAccountDetailsInfo.EmailAddress), "Email Address"},
                {nameof(UserAccountDetailsInfo.SystemRoles), "System Roles"},
                {nameof(UserAccountDetailsInfo.OnboardingStatus), "Onboarding Status (Yes/No)"},
                {nameof(UserAccountDetailsInfo.LastLoginDate), "Last Login Date"},
                {nameof(UserAccountDetailsInfo.DateOnboarded), "Date Onboarded"},
                {nameof(UserAccountDetailsInfo.AccountStatus), "Account Status"},
            };
        public static Dictionary<string, dynamic> DefaultPrivilegedUserAccountFieldMappings =
            new Dictionary<string, dynamic>
            {
                {nameof(PrivilegedUserAccountInfo.TypeOfOrganization), "Type of Organisation"},
                {nameof(PrivilegedUserAccountInfo.DepartmentPathName), "Place of work"},
                {nameof(PrivilegedUserAccountInfo.FullName), "Full Name"},
                {nameof(PrivilegedUserAccountInfo.Designation), "Designation"},
                {nameof(PrivilegedUserAccountInfo.EmailAddress), "Email Address"},
                {nameof(PrivilegedUserAccountInfo.SystemRoles), "System Roles"},
                {nameof(PrivilegedUserAccountInfo.LastLoginDate), "Last Login Date"}

            };
        public static bool NeedToGetRole(this Dictionary<string, dynamic> exportFields)
        {
            return exportFields != null && exportFields.Keys.Any(k => ExportHelper.NeedRoleFields.Contains(k, StringComparer.CurrentCultureIgnoreCase));
        }
        public static bool NeedToGetDepartment(this Dictionary<string, dynamic> exportFields)
        {
            return exportFields != null && exportFields.Keys.Any(k => ExportHelper.NeedDepartmentFields.Contains(k, StringComparer.CurrentCultureIgnoreCase));
        }
        public static string ToLowerFirstLetter(this string source)
        {
            return Char.ToLowerInvariant(source[0]) + source.Substring(1);
        }
        public static string GetLocalizedText(this List<LocalizedDataDto> source, string fieldName, string locale)
        {
            var localizedData = source?.FirstOrDefault(l => string.Equals(l.LanguageCode, locale, StringComparison.CurrentCultureIgnoreCase));
            if (localizedData != null)
            {
                return localizedData.Fields?.FirstOrDefault(t => t.Name == fieldName)?.LocalizedText;
            }
            return string.Empty;
        }
        public static DataTable CreateSummaryTable(string name)
        {
            var table = new DataTable(name);
            table.ExtendedProperties.Add("IsSummaryTable", true);
            return table;
        }

        public static bool IsSummaryTable(this DataTable table)
        {
            return (bool?)table.ExtendedProperties["IsSummaryTable"] == true;
        }

        public static bool ShouldHideHeader(this DataTable table)
        {
            return (bool?)table.ExtendedProperties["HideHeader"] == true;
        }
        public static bool ShouldHideHeader(this DataColumn column)
        {
            return (bool?)column.ExtendedProperties["HideHeader"] == true;
        }

        public static void SetFlagToHideHeader(this DataTable table)
        {
            table.ExtendedProperties.Add("HideHeader", true);

        }
        public static void SetFlagToHideHeader(this DataColumn column)
        {
            column.ExtendedProperties.Add("HideHeader", true);

        }
        public static void SetFormat(this DataColumn column, string format)
        {
            column.ExtendedProperties.Add("Format", format);
        }
        public static string GetFormat(this DataColumn column)
        {
            return column.ExtendedProperties["Format"] as string;
        }
        public static string GetStorageFullFilePath(char filePathDelimiter, AppSettings appSettings,  string subfolder, int ownerId, int customerId, string fileName)
        {
            var folder =
                $"{appSettings.ExportStorageFolder.Trim(filePathDelimiter)}{filePathDelimiter}{subfolder.Trim(filePathDelimiter)}";
           
            var fullFilePath = $"{folder}{filePathDelimiter}{ownerId}_{customerId}{filePathDelimiter}{fileName}".Trim(filePathDelimiter); ;
            return fullFilePath;
        }

        public static ExportFieldInfo ConvertToExportFieldInfo(dynamic value)
        {
            if (value == null) return null;
            if (value is ExportFieldInfo info)
            {
                return info;
            }

            if (value is JObject jObject)
            {
                var exportFieldInfo = jObject.ToObject<ExportFieldInfo>();
                if (exportFieldInfo != null) return exportFieldInfo;
            }

            return new ExportFieldInfo() { Caption = value.ToString() };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using cxOrganization.Domain.Extensions;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Settings
{
    public class EmailTemplates
    {
        public const string AllSymbol = "*";

        public MultiLanguageEmailTemplate ExportUserTemplate { get; set; }
        public MultiLanguageEmailTemplate ExportUserEventLogTemplate { get; set; }
        public MultiLanguageEmailTemplate ExportUserStatisticTemplate { get; set; }  
        public MultiLanguageEmailTemplate ExportApprovingTemplate { get; set; }
        public MultiLanguageEmailTemplate ExportUserAccountDetailsTemplate { get; set; }
        public MultiLanguageEmailTemplate ExportPrivilegedUserAccountTemplate { get; set; }


        public Dictionary<string, UserEmailTemplate> UserEmailTemplates { get; set; }
    }

    public class UserEmailTemplate : MultiLanguageEmailTemplate
    {
        public ApplyWhenDto ApplyWhen { get; set; }
        public string ApplyWithUserEntityExpression { get; set; }
    }


    public class MultiLanguageEmailTemplate
    {
        public bool Disabled { get; set; }
        public bool IsWelcomeEmail { get; set; }
        public List<ArchetypeEnum> ApplyForObjectiveUserArchetypes { get; set; }
        public List<string> ApplyForObjectiveUserRoles { get; set; }
        public List<EntityStatusEnum> ApplyForObjectiveUserEntityStatuses { get; set; }
        public List<string> DoNotApplyForObjectiveUserRoles { get; set; }
        public SendEmailToDto SendTo { get; set; }

        public Dictionary<string, string> Subject { get; set; }
        public CommunicationApiTemplate CommunicationApiTemplate { get; set; }

        public string GetSubject(IList<Locale> locales, string fallbackLanguageCode)
        {
            return GetTextByLocale(Subject, locales, fallbackLanguageCode);
        }


        private static string GetTextByLocale(Dictionary<string, string> texts, IList<Locale> locales, string fallbackLanguageCode)
        {
            if (texts == null || texts.Count == 0) return null;

            if (locales == null || locales.Count == 0)
            {
                var fallbackLocale = string.IsNullOrEmpty(fallbackLanguageCode)
                    ? "en-US"
                    : fallbackLanguageCode;

                if (texts.TryGetValue(fallbackLocale, out var value) && !string.IsNullOrEmpty(value))
                    return value;
            }
            else
            {
                foreach (var locale in locales)
                {
                    if (texts.TryGetValue(locale.LanguageCode, out var value) && !string.IsNullOrEmpty(value))
                        return value;
                }

            }


            return texts.Values.FirstOrDefault();
        }
    }

    public class CommunicationApiTemplate
    {
        public string Module { get; set; }
        public string Project { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string> Data { get; set; }

    }

    public class SendEmailToDto
    {
        public bool ObjectiveUser { get; set; }
        public bool ObjectiveUserOldEmail { get; set; }
        public bool ExecutorUser { get; set; }
        public List<SendToUserDto> OtherUsers { get; set; }

    }

    public class ApplyWhenDto
    {
        public ApplyWhenCreateUserDto CreateUser { get; set; }
        public ApplyWhenChangeEntityStatusDto ChangeEntityStatus { get; set; }
        public bool ResetOtp { get; set; }
        public ApplyWhenBasicDto ChangeEmail { get; set; }
        public ApplyWhenBasicDto ManuallyExecute { get; set; }
        public ApplyWhenMoveUserDto MoveUser { get; set; }
        public ApplyWhenBasicDto SchedulyExecute { get; set; }
    }

    public class ApplyWhenChangeEntityStatusDto
    {
        public List<EntityStatusEnum> FromEntityStatuses { get; set; }
        public List<EntityStatusEnum> ToEntityStatuses { get; set; }

        public bool ShouldApply(EntityStatusEnum fromEntityStatus, EntityStatusEnum toEntityStatus)
        {
            var matchFromEntityStatus = FromEntityStatuses.IsNullOrEmpty()
                                        || FromEntityStatuses.Contains(EntityStatusEnum.All)
                                        || FromEntityStatuses.Contains(fromEntityStatus);



            var matchToEntityStatus = ToEntityStatuses.IsNullOrEmpty()
                                      || ToEntityStatuses.Contains(EntityStatusEnum.All)
                                      || ToEntityStatuses.Contains(toEntityStatus);

            return matchFromEntityStatus && matchToEntityStatus;
           
        }
    }
  
    public class ApplyWhenBasicDto
    {
        public List<bool> ExternallyMasteredValues { get; set; }
        public List<EntityStatusEnum> EntityStatuses { get; set; }

        public bool ShouldApply(EntityStatusEnum entityStatus, bool externallyMastered)
        {
            var matchEntityStatus =  EntityStatuses.IsNullOrEmpty() ||
                                    EntityStatuses.Contains(EntityStatusEnum.All) ||
                                    EntityStatuses.Contains(entityStatus);
            var matchExternallyMasteredValue =  ExternallyMasteredValues.IsNullOrEmpty() || ExternallyMasteredValues.Contains(externallyMastered);

            return matchEntityStatus && matchExternallyMasteredValue;

        }

    }

    public class ApplyWhenCreateUserDto : ApplyWhenBasicDto
    {
        public List<string> NotInDepartmentExtIds { get; set; }

        public bool ShouldApply(EntityStatusEnum entityStatus, bool externallyMastered, string departmentExtId)
        {
            if (NotInDepartmentExtIds != null
                && (NotInDepartmentExtIds.Contains(EmailTemplates.AllSymbol)
                    || NotInDepartmentExtIds.Contains(departmentExtId, StringComparer.CurrentCultureIgnoreCase)))
            {
                return false;

            }
            return base.ShouldApply(entityStatus, externallyMastered);
        }
    }
    public class ApplyWhenMoveUserDto: ApplyWhenBasicDto
    {
        public List<string> FromDepartmentExtIds { get; set; }
        public List<string> ToDepartmentExtIds { get; set; }

        public bool ShouldApply(EntityStatusEnum entityStatus, bool externallyMastered, string fromDepartmentExtId,
            string toDepartmentExtId)
        {
            if (!base.ShouldApply(entityStatus, externallyMastered))
                return false;

            var matchFromDepartmentExtId = FromDepartmentExtIds.IsNullOrEmpty()
                                            || FromDepartmentExtIds.Contains(EmailTemplates.AllSymbol)
                                            || FromDepartmentExtIds.Contains(fromDepartmentExtId);

            var matchToDepartmentExtId = ToDepartmentExtIds.IsNullOrEmpty()
                                           || ToDepartmentExtIds.Contains(EmailTemplates.AllSymbol)
                                           || ToDepartmentExtIds.Contains(toDepartmentExtId);
            
            return matchFromDepartmentExtId && matchToDepartmentExtId;
        }
    }

    public class SendToUserDto
    {
       public List<string> UserTypeExtIds { get; set; }
       public bool InFullHierarchyDepartment { get; set; }
       public bool InSameDepartment { get; set; }
       public bool InAncestorDepartment { get; set; }
       public bool InDescendantDepartment { get; set; }
    }
}

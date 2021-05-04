using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Settings
{
    public class AccessSettings
    {
        public AccessSettings()
        {
            ReadUserAccess = new Dictionary<string, Dictionary<string, AccessSettingElement>>(StringComparer.CurrentCultureIgnoreCase);
        }
        public Dictionary<string, AccessSettingElement> ReadDepartmentAccess { get; set; }

        public Dictionary<string, Dictionary<string, AccessSettingElement>> ReadUserAccess { get; set; }
        public Dictionary<string, EditabilityAccessSettingElement> EditUserAccess { get; set; }
        public Dictionary<string, EditabilityAccessSettingElement> CreateUserAccess { get; set; }

        public Dictionary<string, AccessSettingElement> ReadApprovalGroupAccess { get; set; }
        public Dictionary<string, AccessSettingElement> ReadUserPoolAccess { get; set; }
        public bool DisableReadUserAccessChecking { get; set; }
        public bool DisableEditUserAccessChecking { get; set; } 
        public bool DisableCreateUserAccessChecking { get; set; }
        public bool DisableReadApprovalGroupAccessChecking { get; set; }
        public bool DisableReadUserPoolAccessChecking { get; set; }

    }
    public class EditabilityAccessSettingElement : AccessSettingElement
    {
        public Dictionary<string, RestrictedProperty> RestrictedProperties { get; set; }
        public bool NotAllowSelfAccess { get; set; }

    }

    public class AccessSettingElement
    {
        public const string AllSymbol = "*";

        public AccessSettingElement()
        {
            InAncestorDepartmentTypeExtIds = new List<string>();
            InDescendantDepartmentTypeExtIds = new List<string>();
            InOwnedUserGroupArchetypes = new List<string>();
            InDepartmentUserGroupArchetypes = new List<string>();
            OnlyUserWithUserTypeExtIds = new Dictionary<ArchetypeEnum, List<string>>();
            InRelativeDepartmentTypeExtIds = new List<string>();

        }
        public bool HasFullAccessOnHierarchy { get; set; }

        public bool InSameDepartment { get; set; }

        public bool AccessToAllUserGroup { get; set; }

        public bool OnlyMoveUpOneAncestor { get; set; }

        public List<string> InAncestorDepartmentTypeExtIds { get; set; }

        public List<string> InRelativeDepartmentTypeExtIds { get; set; }

        public List<string> InDescendantDepartmentTypeExtIds { get; set; }

        public List<string> InOwnedUserGroupArchetypes { get; set; }

        public List<string> InDepartmentUserGroupArchetypes { get; set; }

        public Dictionary<ArchetypeEnum,List<string>> OnlyUserWithUserTypeExtIds { get; set; }

        public static bool ContainsAllSymbol(List<string> values)
        {
            return values != null && values.Contains(AllSymbol);
        }
    }



    public class RestrictedProperty
    {
        public RestrictedProperty()
        {
            AllowedValues = new List<string>();
            AllowedChangeEntityStatuses = new List<ChangeEntityStatusValue>();
            AllowedSelfChangeEntityStatuses = new List<ChangeEntityStatusValue>();
        }

        public List<string> AllowedValues { get; set; }
        public bool AllowSelfAccess { get; set; }
        public bool? RestrictForExternallyMasteredUserOnly { get; set; }
        public List<ChangeEntityStatusValue> AllowedSelfChangeEntityStatuses { get; set; }
        public List<ChangeEntityStatusValue> AllowedChangeEntityStatuses { get; set; }

    }

    public class ChangeEntityStatusValue
    {
        public EntityStatusEnum From { get; set; }
        public EntityStatusEnum To{ get; set; }

    }
}

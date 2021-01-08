using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Settings.MassUserCreationMessageSetting
{
    public static class MassUserCreationMessageValidationConstance
    {
        // Message Name
        public static string Salutation = "Salutation";
        public static string Name = "Name";
        public static string OfficialEmail = "OfficialEmail";
        public static string Gender = "Gender";
        public static string PlaceOfWork = "PlaceOfWork";
        public static string AccountActiveFrom = "AccountActiveFrom";
        public static string DateOfExpiryAccount = "DateOfExpiryAccount";
        public static string ReasonForUserAccountRequest = "ReasonForUserAccountRequest";
        public static string SystemRole = "SystemRole";
        public static string PersonalSpaceLimitation = "PersonalSpaceLimitation";

        // Validation Message Name
        public static string Empty = "Empty";
        public static string NotExist = "NotExist";
        public static string SpecialCharacters = "SpecialCharacters";
        public static string NotInDepartment = "NotInDepartment";
        public static string OutOfLimit = "OutOfLimit";
        public static string InvalidFormat = "InvalidFormat";
        public static string InvalidDomainFormat = "InvalidDomainFormat";
        public static string DuplicateSystemData = "DuplicateSystemData";
        public static string DuplicateImportedData = "DuplicateImportedData";
        public static string Irrelevance = "Irrelevance";
        public static string InPast = "InPast";
        public static string InvalidDateRange = "InvalidDateRange";
        public static string NoPermission = "NoPermission";
    }
}

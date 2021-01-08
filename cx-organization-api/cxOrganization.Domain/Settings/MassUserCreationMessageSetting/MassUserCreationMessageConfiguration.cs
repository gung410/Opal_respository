using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Settings.MassUserCreationMessageSetting
{
    public class MassUserCreationMessageConfiguration
    {
        public MassUserCreationMessageValidationConfiguration Validation { get; set; }

        public string GetValidationMessage(string type, string actiontype, string locale)
        {
            if (Validation.Message.ContainsKey(type) && Validation.Message.TryGetValue(type, out Dictionary<string, Dictionary<string, string>> typeValue))
            {
                if (typeValue.ContainsKey(actiontype) && typeValue.TryGetValue(actiontype, out Dictionary<string, string> actiontypeValue))
                {
                    if (actiontypeValue.ContainsKey(locale) && actiontypeValue.TryGetValue(locale, out string localeValue))
                    {
                        return localeValue;
                    }
                }
            }

            return string.Empty;
        }
    }
    public class MassUserCreationMessageValidationConfiguration
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Message { get; set; }
    }
}

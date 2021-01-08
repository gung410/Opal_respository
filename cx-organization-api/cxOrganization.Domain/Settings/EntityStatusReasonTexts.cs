using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Settings
{
    public class EntityStatusReasonTexts: Dictionary<EntityStatusReasonEnum, Dictionary<string, string>>
    {
      
        public string GetText(EntityStatusReasonEnum entityStatusReason, IList<Locale> locales)
        {
            if (this.TryGetValue(entityStatusReason, out var textsByLocale))
            {
                if (locales == null || locales.Count == 0)
                {
                    return textsByLocale.Values.FirstOrDefault();
                }

                foreach (var locale in locales)
                {
                    if (textsByLocale.TryGetValue(locale.LanguageCode, out var text) && !string.IsNullOrEmpty(text))
                        return text;
                }
            }

            return null;
        }
    }
}

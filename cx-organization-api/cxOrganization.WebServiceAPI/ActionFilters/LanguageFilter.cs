using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace cxOrganization.WebServiceAPI.ActionFilters
{
    public class LanguageFilter : IActionFilter
    {
        private readonly IAdvancedWorkContext _workContext;
        private readonly string FallBackLanguageCode;
        private static Dictionary<string, LanguageEntity> LanguagesDic
        {
            get;
            set;
        }

        public LanguageFilter(ILanguageRepository languageRepository, IAdvancedWorkContext workContext, IOptions<AppSettings> options)
        {
            _workContext = workContext;
            if (LanguagesDic == null || !LanguagesDic.Any())
            {
                LanguagesDic = languageRepository.GetLanguages().ToDictionary(t => t.LanguageCode);
            }
            FallBackLanguageCode = options.Value?.FallBackLanguageCode != null ? options.Value.FallBackLanguageCode : "en-US";
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            List<Locale> curentLocales = null;
            if (context.HttpContext.Request.Headers.TryGetValue("Content-Language", out var contentLanguages))
            {
                //this is the old implementation, we have been getting the locale by content languages

                curentLocales = GetGivenLocales(contentLanguages);
            }
            else if (context.HttpContext.Request.Headers.TryGetValue("Accept-Language", out var acceptLanguages))
            {
                curentLocales = GetGivenLocales(acceptLanguages);
            }

            curentLocales = curentLocales ?? new List<Locale>();
         
            var fallbackLocale = GetFallbackLocale();

            if (curentLocales.Count == 0 && fallbackLocale != null)
            {
                curentLocales.Add(fallbackLocale);
            }

            SetLanguageToWorkContext(curentLocales.FirstOrDefault());

            _workContext.CurrentLocales = curentLocales;
            _workContext.FallBackLanguage = fallbackLocale;

        }

        private void SetLanguageToWorkContext(Locale locale)
        {
            if (locale != null)
            {
                _workContext.CurrentLanguageId = locale.LanguageId;
                _workContext.CurrentLocale = locale.LanguageCode;
            }
        }

        private LanguageEntity GetLanguageEntity(string languageCode)
        {

            LanguageEntity languageEntity;
            if (LanguagesDic.TryGetValue(languageCode, out languageEntity))
            {
                return languageEntity;
            }

            return null;

        }

        private Locale GetFallbackLocale()
        {
            var languageEntity = GetLanguageEntity(FallBackLanguageCode) ?? LanguagesDic.Values.FirstOrDefault();
            if (languageEntity!=null)
                return new Locale()
                {
                    LanguageId = languageEntity.LanguageId,
                    LanguageCode = languageEntity.LanguageCode,
                    Weight = 1
                };
            return null;

        }
        public List<Locale> GetGivenLocales(StringValues givenLanguages)
        {
            var languageWeights = new List<Locale>();
            //Handle this by referring to RFC 7231, section 5.3.5: Accept-Language

  

            foreach (var givenLanguage in givenLanguages)
            {
                if (!string.IsNullOrEmpty(givenLanguage))
                {
                    var languages = givenLanguage.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var language in languages)
                    {
                        var splitedLanguages = language.Split(";");
                        var languageCode = splitedLanguages.First().Trim();
                        if (languageWeights.Any(l =>
                            string.Equals(l.LanguageCode, languageCode, StringComparison.CurrentCultureIgnoreCase)))
                            continue;
                        var languageEntity = GetLanguageEntity(languageCode);
                        if(languageEntity==null) continue;
                      
                        float weight = 1; //Default is 1
                        if (splitedLanguages.Length > 1)
                        {
                            if (float.TryParse(splitedLanguages[1].Trim().Split('=').LastOrDefault(), out var extractedWeight))
                            {
                                weight = extractedWeight;
                            }
                        }

                        languageWeights.Add(new Locale
                            {LanguageCode = languageCode, Weight = weight, LanguageId = languageEntity.LanguageId});
                    }
                }
            }

            return languageWeights.OrderByDescending(l => l.Weight).ToList();
        }
    }
}

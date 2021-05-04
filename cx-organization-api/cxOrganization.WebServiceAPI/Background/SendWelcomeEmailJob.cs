using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using cxOrganization.Domain;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;
using cxPlatform.Core.Logging;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DateTime = System.DateTime;

namespace cxOrganization.WebServiceAPI.Background
{
    public class SendWelcomeEmailJob : ISendWelcomeEmailJob
    {
        private  readonly ILogger _logger;
        public const string JobId = "SendWelcomeEmailJob";
        private readonly IOptions<AppSettings> _appSettingOption;
        private readonly IUserService _userService;
        private readonly ILanguageRepository _languageRepository;
        private readonly RecurringJobSettings _recurringJobSettings;
        private RecurringJobSetting _currentRecurringJobSetting;

        private static Dictionary<string, LanguageEntity> LanguagesDic { get; set; }


        public SendWelcomeEmailJob(ILogger<SendWelcomeEmailJob> logger, IOptions<AppSettings> appSettingOptions, Func<ArchetypeEnum, IUserService> userService, 
            ILanguageRepository languageRepository, IOptions<RecurringJobSettings> recurringJobSettingsOptions)
        {
            _logger = logger;
            _userService = userService(ArchetypeEnum.Unknown);
            _appSettingOption = appSettingOptions;
            _recurringJobSettings = recurringJobSettingsOptions.Value;
            _languageRepository = languageRepository;
        }


        private RecurringJobSetting GetRecurringJobSetting()
        {
            if (_currentRecurringJobSetting != null)
                return _currentRecurringJobSetting;
            if (_recurringJobSettings.TryGetValue(JobId, out _currentRecurringJobSetting))
                return _currentRecurringJobSetting;
            throw new Exception($"Missing RecurringJobSetting for {JobId}");
        }
        public void Execute(PerformContext performContext, params object[] inputs)
        {
            var requestId = Guid.NewGuid().ToString();
            var correlationId = Guid.NewGuid().ToString();
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                {LogPropertyKeys.RequestId, requestId},
                {LogPropertyKeys.CorrelationId, correlationId}
            }))
            {
                try
                {
                    var currentRecurringJobSetting = GetRecurringJobSetting();
                    if (!currentRecurringJobSetting.Enable)
                    {
                        _logger.LogWarning($"Job with id {performContext.BackgroundJob.Id} is disabled by configuration.");
                        return;
                    }

                    var stopWatch = Stopwatch.StartNew();

                    //We get all user has ActiveDate <=current date, to make sure if some user has been sent welcome email fail in previous day, then we can retry it next day.
                   
                    var dateTimeOffSet = _appSettingOption.Value.TimeZoneOffset ?? 0;
                    var currentDate = DateTime.UtcNow.AddHours(dateTimeOffSet).Date.AddHours(-dateTimeOffSet);
                    var endOfDate = currentDate.AddHours(24).AddMilliseconds(-1);

                    _logger.LogInformation(
                        $"Job started - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}. Start sending welcome email for users have active date before {endOfDate.ConvertToISO8601()}.");


                    var workContext = InitWorkContext(currentRecurringJobSetting, requestId, correlationId);
                    _userService.SchedulySendWelcomeEmail(workContext, entityActiveDateBefore: endOfDate);

                    stopWatch.Stop();
                    _logger.LogInformation(
                        $"Job finished - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}. Elapsed {stopWatch.ElapsedMilliseconds}ms.");


                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"Job failed - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}. Exception {e.Message}");
                    throw;
                }

            }
        }

        public Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            return Task.Factory.StartNew(() => { Execute(performContext, inputs); });
        }

        private IAdvancedWorkContext InitWorkContext(RecurringJobSetting currentRecurringJobSetting, string requestId, string correlationId)
        {
            var appSetting = _appSettingOption.Value;

            var workContext = new WorkContext(_appSettingOption, null)
            {
                CurrentUserId = appSetting.CurrentUserId,
                CurrentOwnerId = currentRecurringJobSetting.OwnerId,
                CurrentCustomerId = currentRecurringJobSetting.CustomerId,
                IsEnableFiltercxToken = true,
                RequestId = requestId,
                CorrelationId = correlationId
            };

            var fallbackLanguageCode = appSetting.FallBackLanguageCode;
            if (string.IsNullOrEmpty(fallbackLanguageCode)) fallbackLanguageCode = "en-US";
            var currentLocales = new List<Locale>();
            var currentLocale = GetLocale(currentRecurringJobSetting.LanguageCode);
            if (currentLocale != null)
            {
                currentLocales.Add(currentLocale);
            }

            var fallbackLocale = GetLocale(fallbackLanguageCode);
            if (currentLocales.Count == 0 && fallbackLocale != null)
            {
                currentLocales.Add(fallbackLocale);
            }

            SetLanguageToWorkContext(workContext, currentLocales.FirstOrDefault());
            workContext.CurrentLocales = currentLocales;
            workContext.FallBackLanguage = fallbackLocale;
            return workContext;
        }

        private void SetLanguageToWorkContext(IAdvancedWorkContext workContext, Locale locale)
        {
            if (locale != null)
            {
                workContext.CurrentLanguageId = locale.LanguageId;
                workContext.CurrentLocale = locale.LanguageCode;
            }
        }


        private Locale GetLocale(string languageCode)
        {
            if (LanguagesDic.IsNullOrEmpty())
            {

                LanguagesDic = _languageRepository.GetLanguages().ToDictionary(t => t.LanguageCode);
            }

            if (LanguagesDic.TryGetValue(languageCode, out var languageEntity))
            {
                return new Locale
                {
                    LanguageId = languageEntity.LanguageId,
                    LanguageCode = languageEntity.LanguageCode
                };
            }

            return null;

        }

    }
}
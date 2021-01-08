using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using cxOrganization.Domain;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using cxPlatform.Core.Logging;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.WebServiceAPI.Background
{
    public class SendBroadcastMessageJob : ISendBroadcastMessageJob
    {
        private  readonly ILogger _logger;
        public const string JobId = "SendBroadcastMessageJob";
        private readonly IOptions<AppSettings> _appSettingOption;
        private readonly IBroadcastMessageService _broadcastMessageService;
        private readonly ILanguageRepository _languageRepository;
        private readonly RecurringJobSettings _recurringJobSettings;
        private RecurringJobSetting _currentRecurringJobSetting;

        private static Dictionary<string, LanguageEntity> LanguagesDic { get; set; }


        public SendBroadcastMessageJob(
            ILogger<SendBroadcastMessageJob> logger,
            IOptions<AppSettings> appSettingOptions,
            IBroadcastMessageService broadcastMessageService, 
            ILanguageRepository languageRepository,
            IOptions<RecurringJobSettings> recurringJobSettingsOptions)
        {
            _logger = logger;
            _broadcastMessageService = broadcastMessageService;
            _appSettingOption = appSettingOptions;
            _recurringJobSettings = recurringJobSettingsOptions.Value;
            _languageRepository = languageRepository;
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
                 
                    _logger.LogInformation(
                        $"Job started - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}. Start sending recurring broadcast messages");

                    // Do Work
                    var workContext = InitWorkContext(currentRecurringJobSetting, requestId, correlationId);
                    _broadcastMessageService.SendScheduledBroadcastMessages();

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

        private RecurringJobSetting GetRecurringJobSetting()
        {
            if (_currentRecurringJobSetting != null)
                return _currentRecurringJobSetting;
            if (_recurringJobSettings.TryGetValue(JobId, out _currentRecurringJobSetting))
                return _currentRecurringJobSetting;
            throw new Exception($"Missing RecurringJobSetting for {JobId}");
        }

        private IWorkContext InitWorkContext(RecurringJobSetting currentRecurringJobSetting, string requestId, string correlationId)
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

        private void SetLanguageToWorkContext(IWorkContext workContext, Locale locale)
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
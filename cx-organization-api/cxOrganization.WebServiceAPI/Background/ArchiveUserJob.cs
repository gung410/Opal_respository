using cx.datahub.scheduling.jobs.shared;
using cxOrganization.Business.Extensions;
using cxOrganization.Domain;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Logging;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Background
{
    public class ArchiveUserJob : IArchiveUserJob
    {
        private readonly ILogger _logger;
        public const string JobId = "ArchiveUserJob";
        private RecurringJobSetting _currentRecurringJobSetting;
        private readonly IOptions<AppSettings> _appSettingOption;
        private readonly RecurringJobSettings _recurringJobSettings;
        private readonly IUserService _userService;
        private readonly ILanguageRepository _languageRepository;
        

        private static Dictionary<string, LanguageEntity> LanguagesDic { get; set; }

        public ArchiveUserJob(
            ILogger<ArchiveUserJob> logger,
            IOptions<AppSettings> appSettingOptions,
            ILanguageRepository languageRepository,
            IOptions<RecurringJobSettings> recurringJobSettingsOptions,
            Func<ArchetypeEnum, IUserService> userService
        )
        {
            _logger = logger;
            _recurringJobSettings = recurringJobSettingsOptions.Value;
            _languageRepository = languageRepository;
            _userService = userService(ArchetypeEnum.Unknown);
            _appSettingOption = appSettingOptions;
        }

        public void Execute(PerformContext performContext, params object[] inputs)
        {
            var requestId = Guid.NewGuid().ToString();
            var correlationId = Guid.NewGuid().ToString();
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                {LogPropertyKeys.RequestId, requestId },
                {LogPropertyKeys.CorrelationId, correlationId }
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
                        $"Job started - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}. Start auto archive users");

                    // Do Work
                    var workContext = InitWorkContext(currentRecurringJobSetting, requestId, correlationId);

                    _userService.ProcessAutoArchiveUser(workContext).Wait();

                    stopWatch.Stop();
                    _logger.LogInformation(
                        $"Job finished - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}. Elapsed {stopWatch.ElapsedMilliseconds}ms.");
                }
                catch (Exception ex)
                {
                    throw ex;
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

using cxOrganization.Business.Exceptions;
using cxOrganization.Domain;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Services.ExportService;
using cxOrganization.Domain.Services.Reports;
using cxOrganization.Domain.Services.StorageServices;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Background;
using cxOrganization.WebServiceAPI.Models;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Extentions.Request;
using cxPlatform.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using cxOrganization.WebServiceAPI.Extensions;
using FileExtension = cxOrganization.WebServiceAPI.Models.FileExtension;
using cxOrganization.Domain.AdvancedWorkContext;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    [Route("reports")]
    public class ReportsController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IExportService<UserEventLogInfo> _exportUserEventService;
        private readonly IExportService<UserStatisticsDto> _exportUserStatisticsService;
        private readonly IExportService<ApprovingOfficerInfo> _exportApprovingOfficerService;
        private readonly IExportService<UserAccountDetailsInfo> _exportUserAccountDetailsService;
        private readonly IExportService<PrivilegedUserAccountInfo> _exportPrivilegedUserAccountService;


        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AppSettings _appSettings;

        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        private readonly IUserReportService _userReportService;
        private readonly IFileStorageService _fileStorageService;

        public ReportsController(
            Func<ArchetypeEnum, IUserService> userService,
            IAdvancedWorkContext workContext,
            IExportService<UserEventLogInfo> exportUserEventService,
            IExportService<UserStatisticsDto> exportUserStatisticsService,
            IExportService<ApprovingOfficerInfo> exportApprovingOfficerDto,
            IExportService<UserAccountDetailsInfo> exportUserAccountDetailsService,
            IExportService<PrivilegedUserAccountInfo> exportPrivilegedUserAccountService,
            ILogger<ReportsController> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AppSettings> appSettingOptions,
            IBackgroundTaskQueue backgroundTaskQueue,
            IUserReportService userReportService,
            IFileStorageService fileStorageService)
        {
            _userService = userService(ArchetypeEnum.Unknown);
            _workContext = workContext; 
            _exportUserEventService = exportUserEventService;
            _exportUserStatisticsService = exportUserStatisticsService;
            _exportApprovingOfficerService = exportApprovingOfficerDto;
            _exportUserAccountDetailsService = exportUserAccountDetailsService;
            _exportPrivilegedUserAccountService = exportPrivilegedUserAccountService;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _appSettings = appSettingOptions.Value;
            _backgroundTaskQueue = backgroundTaskQueue;
            _userReportService = userReportService;
            _fileStorageService = fileStorageService;
        }


        /// <summary>
        /// Get user event logs. If there is no filter on datetime, it return log in latest 7 days
        /// </summary>
        /// <param name="eventTypes">List of user event type, it's required.</param>
        /// <param name="eventCreatedAfter"></param>
        /// <param name="eventCreatedBefore"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="getDepartment"></param>
        /// <param name="getRole"></param>
        /// <returns></returns>
        [HttpGet("users/eventlogs")]
        [ProducesResponseType(typeof(PaginatedList<UserEventLogInfo>), StatusCodes.Status200OK)]
        public IActionResult UserEventLogs([FromQuery] [Required] List<UserEventType> eventTypes,
            DateTime? eventCreatedAfter = null, DateTime? eventCreatedBefore = null, int pageSize = 100,
            int pageIndex = 1, bool getDepartment = false, bool getRole = false)
        {
            if (eventTypes.IsNullOrEmpty())
            {
                return BadRequest(new {Messagge = $"Parameter '{nameof(eventTypes)}' is required."});
            }

            if (pageIndex <= 0) pageIndex = 1;

            SetDefaultDatetimeIfNull(ref eventCreatedAfter, ref eventCreatedBefore);

            var paginatedEventInfos = GetPaginatedEventInfosAsync(_workContext,
                _userReportService, eventTypes, eventCreatedAfter,
                eventCreatedBefore,
                pageSize, pageIndex,
                getDepartment, getRole, false);

            return Ok(paginatedEventInfos);
        }

        /// <summary>
        /// Export list of user synchronously. 
        /// </summary>
        /// <param name="exportUserEventLogInfoDto"></param>
        /// <returns></returns>
        [Route("users/eventlogs/export")]
        [HttpPost]
        public async Task<IActionResult> ExportUserEvents(
            [FromBody] [Required] ExportUserEventLogInfoDto exportUserEventLogInfoDto)
        {
            if (exportUserEventLogInfoDto.SendEmail)
            {
                throw new InvalidException("Do not support sending email when exporting user event logs with this action");
            }

            var fileExtension = GetFileExtension(exportUserEventLogInfoDto.ExportOption.ExportType);
           var fileName = GenerateUserAuditExportFileName(fileExtension);

            var eventTypeAsString = string.Join(", ", exportUserEventLogInfoDto.EventTypes);

            _logger.LogInformation($"Start processing for exporting user event logs of {eventTypeAsString} to {fileName}.");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var exportedData = await ExecuteExportingUserEvents(_logger, exportUserEventLogInfoDto, _workContext, _userReportService, _exportUserEventService, false);

            watch.Stop();
            _logger.LogInformation($"End processing for exporting user event logs  of {eventTypeAsString}. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");
            var contentType = GetContentTypeForFileExtension(fileExtension);
            return File(exportedData.Data, contentType, fileName);
        }

        /// <summary>
        /// Export list of user asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportUserEventLogInfoDto"></param>
        /// <returns></returns>
        [Route("users/eventlogs/export/async")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public IActionResult ExportUserEventsAsync([FromBody][Required] ExportUserEventLogInfoDto exportUserEventLogInfoDto)

        {
            if (exportUserEventLogInfoDto.EventTypes.IsNullOrEmpty())
            {
                return BadRequest(new { Messagge = $"Parameter '{nameof(ExportUserEventLogInfoDto.EventTypes)}' is required." });
            }
            var fileExtension = GetFileExtension(exportUserEventLogInfoDto.ExportOption.ExportType);
            var fileName = GenerateUserAuditExportFileName(fileExtension);
            var filePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings, _appSettings.ExportUserAuditEventStorageSubFolder,
                _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            //For thread safe
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //TODO: deal with IAdvancedWorkContext injection when calling async
            var apiDownloadUrl = this.Url.Link("DownloadExportUserEvents", new { fileName = fileName });
            var fallBackLanguageCode = _appSettings.FallBackLanguageCode;
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<ReportsController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            var eventTypeAsString = string.Join(", ", exportUserEventLogInfoDto.EventTypes);
                            scopedLogger.LogInformation($"Start processing for exporting user event logs of {eventTypeAsString} to {fileName}.");
                            var scopeUserService =
                                serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(
                                    ArchetypeEnum
                                        .Unknown);
                            var scopeExportService =
                                serviceScope.ServiceProvider.GetService<IExportService<UserEventLogInfo>>();

                            var scopeUserEventLogService =
                                serviceScope.ServiceProvider.GetService<IUserReportService>();

                            var scopeFileStorageService =
                                serviceScope.ServiceProvider.GetService<IFileStorageService>();


                            var exportedData = await ExecuteExportingUserEvents(scopedLogger, exportUserEventLogInfoDto,
                                copiedWorkContext, scopeUserEventLogService, scopeExportService, true);

                            var uploadedFilePath =  await scopeFileStorageService.UploadFileAsync(copiedWorkContext, exportedData.Data, filePath);
                            if (!string.IsNullOrEmpty(uploadedFilePath))
                            {
                                scopedLogger.LogInformation(
                                    $"Exporting data has been saved to storage under path {uploadedFilePath}");

                                if (exportUserEventLogInfoDto.SendEmail)
                                {
                                    SendEmailWhenExportingUserEvents(exportUserEventLogInfoDto, copiedWorkContext,
                                        scopeUserService, scopedLogger, serviceScope, apiDownloadUrl,
                                        filePath, fallBackLanguageCode);
                                }
                            }
                            else
                            {
                                scopedLogger.LogError(
                                    $"Failed to save exporting data to storage under path {filePath}");
                            }

                            watch.Stop();
                            scopedLogger.LogInformation($"End processing for exporting user event logs of {eventTypeAsString}. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} log entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");

                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e, $"Unexpected error occurred when processing for exporting users event logs to {fileName}. {e.Message}");
                        }

                    }
                }
            });

            return Accepted(new AcceptExportDto
            {
                Message = "Request is accepted for process data. Please download data from url when it's ready.",
                FileName = fileName,
                DownloadUrl = apiDownloadUrl,
                FilePath = filePath
            });
        }

        /// <summary>
        /// Download user audit events file that has been exported asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>

        [Route("users/eventlogs/export/download", Name = "DownloadExportUserEvents")]
        [HttpPost]
        public async Task<IActionResult> DownloadExportUserEvents(string fileName)
        {
            return await DownloadFile(_appSettings.ExportUserAuditEventStorageSubFolder, fileName);
        }

        /// <summary>
        /// Get user statistics logs. If there is no filter on datetime, it return log in latest 7 days
        /// </summary>
        /// <param></param>
        /// <param name="fromDate">Date time value that user statistics is calculated from</param>
        /// <param name="toDate">Date time value that user statistics is calculated to</param>
        /// <param name="accountStatisticsEntityStatuses">Set the lis of user entity statues to get account statistics. Default is true</param>
        /// <param name="eventStatisticsTypes">List of event types to get event statistics. Default is true</param>
        /// <param name="onBoardingStatistics">Set true/false to get or exclude on boarding statistics. Default is true</param>
        /// <returns></returns>
        [HttpGet("users/statistics")]
        [ProducesResponseType(typeof(UserStatisticsDto), StatusCodes.Status200OK)]
        public IActionResult GetUserStatistics(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            [FromQuery] List<EntityStatusEnum> accountStatisticsEntityStatuses =  null,
            bool onBoardingStatistics = false,
            [FromQuery] List<UserEventType> eventStatisticsTypes = null)
        {
            SetDefaultDatetimeIfNull(ref fromDate, ref toDate);

            var userStatistics = _userReportService.GetUserStatisticsAsync(_workContext,
                accountStatisticsEntityStatuses: accountStatisticsEntityStatuses,
                eventStatisticsTypes: eventStatisticsTypes,
                getOnBoardingStatistics: onBoardingStatistics,
                fromDate: fromDate,
                toDate: toDate);

            return Ok(userStatistics);
        }
        /// <summary>
        /// Export user statistics.
        /// </summary>
        /// <param name="exportUserStatistics"></param>
        /// <returns></returns>
        [Route("users/statistics/export")]
        [HttpPost]
        public async Task<IActionResult> ExportUserStatistics(
            [FromBody] [Required] ExportUserStatisticsDto exportUserStatistics)
        {
            if (exportUserStatistics.SendEmail)
            {
                throw new InvalidException("Do not support sending email when exporting user statistics with this action");
            }

            var fileExtension = GetFileExtension(exportUserStatistics.ExportOption.ExportType);
            var fileName = GenerateUserStatisticsExportFileName(fileExtension);

            _logger.LogInformation($"Start processing for exporting user statistics into {fileName}.");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var exportedData = await ExecuteExportingUserStatisticsAsync(_logger, exportUserStatistics, _workContext, _userReportService, _exportUserStatisticsService);

            watch.Stop();
            _logger.LogInformation($"End processing for exporting user statistics into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");
            var contentType = GetContentTypeForFileExtension(fileExtension);
            return File(exportedData, contentType, fileName);
        }

        /// <summary>
        /// Export list of user statistics asynchronously.
        /// </summary>
        /// <param name="exportUserStatistics"></param>
        /// <returns></returns>
        [Route("users/statistics/export/async")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public IActionResult ExportUserStatisticsAsync([FromBody][Required] ExportUserStatisticsDto exportUserStatistics)
        {
         
            var fileExtension = GetFileExtension(exportUserStatistics.ExportOption.ExportType);
            var fileName = GenerateUserStatisticsExportFileName(fileExtension);
            var filePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings, _appSettings.ExportUserStatisticsStorageSubFolder,
                _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            //For thread safe
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //TODO: deal with IAdvancedWorkContext injection when calling async
            var apiDownloadUrl = this.Url.Link("DownloadExportUserStatistics", new { fileName = fileName });
            var fallBackLanguageCode = _appSettings.FallBackLanguageCode;
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<ReportsController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            scopedLogger.LogInformation($"Start processing for exporting user statistics into {fileName}.");
                            var scopeUserService =
                                serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(
                                    ArchetypeEnum
                                        .Unknown);
                            var scopeExportService =
                                serviceScope.ServiceProvider.GetService<IExportService<UserStatisticsDto>>();

                            var scopeUserEventLogService =
                                serviceScope.ServiceProvider.GetService<IUserReportService>();

                            var scopeFileStorageService =
                                serviceScope.ServiceProvider.GetService<IFileStorageService>();


                            var exportedData = await ExecuteExportingUserStatisticsAsync(scopedLogger, exportUserStatistics, copiedWorkContext, scopeUserEventLogService, scopeExportService);

                            var uploadedFilePath = await scopeFileStorageService.UploadFileAsync(copiedWorkContext, exportedData, filePath);
                            if (!string.IsNullOrEmpty(uploadedFilePath))
                            {
                                scopedLogger.LogInformation($"Exporting user statistics has been saved to storage under path {uploadedFilePath}");

                                if (exportUserStatistics.SendEmail)
                                {
                                    SendEmailWhenExportingUserStatistics(exportUserStatistics, copiedWorkContext,
                                        scopeUserService, scopedLogger, serviceScope, apiDownloadUrl,
                                        filePath, fallBackLanguageCode);
                                }
                            }
                            else
                            {
                                scopedLogger.LogError($"Failed to save exporting user statistics to storage under path {filePath}");
                            }

                            watch.Stop();
                            scopedLogger.LogInformation($"Start processing for exporting user statistics into {fileName} in {watch.ElapsedMilliseconds}ms.");

                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e, $"Unexpected error occurred when processing for exporting users statics logs to {fileName}. {e.Message}");
                        }

                    }
                }
            });

            return Accepted(new AcceptExportDto
            {
                Message = "Request is accepted for process data. Please download data from url when it's ready.",
                FileName = fileName,
                DownloadUrl = apiDownloadUrl,
                FilePath = filePath
            });
        }
        /// <summary>
        /// Download user statistics file that has been exported asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Route("users/statistics/export/download", Name = "DownloadExportUserStatistics")]
        [HttpPost]
        public async Task<IActionResult> DownloadExportUserStatistics(string fileName)
        {
            return await DownloadFile(_appSettings.ExportUserStatisticsStorageSubFolder, fileName);
        }

        /// <summary>
        /// Get list of approving officers
        /// </summary>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="filterOnSubDepartment"></param>
        /// <param name="getRole"></param>
        /// <param name="getDepartment"></param>
        /// <param name="userCreatedAfter"></param>
        /// <param name="userCreatedBefore"></param>
        /// <param name="userEntityStatuses"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("users/approvingofficers")]
        [ProducesResponseType(typeof(PaginatedList<ApprovingOfficerInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApprovingOfficers(
            [FromQuery] List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            bool getRole,
            bool getDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? countMemberCreatedAfter,
            DateTime? countMemberCreatedBefore,
            [FromQuery] List<EntityStatusEnum> userEntityStatuses,
            int pageIndex = 1,
            int pageSize = 100)
        {
            var paginatedApprovingOfficerInfos = await GetPaginatedApprovingOfficerInfosAsync(_workContext, _userReportService,
                 parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                getRole: getRole,
                getDepartment: getDepartment,
                userCreatedAfter: userCreatedAfter,
                userCreatedBefore: userCreatedBefore,
                countMemberCreatedAfter: countMemberCreatedAfter,
                countMemberCreatedBefore: countMemberCreatedBefore,
                userEntityStatuses: userEntityStatuses,
                pageIndex: pageIndex,
                pageSize: pageSize,
                skipPaging: false);

            return Ok(paginatedApprovingOfficerInfos);
        }

      
        /// <summary>
        /// Export list of approving officers synchronously. 
        /// </summary>
        /// <param name="exportApprovingOfficerDto"></param>
        /// <returns></returns>
        [Route("users/approvingofficers/export")]
        [HttpPost]
        public async Task<IActionResult> ExportApprovingOfficers([FromBody] [Required] ExportApprovingOfficerDto exportApprovingOfficerDto)
        {
            if (exportApprovingOfficerDto.SendEmail)
            {
                throw new InvalidException("Do not support sending email when exporting approving officers with this action");
            }

            var fileExtension = GetFileExtension(exportApprovingOfficerDto.ExportOption.ExportType);
            var fileName = GenerateApprovingOfficerExportFileName(fileExtension);           

            _logger.LogInformation($"Start processing for exporting approving officers into {fileName}.");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var exportedData = await ExecuteExportingApprovingOfficers(_logger, exportApprovingOfficerDto, _workContext, _userReportService, _exportApprovingOfficerService, false);

            watch.Stop();
            _logger.LogInformation($"End processing for exporting approving officers. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");
            var contentType = GetContentTypeForFileExtension(fileExtension);

            return File(exportedData.Data, contentType, fileName);
        }

        /// <summary>
        /// Export list of approving officer asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportApprovingOfficerDto"></param>
        /// <returns></returns>
        [Route("users/approvingofficers/export/async")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public IActionResult ExportApprovingOfficersAsync([FromBody][Required] ExportApprovingOfficerDto exportApprovingOfficerDto)
        {
         
            var fileExtension = GetFileExtension(exportApprovingOfficerDto.ExportOption.ExportType);
            var fileName = GenerateApprovingOfficerExportFileName(fileExtension);
            var filePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings, _appSettings.ExportApprovingOfficerStorageSubFolder,
                _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            //For thread safe
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //TODO: deal with IAdvancedWorkContext injection when calling async
            var apiDownloadUrl = this.Url.Link("DownloadExportApprovingOfficers", new { fileName = fileName });
            var fallBackLanguageCode = _appSettings.FallBackLanguageCode;
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<ReportsController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            scopedLogger.LogInformation($"Start processing for exporting approving officers to {fileName}.");
                            var scopeUserService =
                                serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(
                                    ArchetypeEnum
                                        .Unknown);
                            var scopeExportService =
                                serviceScope.ServiceProvider.GetService<IExportService<ApprovingOfficerInfo>>();

                            var scopeUserEventLogService =
                                serviceScope.ServiceProvider.GetService<IUserReportService>();

                            var scopeFileStorageService =
                                serviceScope.ServiceProvider.GetService<IFileStorageService>();


                            var exportedData = await ExecuteExportingApprovingOfficers(scopedLogger,
                                exportApprovingOfficerDto,
                                copiedWorkContext, scopeUserEventLogService, scopeExportService, true);

                            var uploadedFilePath =  await scopeFileStorageService.UploadFileAsync(copiedWorkContext, exportedData.Data, filePath);
                            if (!string.IsNullOrEmpty(uploadedFilePath))
                            {

                                scopedLogger.LogInformation(
                                    $"Exporting data has been saved to storage under path {filePath}");

                                if (exportApprovingOfficerDto.SendEmail)
                                {
                                    SendEmailWhenExportingApprovingOfficers(exportApprovingOfficerDto,
                                        copiedWorkContext, scopeUserService, scopedLogger, serviceScope, apiDownloadUrl,
                                        filePath, fallBackLanguageCode);
                                }
                            }
                            else
                            {
                                scopedLogger.LogError($"Failed to save exporting data to storage under path {filePath}");
                            }

                            watch.Stop();
                            scopedLogger.LogInformation($"End processing for exporting approving officers. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");

                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e, $"Unexpected error occurred when processing for exporting approving officers to {fileName}. {e.Message}");
                        }

                    }
                }
            });

            return Accepted(new AcceptExportDto
            {
                Message = "Request is accepted for process data. Please download data from url when it's ready.",
                FileName = fileName,
                DownloadUrl = apiDownloadUrl,
                FilePath = filePath
            });
        }

        /// <summary>
        /// Download approving officers file that has been exported asynchronously
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>

        [Route("users/approvingofficers/export/download", Name = "DownloadExportApprovingOfficers")]
        [HttpPost]
        public async Task<IActionResult> DownloadExportApprovingOfficers(string fileName)
        {
            return await DownloadFile(_appSettings.ExportApprovingOfficerStorageSubFolder, fileName);
        }

        [HttpGet("users/accountsummary")]
        [ProducesResponseType(typeof(PaginatedList<UserAccountDetailsInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAccountDetails(
            [FromQuery] List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            [FromQuery]  List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            int pageSize = 100,
            int pageIndex = 0)
        {
            if (Request.ShouldLimitPageSize())
            {
                pageSize = _appSettings.LimitUserPageSize(pageSize);
            }

            var paginatedUserAccountDetailsInfos = await _userReportService.GetPaginatedUserAccountDetailsInfosAsync(
                workContext: _workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore,
                firstLoginAfter: firstLoginAfter,
                firstLoginBefore: firstLoginBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore,
                onboardingAfter: onboardingAfter,
                onboardingBefore: onboardingBefore,
                pageSize: pageSize,
                pageIndex: pageIndex);

            return Ok(paginatedUserAccountDetailsInfos);
        }

        /// <summary>
        /// Export list of user account details asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportUserAccountDetailsDto"></param>
        /// <returns></returns>
        [Route("users/accountsummary/export")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ExportUserAccountDetails([FromBody][Required] ExportUserAccountDetailsDto exportUserAccountDetailsDto)
        {

            if (exportUserAccountDetailsDto.SendEmail)
            {
                throw new InvalidException("Do not support sending email when exporting user account details with this action");
            }

            var fileExtension = GetFileExtension(exportUserAccountDetailsDto.ExportOption.ExportType);
            var fileName = GenerateUserAccountDetailsFileName(fileExtension);

            _logger.LogInformation($"Start processing for exporting user account details into {fileName}.");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var exportedData = await ExecuteUserAccountDetails(_logger, exportUserAccountDetailsDto, _workContext, _userReportService, _exportUserAccountDetailsService);

            watch.Stop();
            _logger.LogInformation($"End processing for exporting user account details. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");
            var contentType = GetContentTypeForFileExtension(fileExtension);

            return File(exportedData.Data, contentType, fileName);
        }

        /// <summary>
        /// Export list of user account details asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportUserAccountDetailsDto"></param>
        /// <returns></returns>
        // This route used to be [Route("users/accountsummary/export/async")] due to requirement changes. We change the route into "users/accountDetails/export/async"
        [Route("users/accountDetails/export/async")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public  IActionResult ExportUserAccountDetailsAsync(
            [FromBody] [Required] ExportUserAccountDetailsDto exportUserAccountDetailsDto)
        {

            var fileExtension = GetFileExtension(exportUserAccountDetailsDto.ExportOption.ExportType);
            var fileName = GenerateUserAccountDetailsFileName(fileExtension);
            var filePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings,
                _appSettings.ExportUserAccountDetailsStorageSubFolder,
                _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            //For thread safe
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //TODO: deal with IAdvancedWorkContext injection when calling async
            var apiDownloadUrl = this.Url.Link("DownloadExportUserAccountDetails", new {fileName = fileName});
            var fallBackLanguageCode = _appSettings.FallBackLanguageCode;
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<ReportsController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            scopedLogger.LogInformation(
                                $"Start processing for user account details to {fileName}.");
                            var scopeUserService =
                                serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(
                                    ArchetypeEnum
                                        .Unknown);
                            var scopeExportService =
                                serviceScope.ServiceProvider.GetService<IExportService<UserAccountDetailsInfo>>();

                            var scopeUserEventLogService =
                                serviceScope.ServiceProvider.GetService<IUserReportService>();

                            var scopeFileStorageService =
                                serviceScope.ServiceProvider.GetService<IFileStorageService>();

                            var exportedData = await ExecuteUserAccountDetails(scopedLogger,
                                exportUserAccountDetailsDto,
                                copiedWorkContext, scopeUserEventLogService, scopeExportService);

                            var uploadedFilePath =
                                await scopeFileStorageService.UploadFileAsync(copiedWorkContext, exportedData.Data,
                                    filePath);
                            if (!string.IsNullOrEmpty(uploadedFilePath))
                            {

                                scopedLogger.LogInformation(
                                    $"Exporting data has been saved to storage under path {filePath}");

                                if (exportUserAccountDetailsDto.SendEmail)
                                {
                                    SendEmailWhenExportingUserAccountDetails(exportUserAccountDetailsDto,
                                        copiedWorkContext, scopeUserService, scopedLogger, serviceScope, apiDownloadUrl,
                                        filePath, fallBackLanguageCode);
                                }
                            }
                            else
                            {
                                scopedLogger.LogError(
                                    $"Failed to save exporting data to storage under path {filePath}");
                            }

                            watch.Stop();
                            scopedLogger.LogInformation(
                                $"End processing for exporting user account details. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");

                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e,
                                $"Unexpected error occurred when processing for exporting user account details to {fileName}. {e.Message}");
                        }

                    }
                }
            });
            return Accepted(new AcceptExportDto
            {
                Message = "Request is accepted for process data. Please download data from url when it's ready.",
                FileName = fileName,
                DownloadUrl = apiDownloadUrl,
                FilePath = filePath
            });
        }

        [Route("users/accountsummary/export/download", Name = "DownloadExportUserAccountDetails")]
        [HttpPost]
        public async Task<IActionResult> DownloadExportUserAccountDetails(string fileName)
        {
            return await DownloadFile(_appSettings.ExportUserAccountDetailsStorageSubFolder, fileName);
        }
        private async Task<IActionResult> DownloadFile(string subfolder, string fileName)
        {
            var fullFilePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings, subfolder, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            var fileContent = await _fileStorageService.DownloadFileAsync(fullFilePath);

            var contentType = GetContentTypeForFileExtension(System.IO.Path.GetExtension(fileName));
            var ms = new MemoryStream();
            fileContent.CopyTo(ms);

            return File(ms.ToArray(), contentType, fileName);
        }


        [HttpGet("users/privilegedaccount")]
        [ProducesResponseType(typeof(PaginatedList<UserAccountDetailsInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPrivilegedUserAccount(
            [FromQuery] List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            [FromQuery] List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null, 
            int pageSize = 100, 
            int pageIndex = 0,
            bool? getDepartmentPathName = null)
        {
            if (Request.ShouldLimitPageSize())
            {
                pageSize = _appSettings.LimitUserPageSize(pageSize);
            }

            var paginatedApprovingOfficerInfos = await _userReportService.GetPaginatedPrivilegedUserAccountInfosAsync(
                workContext: _workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore,
                firstLoginAfter: firstLoginAfter,
                firstLoginBefore: firstLoginBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore,
                onboardingAfter: onboardingAfter,
                onboardingBefore: onboardingBefore,
                pageSize: pageSize, 
                pageIndex: pageIndex, 
                orderBy: null,
                needDepartmentPathName: getDepartmentPathName);

            return Ok(paginatedApprovingOfficerInfos);
        }
        /// <summary>
        /// Export list of privileged account asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportPrivilegedUserAccountDto"></param>
        /// <returns></returns>
        [Route("users/privilegedaccount/export")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ExportPrivilegedUserAccount([FromBody][Required] ExportPrivilegedUserAccountDto exportPrivilegedUserAccountDto)
        {

            if (exportPrivilegedUserAccountDto.SendEmail)
            {
                throw new InvalidException("Do not support sending email when exporting privileged user account with this action");
            }

            var fileExtension = GetFileExtension(exportPrivilegedUserAccountDto.ExportOption.ExportType);
            var fileName = GeneratePrivilegedUserAccountFileName(fileExtension);

            _logger.LogInformation($"Start processing for exporting privileged user account {fileName}.");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var exportedData = await ExecutePrivilegedUserAccount(_logger, exportPrivilegedUserAccountDto, _workContext, _userReportService, _exportPrivilegedUserAccountService);

            watch.Stop();
            _logger.LogInformation($"End processing for exporting privileged user account . {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");
            var contentType = GetContentTypeForFileExtension(fileExtension);

            return File(exportedData.Data, contentType, fileName);
        }
        /// <summary>
        /// Export list of privileged account asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportPrivilegedUserAccountDto"></param>
        /// <returns></returns>
        [Route("users/privilegedaccount/export/async")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public IActionResult ExportPrivilegedUserAccountAsync(
            [FromBody][Required] ExportPrivilegedUserAccountDto exportPrivilegedUserAccountDto)
        {

            var fileExtension = GetFileExtension(exportPrivilegedUserAccountDto.ExportOption.ExportType);
            var fileName = GeneratePrivilegedUserAccountFileName(fileExtension);
            var filePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings,
                _appSettings.ExportPrivilegedUserAccountStorageSubFolder,
                _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            //For thread safe
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //TODO: deal with IAdvancedWorkContext injection when calling async
            var apiDownloadUrl = this.Url.Link("DownloadExportPrivilegedUserAccount", new { fileName = fileName });
            var fallBackLanguageCode = _appSettings.FallBackLanguageCode;
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<ReportsController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            scopedLogger.LogInformation(
                                $"Start processing for privileged user account to {fileName}.");
                            var scopeUserService =
                                serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(
                                    ArchetypeEnum
                                        .Unknown);
                            var scopeExportService =
                                serviceScope.ServiceProvider.GetService<IExportService<PrivilegedUserAccountInfo>>();

                            var scopeUserEventLogService =
                                serviceScope.ServiceProvider.GetService<IUserReportService>();

                            var scopeFileStorageService =
                                serviceScope.ServiceProvider.GetService<IFileStorageService>();

                            var exportedData = await ExecutePrivilegedUserAccount(scopedLogger,
                                exportPrivilegedUserAccountDto,
                                copiedWorkContext, scopeUserEventLogService, scopeExportService);

                            var uploadedFilePath =
                                await scopeFileStorageService.UploadFileAsync(copiedWorkContext, exportedData.Data,
                                    filePath);
                            if (!string.IsNullOrEmpty(uploadedFilePath))
                            {

                                scopedLogger.LogInformation(
                                    $"Exporting data has been saved to storage under path {filePath}");

                                if (exportPrivilegedUserAccountDto.SendEmail)
                                {
                                    SendEmailWhenExportingPrivilegedUserAccount(exportPrivilegedUserAccountDto,
                                        copiedWorkContext, scopeUserService, scopedLogger, serviceScope, apiDownloadUrl,
                                        filePath, fallBackLanguageCode);
                                }
                            }
                            else
                            {
                                scopedLogger.LogError(
                                    $"Failed to save exporting data to storage under path {filePath}");
                            }

                            watch.Stop();
                            scopedLogger.LogInformation(
                                $"End processing for exporting privileged user account. {exportedData.ExportedUserCount} of {exportedData.TotalUserCount} entries have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");

                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e,
                                $"Unexpected error occurred when processing for exporting privileged user account to {fileName}. {e.Message}");
                        }

                    }
                }
            });
            return Accepted(new AcceptExportDto
            {
                Message = "Request is accepted for process data. Please download data from url when it's ready.",
                FileName = fileName,
                DownloadUrl = apiDownloadUrl,
                FilePath = filePath
            });
        }

        [Route("users/privilegedaccount/export/download", Name = "DownloadExportPrivilegedUserAccount")]
        [HttpPost]
        public async Task<IActionResult> DownloadExportPrivilegedUserAccount(string fileName)
        {
            return await DownloadFile(_appSettings.ExportUserAccountDetailsStorageSubFolder, fileName);
        }
        private static async Task<PaginatedList<ApprovingOfficerInfo>> GetPaginatedApprovingOfficerInfosAsync(
            IAdvancedWorkContext workContext, IUserReportService userReportService,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            bool getRole,
            bool getDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? countMemberCreatedAfter,
            DateTime? countMemberCreatedBefore,
            List<EntityStatusEnum> userEntityStatuses,
            int pageIndex, int pageSize, bool skipPaging)
        {
            if (skipPaging)
            {
                var items = await userReportService.GetApprovingOfficerInfosAsync(workContext,
                    parentDepartmentIds: parentDepartmentIds,
                    filterOnSubDepartment: filterOnSubDepartment,
                    getRole: getRole,
                    getDepartment: getDepartment,
                    userCreatedAfter: userCreatedAfter,
                    userCreatedBefore: userCreatedBefore,
                    countMemberCreatedAfter: countMemberCreatedAfter,
                    countMemberCreatedBefore: countMemberCreatedBefore,
                    userEntityStatuses: userEntityStatuses);
                return new PaginatedList<ApprovingOfficerInfo>(items, 1, items.Count, false)
                {
                    TotalItems = items.Count
                };
            }

            return await userReportService.GetPaginatedApprovingOfficerInfosAsync(workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                getRole: getRole,
                getDepartment: getDepartment,
                userCreatedAfter: userCreatedAfter,
                userCreatedBefore: userCreatedBefore,
                countMemberCreatedAfter: countMemberCreatedAfter,
                countMemberCreatedBefore: countMemberCreatedBefore,
                userEntityStatuses: userEntityStatuses,
                pageIndex: pageIndex,
                pageSize: pageSize);
        }

        private static async Task<PaginatedList<UserAccountDetailsInfo>> GetPaginatedUserAccountDetailsInfosAsync(
            IAdvancedWorkContext workContext, IUserReportService userReportService,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? lastLoginAfter,
            DateTime? lastLoginBefore,
            List<EntityStatusEnum> userEntityStatuses,
            int pageIndex, int pageSize, bool skipPaging)
        {
            if (skipPaging)
            {
                var items = await userReportService.GetUserAccountDetailsInfosAsync(workContext,
                    parentDepartmentIds: parentDepartmentIds,
                    filterOnSubDepartment: filterOnSubDepartment,
                    userEntityStatuses: userEntityStatuses,
                    createdAfter: userCreatedAfter,
                    createdBefore: userCreatedBefore,
                    lastLoginAfter: lastLoginAfter,
                    lastLoginBefore: lastLoginBefore);
                return new PaginatedList<UserAccountDetailsInfo>(items, 1, items.Count, false)
                {
                    TotalItems = items.Count
                };
            }

            return await userReportService.GetPaginatedUserAccountDetailsInfosAsync(workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: userCreatedAfter,
                createdBefore: userCreatedBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore, pageSize: pageSize,
                pageIndex: pageIndex);
        }
        private static async Task<PaginatedList<PrivilegedUserAccountInfo>> GetPaginatedPrivilegedUserAccountInfosAsync(
            IAdvancedWorkContext workContext, IUserReportService userReportService,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? lastLoginAfter,
            DateTime? lastLoginBefore,
            List<EntityStatusEnum> userEntityStatuses,
            int pageIndex, int pageSize, bool skipPaging,
            bool needDepartmentPathName)
        {
            if (skipPaging)
            {
                var items = await userReportService.GetPrivilegedUserAccountInfosAsync(workContext,
                    parentDepartmentIds: parentDepartmentIds,
                    filterOnSubDepartment: filterOnSubDepartment,
                    userEntityStatuses: userEntityStatuses,
                    createdAfter: userCreatedAfter,
                    createdBefore: userCreatedBefore,
                    lastLoginAfter: lastLoginAfter,
                    lastLoginBefore: lastLoginBefore,
                    needDepartmentPathName: needDepartmentPathName);
                return new PaginatedList<PrivilegedUserAccountInfo>(items, 1, items.Count, false)
                {
                    TotalItems = items.Count
                };
            }

            return await userReportService.GetPaginatedPrivilegedUserAccountInfosAsync(workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: userCreatedAfter,
                createdBefore: userCreatedBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore, pageSize: pageSize,
                pageIndex: pageIndex,
                needDepartmentPathName: needDepartmentPathName);
        }
        private static void SetDefaultDatetimeIfNull(ref DateTime? fromDate, ref DateTime? toDate)
        {
            if (fromDate == null && toDate == null)
            {
                //If there is no given on datetime, we default get log in latest 7 days due to performance
                toDate = DateTime.Now;
                fromDate = toDate.Value.AddDays(-7);
            }
        }
        private static void AddDefaultInfoRecords(ExportOption exportOption, DateTime? fromDate, DateTime? toDate)
        {
            if (exportOption.AddDateTimeAsInfoRecords)
            {
                exportOption.InfoRecords = exportOption.InfoRecords ?? new List<InfoRecord>();
                const string fromDateCaption = "From Date";
                const string toDateCaption = "To Date";
                if (fromDate.HasValue && exportOption.InfoRecords.All(a => a.Caption != fromDateCaption))
                {
                    exportOption.InfoRecords.Add(new InfoRecord
                    {
                        Caption = fromDateCaption,
                        Value = fromDate.Value
                    });
                }
                if (toDate.HasValue && exportOption.InfoRecords.All(a => a.Caption != toDateCaption))
                {
                    exportOption.InfoRecords.Add(new InfoRecord
                    {
                        Caption = toDateCaption,
                        Value = toDate.Value
                    });
                }
            }
        }
        private static void AddDefaultInfoRecordsForExportingApprovingOfficer(ExportOption exportOption, 
            DateTime? createdfromDate, 
            DateTime? createdtoDate, 
            DateTime? countFromDate, 
            DateTime? countToDate)
        {
            if (exportOption.AddDateTimeAsInfoRecords)
            {
                exportOption.InfoRecords = exportOption.InfoRecords ?? new List<InfoRecord>();
                const string fromDateCaption = "Created From Date";
                const string toDateCaption = "Created To Date";
                const string countFromDateCaption = "Count From Date";
                const string countToDateCaption = "Count To Date";
                if (createdfromDate.HasValue && exportOption.InfoRecords.All(a => a.Caption != fromDateCaption))
                {
                    exportOption.InfoRecords.Add(new InfoRecord
                    {
                        Caption = fromDateCaption,
                        Value = createdfromDate.Value
                    });
                }
                if (createdtoDate.HasValue && exportOption.InfoRecords.All(a => a.Caption != toDateCaption))
                {
                    exportOption.InfoRecords.Add(new InfoRecord
                    {
                        Caption = toDateCaption,
                        Value = createdtoDate.Value
                    });
                }
                if (countFromDate.HasValue && exportOption.InfoRecords.All(a => a.Caption != countFromDateCaption))
                {
                    exportOption.InfoRecords.Add(new InfoRecord
                    {
                        Caption = countFromDateCaption,
                        Value = countFromDate.Value
                    });
                }
                if (countToDate.HasValue && exportOption.InfoRecords.All(a => a.Caption != countToDateCaption))
                {
                    exportOption.InfoRecords.Add(new InfoRecord
                    {
                        Caption = countToDateCaption,
                        Value = countToDate.Value
                    });
                }
            }
        }

        private static async Task<PaginatedList<UserEventLogInfo>> GetPaginatedEventInfosAsync(IAdvancedWorkContext workContext,
            IUserReportService userReportService, List<UserEventType> eventTypes,  DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore,
            int pageSize, int pageIndex, bool getDepartment, bool getRole, bool skipPaging)
        {

            if (skipPaging)
            {
                var eventLogs = await userReportService.GetUserEventLogInfosAsync(workContext, eventTypes,
                    eventCreatedAfter,
                    eventCreatedBefore, getDepartment, getRole);
                return new PaginatedList<UserEventLogInfo>(eventLogs, 1, eventLogs.Count, false)
                {
                    TotalItems = eventLogs.Count
                };
            }

            return await userReportService.GetPaginatedUserEventLogInfosAsync(workContext, eventTypes,
                eventCreatedAfter, eventCreatedBefore, pageSize, pageIndex, getDepartment, getRole);
        }

        private static async Task<(byte[]  Data, int ExportedUserCount, int TotalUserCount) > ExecuteExportingUserEvents(ILogger logger, ExportUserEventLogInfoDto exportUserDto,
            IAdvancedWorkContext workContext, IUserReportService userReportService, IExportService<UserEventLogInfo> exportService, bool isAsync)
        {
            bool skipPaging = exportUserDto.PageIndex <= 0 && exportUserDto.PageSize <= 0;

            if (exportUserDto.ExportOption.ExportFields == null ||
                exportUserDto.ExportOption.ExportFields.Count == 0)
            {
                exportUserDto.ExportOption.ExportFields = ExportHelper.DefaultExportUserEventInfoFieldMappings;
            }
          
            var getDepartment = exportUserDto.ExportOption.ExportFields.NeedToGetDepartment();
            var getRole = exportUserDto.ExportOption.ExportFields.NeedToGetRole();

            var eventTypeAsString = string.Join(", ", exportUserDto.EventTypes);
            logger.LogInformation($"Start retrieving user event logs of {eventTypeAsString} for exporting");

            var eventCreatedAfter = exportUserDto.EventCreatedAfter;
            var eventCreatedBefore = exportUserDto.EventCreatedBefore;

            SetDefaultDatetimeIfNull(ref eventCreatedAfter, ref eventCreatedBefore);

            var paginatedEventLogInfos = await GetPaginatedEventInfosAsync(workContext, userReportService, exportUserDto.EventTypes,
                eventCreatedAfter, eventCreatedBefore,
                exportUserDto.PageSize, exportUserDto.PageIndex,
                getDepartment, getRole, skipPaging);

            var exportedUserCount = paginatedEventLogInfos.Items.Count;
            var totalUserCount = paginatedEventLogInfos.TotalItems;

            logger.LogInformation($"End retrieving user event log of {eventTypeAsString} for exporting. {exportedUserCount} of {totalUserCount} entries have been retrieved.");

            AddDefaultInfoRecords(exportUserDto.ExportOption, eventCreatedAfter, eventCreatedBefore);

            logger.LogInformation($"Start exporting {exportedUserCount} user event log entries into file");

            var exportedData = exportService.ExportDataToBytes(paginatedEventLogInfos.Items, exportUserDto.ExportOption,
                workContext);

            logger.LogInformation($"End exporting {exportedUserCount} user event log entries into file");

            return (exportedData, exportedUserCount, totalUserCount);
        }
        private static async Task<byte[]> ExecuteExportingUserStatisticsAsync(ILogger logger, ExportUserStatisticsDto exportUserDto,
            IAdvancedWorkContext workContext, IUserReportService userReportService, IExportService<UserStatisticsDto> exportService)
        {

            if (exportUserDto.ExportOption.ExportFields == null ||
                exportUserDto.ExportOption.ExportFields.Count == 0)
            {
                exportUserDto.ExportOption.ExportFields = ExportHelper.DefaultExportUserStatisticsFieldMappings;
            }

            if (exportUserDto.ExportOption.VerticalExportFields == null ||
                exportUserDto.ExportOption.VerticalExportFields.Count == 0)
            {
                exportUserDto.ExportOption.VerticalExportFields = ExportHelper.DefaultExportUserStatisticsVerticalFieldMappings;
            }

            var verticalExportFields = exportUserDto.ExportOption.VerticalExportFields.Keys.ToList();
          
            var getOnBoardingStatistics =
                verticalExportFields.Any(f => f.StartsWith($"{nameof(UserStatisticsDto.OnBoardingStatistics)}.", StringComparison.CurrentCultureIgnoreCase));
           
            var accountStatisticsPrefix = $"{nameof(UserStatisticsDto.AccountStatistics)}.";
          
            var userEventTypes = ExtractEventStatisticsEventTypes(verticalExportFields);

            var accountStatisticsEntityStatuses = ExtractAccountStatisticsEntityStatuses(verticalExportFields, accountStatisticsPrefix);

            var fromDate = exportUserDto.FromDate;
            var toDate = exportUserDto.ToDate;

            SetDefaultDatetimeIfNull(ref fromDate, ref toDate);

            logger.LogInformation($"Start retrieving user statistics for exporting.");

            var userStatistics = await userReportService.GetUserStatisticsAsync(workContext, accountStatisticsEntityStatuses, userEventTypes, getOnBoardingStatistics, fromDate, toDate);      

            logger.LogInformation($"End retrieving user event statistics for exporting.");
          
            AddDefaultInfoRecords(exportUserDto.ExportOption, fromDate, toDate);
           
            logger.LogInformation($"Start exporting user statistics into file");
             
            var exportedData = exportService.ExportDataToBytes(userStatistics, exportUserDto.ExportOption, workContext);

            logger.LogInformation($"End exporting user statistics into file");

            return exportedData;
        }

        private static List<EntityStatusEnum> ExtractAccountStatisticsEntityStatuses(List<string> verticalExportFields, string accountStatisticsPrefix)
        {
            var accountStatisticsEntityStatusNames = verticalExportFields.Where(f =>
                    f.StartsWith(accountStatisticsPrefix, StringComparison.CurrentCultureIgnoreCase))
                .Select(e => e.Remove(0, accountStatisticsPrefix.Length))
                .Distinct()
                .ToList();

            var accountStatisticsEntityStatuses = new List<EntityStatusEnum>();
            foreach (var entityStatusName in accountStatisticsEntityStatusNames)
            {
                if (Enum.TryParse(typeof(EntityStatusEnum), entityStatusName, true, out object entityStatusObj))
                {
                    accountStatisticsEntityStatuses.Add((EntityStatusEnum) entityStatusObj);
                }
            }

            return accountStatisticsEntityStatuses;
        }

        private static List<UserEventType> ExtractEventStatisticsEventTypes(List<string> verticalExportFields)
        {
           var eventStatisticsPrefix = $"{nameof(UserStatisticsDto.EventStatistics)}.";

            var userEventTypeNames = verticalExportFields.Where(f =>
                    f.StartsWith(eventStatisticsPrefix, StringComparison.CurrentCultureIgnoreCase))
                .Select(e => e.Remove(0, eventStatisticsPrefix.Length).Split('.').FirstOrDefault())
                .Distinct().ToList();

            var userEventTypes = new List<UserEventType>();
            foreach (var userEventTypeName in userEventTypeNames)
            {
                if (Enum.TryParse(typeof(UserEventType), userEventTypeName, true, out object evenTypeObj))
                {
                    userEventTypes.Add((UserEventType) evenTypeObj);
                }
            }

            return userEventTypes;
        }

        private static async Task<(byte[] Data,  int ExportedUserCount, int TotalUserCount)> ExecuteExportingApprovingOfficers(ILogger logger, ExportApprovingOfficerDto exportUserDto,
             IAdvancedWorkContext workContext,
             IUserReportService userReportService, 
             IExportService<ApprovingOfficerInfo> exportService, bool isAsync)
        {

            bool skipPaging = exportUserDto.PageIndex <= 0 && exportUserDto.PageSize <= 0;

            if (exportUserDto.ExportOption.ExportFields == null ||
                exportUserDto.ExportOption.ExportFields.Count == 0)
            {
                exportUserDto.ExportOption.ExportFields = ExportHelper.DefaultApprovingOfficerFieldMappings;
            }

            var getDepartment = exportUserDto.ExportOption.ExportFields.NeedToGetDepartment();
            var getRole = exportUserDto.ExportOption.ExportFields.NeedToGetRole();

            logger.LogInformation($"Start retrieving approving officers for exporting");

            var userCreatedAfter = exportUserDto.UserCreatedAfter;
            var userCreatedBefore = exportUserDto.UserCreatedBefore;

            var paginatedEventLogInfos = await GetPaginatedApprovingOfficerInfosAsync(workContext, userReportService,
                exportUserDto.ParentDepartmentIds,
                exportUserDto.FilterOnSubDepartment,
                getRole,
                getDepartment,
                exportUserDto.UserCreatedAfter,
                exportUserDto.UserCreatedBefore,
                exportUserDto.CountMemberCreatedAfter,
                exportUserDto.CountMemberCreatedBefore,
                exportUserDto.UserEntityStatuses,
                exportUserDto.PageIndex,
                exportUserDto.PageSize,
                skipPaging);

            var exportedUserCount = paginatedEventLogInfos.Items.Count;
            var totalUserCount = paginatedEventLogInfos.TotalItems;

            logger.LogInformation($"End retrieving approving officers for exporting. {exportedUserCount} of {totalUserCount} entries have been retrieved.");

            AddDefaultInfoRecordsForExportingApprovingOfficer(exportUserDto.ExportOption, userCreatedAfter, userCreatedBefore, exportUserDto.CountMemberCreatedAfter, exportUserDto.CountMemberCreatedBefore);

            logger.LogInformation($"Start exporting {exportedUserCount} approving officers into file");

            var exportedData = exportService.ExportDataToBytes(paginatedEventLogInfos.Items, exportUserDto.ExportOption,
                workContext);

            logger.LogInformation($"End exporting {exportedUserCount} approving officers into file");

            return (exportedData, exportedUserCount, totalUserCount);
        }
        private async Task<(byte[] Data, int ExportedUserCount, int TotalUserCount)> ExecuteUserAccountDetails(ILogger logger, ExportUserAccountDetailsDto exportUserAccountDetailsDto,
             IAdvancedWorkContext workContext,
             IUserReportService userReportService,
             IExportService<UserAccountDetailsInfo> exportService)
        {

            bool skipPaging = exportUserAccountDetailsDto.PageIndex <= 0 && exportUserAccountDetailsDto.PageSize <= 0;

            if (exportUserAccountDetailsDto.ExportOption.ExportFields == null ||
                exportUserAccountDetailsDto.ExportOption.ExportFields.Count == 0)
            {
                exportUserAccountDetailsDto.ExportOption.ExportFields = ExportHelper.DefaultUserAccountDetailsFieldMappings;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            logger.LogInformation($"Start retrieving user account details infos for exporting");

            var userCreatedAfter = exportUserAccountDetailsDto.UserCreatedAfter;
            var userCreatedBefore = exportUserAccountDetailsDto.UserCreatedBefore;

            var paginatedEventLogInfos = await GetPaginatedUserAccountDetailsInfosAsync(workContext, userReportService,
                exportUserAccountDetailsDto.ParentDepartmentIds,
                exportUserAccountDetailsDto.FilterOnSubDepartment,
                userCreatedAfter: exportUserAccountDetailsDto.UserCreatedAfter,
                userCreatedBefore: exportUserAccountDetailsDto.UserCreatedBefore,
                lastLoginAfter: exportUserAccountDetailsDto.LastLoginAfter,
                lastLoginBefore: exportUserAccountDetailsDto.LastLoginBefore,
                userEntityStatuses: exportUserAccountDetailsDto.UserEntityStatuses,
                pageIndex: exportUserAccountDetailsDto.PageIndex,
                pageSize: exportUserAccountDetailsDto.PageSize,
                skipPaging: skipPaging);
            var exportedUserCount = paginatedEventLogInfos.Items.Count;
            var totalUserCount = paginatedEventLogInfos.TotalItems;

            watch.Stop();
            logger.LogInformation($"End retrieving user account details for exporting. {exportedUserCount} of {totalUserCount} entries have been retrieved in {watch.ElapsedMilliseconds}ms.");

            AddDefaultInfoRecords(exportUserAccountDetailsDto.ExportOption, userCreatedAfter, userCreatedBefore);
           
            watch.Start();
            logger.LogInformation($"Start exporting {exportedUserCount} user account details into file");

            byte[] exportedData;
            if (exportUserAccountDetailsDto.SeparatedByAccountType)
            {
                var externalMasteredUsers =
                    paginatedEventLogInfos.Items.Where(i => i.AccountType == AccountType.ExternalMastered).ToList();
                var nonExternalMasteredUsers = paginatedEventLogInfos.Items.Except(externalMasteredUsers).ToList();

                var exportSource = new Dictionary<string, List<UserAccountDetailsInfo>>
                {
                    {_appSettings.ExternallyMasteredUserReportDisplayText, externalMasteredUsers},
                    {_appSettings.NonExternallyMasteredUserReportDisplayText, nonExternalMasteredUsers}
                };
             
                exportedData = exportService.ExportDataToBytes(exportSource,
                    exportUserAccountDetailsDto.ExportOption,
                    workContext);
            }
            else
            {
                exportedData = exportService.ExportDataToBytes(paginatedEventLogInfos.Items,
                    exportUserAccountDetailsDto.ExportOption,
                    workContext);
            }
            watch.Stop();

            logger.LogInformation($"End exporting {exportedUserCount} user account details  into file in {watch.ElapsedMilliseconds}ms.");

            return (exportedData, exportedUserCount, totalUserCount);
        }
        private async Task<(byte[] Data, int ExportedUserCount, int TotalUserCount)> ExecutePrivilegedUserAccount(ILogger logger, ExportPrivilegedUserAccountDto exportPrivilegedUserAccountDto,
            IAdvancedWorkContext workContext,
            IUserReportService userReportService,
            IExportService<PrivilegedUserAccountInfo> exportService)
        {

            bool skipPaging = exportPrivilegedUserAccountDto.PageIndex <= 0 && exportPrivilegedUserAccountDto.PageSize <= 0;

            if (exportPrivilegedUserAccountDto.ExportOption.ExportFields == null ||
                exportPrivilegedUserAccountDto.ExportOption.ExportFields.Count == 0)
            {
                exportPrivilegedUserAccountDto.ExportOption.ExportFields = ExportHelper.DefaultPrivilegedUserAccountFieldMappings;
            }

            var needDepartmentPathName = exportPrivilegedUserAccountDto.ExportOption.ExportFields.Keys.Contains(
                nameof(PrivilegedUserAccountInfo.DepartmentPathName), StringComparer.CurrentCultureIgnoreCase);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            logger.LogInformation($"Start retrieving privileged user account infos for exporting");

            var userCreatedAfter = exportPrivilegedUserAccountDto.UserCreatedAfter;
            var userCreatedBefore = exportPrivilegedUserAccountDto.UserCreatedBefore;

            var paginatedEventLogInfos = await GetPaginatedPrivilegedUserAccountInfosAsync(workContext,
                userReportService,
                exportPrivilegedUserAccountDto.ParentDepartmentIds,
                exportPrivilegedUserAccountDto.FilterOnSubDepartment,
                userCreatedAfter: exportPrivilegedUserAccountDto.UserCreatedAfter,
                userCreatedBefore: exportPrivilegedUserAccountDto.UserCreatedBefore,
                lastLoginAfter: exportPrivilegedUserAccountDto.LastLoginAfter,
                lastLoginBefore: exportPrivilegedUserAccountDto.LastLoginBefore,
                userEntityStatuses: exportPrivilegedUserAccountDto.UserEntityStatuses,
                pageIndex: exportPrivilegedUserAccountDto.PageIndex,
                pageSize: exportPrivilegedUserAccountDto.PageSize,
                skipPaging: skipPaging,
                needDepartmentPathName: needDepartmentPathName);

            var exportedUserCount = paginatedEventLogInfos.Items.Count;
            var totalUserCount = paginatedEventLogInfos.TotalItems;

            watch.Stop();
            logger.LogInformation($"End retrieving privileged user account for exporting. {exportedUserCount} of {totalUserCount} entries have been retrieved in {watch.ElapsedMilliseconds}ms.");

            AddDefaultInfoRecords(exportPrivilegedUserAccountDto.ExportOption, userCreatedAfter, userCreatedBefore);

            watch.Start();
            logger.LogInformation($"Start exporting {exportedUserCount} privileged user account into file");

            byte[] exportedData;
            if (exportPrivilegedUserAccountDto.SeparatedByAccountType)
            {
                var externalMasteredUsers =
                    paginatedEventLogInfos.Items.Where(i => i.AccountType == AccountType.ExternalMastered).ToList();
                var nonExternalMasteredUsers = paginatedEventLogInfos.Items.Except(externalMasteredUsers).ToList();

                var exportSource = new Dictionary<string, List<PrivilegedUserAccountInfo>>
                {
                    {_appSettings.ExternallyMasteredUserReportDisplayText, externalMasteredUsers},
                    {_appSettings.NonExternallyMasteredUserReportDisplayText, nonExternalMasteredUsers}
                };

                exportedData = exportService.ExportDataToBytes(exportSource, exportPrivilegedUserAccountDto.ExportOption, workContext);
            }
            else
            {
                exportedData = exportService.ExportDataToBytes(paginatedEventLogInfos.Items,
                    exportPrivilegedUserAccountDto.ExportOption,
                    workContext);
            }
            watch.Stop();

            logger.LogInformation($"End exporting {exportedUserCount} privileged user account  into file in {watch.ElapsedMilliseconds}ms.");

            return (exportedData, exportedUserCount, totalUserCount);
        }
        private static string GetFileExtension(ExportType exportType)
        {
            if (exportType == ExportType.Excel)
                return FileExtension.ExcelOpenXML;
            return FileExtension.Csv;
        }

        private static void SendEmailWhenExportingUserEvents(ExportUserEventLogInfoDto exportUserDto,
            IAdvancedWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            var emailTemplates = serviceScope.ServiceProvider.GetService<IOptions<EmailTemplates>>().Value;
            var exportEmailTemplate = emailTemplates.ExportUserEventLogTemplate;
            if (exportEmailTemplate == null || exportEmailTemplate.Disabled)
            {
                logger.LogInformation("Sending email has been disabled for exporting user event logs by configuration");
                return;
            }

            SendEmailWhenExportingData(exportEmailTemplate,  exportUserDto.EmailOption, workContext, scopeUserService, logger, serviceScope,
                apiDownloadUrl, filePath, fallBackLanguageCode);
        }
        private static void SendEmailWhenExportingUserStatistics(ExportUserStatisticsDto exportUserDto,
            IAdvancedWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            var emailTemplates = serviceScope.ServiceProvider.GetService<IOptions<EmailTemplates>>().Value;
            var exportEmailTemplate = emailTemplates.ExportUserStatisticTemplate;
            if (exportEmailTemplate == null || exportEmailTemplate.Disabled)
            {
                logger.LogInformation("Sending email has been disabled for exporting user statistics by configuration");
                return;
            }

            SendEmailWhenExportingData(exportEmailTemplate, exportUserDto.EmailOption, workContext, scopeUserService, logger, serviceScope,
                apiDownloadUrl, filePath, fallBackLanguageCode);
        }
        private static void SendEmailWhenExportingApprovingOfficers(ExportApprovingOfficerDto exportUserDto,
            IAdvancedWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            var emailTemplates = serviceScope.ServiceProvider.GetService<IOptions<EmailTemplates>>().Value;
            var exportEmailTemplate = emailTemplates.ExportApprovingTemplate;
            if (exportEmailTemplate == null || exportEmailTemplate.Disabled)
            {
                logger.LogInformation("Sending email has been disabled for exporting approving officer by configuration");
                return;
            }

            SendEmailWhenExportingData(exportEmailTemplate, exportUserDto.EmailOption, workContext, scopeUserService, logger, serviceScope,
                apiDownloadUrl, filePath, fallBackLanguageCode);
        }
        private static void SendEmailWhenExportingUserAccountDetails(ExportUserAccountDetailsDto exportUserDto,
            IAdvancedWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            var emailTemplates = serviceScope.ServiceProvider.GetService<IOptions<EmailTemplates>>().Value;
            var exportEmailTemplate = emailTemplates.ExportUserAccountDetailsTemplate;
            if (exportEmailTemplate == null || exportEmailTemplate.Disabled)
            {
                logger.LogInformation("Sending email has been disabled for user account details by configuration");
                return;
            }

            SendEmailWhenExportingData(exportEmailTemplate, exportUserDto.EmailOption, workContext, scopeUserService, logger, serviceScope,
                apiDownloadUrl, filePath, fallBackLanguageCode);
        }
        private static void SendEmailWhenExportingPrivilegedUserAccount(ExportPrivilegedUserAccountDto exportUserDto,
            IAdvancedWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            var emailTemplates = serviceScope.ServiceProvider.GetService<IOptions<EmailTemplates>>().Value;
            var exportEmailTemplate = emailTemplates.ExportPrivilegedUserAccountTemplate;
            if (exportEmailTemplate == null || exportEmailTemplate.Disabled)
            {
                logger.LogInformation("Sending email has been disabled for privileged user account by configuration");
                return;
            }

            SendEmailWhenExportingData(exportEmailTemplate, exportUserDto.EmailOption, workContext, scopeUserService, logger, serviceScope,
                apiDownloadUrl, filePath, fallBackLanguageCode);
        }
        private static void SendEmailWhenExportingData(MultiLanguageEmailTemplate exportEmailTemplate,
            EmailOption emailOption,
            IAdvancedWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            try
            {
                var currentUser = DomainHelper.GetUserFromWorkContext(workContext, scopeUserService,
                    getRoles: false,
                    getLoginServiceClaims: true);
                if (currentUser == null)
                {
                    logger.LogWarning($"Unable to find login user with sub '{workContext.Sub}' for sending email.");
                }
                else
                {
                    emailOption = emailOption ?? new EmailOption();

                    var emailSubject = emailOption.Subject;

                    //If there is no given email subject, email body from client, we get from app setting of email templates
                    if (string.IsNullOrEmpty(emailSubject))
                    {
                        emailSubject = exportEmailTemplate.GetSubject(workContext.CurrentLocales, fallBackLanguageCode);
                    }

                    //deep clone template to do not change original data
                    var templateDate = exportEmailTemplate.CommunicationApiTemplate.DeepClone();

                    var dataKeys = templateDate.Data.Keys.ToList();

                    foreach (var dataProperty in dataKeys)
                    {
                        var propertyValue = templateDate.Data[dataProperty];
                        templateDate.Data[dataProperty] = propertyValue
                            .Replace("{filepath}", HttpUtility.UrlEncode(filePath))
                            .Replace("{UserFullName}", currentUser.GetFullName());
                    }

                    var scopedHierarchyDepartmentService =
                        serviceScope.ServiceProvider.GetService<IHierarchyDepartmentService>();
                    var scopedAppSetting = serviceScope.ServiceProvider.GetService<IOptions<AppSettings>>().Value;
                    var sendEmailCommand = DomainHelper.GenerateCommunicationCommand(workContext.CorrelationId,
                        scopeUserService,
                        executorUser: currentUser,
                        objectiveUser: null,
                        emailSubject: emailSubject,
                        communicationApiTemplate: templateDate,
                        routingAction: scopedAppSetting.EmailMessageRoutingAction,
                        sendEmailToDto: exportEmailTemplate.SendTo);

                    var scopedDatahubLog = serviceScope.ServiceProvider.GetService<IDatahubLogger>();
                    scopedDatahubLog.WriteCommandLog(sendEmailCommand, useMessageRoutingActionAsRoutingKey: true);

                    logger.LogInformation(
                        $"An command message of sending email '{emailSubject}' has been published when exporting data");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unexpected error occurred when sending email for exporting data. {e.Message}");
            }
        }

        private string GenerateUserStatisticsExportFileName(string fileExtension)
        {
            return GenerateFileName("user-statistics", fileExtension);
        }
        private string GenerateUserAuditExportFileName(string fileExtension)
        {
            return GenerateFileName("user-audit-events", fileExtension);
        }
        private string GenerateApprovingOfficerExportFileName(string fileExtension)
        {
            return GenerateFileName("approving-officers", fileExtension);
        }
        private string GenerateUserAccountDetailsFileName(string fileExtension)
        {
            return GenerateFileName("user-accounts-details", fileExtension);
        }
        private string GeneratePrivilegedUserAccountFileName(string fileExtension)
        {
            return GenerateFileName("privileged-user-account", fileExtension);
        }
        private string GenerateFileName(string prefix, string fileExtension)
        {
            return $"{prefix}-{Request.GetRequestId()}{fileExtension}";
        }
        private string GetContentTypeForFileExtension(string fileExtension)
        {
            if (FileExtension.FileTypeContentTypeMappings.TryGetValue(fileExtension, out var contentType))
            {
                return contentType;
            }
            return "application/json";
        }

    }


}
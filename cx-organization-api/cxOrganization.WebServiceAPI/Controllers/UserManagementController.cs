using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

using cxOrganization.Business.Exceptions;
using cxOrganization.Domain;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Services.ExportService;
using cxOrganization.Domain.Services.StorageServices;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;
using cxOrganization.WebServiceAPI.ActionFilters;
using cxOrganization.WebServiceAPI.Background;
using cxOrganization.WebServiceAPI.Extensions;
using cxOrganization.WebServiceAPI.Models;

using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Extentions.Request;
using cxPlatform.Core.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FileExtension = cxOrganization.WebServiceAPI.Models.FileExtension;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    [Route("usermanagement")]
    public partial class UserManagementController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly ILoginServiceUserService _loginServiceUserService;
        private readonly IIdentityServerClientService _identityServerClientService;
        private readonly IExportService<UserGenericDto> _exportService;

        private readonly ILogger<UserManagementController> _logger;
        private readonly ISuspendOrDeactiveUserBackgroundJob _suspendOrDeactiveUserBackgroundJob;
        private readonly IUGMemberService _uGMemberService;
        private readonly IUserGroupService _userGroupService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IFileStorageService _fileStorageService;
        private readonly IOptions<AppSettings> _appSettings;

        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IHierarchyDepartmentPermissionService _hierarchyDepartmentPermissionService;

        private readonly UserGenericMappingService _userGenericMappingService;
        private readonly IUserAccessService _userAccessService;
        private readonly IConfiguration _configuration;
        private readonly IDepartmentService _departmentService;
        private readonly IMassCreationUserService _massCreationUserService;
        public UserManagementController(
            Func<ArchetypeEnum, IUserService> userService,
            IWorkContext workContext,
            ISuspendOrDeactiveUserBackgroundJob suspendOrDeactiveUserBackgroundJob,
            IUserGroupService userGroupService,
            ILoginServiceUserService loginServiceUserService,
            IIdentityServerClientService identityServerClientService,
            IExportService<UserGenericDto> exportService,
            ILogger<UserManagementController> logger,
            IUGMemberService uGMemberService,
            IServiceScopeFactory serviceScopeFactory,
            IFileStorageService fileStorageService,
            IOptions<AppSettings> appSettingOptions,
            IBackgroundTaskQueue backgroundTaskQueue,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService,
            UserGenericMappingService userGenericMappingService,
            IUserAccessService userAccessService,
            IMassCreationUserService massCreationUserService,
            IConfiguration configuration,
            IDepartmentService departmentService)
        {
            _userService = userService(ArchetypeEnum.Unknown);
            _workContext = workContext;
            _loginServiceUserService = loginServiceUserService;
            _identityServerClientService = identityServerClientService;
            _exportService = exportService;
            _suspendOrDeactiveUserBackgroundJob = suspendOrDeactiveUserBackgroundJob;
            _logger = logger;
            _uGMemberService = uGMemberService;
            _userGroupService = userGroupService;
            _serviceScopeFactory = serviceScopeFactory;
            _fileStorageService = fileStorageService;
            _appSettings = appSettingOptions;
            _backgroundTaskQueue = backgroundTaskQueue;
            _hierarchyDepartmentPermissionService = hierarchyDepartmentPermissionService;
            _userGenericMappingService = userGenericMappingService;
            _userAccessService = userAccessService;
            _configuration = configuration;
            _massCreationUserService = massCreationUserService;
            _departmentService = departmentService;
        }

        /// <summary>
        /// Get list of user 
        /// </summary>
        /// <param name="parentDepartmentId"></param>
        /// <param name="userArchetypes"></param>
        ///  <param name="userArchetypes"></param>
        /// <param name="userEntityStatuses"></param>
        /// <param name="filterOnParentHd">All given departmentid must belong the the same hierachy tree</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        [Produces("application/json", "text/csv")]
        [Route("users")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<UserGenericDto>), 200)]
        public async Task<IActionResult> GetUserList([FromQuery] List<int> parentDepartmentId = null,
            [FromQuery] List<int> userIds = null,
            [FromQuery] List<ArchetypeEnum> userArchetypes = null,
            [FromQuery] List<string> extIds = null, [FromQuery] List<int> usertypeIds = null,
            [FromQuery] List<string> loginServiceClaims = null,
            [FromQuery] List<EntityStatusEnum> userEntityStatuses = null,
            [FromQuery] List<string> ssnList = null,
            [FromQuery] bool getUserGroups = true,
            bool filterOnParentHd = false,
            bool getRoles = true,
            bool getDeapartments = true,
            [FromQuery] List<AgeRange> ageRanges = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 100,
            string orderBy = "",
            string searchKey = null,
            [FromQuery] List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            [FromQuery] Dictionary<string, string> exportFields = null,
            [FromQuery] DateTime? createdAfter = null,
            [FromQuery] DateTime? createdBefore = null,
            [FromQuery] DateTime? expirationDateAfter = null,
            [FromQuery] DateTime? expirationDateBefore = null,
            [FromQuery] List<int> orgUnittypeIds = null,
            [FromQuery] string exportFileName = null,
            [FromQuery] List<List<int>> multiUserTypeFilters = null,
            [FromQuery] List<int> userGroupIds = null,
            [FromQuery] List<List<int>> multiUserGroupFilters = null,
            [FromQuery] List<string> userTypeExtIds = null,
            [FromQuery] List<List<string>> multiUserTypeExtIdFilters = null,
            [FromQuery] List<string> emails = null,
            bool getLoginServiceClaims = false,
            bool? filterOnSubDepartment = null,
            bool getOwnGroups = false,
            [FromQuery] DateTime? activeDateBefore = null,
            [FromQuery] DateTime? activeDateAfter = null,
            [FromQuery] List<int> exceptUserIds = null,
            [FromQuery] bool isCrossOrganizationalUnit = false)
        {
            //var departmentIds = _hierarchyDepartmentService.GetAllDepartmentIdsFromAHierachyDepartmentToTheTop(14350);
            bool returnAsCsv = false;
            bool skipPaging = false;
            if (Request.Headers.TryGetValue("Accept", out var acceptHeaderValues))
            {
                if (acceptHeaderValues.Contains("text/csv"))
                {
                    returnAsCsv = true;
                    skipPaging = pageSize == 0 && pageIndex == 0;
                }
            }

            if (!ValidateMinimalFilter(parentDepartmentId, userIds, extIds, loginServiceClaims, searchKey, userGroupIds,
                multiUserGroupFilters, emails, ssnList))
            {
                return NoContent();
            }

            if (!skipPaging && Request.ShouldLimitPageSize())
            {
                pageSize = _appSettings.Value.LimitUserPageSize(pageSize);
            }

            var users = await _userService.GetUsersAsync<UserGenericDto>(parentDepartmentIds: parentDepartmentId, userIds: userIds,
                extIds: extIds, archetypeIds: userArchetypes,
                statusIds: userEntityStatuses, searchKey: searchKey,
                pageIndex: pageIndex, userTypeIds: usertypeIds,
                pageSize: pageSize, includeUGMembers: getUserGroups,
                filterOnParentHd: filterOnParentHd, ageRanges: ageRanges,
                getRoles: getRoles, includeDepartment: getDeapartments,
                loginServiceClaims: loginServiceClaims, orderBy: orderBy,
                ssnList: ssnList, userTypeExtIds: userTypeExtIds,
                jsonDynamicData: jsonDynamicData, externallyMastered: externallyMastered,
                expirationDateAfter: expirationDateAfter, expirationDateBefore: expirationDateBefore,
                createdAfter: createdAfter, createdBefore: createdBefore,
                getLoginServiceClaims: getLoginServiceClaims, orgUnittypeIds: orgUnittypeIds,
                skipPaging: skipPaging,
                multiUserTypefilters: multiUserTypeFilters,
                filterOnSubDepartment: filterOnSubDepartment,
                userGroupIds: userGroupIds,
                multiUserGroupFilters: multiUserGroupFilters,
                multiUserTypeExtIdFilters: multiUserTypeExtIdFilters,
                emails: emails,
                includeOwnUserGroups: getOwnGroups,
                activeDateBefore: activeDateBefore,
                activeDateAfter: activeDateAfter,
                exceptUserIds: exceptUserIds,
                isCrossOrganizationalUnit: isCrossOrganizationalUnit);

            if (returnAsCsv)
            {
                Dictionary<string, dynamic> exportOptionFields;
                if (exportFields != null && exportFields.Count > 0)
                {
                    exportOptionFields = new Dictionary<string, dynamic>();
                    foreach (var exportField in exportFields)
                    {
                        exportOptionFields.Add(exportField.Key, new ExportFieldInfo { Caption = exportField.Value });
                    }
                }
                else
                {
                    exportOptionFields = ExportHelper.DefaultExportUserFieldMappings;

                }

                var exportOption = new ExportOption()
                {
                    CsvDelimiter = ",",
                    ExportFields = exportOptionFields,
                    ExportType = ExportType.Csv
                };
                return File(_exportService.ExportDataToBytes(users.Items, exportOption),
                    "text/csv",
                    string.IsNullOrEmpty(exportFileName)
                        ? $"user-accounts-{DateTime.Today.ToString("dd/MM/yyyy")}.csv"
                        : exportFileName);
            }
            else
                return CreateResponse(users);
        }


        /// <summary>
        /// Export list of user asynchronously. Response return the url of file that data is exported to.
        /// </summary>
        /// <param name="exportUserDto"></param>
        /// <returns></returns>
        [Route("users/export/async")]
        [HttpPost]
        [ProducesResponseType(typeof(AcceptExportDto), StatusCodes.Status202Accepted)]
        public IActionResult ExportUserListAsync([FromBody][Required] ExportUserDto exportUserDto)
        {
            if (!ValidateMinimalFilter(exportUserDto))
            {
                return NoContent();
            }
            var fileExtension = GetFileExtension(exportUserDto.ExportOption.ExportType);
            var fileName = $"user-accounts-{Request.GetRequestId()}{fileExtension}";
            var filePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings.Value, _appSettings.Value.ExportUserManagementStorageSubFolder,
                _workContext.CurrentOwnerId,
                _workContext.CurrentCustomerId, fileName);

            //For thread safe
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //TODO: deal with IWorkContext injection when calling async
            var apiDownloadUrl = this.Url.Link("DownloadExportUser", new { fileName = fileName });
            var fallBackLanguageCode = _appSettings.Value.FallBackLanguageCode;
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<UserManagementController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();

                            scopedLogger.LogInformation($"Start processing for exporting users to {fileName}.");
                            var scopeUserService =
                                serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(
                                    ArchetypeEnum
                                        .Unknown);
                            var scopeExportService =
                                serviceScope.ServiceProvider.GetService<IExportService<UserGenericDto>>();
                            var scopeFileStorageService =
                                serviceScope.ServiceProvider.GetService<IFileStorageService>();
                            var scopedHierarchyDepartmentPermissionService = serviceScope.ServiceProvider
                                .GetService<IHierarchyDepartmentPermissionService>();

                            var (exportedData, exportedUserCount, totalUserCount) = await ExecuteExportingUsers(scopedLogger, exportUserDto, scopeUserService,
                                scopedHierarchyDepartmentPermissionService, copiedWorkContext, scopeExportService, true);

                            var uploadedFile = await scopeFileStorageService.UploadFileAsync(copiedWorkContext, exportedData, filePath);
                            if (!string.IsNullOrEmpty(uploadedFile))
                            {
                                scopedLogger.LogInformation(
                                    $"Exporting data has been saved to storage under path {filePath}");

                                if (exportUserDto.SendEmail)
                                {
                                    SendEmailWhenExportingUser(exportUserDto, copiedWorkContext, scopeUserService,
                                        scopedLogger, serviceScope, apiDownloadUrl,
                                        filePath, fallBackLanguageCode);
                                }
                            }
                            else
                            {
                                scopedLogger.LogError(
                                    $"Failed to save exporting data to storage under path {filePath}");
                            }

                            watch.Stop();
                            scopedLogger.LogInformation($"End processing for exporting users. {exportedUserCount} of {totalUserCount} users have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");

                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e, $"Unexpected error occurred when processing for exporting users to {fileName}. {e.Message}");
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
        /// Export list of user synchronously. Response return the file.
        /// </summary>
        /// <param name="exportUserDto"></param>
        /// <returns></returns>
        [Route("users/export")]
        [HttpPost]
        public async Task<IActionResult> ExportUserList([FromBody][Required] ExportUserDto exportUserDto)
        {
            if (exportUserDto.SendEmail)
            {
                throw new InvalidException("Do not support sending email when exporting user with this action");
            }
            if (!ValidateMinimalFilter(exportUserDto))
            {
                return NoContent();
            }
            var fileExtension = GetFileExtension(exportUserDto.ExportOption.ExportType);
            var fileName = $"user-accounts-{Request.GetRequestId()}{fileExtension}";

            _logger.LogInformation(
                $"Start processing for exporting users to {fileName}.");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var (exportedData, exportedUserCount, totalUserCount) = await ExecuteExportingUsers(_logger, exportUserDto, _userService, _hierarchyDepartmentPermissionService, _workContext, _exportService, false);

            watch.Stop();
            _logger.LogInformation($"End processing for exporting users. {exportedUserCount} of {totalUserCount} users have been exported into file '{fileName}' in {watch.ElapsedMilliseconds}ms.");
            var contentType = GetContentTypeForFileExtension(fileExtension);
            return File(exportedData, contentType, fileName);
        }


        [Route("users/export/download", Name = "DownloadExportUser")]
        [HttpPost]
        public async Task<IActionResult> DownloadUserListAsync(string fileName)
        {
            var fullFilePath = ExportHelper.GetStorageFullFilePath(_fileStorageService.FilePathDelimiter, _appSettings.Value, _appSettings.Value.ExportUserManagementStorageSubFolder, _workContext.CurrentOwnerId, _workContext.CurrentCustomerId, fileName);

            var fileContent = await _fileStorageService.DownloadFileAsync(fullFilePath);

            var contentType = GetContentTypeForFileExtension(System.IO.Path.GetExtension(fileName));
            var ms = new MemoryStream();
            fileContent.CopyTo(ms);

            return File(ms.ToArray(), contentType, fileName);
        }

        private static string GetFileExtension(ExportType exportType)
        {
            if (exportType == ExportType.Excel)
                return FileExtension.ExcelOpenXML;
            return FileExtension.Csv;
        }
        private string GetContentTypeForFileExtension(string fileExtension)
        {
            if (FileExtension.FileTypeContentTypeMappings.TryGetValue(fileExtension, out var contentType))
            {
                return contentType;
            }
            return "application/json";
        }

        /// <summary>
        /// Get list of user 
        /// </summary>
        /// <returns></returns>
        [Route("get_users")]
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedList<UserGenericDto>), 200)]

        public async Task<IActionResult> GetUserListByBody([FromBody] SearchInput searchInput)
        {
            var token = HttpContext.Request.Headers["Authorization"][0];

            if (token is null)
            {
                return BadRequest();
            }


            if (!ValidateMinimalFilter(searchInput))
            {
                return NoContent();
            }

            if (Request.ShouldLimitPageSize())
            {
                searchInput.PageSize = _appSettings.Value.LimitUserPageSize(searchInput.PageSize);
            }

            var users = await GetUsersBySearchInput(searchInput, _userService, _hierarchyDepartmentPermissionService,
                _workContext, false, isAsync: false, token);

            return CreateResponse(users);
        }

        /// <summary>
        /// Get list of user 
        /// </summary>
        /// <returns></returns>
        [Route("count_users")]
        [HttpPost]
        [ProducesResponseType(typeof(CountUserResultDto), 200)]

        public async Task<IActionResult> GetUserListByBody(CountUserDto countUserDto)
        {
            var data = await _userService.CountUserGroupByAsync(_workContext.CurrentOwnerId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                userIds: countUserDto.UserIds,
                userGroupIds: countUserDto.UserGroupIds,
                statusIds: countUserDto.UserEntityStatuses,
                archetypeIds: countUserDto.UserArchetypes,
                userTypeIds: countUserDto.UserTypeIds,
                userTypeExtIds: countUserDto.UserTypeExtIds,
                departmentIds: countUserDto.ParentDepartmentIds,
                extIds: countUserDto.ExtIds,
                jsonDynamicData: countUserDto.JsonDynamicData,
                exceptUserIds: countUserDto.ExceptUserIds,
                multiUserTypeFilters: countUserDto.MultiUserTypeFilters,
                multiUserTypeExIdFilters: countUserDto.MultiUserTypeExtIdFilters,
                multiUserGroupFilters: countUserDto.MultiUserGroupFilters,
                lastUpdatedBefore: countUserDto.LastUpdatedBefore,
                lastUpdatedAfter: countUserDto.LastUpdatedAfter, createdBefore: countUserDto.CreatedBefore,
                createdAfter: countUserDto.CreatedAfter, groupByField: countUserDto.GroupByField);

            return CreateResponse(data);
        }

        [Route("suspend_users")]
        [HttpGet]
        public async Task<IActionResult> AutoSuspendUser()
        {
            await _suspendOrDeactiveUserBackgroundJob.SuspendUserStatus();
            await Task.CompletedTask;
            return Accepted();
        }

        [Route("deactive_users")]
        [HttpGet]
        public async Task<IActionResult> AutoDeActiveUser()
        {
            await _suspendOrDeactiveUserBackgroundJob.DeActiveUserStatus();
            await Task.CompletedTask;
            return Accepted();
        }

        /// <summary>
        /// update user generic
        /// </summary>
        /// <returns></returns>
        [Route("users/{userId}")]
        [HttpPut]
        [ProducesResponseType(typeof(UserGenericDto), 200)]

        [TypeFilter(typeof(PreventXSSFilter))]
        public async Task<IActionResult> UpdateUser(long userId, [FromBody] UserGenericDto userGenericDto, bool skipCheckingEntityVersion = true, [FromQuery] bool syncToIdp = true)
        {
            userGenericDto.Identity.Id = userId;
            var checkAccessResult = await _userAccessService.CheckEditUserAccessAsync(_workContext, userGenericDto, _userGenericMappingService);

            if (checkAccessResult.AccessStatus == AccessStatus.DataNotFound)
            {
                return StatusCode((int)HttpStatusCode.NotFound, $"User with id {userId} is not found.");
            }

            //if (checkAccessResult.AccessStatus == AccessStatus.AccessDenied)
            //{
            //    return AccessDenied();
            //}

            var executor = checkAccessResult.ExecutorUser;

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(userGenericDto.DepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();

            userGenericDto.EmailAddress = userGenericDto.EntityStatus.StatusId == EntityStatusEnum.Rejected
                ? DomainHelper.GenerateDummyEmail()
                : userGenericDto.EmailAddress;

            SetEntityActiveDate(userGenericDto);
            if (syncToIdp)
            {
                try
                {
                    var keepUserLogin = executor == null || (executor != null && executor.UserId == userId);
                    var userIdentityResponse = (await _identityServerClientService.UpdateUserAsync(
                        userId,
                        userGenericDto,
                        keepUserLogin,
                        userGenericDto.EntityStatus.StatusId == EntityStatusEnum.Rejected));
                    userGenericDto.OtpValue = userIdentityResponse.Otp;
                    userGenericDto.OtpExpiration = userIdentityResponse.OtpExpiration;
                }
                catch (cxHttpResponseException e)
                {
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"Skip updating user on identity server because '{e.Message}'");
                    }
                    else throw;
                }
            }
            var user = _userService.UpdateUser(validationSpecification, userGenericDto, skipCheckingEntityVersion);
            return CreateResponse(user);
        }

        private void SetEntityActiveDate(UserGenericDto user)
        {
            var entityActiveDateConfigs = _configuration.GetSection("UserEntityActiveDateConfiguration").Get<List<UserEntityActiveDateDto>>();
            if (entityActiveDateConfigs == null)
                return;
            var userDepartment = _departmentService.GetDepartmentByIdIncludeHd(user.DepartmentId, user.Identity.OwnerId, user.Identity.CustomerId);

            var currentDepartmentType = userDepartment.DT_Ds.Select(x => x.DepartmentType);

            foreach (var item in entityActiveDateConfigs)
            {
                var isMatchDepartmentType = currentDepartmentType.Any(x => item.DepartmentType.Contains(x.ExtId));
                if (isMatchDepartmentType
                    && item.EntityActiveDate >= DateTime.Now
                    && !user.EntityStatus.ActiveDate.HasValue
                    && userDepartment.H_D.Any(x => x.PathName.Contains($"\\{item.Path}\\")))
                {
                    user.EntityStatus.ActiveDate = item.EntityActiveDate;
                }
            }
        }

        /// <summary>
        /// create update user generic
        /// </summary>
        /// <returns></returns>
        [Route("users")]
        [HttpPost]
        [ProducesResponseType(typeof(UserGenericDto), 201)]
        [TypeFilter(typeof(PreventXSSFilter))]
        public async Task<IActionResult> CreateUser([FromBody] UserGenericDto userGenericDto, [FromQuery] bool syncToIdp = true)
        {
            userGenericDto.Identity.Id = 0;

            //if (!(await _userAccessService.CheckCreateUserAccessAsync(_workContext, userGenericDto)))
            //{
            //    return AccessDenied();
            //}

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment(userGenericDto.DepartmentId, ArchetypeEnum.Unknown)
            .SkipCheckingArchetype()
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            UserIdentityDto userIdentity = null;
            SetEntityActiveDate(userGenericDto);
            if (syncToIdp)
            {
                var responseDto = (await _identityServerClientService.CreateUserAsync(userGenericDto));
                userIdentity = responseDto.User;
                userGenericDto.Identity.ExtId = userIdentity.Id;
                userGenericDto.OtpValue = responseDto.Otp;
                userGenericDto.OtpExpiration = responseDto.OtpExpiration;
            }

            var user = _userService.InsertUser(validationSpecification, userGenericDto);

            if (syncToIdp)
            {
                _loginServiceUserService.Insert(new LoginServiceUserDto
                {
                    ClaimValue = userIdentity.Id,
                    Created = DateTime.UtcNow,
                    UserIdentity = user.Identity,
                    AcceptedUserEntityStatuses = new List<EntityStatusEnum> { user.EntityStatus.StatusId }
                });
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.Created, user));
        }

        [HttpPost("masscreations/create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateMassUser(
            [FromForm] MassUserCreationValidationContract massUserCreationValidationContract)
        {
            var fileName = massUserCreationValidationContract.File.FileName.Replace(" ", "_");
            var fileType = Domain.Common.FileExtension.GetValidFileType(fileName);
            if (fileType != FileType.Csv)
            {
                throw new InvalidException($"File is not supported to import");
            }

            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            //Need to copy file to MemoryStream to keep it alive after request completed for handing background job
            var file = massUserCreationValidationContract.File.OpenReadStream();
            var fileOnMemory = new MemoryStream();
            await file.CopyToAsync(fileOnMemory);
            fileOnMemory.ResetPosition();

            var users = await _massCreationUserService.getUsersFromFileAsync(massUserCreationValidationContract, fileOnMemory, copiedWorkContext);

            users.ForEach(async user =>
            {
                await processUserCreation(user, true);
            });

            return await Task.FromResult(StatusCode((int)HttpStatusCode.Created, users));
        }


        /// <summary>
        /// validate mass nomination file as soon as uploading file
        /// </summary>
        /// <param name="massUserCreationValidationContract"></param>
        /// <returns></returns>
        [HttpPost("masscreations/file/validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MassUserCreationValidationResultDto> ValidateMassUserCreationFile(
        [FromForm] MassUserCreationValidationContract massUserCreationValidationContract)
        {
            var file = massUserCreationValidationContract.File.OpenReadStream();

            var data = _massCreationUserService.ValidateMassUserCreationFile(
                file, massUserCreationValidationContract.File.FileName);

            return Ok(data);
        }

        /// <summary>
        /// check valid mass creation data from file
        /// </summary>
        /// <param name="massUserCreationValidationContract"></param>
        /// <returns></returns>
        [HttpPost("masscreations/data/validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MassUserCreationValidationResultDto>> ValidateMassUserCreationData(
            [FromForm] MassUserCreationValidationContract massUserCreationValidationContract)
        {
            var fileName = massUserCreationValidationContract.File.FileName.Replace(" ", "_");
            var fileType = Domain.Common.FileExtension.GetValidFileType(fileName);
            if (fileType != FileType.Csv)
            {
                throw new InvalidException($"File is not supported to import");
            }
            //if (string.IsNullOrEmpty(massAssignPDOpportunityValidationContract.KeyLearningProgramExtId))
            //{
            //    throw new Exception("Nomination to KLP but no KeyLearningProgramExtId provided");
            //}

            var copiedWorkContext = WorkContext.CopyFrom(_workContext);

            var file = massUserCreationValidationContract.File.OpenReadStream();

            var data = await _massCreationUserService.ValidateMassUserCreationData(
                copiedWorkContext,
                file,
                fileName);

            return Ok(data);
        }

        /// <summary>
        /// create update user generic
        /// </summary>
        /// <returns></returns>
        [Route("users/{userid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(UserGenericDto), 200)]

        public async Task<IActionResult> Delete(int userid, [FromQuery] bool syncToIdp = true, [FromQuery] EntityStatusReasonEnum? entityStatusReason = null)
        {
            var userDto = _userService.GetUsers<UserGenericDto>(userIds: new List<int> { userid }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (userDto == null)
            {
                return NotFound();
            }
            if (syncToIdp)
            {
                try
                {
                    await _identityServerClientService.DeleteUserAsync(userid);

                }
                catch (cxHttpResponseException e)
                {
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"Skip deleting user on identity server because '{e.Message}'");
                    }
                    else throw;
                }
            }

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
           .ValidateDepartment(userDto.DepartmentId, ArchetypeEnum.Unknown)
           .SkipCheckingArchetype()
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();
            //userDto.EntityStatus.Deleted = true;
            userDto.EntityStatus.StatusId = EntityStatusEnum.Deactive;
            if (entityStatusReason.HasValue)
            {
                userDto.EntityStatus.StatusReasonId = entityStatusReason.Value;
            }
            //AnonymizeUser(userDto);
            var user = _userService.UpdateUser(validationSpecification, userDto);
            var userGroups = _userGroupService.GetUserGroups<ConexusBaseDto>(_workContext.CurrentOwnerId, customerIds: new List<int> { _workContext.CurrentCustomerId }, groupUserIds: new List<int> { (int)userDto.Identity.Id.Value }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.Active }).Items.ToList();

            foreach (var group in userGroups)
            {
                group.EntityStatus.StatusId = EntityStatusEnum.Deactive;
                _userGroupService.UpdateUserGroup(validationSpecification, new UserGroupDto
                {
                    Identity = group.Identity,
                    EntityStatus = group.EntityStatus
                });
            }
            if (userGroups.Any())
            {
                var ugMembers = _uGMemberService.GetMemberships(_workContext.CurrentOwnerId, customerIds: new List<int> { _workContext.CurrentCustomerId }, userGroupIds: userGroups.Select(x => (int)x.Identity.Id.Value).ToList(), ugMemberStatuses: new List<EntityStatusEnum> { EntityStatusEnum.Active },
                    userGroupStatus: new List<EntityStatusEnum> { EntityStatusEnum.Deactive });
                foreach (var ugMember in ugMembers)
                {
                    ugMember.EntityStatus.StatusId = EntityStatusEnum.Deactive;
                    _uGMemberService.Update(ugMember);
                }
            }
            //set ugmember of usergroup deactive
            var ugMemberValues = _uGMemberService.GetMemberships(_workContext.CurrentOwnerId, customerIds: new List<int> { _workContext.CurrentCustomerId }, userIds: new List<int> { (int)userDto.Identity.Id.Value });
            if (ugMemberValues != null)
            {
                foreach (var ugMemberValue in ugMemberValues)
                {
                    if (ugMemberValue.EntityStatus.StatusId == EntityStatusEnum.Active)
                    {
                        ugMemberValue.EntityStatus.StatusId = EntityStatusEnum.Deactive;
                        _uGMemberService.Update(ugMemberValue);
                    }
                }
            }
            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, user));
        }

        [Route("archive/{userid}")]
        [HttpPost]
        [ProducesResponseType(typeof(UserGenericDto), 200)]

        public async Task<IActionResult> ArchiveUser(int userid, [FromQuery] EntityStatusReasonEnum? entityStatusReason = null)
        {
            try
            {
                var archivedUser = await _userService.ArchiveUserByIdAsync(userid, true, entityStatusReason.Value, false);

                if(archivedUser == null)
                {
                    return NotFound();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, archivedUser));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("unarchive/{userid}")]
        [HttpPost]
        [ProducesResponseType(typeof(UserGenericDto), 200)]

        public async Task<IActionResult> UnarchiveUser(int userid)
        {
            try
            {
                var unArchivedUser = await _userService.UnarchiveAsync(userid, true);

                if (unArchivedUser == null)
                {
                    return NotFound();
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, unArchivedUser));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert loginServiceUser that contains claim value for a employee in a login service.
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("users/loginservices")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult InsertLoginServiceClaim([Required][FromBody] LoginServiceUserDto loginServiceUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var insertedLoginService = _loginServiceUserService.InsertOrUpdate(loginServiceUser);
            return StatusCode((int)HttpStatusCode.Created, insertedLoginService);
        }
        /// <summary>
        /// Send welcome email for user that is not sent before and match with configured status and externally mastered 
        /// </summary>
        /// <param name="loginServiceUser"></param>
        /// <returns></returns>
        [Route("send_welcomeemails")]
        [HttpPost]
        [ProducesResponseType(typeof(LoginServiceUserDto), 200)]
        //[ValidateIdentityCxToken]
        public IActionResult SendWelcomeEmail(SendWelcomeEmailDto sendWelcomeEmailDto)
        {
            var copiedWorkContext = WorkContext.CopyFrom(_workContext);
            _backgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<UserManagementController>>();
                    var requestContext = new RequestContext(copiedWorkContext);
                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        try
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();

                            scopedLogger.LogInformation($"Start processing for manual sending welcome email for user.");
                            var scopeUserService = serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(ArchetypeEnum.Unknown);

                            var userFilter = sendWelcomeEmailDto?.UserFilter ?? new SendWelcomeEmailUserFilterDto();
                            scopeUserService.ManuallySendWelcomeEmail(copiedWorkContext,
                                userIds: userFilter.UserIds,
                                userExtIds: userFilter.ExtIds,
                                emails: userFilter.Emails,
                                parentDepartmentIds: userFilter.ParentDepartmentIds,
                                createdBefore: userFilter.CreatedBefore,
                                createdAfter: userFilter.CreatedAfter,
                                lastUpdatedAfter: userFilter.LastUpdatedAfter,
                                lastUpdatedBefore: userFilter.LastUpdatedBefore,
                                expirationDateAfter: userFilter.ExpirationDateAfter,
                                expirationDateBefore: userFilter.ExpirationDateBefore,
                                pageSize: userFilter.PageSize,
                                pageIndex: userFilter.PageIndex,
                                orderBy: userFilter.OrderBy,
                                externallyMasteredValues: userFilter.ExternallyMasteredValues,
                                userEntityStatuses: userFilter.UserEntityStatuses);

                            watch.Stop();
                            scopedLogger.LogInformation($"End processing for manual sending welcome email for user. in {watch.ElapsedMilliseconds}ms.");
                        }
                        catch (Exception e)
                        {
                            scopedLogger.LogError(e, $"Unexpected error occurred when processing for manual sending welcome email. {e.Message}");
                        }

                    }
                }
            });

            return Accepted(new AcceptExportDto
            {
                Message = "Request is accepted for process data. Please check the log to view more detail."

            });
        }

        /// <summary>
        /// Send users information to the queue as update event to support sync data to other modules
        /// </summary>
        /// <returns></returns>
        [Route("send-userinfor-to-datahub")]
        [HttpPost]
        public async Task SendUserinforToDatahub()
        {
            var appSetting = _appSettings.Value;

            var workContext = new WorkContext(_appSettings, null)
            {
                CurrentUserId = appSetting.CurrentUserId,
                CurrentOwnerId = 3001,
                CurrentCustomerId = 2052,
                IsEnableFiltercxToken = true,
                RequestId = Guid.Empty.ToString(),
                CorrelationId = Guid.Empty.ToString()
            };

            var usersList = new List<UserEntity>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                try
                {
                    var paginatedUserEntities = await _userService.GetAllUsers(pageIndex);
                    hasMoreData = paginatedUserEntities.HasMoreData;
                    pageIndex++;
                    if (paginatedUserEntities.Items.Count > 0)
                    {
                        usersList.AddRange(paginatedUserEntities.Items);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error for getting users at index: {pageIndex}' {ex.Message}'");
                }
            } while (hasMoreData);

            foreach (var user in usersList)
            {
                try
                {
                    var currentUserGenericDto = _userGenericMappingService.ToUserDto(user, true, false, false) as UserGenericDto;
                    var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                    .ValidateDepartment(currentUserGenericDto.DepartmentId, ArchetypeEnum.Unknown)
                    .SkipCheckingArchetype()
                     .WithStatus(EntityStatusEnum.All)
                    .IsDirectParent()
                    .Create();
                    SetEntityActiveDate(currentUserGenericDto);

                    _userService.UpdateUser(validationSpecification, currentUserGenericDto, false, workContext);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error for proceed sending event of user's email: {user.Email}:' {ex.Message}'");
                }
            }

        }


        /// <summary>
        /// Gets the user info of the currently login user by authorization header's token.
        /// </summary>
        /// <param name="userEntityStatuses"></param>
        /// <param name="getUserGroups"></param>
        /// <param name="getRoles"></param>
        /// <param name="getDepartment"></param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getOwnGroups"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [Route("me")]
        [HttpGet]
        [ProducesResponseType(typeof(UserGenericDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyInfo(
            [FromQuery] List<EntityStatusEnum> userEntityStatuses = null,
            [FromQuery] bool getUserGroups = false,
            bool getRoles = false,
            bool getDepartment = false,
            bool getLoginServiceClaims = false,
            bool getOwnGroups = false)
        {
            if (string.IsNullOrEmpty(_workContext.Sub))
            {
                _logger.LogWarning($"Could not retrieve user due to not existing the 'sub' in the request.");
                return BadRequest();
            }
            var users = await _userService.GetUsersAsync<UserGenericDto>(
                extIds: new List<string>() { _workContext.Sub },
                statusIds: userEntityStatuses,
                includeUGMembers: getUserGroups,
                getRoles: getRoles,
                includeDepartment: getDepartment,
                getLoginServiceClaims: getLoginServiceClaims,
                includeOwnUserGroups: getOwnGroups,
                ignoreCheckReadUserAccess: true);

            if (users.Items.Count == 0)
            {
                _logger.LogError($"Could not found user with extId '{_workContext.Sub}'.");
                return NotFound();
            }

            if (users.Items.Count > 1)
            {
                _logger.LogWarning($"Found multi users having the same extId '{_workContext.Sub}'. These are the list of user identifiers {string.Join(',', users.Items.Select(p => p.Identity.Id))}");
            }

            return CreateResponse(users.Items.First());
        }

        private async Task<ConexusBaseDto> processUserCreation(UserGenericDto userGenericDto, bool syncToIdp)
        {
            try
            {
                //For thread safe
                var copiedWorkContext = WorkContext.CopyFrom(_workContext);
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var scopedLogger = serviceScope.ServiceProvider.GetService<ILogger<UserManagementController>>();
                    var loginServiceUserService = serviceScope.ServiceProvider.GetService<ILoginServiceUserService>();
                    var userService = serviceScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(ArchetypeEnum.Unknown);
                    var identityServerClientService = serviceScope.ServiceProvider.GetService<IIdentityServerClientService>();
                    var requestContext = new RequestContext(copiedWorkContext);

                    using (scopedLogger.SetContextToScope(requestContext))
                    {
                        var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                      .ValidateDepartment(userGenericDto.DepartmentId, ArchetypeEnum.Unknown)
                      .SkipCheckingArchetype()
                      .WithStatus(EntityStatusEnum.All)
                      .IsDirectParent()
                      .Create();
                        UserIdentityDto userIdentity = null;
                        SetEntityActiveDate(userGenericDto);
                        if (syncToIdp)
                        {
                            var responseDto = (await identityServerClientService.CreateUserAsync(userGenericDto));
                            userIdentity = responseDto.User;
                            userGenericDto.Identity.ExtId = userIdentity.Id;
                            userGenericDto.OtpValue = responseDto.Otp;
                            userGenericDto.OtpExpiration = responseDto.OtpExpiration;
                        }

                        var createdUser = userService.InsertUser(validationSpecification, userGenericDto, copiedWorkContext, true);

                        if (syncToIdp)
                        {
                            loginServiceUserService.Insert(new LoginServiceUserDto
                            {
                                ClaimValue = userIdentity.Id,
                                Created = DateTime.UtcNow,
                                UserIdentity = createdUser.Identity,
                                AcceptedUserEntityStatuses = new List<EntityStatusEnum> { createdUser.EntityStatus.StatusId }
                            });
                        }

                        return createdUser;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void AnonymizeUser(UserGenericDto user)
        {
            var dummyEmail = GenerateDummyEmail(user.Identity.ExtId);
            user.FirstName = "";
            user.LastName = "";
            user.MobileNumber = null;
            user.SSN = "";
            user.EmailAddress = dummyEmail;
        }
        private string GenerateDummyEmail(string userExtId)
        {
            if (string.IsNullOrEmpty(userExtId)) userExtId = Guid.NewGuid().ToString();
            return string.Format("{0}@dummy.net", userExtId.Replace("-", ".").Replace("@", "."));
        }

        private static async Task<PaginatedList<UserGenericDto>> GetUsersBySearchInput(SearchInput searchInput,
            IUserService userService,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService,
            IWorkContext copiedWorkContent, bool skipPaging, bool isAsync, string token = null)
        {
            int ownerId = 0;
            List<int> customerIds = null;
            if (isAsync)
            {
                //When handle async, repository will do not handle filtering on cxToken,
                //we need to handle filter by given paraameter
                ownerId = copiedWorkContent.CurrentOwnerId;
                customerIds = new List<int> { copiedWorkContent.CurrentCustomerId };
            }

            var users = await userService.GetUsersAsync<UserGenericDto>(
                ownerId: ownerId,
                customerIds: customerIds,
                parentDepartmentIds:
                searchInput.ParentDepartmentId,
                departmentExtIds: searchInput.DepartmentExtIds,
                userIds: searchInput.UserIds,
                extIds: searchInput.ExtIds,
                archetypeIds: searchInput.UserArchetypes,
                statusIds: searchInput.UserEntityStatuses,
                searchKey: searchInput.SearchKey,
                userTypeIds: searchInput.UserTypeIds,
                includeUGMembers: searchInput.GetGroups,
                filterOnParentHd: searchInput.FilterOnParentHd,
                ageRanges: searchInput.AgeRanges,
                getRoles: searchInput.GetRoles,
                includeDepartment: searchInput.GetDepartment,
                loginServiceClaims: searchInput.LoginServiceClaims,
                ssnList: searchInput.SsnList,
                emails: searchInput.Emails,
                userTypeExtIds: searchInput.UserTypeExtIds,
                jsonDynamicData: searchInput.JsonDynamicData,
                externallyMastered: searchInput.ExternallyMastered,
                expirationDateAfter: searchInput.ExpirationDateAfter,
                expirationDateBefore: searchInput.ExpirationDateBefore,
                createdAfter: searchInput.CreatedAfter,
                createdBefore: searchInput.CreatedBefore,
                getLoginServiceClaims: searchInput.GetLoginServiceClaims,
                orgUnittypeIds: searchInput.OrgUnitTypeIds,
                multiUserTypefilters: searchInput.MultiUserTypeFilters,
                filterOnSubDepartment: searchInput.FilterOnSubDepartment,
                userGroupIds: searchInput.UserGroupIds,
                multiUserGroupFilters: searchInput.MultiUserGroupFilters,
                multiUserTypeExtIdFilters: searchInput.MultiUserTypeExtIdFilters,
                currentWorkContext: copiedWorkContent,
                pageIndex: searchInput.PageIndex,
                pageSize: searchInput.PageSize,
                skipPaging: skipPaging,
                orderBy: searchInput.OrderBy,
                includeOwnUserGroups: searchInput.GetOwnGroups,
                activeDateBefore: searchInput.ActiveDateBefore,
                activeDateAfter: searchInput.ActiveDateAfter,
                exceptUserIds: searchInput.ExceptUserIds,
                systemRolePermissions: searchInput.SystemRolePermissions,
                token: token);
            return users;
        }

        private static async Task<(byte[], int, int)> ExecuteExportingUsers(ILogger logger,
            ExportUserDto exportUserDto,
            IUserService userService,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService,
            IWorkContext workContext,
            IExportService<UserGenericDto> exportService,
            bool isAsync)
        {

            bool skipPaging = exportUserDto.PageIndex <= 0 && exportUserDto.PageSize <= 0;

            if (exportUserDto.ExportOption.ExportFields == null ||
                exportUserDto.ExportOption.ExportFields.Count == 0)
            {
                exportUserDto.ExportOption.ExportFields = ExportHelper.DefaultExportUserFieldMappings;
            }

            if (!exportUserDto.GetDepartment)
            {
                exportUserDto.GetDepartment = exportUserDto.ExportOption.ExportFields.NeedToGetDepartment();
            }

            if (!exportUserDto.GetRoles)
            {
                exportUserDto.GetRoles = exportUserDto.ExportOption.ExportFields.NeedToGetRole();
            }

            logger.LogInformation("Start retrieving user for exporting");

            var users = await GetUsersBySearchInput(exportUserDto, userService, hierarchyDepartmentPermissionService, workContext, skipPaging, isAsync);
            int exportedUserCount = users.Items.Count;
            int totalUserCount = users.TotalItems;

            logger.LogInformation($"End retrieving users for exporting. {exportedUserCount} of {totalUserCount} users has been retrieved.");

            logger.LogInformation($"Start exporting {exportedUserCount} users into file");

            var exportedData = exportService.ExportDataToBytes(users.Items, exportUserDto.ExportOption, workContext);

            logger.LogInformation($"End exporting {exportedUserCount} users into file");

            return (exportedData, exportedUserCount, totalUserCount);
        }


        private static void SendEmailWhenExportingUser(ExportUserDto exportUserDto, IWorkContext workContext,
            IUserService scopeUserService, ILogger logger, IServiceScope serviceScope, string apiDownloadUrl,
            string filePath, string fallBackLanguageCode)
        {
            try
            {
                var emailTemplates = serviceScope.ServiceProvider.GetService<IOptions<EmailTemplates>>().Value;
                if (emailTemplates.ExportUserTemplate == null || emailTemplates.ExportUserTemplate.Disabled)
                {
                    logger.LogInformation("Sending email has been disabled for exporting user by configuration");
                    return;
                }

                var currentUser = DomainHelper.GetUserFromWorkContext(workContext, scopeUserService,
                    getRoles: false,
                    getLoginServiceClaims: true);
                if (currentUser == null)
                {
                    logger.LogWarning($"Unable to find login user with sub '{workContext.Sub}' for sending email.");
                }
                else
                {

                    var emailOption = exportUserDto.EmailOption ?? new EmailOption();

                    var emailSubject = emailOption.Subject;

                    //If there is no given email subject, email body from client, we get from app setting of email templates
                    if (string.IsNullOrEmpty(emailSubject))
                    {
                        emailSubject =
                            emailTemplates.ExportUserTemplate.GetSubject(workContext.CurrentLocales,
                                fallBackLanguageCode);
                    }

                    //deep clone template to do not change original data
                    var templateDate = emailTemplates.ExportUserTemplate.CommunicationApiTemplate.DeepClone();

                    var dataKeys = templateDate.Data.Keys.ToList();

                    foreach (var dataProperty in dataKeys)
                    {
                        var propertyValue = templateDate.Data[dataProperty];
                        templateDate.Data[dataProperty] = propertyValue
                            .Replace("{filePath}", HttpUtility.UrlEncode(filePath), StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{UserFullName}", currentUser.GetFullName());

                    }

                    var scopedHierarchyDepartmentService = serviceScope.ServiceProvider.GetService<IHierarchyDepartmentService>();
                    var scopedAppSetting = serviceScope.ServiceProvider.GetService<IOptions<AppSettings>>().Value;
                    var sendEmailCommand = DomainHelper.GenerateCommunicationCommand(
                        workContext.CorrelationId,
                        scopeUserService,
                        executorUser: currentUser,
                        objectiveUser: null,
                        emailSubject: emailSubject,
                        communicationApiTemplate: templateDate,
                        routingAction: scopedAppSetting.EmailMessageRoutingAction,
                        sendEmailToDto: emailTemplates.ExportUserTemplate.SendTo);

                    var scopedDatahubLog = serviceScope.ServiceProvider.GetService<IDatahubLogger>();
                    scopedDatahubLog.WriteCommandLog(sendEmailCommand, useMessageRoutingActionAsRoutingKey: true);

                    logger.LogInformation("An command message of sending email has been published when exporting users");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unexpected error occurred when sending email for exporting users. {e.Message}");
            }
        }

        private bool ValidateMinimalFilter(List<int> parentDepartmentId, List<int> userIds, List<string> extIds,
            List<string> loginServiceClaims,
            string searchKey, List<int> userGroupIds, List<List<int>> multiUserGroupFilters, List<string> emails,
            List<string> ssnList)
        {
            //Only validate when authorized by user token
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {

                var hasFilterOnUserIdentity = !userIds.IsNullOrEmpty()
                                              || !extIds.IsNullOrEmpty()
                                              || !ssnList.IsNullOrEmpty()
                                              || !loginServiceClaims.IsNullOrEmpty()
                                              || !emails.IsNullOrEmpty()
                                              || !string.IsNullOrEmpty(searchKey);

                var hasFilterOnDepartment = !parentDepartmentId.IsNullOrEmpty();
                var hasFilterOnUserGroup = !userGroupIds.IsNullOrEmpty()
                                           || (!multiUserGroupFilters.IsNullOrEmpty() &&
                                               multiUserGroupFilters.Any(g => !g.IsNullOrEmpty()));

                if (!hasFilterOnUserIdentity && !hasFilterOnDepartment && !hasFilterOnUserGroup)
                {
                    _logger.LogWarning(
                        "For security reason, it requires minimal filter on identity of user, department or user group to be able to retrieve users.");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateMinimalFilter(SearchInput searchInput)
        {
            return ValidateMinimalFilter(searchInput.ParentDepartmentId,
                searchInput.UserIds,
                searchInput.ExtIds,
                searchInput.LoginServiceClaims,
                searchInput.SearchKey, searchInput.UserGroupIds,
                searchInput.MultiUserGroupFilters, searchInput.Emails,
                searchInput.SsnList);

        }
    }


}
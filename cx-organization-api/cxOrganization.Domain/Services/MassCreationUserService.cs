using CsvHelper;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Exceptions;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Settings.MassUserCreationMessageSetting;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NPOI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public class MassCreationUserService : IMassCreationUserService
    {
        private readonly IDataFileReader _csvFileReader;
        private readonly IntegrationSetting _integrationSetting;
        private readonly MassUserCreationMessageConfiguration _massUserCreationMessageConfig;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentService _departmentService;
        private readonly IUserTypeService _userTypeService;
        private readonly IDepartmentAccessService _departmentAccessService;
        private readonly IWorkContext _workContext;
        private readonly IHierarchyDepartmentPermissionService _hierarchyDepartmentPermissionService;
        private readonly IUserAccessService _userAccessService;

        public MassCreationUserService(
            IDataFileReader dataFileReader,
            IOptions<IntegrationSetting> integrationSetting,
            IOptions<MassUserCreationMessageConfiguration> massUserCreationMessageConfig,
            IUserRepository userRepository,
            IDepartmentService departmentService,
            IUserTypeService userTypeService,
            IHierarchyDepartmentPermissionService hierarchyDepartmentPermissionService,
            IDepartmentAccessService departmentAccessService,
            IWorkContext workContext,
            IUserAccessService userAccessService)

        {
            _csvFileReader = dataFileReader;
            _integrationSetting = integrationSetting.Value;
            _massUserCreationMessageConfig = massUserCreationMessageConfig.Value;
            _userRepository = userRepository;
            _departmentService = departmentService;
            _userTypeService = userTypeService;
            _workContext = workContext;
            _hierarchyDepartmentPermissionService = hierarchyDepartmentPermissionService;
            _departmentAccessService = departmentAccessService;
            _userAccessService = userAccessService;
        }

        public MassUserCreationValidationResultDto ValidateMassUserCreationFile(Stream fileOnMemory, string fileName)
        {
            var massUserCreationValidationResult = new MassUserCreationValidationResultDto();
            var massCreations = new List<MassUserCreationDto>();
            try
            {
                var fileType = FileExtension.GetValidFileType(fileName);
                if (fileType != FileType.Csv)
                {
                    throw new UnsupportedFileExtensionException($"File type is not supported to import");
                }

                var massCreationsResult = this._csvFileReader.ReadDataFromStream<MassUserCreationDto>(fileOnMemory,
                                                                                                      _integrationSetting.MassUserCreationMappingConfig);

                if (massCreationsResult.IsNullOrEmpty())
                {
                    throw new EmptyFileException();
                }

                if (isEmptyUserRow(massCreationsResult))
                {
                    throw new UserRowEmptyException($"Some rows are empty. Please check and upload again.");
                }

                if (massCreationsResult.Count > MassUserCreationValidationResultDto.LimitRecordsToBeUpLoaded)
                {
                    throw new ExceedLimitRecordFileException($"File should not exceed {MassUserCreationValidationResultDto.LimitRecordsToBeUpLoaded} records");
                }

                massUserCreationValidationResult.TotalRecords = massCreationsResult.Count;
            }
            catch (Exception ex)
            {
                if (ex is UnsupportedFileExtensionException)
                {
                    massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.FileFormatIsNotCorrect, Message = ex.Message };
                }
                if (ex is UnsupportFileTemplateException)
                {
                    massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.FileTemplateIsInvalid, Message = ex.Message };
                }
                if (ex is EmptyFileException)
                {
                    massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.FileIsEmpty, Message = ex.Message };
                }
                if (ex is ExceedLimitRecordFileException)
                {
                    massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.FileExceedLimitRecord, Message = ex.Message };
                }
                if (ex is UserRowEmptyException)
                {
                    massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.UserRowEmptyException, Message = ex.Message };
                }
                if (ex is BadDataException)
                {
                    massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.FileIsEmpty, Message = ex.Message };
                }
            }

            return massUserCreationValidationResult;
        }

        public int GetNumberOfUserCreationRecord(Stream fileOnMemory)
        {
            var massCreationsResult = this._csvFileReader.ReadDataFromStream<MassUserCreationDto>(fileOnMemory,
                                                                                                 _integrationSetting.MassUserCreationMappingConfig);
            return massCreationsResult.Count;
        }

        public async Task<MassUserCreationValidationResultDto> ValidateMassUserCreationData(
            IWorkContext workContext,
            Stream fileOnMemory,
            string fileName)
        {
            var massUserCreationValidationResult = new MassUserCreationValidationResultDto();
            var massUserCreations = new List<MassUserCreationDto>();

            try
            {
                var massCreationsResult = this._csvFileReader.ReadDataFromStream<MassUserCreationDto>(fileOnMemory, _integrationSetting.MassUserCreationMappingConfig);
                massUserCreations.AddRange(massCreationsResult);

                //no data on csv file
                if (massUserCreations.IsNullOrEmpty())
                {
                    throw new EmptyFileException();
                }


                if (massUserCreations.Count > MassUserCreationValidationResultDto.LimitRecordsToBeUpLoaded)
                {
                    throw new ExceedLimitRecordFileException($"file should not exceed {MassUserCreationValidationResultDto.LimitRecordsToBeUpLoaded} records");

                }

                massUserCreations = MassUserCreationDto.GenerateRowNumber(massUserCreations);

                massUserCreationValidationResult.TotalRecords = massUserCreations.Count;

                var invalidUserCreationResults = await GetInValidUserCreation(workContext, massUserCreations);

                invalidUserCreationResults = InvalidMassUserCreationDto.GenerateRowNumber(invalidUserCreationResults);

                massUserCreationValidationResult.invalidMassUserCreationDto = invalidUserCreationResults;
            }
            catch (Exception ex)
            {
                massUserCreationValidationResult.Exception = new MassUserCreationException { ErrorType = MassUserCreationErrorTypeEnum.GeneralError, Message = ex.Message };
            }

            return massUserCreationValidationResult;
        }

        public async Task<List<UserGenericDto>> getUsersFromFileAsync(
            MassUserCreationValidationContract massUserCreationValidationContract,
             Stream fileOnMemory,
             IWorkContext workContext)
        {
            var createdUsers = new List<UserGenericDto>();
            var massUserCreations = _csvFileReader.ReadDataFromStream<MassUserCreationDto>(fileOnMemory, _integrationSetting.MassUserCreationMappingConfig);

            var importedUsers = MassUserCreationDto.GenerateRowNumber(massUserCreations);

            var departmentInfoDic = _departmentService.GetDepartmentByNames(importedUsers.Select(user => user.PlaceOfWork).Distinct().ToList())
                                                      .ToDictionary(departmentInfo => departmentInfo.Name);

            var allUserTypes = _userTypeService.GetUserTypes(
                           extIds: null,
                           archetypeIds: new List<ArchetypeEnum> { ArchetypeEnum.SystemRole },
                           userTypeIds: null,
                           ownerId: workContext.CurrentOwnerId,
                           includeLocalizedData: true);

            var authorizedtAccountExtIdRoles = new List<string> { "overallsystemadministrator", "useraccountadministrator" };

            var systemAdminRoleName = "System Administrator";

            var currentUserRoles = await _userRepository.GetOrSetUserRoleFromWorkContext(workContext);

            var hasAuthorizedtAccountRoles = currentUserRoles.Any(currentRole => authorizedtAccountExtIdRoles.Contains(currentRole.ExtId));

            var properEntityStatus = hasAuthorizedtAccountRoles ? EntityStatusEnum.New : EntityStatusEnum.PendingApproval2nd;

            // Mapping csv Users to UserGenericDto
            try
            {
                massUserCreations = massUserCreations
                    .Where(importedUser => !string.IsNullOrEmpty(importedUser.OfficialEmail) && !string.IsNullOrEmpty(importedUser.Name))
                    .ToList();

                massUserCreations.ForEach( importedUser =>
               {
                   var newUser = new UserGenericDto();
                   var importedUserSystemRoles = importedUser.SystemRole.Split(";");
                   var userTypes = GetUserTypesJson(importedUserSystemRoles, allUserTypes);

                   newUser.DepartmentId = departmentInfoDic[importedUser.PlaceOfWork].DepartmentId;
                   newUser.DepartmentName = importedUser.PlaceOfWork;
                   newUser.EmailAddress = importedUser.OfficialEmail;
                   newUser.FirstName = importedUser.Name;
                   newUser.Gender = importedUser.Gender.ToLower() == "male" ? (short?)0 : 1;
                   newUser.CustomData = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
                   newUser.CustomData.Add(BuildCustomDataKey(ArchetypeEnum.SystemRole), userTypes);

                   newUser.Identity = new IdentityDto()
                   {
                       Id = 0,
                       OwnerId = workContext.CurrentOwnerId,
                       CustomerId = workContext.CurrentCustomerId,
                       Archetype = ArchetypeEnum.Employee
                   };

                   #region Entity Status

                   newUser.EntityStatus = new EntityStatusDto()
                   {
                       Deleted = false,
                       ExternallyMastered = false,
                       StatusReasonId = EntityStatusReasonEnum.Unknown,
                       LastUpdatedBy = 0,
                       StatusId = properEntityStatus,
                       ActiveDate = importedUser.AccountActiveFrom.toDateTimeWithFormat(DateTimeConst.DATE_MONTH_YEAR)?
                                                                       .ToString(DateTimeConst.MONTH_DATE_YEAR, CultureInfo.InvariantCulture)
                                                                       .toDateTimeWithFormat(DateTimeConst.MONTH_DATE_YEAR),
                       ExpirationDate = importedUser.DateOfExpiryAccount.toDateTimeWithFormat(DateTimeConst.DATE_MONTH_YEAR)?
                                                                       .ToString(DateTimeConst.MONTH_DATE_YEAR, CultureInfo.InvariantCulture)
                                                                       .toDateTimeWithFormat(DateTimeConst.MONTH_DATE_YEAR)
                   };


                   #endregion

                   #region Dynamic Json Attribues

                   newUser.JsonDynamicAttributes = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);

                   var isStorageUnlimited = importedUserSystemRoles.Select(role => role.Trim()).Contains(systemAdminRoleName);

                   var dynamicAttributes = buildDynamicAttributes(importedUser.ReasonForUserAccountRequest,
                                                                  importedUser.Salutation,
                                                                  importedUser.AccountActiveFrom,
                                                                  importedUser.DateOfExpiryAccount,
                                                                  importedUser.PersonalSpaceLimitation,
                                                                  isStorageUnlimited);

                   JsonConvert.PopulateObject(dynamicAttributes, newUser.JsonDynamicAttributes);

                   #endregion

                   createdUsers.Add(newUser);
               });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return createdUsers;
        }

        private string BuildCustomDataKey(ArchetypeEnum archetype)
        {
            var propertyName = archetype.ToString().ToCharArray();
            propertyName[0] = Char.ToLower(propertyName[0]);
            return $"{new string(propertyName)}s";
        }

        private string buildDynamicAttributes(string reason, string salution, string activatedDate, string expiredDate, string personalStorageSize, bool isStorageUnlimited)
        {
            JObject jDyanamicAttributeResult = new JObject();
            JProperty signupReason = new JProperty("signupReason", reason);
            JProperty titleSalutation = new JProperty("titleSalutation", salution);
            JProperty activeDate = new JProperty("activeDate", activatedDate);
            JProperty expirationDate = new JProperty("expirationDate", expiredDate);
            JProperty finishOnBoarding = new JProperty("finishOnBoarding", false);
            JProperty personalStorage = new JProperty("personalStorageSize", personalStorageSize);
            JProperty isStorageUnlimit = new JProperty("isStorageUnlimited", isStorageUnlimited);

            jDyanamicAttributeResult.Add(signupReason);
            jDyanamicAttributeResult.Add(titleSalutation);
            jDyanamicAttributeResult.Add(activeDate);
            jDyanamicAttributeResult.Add(expirationDate);
            jDyanamicAttributeResult.Add(finishOnBoarding);
            jDyanamicAttributeResult.Add(personalStorage);
            jDyanamicAttributeResult.Add(isStorageUnlimit);

            return jDyanamicAttributeResult.ToString();
        }
        private JArray GetUserTypesJson(string[] importedRoles, List<UserTypeDto> userTypeDtos)
        {
            importedRoles = importedRoles.Select(role => role.Trim()).ToArray();
            var userTypes = userTypeDtos.Where(userType => importedRoles.Contains(userType.LocalizedData[0].Fields[0].LocalizedText)).ToList();
            int userTypeLength = userTypes.Count;

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var userTypesJson = JsonConvert.SerializeObject(userTypes, serializerSettings);

            JArray jUserTypes = JArray.Parse(userTypesJson);

            for (int index = 0; index < userTypeLength; index++)
            {
                JObject jIdentity = (JObject)jUserTypes[index]["identity"];
                JObject jEntityStatus = (JObject)jUserTypes[index]["entityStatus"];
                jIdentity["archetype"] = userTypes[index].Identity.Archetype.ToString();
                jEntityStatus["statusId"] = userTypes[index].EntityStatus.StatusId.ToString();
                jEntityStatus["statusReasonId"] = userTypes[index].EntityStatus.StatusReasonId.ToString();
            }

            return jUserTypes;
        }

        private async Task<List<InvalidMassUserCreationDto>> GetInValidUserCreation(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidMassUserCreationResult = new List<InvalidMassUserCreationDto>();

            if (massUserCreationDtos.IsNullOrEmpty())
                return invalidMassUserCreationResult;

            var invalidSalutationFieldResults = ValidateSalutionField(workContext, massUserCreationDtos);

            var invalidNameFieldResults = ValidateNameField(workContext, massUserCreationDtos);

            var invalidEmailFieldResults = await ValidateOfficialEmail(workContext, massUserCreationDtos);

            var invalidGenderFieldResults = ValidateGender(workContext, massUserCreationDtos, invalidSalutationFieldResults);

            var invalidPlaceOfWorkFieldResults = await ValidatePlaceOfWork(workContext, massUserCreationDtos);

            var invalidAccountActiveDateFieldResults = ValidateAccountActiveFrom(workContext, massUserCreationDtos);

            var invalidDateOfExpiryAccountFieldResults = ValidateDateOfExpiryAccount(workContext, massUserCreationDtos, invalidAccountActiveDateFieldResults);

            var invalidReasonFieldResults = ValidateReason(workContext, massUserCreationDtos);

            var invalidSystemRoleFieldResults = ValidateSystemRole(workContext, massUserCreationDtos);

            var invalidPersonalSpaceLimitationFieldResults = ValidatePersonalSpaceLimitation(workContext, massUserCreationDtos);

            invalidMassUserCreationResult.AddRange(invalidSalutationFieldResults);
            invalidMassUserCreationResult.AddRange(invalidNameFieldResults);
            invalidMassUserCreationResult.AddRange(invalidEmailFieldResults);
            invalidMassUserCreationResult.AddRange(invalidGenderFieldResults);
            invalidMassUserCreationResult.AddRange(invalidPlaceOfWorkFieldResults);
            invalidMassUserCreationResult.AddRange(invalidAccountActiveDateFieldResults);
            invalidMassUserCreationResult.AddRange(invalidDateOfExpiryAccountFieldResults);
            invalidMassUserCreationResult.AddRange(invalidReasonFieldResults);
            invalidMassUserCreationResult.AddRange(invalidSystemRoleFieldResults);
            invalidMassUserCreationResult.AddRange(invalidPersonalSpaceLimitationFieldResults);

            return invalidMassUserCreationResult;
        }

        private List<InvalidMassUserCreationDto> ValidateSalutionField(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidSalutationFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptySalutionMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Salutation,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var notExistSalutionMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Salutation,
                                                  MassUserCreationMessageValidationConstance.NotExist,
                                                  workContext.CurrentLocale);

            string[] validSalutions = { "Dr", "Mdm", "Miss", "Mr", "Mrs", "Ms" };

            // Empty salution
            var emptySalutions = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.Salutation));

            invalidSalutationFieldResults.AddRange(emptySalutions.Select(u => u.CreateInvalidResult(emptySalutionMessage)));

            // Not exist salution
            var notExistSalutions = new List<MassUserCreationDto>();
            var nonEmptySalutions = massUserCreationDtos.Except(emptySalutions);
            foreach (var importedUser in nonEmptySalutions)
            {
                if (!validSalutions.Contains(importedUser.Salutation))
                {
                    notExistSalutions.Add(importedUser);
                }
            }

            invalidSalutationFieldResults.AddRange(notExistSalutions.Select(u => u.CreateInvalidResult(notExistSalutionMessage)));

            return invalidSalutationFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidateNameField(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidNameFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptyNameMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Name,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var specialCharsMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Name,
                                                  MassUserCreationMessageValidationConstance.SpecialCharacters,
                                                  workContext.CurrentLocale);

            var outOfLimitMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.Name,
                                                              MassUserCreationMessageValidationConstance.OutOfLimit,
                                                              workContext.CurrentLocale);

            // Empty name
            var emptyNames = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.Name));

            invalidNameFieldResults.AddRange(emptyNames.Select(u => u.CreateInvalidResult(emptyNameMessage)));

            // Out of limit name
            var outOfLimitNames = massUserCreationDtos.Except(emptyNames).Where(u => u.Name.Trim().Length > 100);

            invalidNameFieldResults.AddRange(outOfLimitNames.Select(u => u.CreateInvalidResult(outOfLimitMessage)));

            // Special characters
            var specialCharsNames = massUserCreationDtos.Except(emptyNames)
                                                        .Except(outOfLimitNames)
                                                        .Where(u => !u.IsValidName());

            invalidNameFieldResults.AddRange(specialCharsNames.Select(u => u.CreateInvalidResult(specialCharsMessage)));

            return invalidNameFieldResults;
        }

        private async Task<List<InvalidMassUserCreationDto>> ValidateOfficialEmail(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidEmailFieldResults = new List<InvalidMassUserCreationDto>();
            var massUserCreationDic = massUserCreationDtos.GroupBy(u => u.OfficialEmail)
                                                          .Select(user => user.First())
                                                          .ToDictionary(u => u.OfficialEmail);


            // Get messages
            var emptyEmailMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.OfficialEmail,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var invalidEmailFormatMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.OfficialEmail,
                                                  MassUserCreationMessageValidationConstance.InvalidFormat,
                                                  workContext.CurrentLocale);

            var invalidDomainFormatMessage = this._massUserCreationMessageConfig
                          .GetValidationMessage(MassUserCreationMessageValidationConstance.OfficialEmail,
                                                MassUserCreationMessageValidationConstance.InvalidDomainFormat,
                                                workContext.CurrentLocale);

            var duplicateSystemDataMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.OfficialEmail,
                                                  MassUserCreationMessageValidationConstance.DuplicateSystemData,
                                                  workContext.CurrentLocale);

            var duplicateImportedDataMessage = this._massUserCreationMessageConfig
                          .GetValidationMessage(MassUserCreationMessageValidationConstance.OfficialEmail,
                                                MassUserCreationMessageValidationConstance.DuplicateImportedData,
                                                workContext.CurrentLocale);

            // Empty email case
            var emptyEmails = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.OfficialEmail));

            invalidEmailFieldResults.AddRange(emptyEmails.Select(u => u.CreateInvalidResult(emptyEmailMessage)));

            // Invalid email format case
            var invalidFormatEmails = massUserCreationDtos.Except(emptyEmails)
                                                          .Where(u => !RegexUtils.IsValidEmailFormat(u.OfficialEmail));

            invalidEmailFieldResults.AddRange(invalidFormatEmails.Select(x => x.CreateInvalidResult(invalidEmailFormatMessage)));

            // Invalid domain email case
            var invalidDoaminFormatEmails = massUserCreationDtos
                                                         .Except(emptyEmails)
                                                         .Except(invalidFormatEmails)
                                                         .Where(user => InvalidEmailDomain.INVALID_EMAIL_DOMAIN
                                                            .Any(invalidEmailDomain => user.OfficialEmail.EndsWith(invalidEmailDomain)));

            invalidEmailFieldResults.AddRange(invalidDoaminFormatEmails.Select(x => x.CreateInvalidResult(invalidDomainFormatMessage)));

            // Duplicate imported email case
            var usersWithValidEmail = massUserCreationDtos.Except(emptyEmails)
                                                          .Except(invalidFormatEmails)
                                                          .Except(invalidDoaminFormatEmails);

            var duplicateImportedData = usersWithValidEmail.GroupBy(u => u.OfficialEmail)
                                                           .Where(g => g.Count() > 1)
                                                           .Select(x => massUserCreationDic[x.Key].CreateInvalidResult(duplicateImportedDataMessage))
                                                           .ToList();

            invalidEmailFieldResults.AddRange(duplicateImportedData);

            // Duplicate System email case

            var importedEmails = massUserCreationDtos.Except(emptyEmails)
                                                     .Except(invalidFormatEmails)
                                                     .Except(invalidDoaminFormatEmails)
                                                     .Except(duplicateImportedData)
                                                     .Select(u => u.OfficialEmail).ToList();

            if (importedEmails.Count > 0)
            {
                var existdUsers = await this._userRepository.GetUsersAsync(emails: importedEmails);
                var duplicateSystemEmails = existdUsers.Items.Select(u => massUserCreationDic[u.Email].CreateInvalidResult(duplicateSystemDataMessage));
                invalidEmailFieldResults.AddRange(duplicateSystemEmails);
            }

            //var duplicateSystemEmails = massUserCreationDtos.Except(invalidDoaminFormatEmails)
            //                            .Where(u => existdUsers.Items.Any(existedUser => existedUser.Email.EqualsIgnoreCase(u.OfficialEmail)));


            return invalidEmailFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidateGender(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos, List<InvalidMassUserCreationDto> invalidSalutationFieldResults)
        {
            var invalidGenderFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptyGenderMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Gender,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var notExistGenderMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Gender,
                                                  MassUserCreationMessageValidationConstance.NotExist,
                                                  workContext.CurrentLocale);

            var irrelevanceGenderMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.Gender,
                                                  MassUserCreationMessageValidationConstance.Irrelevance,
                                                  workContext.CurrentLocale);

            string[] validSalutions = { "Dr", "Mdm", "Miss", "Mr", "Mrs", "Ms" };
            string[] genders = { "Male", "Female" };
            var genderFollowingSalutions = new Dictionary<string, string[]>()
            {
                { "Male", new string[] {"Dr", "Mr"} },
                { "Female", new string[] { "Mdm", "Miss", "Mrs", "Ms", "Dr" } }
            };

            // Empty Gender
            var emptyGenders = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.Gender));

            invalidGenderFieldResults.AddRange(emptyGenders.Select(u => u.CreateInvalidResult(emptyGenderMessage)));

            // Not exist Gender
            var notExistGender = massUserCreationDtos.Except(emptyGenders)
                                        .Where(u => !genders.Any(sex => sex.EqualsIgnoreCase(u.Gender)));

            invalidGenderFieldResults.AddRange(notExistGender.Select(u => u.CreateInvalidResult(notExistGenderMessage)));

            // Irrelevant
            var indexOfinvalidSalutationAccount = invalidSalutationFieldResults.Select(u => u.Number);
            var invalidSalutationAccount = massUserCreationDtos.Where(user => indexOfinvalidSalutationAccount.Any(userIndex => userIndex == user.Number));

            var irrelevantSalutionToUserGenderList = massUserCreationDtos
                                                        .Except(emptyGenders)
                                                        .Except(notExistGender)
                                                        .Except(invalidSalutationAccount)
                                                        .Where(u => !genderFollowingSalutions[u.Gender].Contains(u.Salutation.ToTitleCase()));

            invalidGenderFieldResults.AddRange(irrelevantSalutionToUserGenderList.Select(u => u.CreateInvalidResult(irrelevanceGenderMessage)));

            return invalidGenderFieldResults;
        }

        private async Task<List<InvalidMassUserCreationDto>> ValidatePlaceOfWork(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidPlaceOfWorkFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptyPlaceOfWorkMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.PlaceOfWork,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var notExistPlaceOfWorkMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.PlaceOfWork,
                                                  MassUserCreationMessageValidationConstance.NotExist,
                                                  workContext.CurrentLocale);

            var DepartmentProhibitedMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.PlaceOfWork,
                                                  MassUserCreationMessageValidationConstance.NoPermission,
                                                  workContext.CurrentLocale);

            // Empty place of work
            var emptyPlaceOfWorks = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.PlaceOfWork));

            invalidPlaceOfWorkFieldResults.AddRange(emptyPlaceOfWorks.Select(u => u.CreateInvalidResult(emptyPlaceOfWorkMessage)));

            // Not exist or permission denied case (not fulfil permision case)
            //var customerids = workContext.CurrentCustomerId > 0 ? new List<int> { workContext.CurrentCustomerId } : null;
            var improtedDepartments = massUserCreationDtos.Except(emptyPlaceOfWorks).Select(user => user.PlaceOfWork).Distinct().ToList();

            var existingDepartments = _departmentService.GetDepartmentByNames(improtedDepartments)
                                                        .Select(department => department.Name)
                                                        .ToList();

            var intersectDepartmentNames = existingDepartments.Intersect(improtedDepartments);

            var notExistUserWithImprotedDepartment = massUserCreationDtos.Except(emptyPlaceOfWorks)
                                                                         .Where(user => !intersectDepartmentNames
                                                                                        .Any(departmentName => departmentName == user.PlaceOfWork));

            invalidPlaceOfWorkFieldResults.AddRange(notExistUserWithImprotedDepartment.Select(u => u.CreateInvalidResult(notExistPlaceOfWorkMessage)));

            // Imported departments are not based on current user's org

            var topHierarchyDepartmentInfo = (await _departmentAccessService.GetTopHierarchyDepartmentsByWorkContext(_workContext));
            var topHierarchyDepartmentIdentity = topHierarchyDepartmentInfo.TopHierachyDepartmentIdentity;

            var defaultRootDepartmentId = topHierarchyDepartmentIdentity is object
                ? (int)topHierarchyDepartmentIdentity.Identity.Id
                : workContext.CurrentDepartmentId;


            var hierarchyDepartmentIdentities = await _departmentService.GetDepartmentHierachyDepartmentIdentitiesAsync(
                defaultRootDepartmentId,
                false,
                true,
                _workContext.CurrentOwnerId,
                _workContext.CurrentCustomerId > 0 ? new List<int> { _workContext.CurrentCustomerId } : null,
                null,
                null,
                false,
                null,
                null,
                false,
                getParentNode: false,
                countUser: false,
                countUserEntityStatuses: null,
                jsonDynamicData: null,
                checkPermission: !(await _hierarchyDepartmentPermissionService.IgnoreSecurityCheckAsync()));

            var existingDepartmentNamesByCurrentUser = hierarchyDepartmentIdentities.Select(departmentInfor => departmentInfor.DepartmentName);

            var userWithInvalidDepartmentName = massUserCreationDtos.Except(emptyPlaceOfWorks)
                                                                    .Except(notExistUserWithImprotedDepartment)
                                                                    .Where(user => !existingDepartmentNamesByCurrentUser
                                                                        .Contains(user.PlaceOfWork));


            invalidPlaceOfWorkFieldResults.AddRange(userWithInvalidDepartmentName.Select(u => u.CreateInvalidResult(DepartmentProhibitedMessage)));

            return invalidPlaceOfWorkFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidateAccountActiveFrom(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidAccountActiveDateFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptyAccountActiveDateMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.AccountActiveFrom,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var invalidAccountActiveDateMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.AccountActiveFrom,
                                                  MassUserCreationMessageValidationConstance.InvalidFormat,
                                                  workContext.CurrentLocale);

            var accountActiveDateInPassMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.AccountActiveFrom,
                                                              MassUserCreationMessageValidationConstance.InPast,
                                                              workContext.CurrentLocale);

            // Empty name case
            var emptyAccountActiveDate = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.AccountActiveFrom));

            invalidAccountActiveDateFieldResults.AddRange(emptyAccountActiveDate.Select(u => u.CreateInvalidResult(emptyAccountActiveDateMessage)));

            // Invalid Date format case 
            var invalidDateFormats = massUserCreationDtos.Except(emptyAccountActiveDate)
                                        .Where(u => !DateTimeHelper.isMatchFormat(u.AccountActiveFrom, "dd/MM/yyyy"));

            invalidAccountActiveDateFieldResults.AddRange(invalidDateFormats.Select(u => u.CreateInvalidResult(invalidAccountActiveDateMessage)));



            // In past case 
            var usersWithValidAccountActiveDate = massUserCreationDtos.Except(invalidDateFormats)
                                                                      .Except(emptyAccountActiveDate)
                                                                      .ToList();


            var pastAccountActiveDates = usersWithValidAccountActiveDate.Where(u
                                        => DateTime.ParseExact(u.AccountActiveFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date < DateTime.UtcNow.Date);

            invalidAccountActiveDateFieldResults.AddRange(pastAccountActiveDates.Select(u => u.CreateInvalidResult(accountActiveDateInPassMessage)));

            return invalidAccountActiveDateFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidateDateOfExpiryAccount(IWorkContext workContext,
                                                                                         List<MassUserCreationDto> massUserCreationDtos,
                                                                                         List<InvalidMassUserCreationDto> usersWithInvalidAccountActiveDateField)
        {
            var invalidDateOfExpiryAccountFieldResults = new List<InvalidMassUserCreationDto>();
            //var massUserCreationDic = massUserCreationDtos.ToDictionary(u => u.OfficialEmail);

            // Get messages
            var emptyDateOfExpiryAccountMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.DateOfExpiryAccount,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var invalidDateOfExpiryAccountMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.DateOfExpiryAccount,
                                                  MassUserCreationMessageValidationConstance.InvalidFormat,
                                                  workContext.CurrentLocale);

            var dateOfExpiryAccountInPassMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.DateOfExpiryAccount,
                                                              MassUserCreationMessageValidationConstance.InPast,
                                                              workContext.CurrentLocale);

            var invalidDateRangeMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.DateOfExpiryAccount,
                                                              MassUserCreationMessageValidationConstance.InvalidDateRange,
                                                              workContext.CurrentLocale);

            // Empty date of expiry account case
            var emptyDateOfExpiryAccounts = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.DateOfExpiryAccount));

            invalidDateOfExpiryAccountFieldResults.AddRange(emptyDateOfExpiryAccounts.Select(u => u.CreateInvalidResult(emptyDateOfExpiryAccountMessage)));

            // Invalid Date format case 
            var invalidDateFormats = massUserCreationDtos.Except(emptyDateOfExpiryAccounts)
                                                         .Where(u => !DateTimeHelper.isMatchFormat(u.DateOfExpiryAccount, "dd/MM/yyyy"));

            invalidDateOfExpiryAccountFieldResults.AddRange(invalidDateFormats.Select(u => u.CreateInvalidResult(invalidDateOfExpiryAccountMessage)));

            // In past case 
            var pastDateOfExpiryAccount = massUserCreationDtos.Except(invalidDateFormats)
                                                              .Except(emptyDateOfExpiryAccounts)
                                        .Where(u => DateTime.ParseExact(u.DateOfExpiryAccount, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date < DateTime.UtcNow.Date);

            invalidDateOfExpiryAccountFieldResults.AddRange(pastDateOfExpiryAccount.Select(u => u.CreateInvalidResult(dateOfExpiryAccountInPassMessage)));

            // Invalid Date Range Case: Expiry Date compares with Account active date (Expriry date > Active date).
            // Thus, comparision happens only when Account active date field is VALID first

            // get users with valid account active date
            var indexOfinvalidAccountActive = usersWithInvalidAccountActiveDateField.Select(u => u.Number);
            var validAccountActiveDate = massUserCreationDtos.Where(user => !indexOfinvalidAccountActive.Any(userIndex => userIndex == user.Number));
            // get users who satisfied conditions of Date of expiry field from list of users with valid account active date
            var validFormatDateOfExpiryAccounts = validAccountActiveDate.Where(u => !string.IsNullOrEmpty(u.DateOfExpiryAccount)
                                                                                    && DateTimeHelper.isMatchFormat(u.DateOfExpiryAccount, DateTimeConst.DATE_MONTH_YEAR)
                                                                                    && u.DateOfExpiryAccount.toDateTimeWithFormat(DateTimeConst.DATE_MONTH_YEAR).Value.Date >= DateTime.UtcNow.Date);

            // get list of users with invalid Date of expiry field
            var invalidDateOfExpiryAccounts = validFormatDateOfExpiryAccounts.Where(u => u.AccountActiveFrom.toDateTimeWithFormat(DateTimeConst.DATE_MONTH_YEAR).Value.Date >=
                                                                                         u.DateOfExpiryAccount.toDateTimeWithFormat(DateTimeConst.DATE_MONTH_YEAR).Value.Date);

            invalidDateOfExpiryAccountFieldResults.AddRange(invalidDateOfExpiryAccounts.Select(u => u.CreateInvalidResult(invalidDateRangeMessage)));

            return invalidDateOfExpiryAccountFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidateReason(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidReasonFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptyReasonMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.ReasonForUserAccountRequest,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var specialCharsMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.ReasonForUserAccountRequest,
                                                  MassUserCreationMessageValidationConstance.SpecialCharacters,
                                                  workContext.CurrentLocale);

            var outOfLimitMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.ReasonForUserAccountRequest,
                                                              MassUserCreationMessageValidationConstance.OutOfLimit,
                                                              workContext.CurrentLocale);

            // Empty reason
            var emptyReasons = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.ReasonForUserAccountRequest));

            invalidReasonFieldResults.AddRange(emptyReasons.Select(u => u.CreateInvalidResult(emptyReasonMessage)));

            // Out of limit reason
            var outOfLimitReasons = massUserCreationDtos.Except(emptyReasons).Where(u => u.ReasonForUserAccountRequest.Trim().Length > 1000);

            invalidReasonFieldResults.AddRange(outOfLimitReasons.Select(u => u.CreateInvalidResult(outOfLimitMessage)));

            // Special characters
            var specialCharsReasons = massUserCreationDtos.Except(emptyReasons)
                                                          .Except(outOfLimitReasons)
                                                          .Where(u => !u.IsValidReason());

            invalidReasonFieldResults.AddRange(specialCharsReasons.Select(u => u.CreateInvalidResult(specialCharsMessage)));

            return invalidReasonFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidateSystemRole(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidSystemRoleFieldResults = new List<InvalidMassUserCreationDto>();
            var indexedUsersDic = massUserCreationDtos.ToDictionary(u => u.Number);

            // Get messages
            var emptyRoleMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.SystemRole,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var notExistRolesMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.SystemRole,
                                                              MassUserCreationMessageValidationConstance.NotExist,
                                                              workContext.CurrentLocale);

            var InvalidFormatRolesMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.SystemRole,
                                                            MassUserCreationMessageValidationConstance.InvalidFormat,
                                                            workContext.CurrentLocale);

            var duplicateImportedRolesMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.SystemRole,
                                                              MassUserCreationMessageValidationConstance.DuplicateImportedData,
                                                              workContext.CurrentLocale);

            var noPermissioneMessage = this._massUserCreationMessageConfig
                                       .GetValidationMessage(MassUserCreationMessageValidationConstance.SystemRole,
                                                             MassUserCreationMessageValidationConstance.NoPermission,
                                                             workContext.CurrentLocale);

            // Empty Role Case
            var emptyRoles = massUserCreationDtos.Where(user => string.IsNullOrEmpty(user.SystemRole));

            invalidSystemRoleFieldResults.AddRange(emptyRoles.Select(u => u.CreateInvalidResult(emptyRoleMessage)));

            // Invalid format Role Case
            var nonEmptyRoles = massUserCreationDtos.Except(emptyRoles).ToList();
            var invalidFormatRoles = new List<MassUserCreationDto>();
            var validRolesDic = new Dictionary<int, string[]>();
            var length = nonEmptyRoles.Count();

            for (int index = 0; index < length; index++)
            {
                string[] splitedRoles = nonEmptyRoles[index].SystemRole.Split(";");
                if (!isValidRoleFormat(nonEmptyRoles[index].SystemRole))
                {
                    invalidFormatRoles.Add(nonEmptyRoles[index]);
                    continue;
                }
                validRolesDic.Add(nonEmptyRoles[index].Number, splitedRoles);
            }

            invalidSystemRoleFieldResults.AddRange(invalidFormatRoles.Select(u => u.CreateInvalidResult(InvalidFormatRolesMessage)));

            // Duplicate imported roles
            var usersWithduplicateRole = massUserCreationDtos
                                                    .Except(emptyRoles)
                                                    .Except(invalidFormatRoles)
                                                    .Where(u => validRolesDic[u.Number].Select(role => role.Trim()).ToArray().hasDuplicateData());

            invalidSystemRoleFieldResults.AddRange(usersWithduplicateRole.Select(user => user.CreateInvalidResult(duplicateImportedRolesMessage)));

            // Not exist
            var userTypes = _userTypeService.GetUserTypes(
                           extIds: null,
                           archetypeIds: new List<ArchetypeEnum> { ArchetypeEnum.SystemRole },
                           userTypeIds: null,
                           ownerId: workContext.CurrentOwnerId,
                           includeLocalizedData: true);

            var existingRoles = userTypes.Select(userType => userType.LocalizedData[0].Fields[0].LocalizedText);

            var notExistUserFromImprotedRoles = massUserCreationDtos
                                    .Except(emptyRoles)
                                    .Except(invalidFormatRoles)
                                    .Except(usersWithduplicateRole)
                                    .Where(user => existingRoles
                                                  .Intersect(validRolesDic[user.Number]
                                                             .Select(role => role.Trim())).Count() < validRolesDic[user.Number].Count());

            invalidSystemRoleFieldResults.AddRange(notExistUserFromImprotedRoles.Select(u => u.CreateInvalidResult(notExistRolesMessage)));

            // No permission to import roles

            var existingRolesDic = userTypes.ToDictionary(userType => userType.LocalizedData[0].Fields[0].LocalizedText);

            var usersWithValidRoles = massUserCreationDtos
                                    .Except(emptyRoles)
                                    .Except(invalidFormatRoles)
                                    .Except(usersWithduplicateRole)
                                    .Except(notExistUserFromImprotedRoles)
                                    .ToList();

            var assignRolePermission = _userAccessService.GetAssignRolePermission(workContext);

            var usersWithoutAuthorizedPermissionToAssignRoles = new List<MassUserCreationDto>();

            usersWithValidRoles.ForEach(user =>
            {
                var userRoles = user.SystemRole.Split(";").Select(roleName => existingRolesDic[roleName.Trim()] ?? null);
                if(!IsAllowToAssignSystemRoles(assignRolePermission, userRoles))
                {
                    usersWithoutAuthorizedPermissionToAssignRoles.Add(user);
                }
            });
          
            invalidSystemRoleFieldResults.AddRange(usersWithoutAuthorizedPermissionToAssignRoles.Select(u => u.CreateInvalidResult(noPermissioneMessage)));

            return invalidSystemRoleFieldResults;
        }

        private List<InvalidMassUserCreationDto> ValidatePersonalSpaceLimitation(IWorkContext workContext, List<MassUserCreationDto> massUserCreationDtos)
        {
            var invalidPersonalSpaceLimitationFieldResults = new List<InvalidMassUserCreationDto>();

            // Get messages
            var emptyPersonalSpaceLimitationMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.PersonalSpaceLimitation,
                                                  MassUserCreationMessageValidationConstance.Empty,
                                                  workContext.CurrentLocale);

            var InvalidFormatPersonalSpaceLimitationMessage = this._massUserCreationMessageConfig
                            .GetValidationMessage(MassUserCreationMessageValidationConstance.PersonalSpaceLimitation,
                                                  MassUserCreationMessageValidationConstance.InvalidFormat,
                                                  workContext.CurrentLocale);

            var outOfLimitPersonalSpaceLimitationMessage = this._massUserCreationMessageConfig
                                        .GetValidationMessage(MassUserCreationMessageValidationConstance.PersonalSpaceLimitation,
                                                              MassUserCreationMessageValidationConstance.OutOfLimit,
                                                  workContext.CurrentLocale);

            // Empty reason
            var emptyPersonalSpaceLimitation = massUserCreationDtos.Where(u => string.IsNullOrEmpty(u.PersonalSpaceLimitation));

            invalidPersonalSpaceLimitationFieldResults.AddRange(emptyPersonalSpaceLimitation.Select(u => u.CreateInvalidResult(emptyPersonalSpaceLimitationMessage)));

            // Invalid format
            var invalidFormatPersonalSpaceLimitation = massUserCreationDtos.Except(emptyPersonalSpaceLimitation)
                                                          .Where(u => !int.TryParse(u.PersonalSpaceLimitation, out int result));

            invalidPersonalSpaceLimitationFieldResults.AddRange(invalidFormatPersonalSpaceLimitation.Select(u => u.CreateInvalidResult(InvalidFormatPersonalSpaceLimitationMessage)));

            // Out of limit reason
            var outOfLimitPersonalSpaceLimitation = massUserCreationDtos
                                                        .Except(emptyPersonalSpaceLimitation)
                                                        .Except(invalidFormatPersonalSpaceLimitation)
                                                        .Where(u => int.Parse(u.PersonalSpaceLimitation) != 10);

            invalidPersonalSpaceLimitationFieldResults.AddRange(outOfLimitPersonalSpaceLimitation.Select(u => u.CreateInvalidResult(outOfLimitPersonalSpaceLimitationMessage)));


            return invalidPersonalSpaceLimitationFieldResults;
        }

        private bool IsAllowToAssignSystemRoles(EditabilityAccessSettingElement assignRolePermission, IEnumerable<UserTypeDto> validImportedRoles)
        {
            if (assignRolePermission.RestrictedProperties == null && assignRolePermission.RestrictedProperties.Count == 0)
            {
                return false;
            }

            foreach (var restrictedPropertyKeyValue in assignRolePermission.RestrictedProperties)
            {
                var propertyName = restrictedPropertyKeyValue.Key;
                var restrictedProperty = restrictedPropertyKeyValue.Value;

                if (MatchProperty(propertyName, "systemRoles"))
                {
                    var restrictAnyValue = restrictedProperty.AllowedValues.IsNullOrEmpty();

                    foreach (var importedRole in validImportedRoles)
                    {
                        if (restrictAnyValue || !restrictedProperty.AllowedValues.Contains(importedRole.Identity.ExtId))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool MatchProperty(string property, string name)
        {
            return string.Equals(property, name, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool isValidRoleFormat(string roles)
        {
            if (roles is null)
            {
                return false;
            }

            var splitedRoles = roles.Split(";").Select(role => role.Trim());
            var excludeAlphabetPattern = @"[^A-Za-z\s]";
            foreach (var role in splitedRoles)
            {
                if (RegexUtils.isMatchPatternInRangeOfWords(role, excludeAlphabetPattern) || string.IsNullOrEmpty(role))
                {
                    return false;
                }
            }

            return true;
        }

        private bool isEmptyUserRow(List<MassUserCreationDto> massUsersCreationDtos)
        {
            foreach (var user in massUsersCreationDtos)
            {
                var isRowEmpty = user.GetType()
                                     .GetProperties()
                                     .Where(property => property.Name != "Number")
                                     .All(property => string.IsNullOrEmpty(property.GetValue(user).ToString()));
                if (isRowEmpty)
                {
                    return true;
                }

            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using cxOrganization.Business.CQRSClientServices;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace cxOrganization.Business.JsonAnalyzer
{
    public class ImportSchoolnAnalyzer : IImportSchoolAnalyzer
    {
        private readonly ILogger _logger;
        private readonly IUserService _learnerService;
        private readonly IUserService _employeeService;
        private readonly IUserGroupService _teachingGroupService;
        private readonly IDepartmentService _schoolService;
        private readonly IDepartmentService _classService;
        private readonly ILevelService _levelService;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IClassMemberService _classMemberService;
        private readonly IUserTypeService _userTypeService;
        private readonly IUGMemberService _uGMemberService;
        private readonly IMoveAssesmentClientService _moveAssesmentClientService;
        private readonly IUGMemberRepository _uGMemberRepository;
        private readonly OrganizationDbContext _organizationUnitOfWork;
        private readonly IDepartmentService _departmentService;
        private readonly ICommonService _commonService;
        public ImportSchoolnAnalyzer(IAdvancedWorkContext workContext,
            IClassMemberService classMemberService,
            Func<ArchetypeEnum, IUGMemberService> uGMemberService,
            ILevelService levelService,
            IMoveAssesmentClientService moveAssesmentClientService,
            IDepartmentService departmentService,
            Func<ArchetypeEnum, IUserService> userServiceResolver,
            Func<ArchetypeEnum, IDepartmentService> departmentServiceResolver,
            Func<ArchetypeEnum, IUserGroupService> userGroupServiceResolver,
            Func<ArchetypeEnum, IUserTypeService> usertypeServiceResolver,
            OrganizationDbContext organizationDbContext,
            IUGMemberRepository uGMemberRepository,
            ILoggerFactory loggerFactory,
            ICommonService commonService)
        {
            _schoolService = departmentServiceResolver(ArchetypeEnum.School);
            _classService = departmentServiceResolver(ArchetypeEnum.Class);
            _teachingGroupService = userGroupServiceResolver(ArchetypeEnum.TeachingGroup);
            _learnerService = userServiceResolver(ArchetypeEnum.Learner);
            _employeeService = userServiceResolver(ArchetypeEnum.Employee);
            _classMemberService = classMemberService;
            _workContext = workContext;
            _levelService = levelService;
            _userTypeService = usertypeServiceResolver(ArchetypeEnum.Learner);
            _uGMemberService = uGMemberService(ArchetypeEnum.TeachingGroup);
            _moveAssesmentClientService = moveAssesmentClientService;
            _uGMemberRepository = uGMemberRepository;
            _organizationUnitOfWork = organizationDbContext;
            //organizationDbContext.AutoDetectChangesEnabled = false;
            _departmentService = departmentService;
            _commonService = commonService;
            _logger = loggerFactory.CreateLogger<ImportSchoolnAnalyzer>();
        }

        public dynamic GetSchoolDataState(string school, int ownerid, int customerId)
        {

            List<string> errors = new List<string>();
            var settings = new JsonSerializerSettings
            {
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    errors.Add(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = false;
                },
                Converters = { new JsonDynamicObjectConverter() }
            };

            dynamic schoolImport = JsonConvert.DeserializeObject<JsonDynamicObject>(school, settings);
            _logger.LogInformation($"Received import data from school {schoolImport.Name}, started getting states...");
            if (school == null)
                throw new OrganizationDomainException(System.Net.HttpStatusCode.BadRequest, "School payload expected");
            //dynamic schoolImport = school;
            List<string> classExtids = GetImportClassExtIds(schoolImport);
            List<string> teachingGroupExtids = GetImportTeachingGroupExtIds(schoolImport);
            List<string> educationProgramExtids = GetImportProgramEducationExtIds(schoolImport);

            List<string> employeesExtids = GetImportEmployeesExtIds(schoolImport);
            List<string> learnersExtids = GetImportLearnersExtIds(schoolImport);

            var schoolInDb = _schoolService.GetDepartments<SchoolDto>(ownerid, customerIds: new List<int> { customerId },
                extIds: new List<string> { schoolImport.externalId }).Items.FirstOrDefault();
            if (schoolInDb == null)
                throw new OrganizationDomainException(System.Net.HttpStatusCode.BadRequest, string.Format("School {0}-{1} was not found in customer {2}, owner {3}", schoolImport.Name, schoolImport.externalId, customerId, ownerid));
            var classesState = new List<ExpandoObject>();
            if (classExtids.Any())
            {
                var classes = _classService.GetDepartments<ClassDto>(ownerid, customerIds: new List<int> { customerId }, parentDepartmentId: (int)schoolInDb.Identity.Id.Value,
                    extIds: classExtids, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;

                classesState = GetClassesStates(ownerid, customerId, (int)schoolInDb.Identity.Id.Value, classes, schoolImport);
            }
            var groupsState = new List<ExpandoObject>();
            if (teachingGroupExtids.Any())
            {
                var groups = _teachingGroupService.GetUserGroups<TeachingGroupDto>(ownerid, customerIds: new List<int> { customerId },
                extIds: teachingGroupExtids, departmentIds: new List<int> { (int)schoolInDb.Identity.Id.Value }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;
                groupsState = GetGroupsStates(ownerid, customerId, (int)schoolInDb.Identity.Id.Value, groups, schoolImport.TeachingGroups, ArchetypeEnum.TeachingGroup);
            }

            var educationPrograms = new List<ExpandoObject>();
            if (educationProgramExtids.Any())
            {
                var groups = _teachingGroupService.GetUserGroups<TeachingGroupDto>(ownerid, customerIds: new List<int> { customerId },
                    extIds: educationProgramExtids, departmentIds: new List<int> { (int)schoolInDb.Identity.Id.Value }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;
                educationPrograms = GetGroupsStates(ownerid, customerId, (int)schoolInDb.Identity.Id.Value, groups, schoolImport.EducationPrograms, ArchetypeEnum.EducationProgram);
            }
            var employeeStates = new List<ExpandoObject>();
            if (employeesExtids.Any())
            {
                var employees = _employeeService.GetUsers<EmployeeDto>(ownerid, new List<int> { customerId }, extIds: employeesExtids,
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;
                employeeStates = GetEmployeesStates(ownerid, customerId, (int)schoolInDb.Identity.Id.Value, employees, schoolImport);
            }
            var learnerStates = new List<ExpandoObject>();
            if (learnersExtids.Any())
            {
                var learners = _learnerService.GetUsers<LearnerDto>(ownerid, new List<int> { customerId }, extIds: learnersExtids,
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items;
                learnerStates = GetLearnersStates(ownerid, customerId, (int)schoolInDb.Identity.Id.Value, learners, schoolImport);
            }
            dynamic state = new ExpandoObject();
            state.Classes = classesState;
            state.TeachingGroups = groupsState;
            state.EducationPrograms = educationPrograms;
            state.Employees = employeeStates;
            state.Learners = learnerStates;

            state.SchoolName = schoolImport.Name;
            state.SchoolExternalId = schoolImport.ExternalId;
            _logger.LogInformation($"Finished getting states for school {schoolImport.Name})");
            return state;
        }

        private List<string> GetImportLearnersExtIds(dynamic schoolImport)
        {
            if (schoolImport.learners == null)
                return new List<string>();
            var data = (schoolImport.learners as List<dynamic>)
            .Select(x => x.ExternalId as string).Where(x => !string.IsNullOrEmpty(x));
            return data.Distinct().ToList();
        }

        private List<string> GetImportEmployeesExtIds(dynamic schoolImport)
        {
            if (schoolImport.employees == null)
                return new List<string>();
            var data = (schoolImport.employees as List<dynamic>)
            .Select(x => x.ExternalId as string).Where(x => !string.IsNullOrEmpty(x)); ;
            return data.Distinct().ToList();
        }


        private List<string> GetImportTeachingGroupExtIds(dynamic schoolImport)
        {
            if (schoolImport.teachinggroups == null && schoolImport.EducationPrograms)
                return new List<string>();
            var data = (schoolImport.teachinggroups as List<dynamic>)
            .Select(x => x.ExternalId as string).Where(x => !string.IsNullOrEmpty(x));

            var dataEducations = (schoolImport.EducationPrograms as List<dynamic>)
                .Select(x => x.ExternalId as string).Where(x => !string.IsNullOrEmpty(x));
            var returnList = new List<string>();
            returnList.AddRange(data.Distinct().ToList());
            returnList.AddRange(dataEducations.Distinct().ToList());
            return returnList;
        }

        private List<string> GetImportProgramEducationExtIds(dynamic schoolImport)
        {
            if (schoolImport.EducationPrograms == null)
                return new List<string>();
            var data = (schoolImport.EducationPrograms as List<dynamic>)
                .Select(x => x.ExternalId as string).Where(x => !string.IsNullOrEmpty(x)); ;
            return data.Distinct().ToList();
        }
        private List<string> GetImportClassExtIds(dynamic schoolImport)
        {
            if (schoolImport.classes == null)
                return new List<string>();
            var data = (schoolImport.classes as List<dynamic>)
            .Select(x => x.ExternalId as string).Where(x => !string.IsNullOrEmpty(x)); ;
            return data.Distinct().ToList();
        }

        private DateTime? GetJsonDate(object dateValue)
        {
            if (dateValue is string)
            {
                throw new Exception("Datetime is not valid " + dateValue);
            }
            if (dateValue == null)
                return null;
            return (DateTime)dateValue;
        }

        public dynamic AnalyzeSchoolState(string schoolDataStates, int ownerid, int customerId)
        {
            dynamic data = JsonConvert.DeserializeObject<JsonDynamicObject>(schoolDataStates, new JsonDynamicObjectConverter());
            if (schoolDataStates == null)
                throw new Exception("School states payload expected");
            dynamic schoolCommands = new ExpandoObject();
            schoolCommands.SchoolName = data.SchoolName;
            schoolCommands.SchoolExternalId = data.SchoolExternalId;
            List<ExpandoObject> commands = new List<ExpandoObject>();
            var jsonSchoolDataStates = schoolDataStates;
            _logger.LogInformation($"Received states from school {data.SchoolName}, started analyzing...");
            commands.AddRange(AnalyzeClassCommands(ownerid, customerId, data));
            commands.AddRange(AnalyzeTeachingGroupCommands(ownerid, customerId, data.TeachingGroups, ArchetypeEnum.TeachingGroup));
            commands.AddRange(AnalyzeTeachingGroupCommands(ownerid, customerId, data.EducationPrograms, ArchetypeEnum.EducationProgram));
            commands.AddRange(AnalyzeEmployeeCommands(ownerid, customerId, data));
            commands.AddRange(AnalyzeLearnerCommands(ownerid, customerId, data));
            _logger.LogInformation($"Finished getting states for school {data.SchoolName}");
            schoolCommands.Commands = commands;
            return schoolCommands;
        }



        public dynamic ExecuteSchoolCommand(string schoolCommands, int ownerId, int customerId)
        {
            dynamic data = JsonConvert.DeserializeObject<JsonDynamicObject>(schoolCommands, new JsonDynamicObjectConverter());
            if (schoolCommands == null)
                throw new Exception("School commands payload expected");
            dynamic schoolCommandsResult = new ExpandoObject();
            var results = new List<dynamic>();
            //return results;
            var commands = data.Commands;
            var listAssessmentInfo = new List<ExpandoObject>();
            _logger.LogInformation($"Received commands from school {data.SchoolName}, started executing...");
            foreach (dynamic command in commands)
            {
                var jsonCommand = command;
                ArchetypeEnum commandArchetype = ArchetypeEnum.Unknown;
                var jsonIdentity = jsonCommand.identity;
                Enum.TryParse(jsonIdentity.archetype, out commandArchetype);

                switch (commandArchetype)
                {
                    case ArchetypeEnum.Class:
                        var commandResult = ExecuteClassCommand(jsonCommand);
                        results.Add(commandResult);
                        break;
                    case ArchetypeEnum.TeachingGroup:
                        commandResult = ExecuteTeachingGroupCommand(jsonCommand);
                        results.Add(commandResult);
                        break;
                    case ArchetypeEnum.EducationProgram:
                        commandResult = ExecuteTeachingGroupCommand(jsonCommand);
                        results.Add(commandResult);
                        break;
                    case ArchetypeEnum.Employee:
                        commandResult = ExecuteEmployeeCommand(jsonCommand);
                        results.Add(commandResult);
                        break;
                    case ArchetypeEnum.Learner:
                        commandResult = ExecuteLearnerCommand(jsonCommand, listAssessmentInfo);
                        results.Add(commandResult);
                        break;
                }
            }
            //ProcessMoveAssessment(listAssessmentInfo, ownerId, customerId);
            schoolCommandsResult.SchoolName = data.SchoolName;
            schoolCommandsResult.SchoolExternalId = data.SchoolExternalId;
            schoolCommandsResult.CommandResults = results;
            _logger.LogInformation($"Finished executing command for school {data.SchoolName}");
            return schoolCommandsResult;
        }

        #region Group

        private List<ExpandoObject> GetGroupsStates(int ownerid, int customerId, int schoolId, List<TeachingGroupDto> currentTeachingGroups, dynamic groups, ArchetypeEnum archetype)
        {
            List<ExpandoObject> groupStates = new List<ExpandoObject>();
            foreach (dynamic importTeachingGroup in groups)
            {
                var importTeachingGroupData = importTeachingGroup;
                var currentGroup = currentTeachingGroups.FirstOrDefault(x => x.Identity.ExtId == importTeachingGroupData.externalid);
                if (currentGroup == null)
                {
                    dynamic groupState = new ExpandoObject();
                    groupState.State = DataStates.New.ToString();
                    groupState.Identity = new IdentityDto
                    {
                        Archetype = archetype,
                        CustomerId = customerId,
                        ExtId = importTeachingGroupData.ExternalId,
                        OwnerId = ownerid,
                        Id = null
                    };
                    groupState.Field = new
                    {
                        SchoolId = schoolId,
                        TeachingGroupName = importTeachingGroupData.Name,
                        Description = importTeachingGroupData.Description,
                        Period = importTeachingGroupData.Period,
                        SubjectCode = importTeachingGroupData.SubjectCode,
                        SubjectUuid = importTeachingGroupData.SubjectUuid,
                    };
                    groupStates.Add(groupState);
                }
                else
                {
                    var isUpdate = false;
                    dynamic groupState = new ExpandoObject();
                    groupState.State = DataStates.Modified.ToString();
                    groupState.Field = new ExpandoObject();
                    groupState.Field.SchoolId = schoolId;
                    groupState.Identity = currentGroup.Identity;
                    var currentName = currentGroup.Name ?? string.Empty;
                    var currentDescription = currentGroup.Description ?? string.Empty;
                    var currentSubjectCode = currentGroup.SubjectCode ?? string.Empty;
                    if (importTeachingGroupData.Name != null && importTeachingGroupData.Name != currentName)
                    {
                        groupState.Field.TeachingGroupName = importTeachingGroupData.Name;
                        isUpdate = true;
                    }
                    if (importTeachingGroupData.Description != null && importTeachingGroupData.Description != currentDescription)
                    {
                        groupState.Field.Description = importTeachingGroupData.Description;
                        isUpdate = true;
                    }

                    if (importTeachingGroupData.SubjectCode != null && importTeachingGroupData.SubjectCode != currentSubjectCode)
                    {
                        groupState.Field.SubjectCode = importTeachingGroupData.SubjectCode;
                        isUpdate = true;
                    }
                    if (isUpdate)
                    {
                        groupStates.Add(groupState);
                    }
                }
            }

            return groupStates;
        }

        private List<ExpandoObject> AnalyzeTeachingGroupCommands(int ownerid, int customerId, dynamic groups, ArchetypeEnum archetype)
        {
            List<ExpandoObject> commands = new List<ExpandoObject>();
            foreach (dynamic groupState in groups)
            {
                var jsonGroupState = groupState;
                DataStates dataState = DataStates.NotModified;
                Enum.TryParse(jsonGroupState.State, out dataState);
                switch (dataState)
                {
                    case DataStates.New:
                        {
                            dynamic insertCommand = new ExpandoObject();
                            insertCommand.CommandType = CommandType.Insert;
                            insertCommand.Data = jsonGroupState.Field;
                            insertCommand.Identity = new IdentityDto
                            {
                                Archetype = archetype,
                                CustomerId = customerId,
                                OwnerId = ownerid,
                                ExtId = jsonGroupState.Identity.ExtId,
                                Id = 0
                            };
                            commands.Add(insertCommand);
                        }
                        break;
                    case DataStates.Modified:
                        {
                            dynamic updateCommand = new ExpandoObject();
                            updateCommand.CommandType = CommandType.Update;
                            updateCommand.Data = jsonGroupState.field;
                            updateCommand.Identity = jsonGroupState.Identity;
                            commands.Add(updateCommand);
                        }
                        break;
                }
            }
            return commands;
        }

        private dynamic ExecuteTeachingGroupCommand(dynamic jsonCommand)
        {
            dynamic commandInfo = new ExpandoObject();
            //commandInfo.Data = jsonCommand.Data;
            commandInfo.Identity = jsonCommand.Identity;
            commandInfo.CommandType = jsonCommand.commandType;
            commandInfo.ExecuteDate = DateTime.Now;
            try
            {
                //Execute
                CommandType commandType = CommandType.Insert;
                Enum.TryParse(jsonCommand.commandtype, out commandType);
                switch (commandType)
                {
                    case CommandType.Insert:
                        commandInfo.Identity = InsertTeachingGroup(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Update:
                        UpdateTeachingGroup(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Delete:
                        commandInfo.ExecuteResult = CommandExecuteResult.NotSupported;
                        break;
                }

            }
            catch (Exception ex)
            {
                commandInfo.ExecuteResult = CommandExecuteResult.Failed;
                commandInfo.ErrorMessage = ex.Message;
            }
            return commandInfo;
        }

        private void UpdateTeachingGroup(dynamic jsonCommand)
        {
            var teachingGroupUpdateData = jsonCommand.data;
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)teachingGroupUpdateData.SchoolId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();


            var identity = jsonCommand.Identity;
            var currentGroup = _teachingGroupService.GetUserGroups<TeachingGroupDto>(userGroupIds: new List<int> { (int)identity.Id },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (teachingGroupUpdateData.TeachingGroupName != null)
                currentGroup.Name = teachingGroupUpdateData.TeachingGroupName;
            if (teachingGroupUpdateData.description != null)
                currentGroup.Description = teachingGroupUpdateData.description;
            if (teachingGroupUpdateData.SubjectCode != null)
                currentGroup.SubjectCode = teachingGroupUpdateData.SubjectCode;
            currentGroup.EntityStatus.LastExternallySynchronized = DateTime.Now;
            currentGroup.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            currentGroup.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource;
            currentGroup.EntityStatus.ExternallyMastered = true;
            currentGroup.EntityStatus.StatusId = EntityStatusEnum.Active;
            _teachingGroupService.UpdateUserGroup(validationSpecification, currentGroup);
        }

        private JsonDynamicObject InsertTeachingGroup(dynamic jsonCommand)
        {
            var teachingGroupData = jsonCommand.data;
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())

           .ValidateDepartment((int)teachingGroupData.SchoolId, ArchetypeEnum.School)
           .WithStatus(EntityStatusEnum.All)
           .IsDirectParent()
           .Create();
            var teachingGroupDto = new TeachingGroupDto
            {
                Identity = CreateIdentity(jsonCommand.Identity),
                SchoolId = (int?)teachingGroupData.SchoolId,
                EntityStatus = new EntityStatusDto
                {
                    LastExternallySynchronized = DateTime.Now,
                    StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource,
                    StatusId = EntityStatusEnum.Active,
                    LastUpdatedBy = _workContext.CurrentUserId,
                    ExternallyMastered = true
                },
                SubjectCode = teachingGroupData.subjectcode,
                Name = teachingGroupData.TeachingGroupName,
                Description = teachingGroupData.Description
            };

            var insertedGroup = _teachingGroupService.InsertUserGroup(validationSpecification, teachingGroupDto);

            return CreateJsonDynamicObjectIdentity(insertedGroup.Identity);
        }

        #endregion

        #region Class


        private List<ExpandoObject> GetClassesStates(int ownerId, int customerId, int schoolId, List<ClassDto> currentClasses, dynamic schoolImport)
        {
            List<ExpandoObject> classStates = new List<ExpandoObject>();
            Dictionary<string, List<MemberDto>> classesWithMembership = new Dictionary<string, List<MemberDto>>();
            if (currentClasses.Any())
                classesWithMembership = _classMemberService.GetClassesMemberships(currentClasses.Select(x => x.Identity.ExtId).ToList(), ownerId, customerId);
            foreach (dynamic importClass in schoolImport.Classes)
            {
                var importClassData = importClass;
                var currentClass = currentClasses.FirstOrDefault(x => x.Identity.ExtId == importClassData.ExternalId);
                List<string> importClassLevels = GetClassLevel(importClassData.externalid, schoolImport);
                if (currentClass == null)
                {
                    dynamic classState = new ExpandoObject();
                    classState.State = DataStates.New.ToString();
                    classState.Identity = new IdentityDto
                    {
                        Archetype = ArchetypeEnum.Class,
                        CustomerId = customerId,
                        ExtId = importClassData.externalId,
                        OwnerId = ownerId,
                        Id = null
                    };
                    classState.Field = new ExpandoObject();

                    classState.Field.ParentDepartmentId = schoolId;
                    classState.Field.ClassName = importClassData.Name;
                    classState.Field.Description = importClassData.Description;
                    if (importClassLevels.Any())
                    {
                        classState.Field.ImportClassLevels = string.Join(",", importClassLevels);
                    }

                    classStates.Add(classState);
                }
                else
                {
                    var isUpdate = false;
                    List<MemberDto> currentClassLevels = classesWithMembership[currentClass.Identity.ExtId].Where(x => x.Identity.Archetype == ArchetypeEnum.Level).ToList();
                    var listCurrentLevel = currentClassLevels.Select(x => x.Identity.Id.ToString());
                    dynamic classState = new ExpandoObject();
                    classState.CurrentLevels = string.Join(",", listCurrentLevel);
                    if (importClassLevels.Any())
                    {
                        foreach (var item in listCurrentLevel)
                        {
                            if (!importClassLevels.Contains(item))
                            {
                                isUpdate = true;
                            }
                        }
                    }
                    classState.State = DataStates.Modified.ToString();
                    classState.Field = new ExpandoObject();
                    if (importClassLevels.Any())
                    {
                        classState.Field.ImportClassLevels = string.Join(",", importClassLevels);
                    }
                    if (currentClass.EntityStatus.StatusId != EntityStatusEnum.Active)
                    {
                        isUpdate = true;
                    }
                    classState.Field.ParentDepartmentId = currentClass.ParentDepartmentId;
                    classState.Identity = currentClass.Identity;
                    var currentName = currentClass.Name ?? string.Empty;
                    var currentDescription = currentClass.Description ?? string.Empty;
                    if (importClassData.Name != null && importClassData.Name != currentName)
                    {
                        classState.Field.ClassName = importClassData.Name;
                        isUpdate = true;
                    }
                    if (importClassData.Description != null && importClassData.Description != currentDescription)
                    {
                        classState.Field.Description = importClassData.Description;
                        isUpdate = true;
                    }
                    if (isUpdate)
                    {
                        classStates.Add(classState);
                    }
                }
            }
            return classStates;
        }

        private List<string> GetClassLevel(string externalid, dynamic schoolImport)
        {
            List<string> levels = new List<string>();
            if (schoolImport.Classes == null || schoolImport.Levels == null)
            {
                return levels;
            }
            foreach (var item in schoolImport.Classes)
            {
                if (item.externalid == externalid && item.LearnerMembers != null)
                {
                    foreach (var learner in item.LearnerMembers)
                    {
                        foreach (var level in schoolImport.Levels)
                        {
                            if (level.LearnerMembers == null)
                                continue;
                            foreach (var learnerLevel in level.LearnerMembers)
                            {
                                var levelValue = level.level.ToString();
                                if (learner.externalid.Equals(learnerLevel.ExternalId) && !levels.Contains(levelValue))
                                {
                                    levels.Add(levelValue);
                                }
                            }
                        }
                    }
                }
            }
            return levels.ToList();
        }

        private List<ExpandoObject> AnalyzeClassCommands(int ownerid, int customerId, dynamic jsonSchoolDataStates)
        {
            List<ExpandoObject> commands = new List<ExpandoObject>();
            foreach (dynamic classState in jsonSchoolDataStates.classes)
            {
                var jsonClassState = classState;
                DataStates dataState = DataStates.NotModified;
                Enum.TryParse(jsonClassState.State, out dataState);
                switch ((dataState))
                {
                    case DataStates.New:
                        {
                            dynamic insertCommand = new ExpandoObject();
                            insertCommand.CommandType = CommandType.Insert;
                            insertCommand.Data = jsonClassState.Field;
                            insertCommand.ParentDepartmentId = jsonClassState.Field.ParentDepartmentId;
                            insertCommand.Identity = new IdentityDto
                            {
                                Archetype = ArchetypeEnum.Class,
                                CustomerId = customerId,
                                OwnerId = ownerid,
                                ExtId = jsonClassState.Identity.ExtId,
                                Id = 0
                            };
                            if (insertCommand.Data.ImportClassLevels != null)
                                insertCommand.Data.NewLevelIds = insertCommand.Data.ImportClassLevels;
                            insertCommand.Data.Remove("ImportClassLevels");
                            commands.Add(insertCommand);
                        }
                        break;
                    case DataStates.Modified:
                        {
                            dynamic updateCommand = new ExpandoObject();
                            updateCommand.CommandType = CommandType.Update;
                            updateCommand.Data = jsonClassState.field;
                            updateCommand.ParentDepartmentId = jsonClassState.Field.ParentDepartmentId;
                            var jsonDynamicClass = jsonClassState.Field;
                            var currentClassLevel = jsonClassState.CurrentLevels == null ? new List<string>()
                                : new List<string>(jsonClassState.CurrentLevels.Split(',')).Where(x => !string.IsNullOrEmpty(x)).ToList();
                            var importclasslevel = jsonDynamicClass.ImportClassLevels == null ? new List<string>() :
                                new List<string>(jsonDynamicClass.ImportClassLevels.Split(',')).Where(x => !string.IsNullOrEmpty(x)).ToList();

                            var newLevelIds = new List<string>();

                            foreach (var importId in importclasslevel)
                            {
                                if (!currentClassLevel.Contains(importId))
                                {
                                    newLevelIds.Add(importId);
                                }
                            }
                            if (newLevelIds.Any())
                                updateCommand.Data.NewLevelIds = string.Join(",", newLevelIds);
                            var removeLevelIds = new List<string>();
                            foreach (var currenId in currentClassLevel)
                            {
                                if (!importclasslevel.Contains(currenId))
                                {
                                    removeLevelIds.Add(currenId);
                                }
                            }
                            if (removeLevelIds.Any())
                                updateCommand.Data.RemoveLevelIds = string.Join(",", removeLevelIds);

                            updateCommand.Identity = jsonClassState.Identity;
                            updateCommand.Data.Remove("ImportClassLevels");
                            commands.Add(updateCommand);
                        }
                        break;
                }
            }

            return commands;
        }

        private dynamic ExecuteClassCommand(dynamic jsonCommand)
        {
            dynamic commandInfo = new ExpandoObject();
            //commandInfo.Data = jsonCommand.Data;
            commandInfo.CommandType = jsonCommand.commandType;
            commandInfo.Identity = jsonCommand.Identity;
            commandInfo.ExecuteDate = DateTime.Now;
            try
            {
                //Execute
                CommandType commandType = CommandType.Insert;
                Enum.TryParse(jsonCommand.commandtype, out commandType);
                switch (commandType)
                {
                    case CommandType.Insert:
                        commandInfo.Identity = InsertClass(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Update:
                        UpdateClass(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Delete:
                        commandInfo.ExecuteResult = CommandExecuteResult.NotSupported;
                        break;
                }
            }
            catch (Exception ex)
            {
                commandInfo.ExecuteResult = CommandExecuteResult.Failed;
                commandInfo.ErrorMessage = ex.Message;
            }
            return commandInfo;
        }

        private JsonDynamicObject InsertClass(dynamic jsonCommand)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
          .ValidateDepartment((int)jsonCommand.data.ParentDepartmentId, ArchetypeEnum.School)
          .WithStatus(EntityStatusEnum.All)
          .IsDirectParent()
          .Create();
            var classInsertData = jsonCommand.data;
            var classDto = new ClassDto
            {
                Identity = CreateIdentity(jsonCommand.Identity),
                ParentDepartmentId = (int)classInsertData.ParentDepartmentId,
                Description = classInsertData.description,
                Name = classInsertData.classname,
                EntityStatus = new EntityStatusDto
                {
                    LastExternallySynchronized = DateTime.Now,
                    StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource,
                    StatusId = EntityStatusEnum.Active,
                    LastUpdatedBy = _workContext.CurrentUserId,
                    ExternallyMastered = true
                }
            };

            var classInserted = _classService.InsertDepartment(validationSpecification, classDto);
            if (classInsertData.NewLevelIds != null)
            {
                foreach (var levelId in classInsertData.NewLevelIds.Split(','))
                {
                    var levelMember = new MemberDto
                    {
                        Identity = new IdentityDto { Id = classInserted.Identity.Id, Archetype = ArchetypeEnum.Class, OwnerId = _workContext.CurrentOwnerId, CustomerId = _workContext.CurrentCustomerId },
                        EntityStatus = new EntityStatusDto { StatusId = EntityStatusEnum.Active, LastExternallySynchronized = DateTime.Now }
                    };
                    _levelService.AddOrRemoveMember(validationSpecification, levelId, levelMember);
                }
            }

            return CreateJsonDynamicObjectIdentity(classInserted.Identity);
        }

        private void UpdateClass(dynamic jsonCommand)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)jsonCommand.data.ParentDepartmentId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();
            var classUpdateData = jsonCommand.data;
            var identity = jsonCommand.Identity;
            var currentClass = _classService.GetDepartments<ClassDto>(departmentIds: new List<int> { (int)identity.Id }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();
            if (classUpdateData.classname != null)
                currentClass.Name = classUpdateData.classname;
            if (classUpdateData.description != null)
                currentClass.Description = classUpdateData.Description;

            currentClass.EntityStatus.LastExternallySynchronized = DateTime.Now;
            currentClass.EntityStatus.ExternallyMastered = true;
            currentClass.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            currentClass.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource;
            currentClass.EntityStatus.StatusId = EntityStatusEnum.Active;
            _classService.UpdateDepartment(validationSpecification, currentClass);

            if (classUpdateData.NewLevelIds != null)
            {
                foreach (var levelId in classUpdateData.NewLevelIds.Split(','))
                {
                    var levelMember = new MemberDto
                    {
                        Identity = new IdentityDto { Id = currentClass.Identity.Id, Archetype = ArchetypeEnum.Class, OwnerId = _workContext.CurrentOwnerId, CustomerId = _workContext.CurrentCustomerId },
                        EntityStatus = new EntityStatusDto { StatusId = EntityStatusEnum.Active, LastExternallySynchronized = DateTime.Now }
                    };
                    _levelService.AddOrRemoveMember(validationSpecification, levelId, levelMember);
                }
            }
            if (classUpdateData.RemoveLevelIds != null)
            {
                foreach (var levelId in classUpdateData.RemoveLevelIds.Split(','))
                {
                    var levelMember = new MemberDto
                    {
                        Identity = new IdentityDto { Id = currentClass.Identity.Id, Archetype = ArchetypeEnum.Class, OwnerId = _workContext.CurrentOwnerId, CustomerId = _workContext.CurrentCustomerId },
                        EntityStatus = new EntityStatusDto { StatusId = EntityStatusEnum.Active, LastExternallySynchronized = DateTime.Now }
                    };
                    _levelService.AddOrRemoveMember(validationSpecification, levelId, levelMember);
                }
            }
        }

        #endregion

        #region Emplpoyee

        private List<ExpandoObject> GetEmployeesStates(int ownerId, int customerId, int schoolId, List<EmployeeDto> currentEmployees, dynamic schoolImport)
        {
            List<ExpandoObject> states = new List<ExpandoObject>();
            var currentEmployeeExtIds = currentEmployees.Select(x => x.Identity.ExtId).Distinct().ToList();
            var usersWithMemberships = new Dictionary<string, List<MemberDto>>();
            if (currentEmployees.Any())
            {
                usersWithMemberships = _learnerService.GetUsersMemberships(currentEmployeeExtIds, ArchetypeEnum.Employee);
            }
            var teachingGroupsExtIds = GetImportTeachingGroupExtIds(schoolImport);
            foreach (dynamic importEmp in schoolImport.Employees)
            {
                string employeeTeachingGroupExtIds = GetEmployeeGroupExtIds(importEmp.externalid, schoolImport);
                var importEmployeeData = importEmp;
                var currentEmployee = currentEmployees.FirstOrDefault(x => x.Identity.ExtId == importEmployeeData.ExternalId);
                var currentClasseExtIds = GetEmployeeClass(importEmp.externalid, schoolImport);
                if (currentEmployee == null)
                {
                    dynamic empolyeeState = new ExpandoObject();
                    empolyeeState.State = DataStates.New.ToString();
                    empolyeeState.Identity = new IdentityDto
                    {
                        Archetype = ArchetypeEnum.Employee,
                        CustomerId = customerId,
                        ExtId = importEmployeeData.externalId,
                        OwnerId = ownerId,
                        Id = null
                    };
                    empolyeeState.Field = new ExpandoObject();


                    empolyeeState.Field.EmployerDepartmentId = schoolId;
                    empolyeeState.Field.FirstName = importEmployeeData.FirstName;
                    empolyeeState.Field.LastName = importEmployeeData.LastName;
                    empolyeeState.Field.SSN = importEmployeeData.SSN;
                    empolyeeState.Field.MobileNumber = importEmployeeData.MobileNumber;
                    if (importEmployeeData.Gender != null)
                    {
                        empolyeeState.Field.Gender = importEmployeeData.Gender;
                    }
                    empolyeeState.Field.EmailAddress = importEmployeeData.EmailAddress;

                    var birthDate = GetJsonDate(importEmployeeData.BirthDate);
                    if (birthDate != null)
                    {
                        empolyeeState.Field.BirthDate = birthDate;
                    }
                    if (!string.IsNullOrEmpty(employeeTeachingGroupExtIds))
                        empolyeeState.Field.ImportTeachingGroupExtIds = employeeTeachingGroupExtIds;
                    empolyeeState.Field.classExtIds = currentClasseExtIds;
                    states.Add(empolyeeState);
                }
                else
                {
                    var isUpdate = false;
                    dynamic employeeState = new ExpandoObject();
                    employeeState.State = DataStates.Modified.ToString();
                    employeeState.Field = new ExpandoObject();
                    employeeState.Field.EmployerDepartmentId = currentEmployee.EmployerDepartmentId;
                    employeeState.Identity = currentEmployee.Identity;

                    var currentEmailAddress = currentEmployee.EmailAddress ?? string.Empty;
                    var currentFirstName = currentEmployee.FirstName ?? string.Empty;
                    var currentLastName = currentEmployee.LastName ?? string.Empty;
                    var currentSSN = currentEmployee.SSN ?? string.Empty;
                    var currentMobileNumber = currentEmployee.MobileNumber ?? string.Empty;

                    List<MemberDto> userMembersShips = usersWithMemberships[currentEmployee.Identity.ExtId];
                    var currentUserGroups = userMembersShips.Where(x => x.Identity.Archetype == ArchetypeEnum.TeachingGroup || x.Identity.Archetype == ArchetypeEnum.EducationProgram).ToList();
                    var lstEmployeeTeachingGroupExtIds = employeeTeachingGroupExtIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    var currentGroups = currentUserGroups.Select(x => x.Identity.ExtId).ToList();
                    if (lstEmployeeTeachingGroupExtIds.Any(x => !currentGroups.Contains(x)))
                        isUpdate = true;
                    HashSet<string> teachingGroupExtIdsHashSet = new HashSet<string>();
                    foreach (var extId in teachingGroupsExtIds)
                    {
                        teachingGroupExtIdsHashSet.Add(extId);
                    }
                    var currentGroupExtIds = currentGroups.Where(x => teachingGroupExtIdsHashSet.Contains(x)).ToList();
                    employeeState.CurrentTeachingGroupExtIds = string.Join(",", currentGroupExtIds);
                    employeeState.Field.ImportTeachingGroupExtIds = employeeTeachingGroupExtIds;
                    if (currentClasseExtIds.Split(',').Length > 0)
                    {
                        employeeState.Field.classExtIds = currentClasseExtIds;
                        isUpdate = true;
                    }
                    if (importEmployeeData.EmailAddress != null && importEmployeeData.EmailAddress != currentEmailAddress)
                    {
                        employeeState.Field.EmailAddress = importEmployeeData.EmailAddress;
                        isUpdate = true;
                    }
                    if (importEmployeeData.FirstName != null && importEmployeeData.FirstName != currentFirstName)
                    {
                        employeeState.Field.FirstName = importEmployeeData.FirstName;
                        isUpdate = true;
                    }
                    if (currentEmployee.EntityStatus.StatusId != EntityStatusEnum.Active)
                    {
                        isUpdate = true;
                    }
                    if (importEmployeeData.LastName != null && importEmployeeData.LastName != currentLastName)
                    {
                        employeeState.Field.LastName = importEmployeeData.LastName;
                        isUpdate = true;
                    }
                    if (importEmployeeData.Ssn != null && importEmployeeData.Ssn != currentSSN)
                    {
                        employeeState.Field.Ssn = importEmployeeData.Ssn;
                        isUpdate = true;
                    }
                    if (importEmployeeData.MobileNumber != null && importEmployeeData.MobileNumber != currentMobileNumber)
                    {
                        employeeState.Field.MobileNumber = importEmployeeData.MobileNumber;
                        isUpdate = true;
                    }
                    employeeState.Field.classExtIds = currentClasseExtIds;

                    var birthDate = GetJsonDate(importEmployeeData.BirthDate);
                    if (birthDate != null && birthDate != currentEmployee.DateOfBirth)
                    {
                        employeeState.Field.BirthDate = birthDate;
                        isUpdate = true;
                    }
                    if (importEmployeeData.Gender != null)
                    {
                        int newGender = EnumValueFromDynamicValue(importEmployeeData.Gender);
                        if (!(currentEmployee.Gender == newGender))
                        {
                            employeeState.Field.Gender = importEmployeeData.Gender;
                            isUpdate = true;
                        }
                    }
                    if (!string.IsNullOrEmpty(employeeTeachingGroupExtIds))
                        employeeState.Field.ImportTeachingGroupExtIds = employeeTeachingGroupExtIds;
                    if (isUpdate)
                    {
                        states.Add(employeeState);
                    }
                }
            }
            return states;
        }

        private int EnumValueFromDynamicValue(dynamic gender)
        {
            var result = Gender.Unknown;
            Enum.TryParse<Gender>(gender.ToString(), out result);
            return (int)result;
        }

        private IEnumerable<ExpandoObject> AnalyzeEmployeeCommands(int ownerid, int customerId, dynamic jsonSchoolDataStates)
        {
            List<ExpandoObject> commands = new List<ExpandoObject>();
            foreach (dynamic state in jsonSchoolDataStates.employees)
            {
                var jsonState = state;
                var jsonStateField = jsonState.field;
                DataStates dataState = DataStates.NotModified;
                Enum.TryParse(jsonState.State, out dataState);
                switch ((dataState))
                {
                    case DataStates.New:
                        {
                            dynamic insertCommand = new ExpandoObject();
                            insertCommand.CommandType = CommandType.Insert;
                            insertCommand.Data = jsonState.field;
                            insertCommand.Identity = new IdentityDto
                            {
                                Archetype = ArchetypeEnum.Employee,
                                CustomerId = customerId,
                                OwnerId = ownerid,
                                ExtId = jsonState.Identity.ExtId,
                                Id = 0
                            };
                            if (jsonStateField.ImportTeachingGroupExtIds != null)
                                insertCommand.Data.NewTeachingGroupExtIds = jsonStateField.ImportTeachingGroupExtIds;
                            insertCommand.Data.Remove("ImportTeachingGroupExtIds");
                            commands.Add(insertCommand);
                        }
                        break;
                    case DataStates.Modified:
                        {
                            dynamic updateCommand = new ExpandoObject();
                            updateCommand.CommandType = CommandType.Update;
                            updateCommand.Data = jsonState.field;
                            updateCommand.Identity = jsonState.Identity;

                            var importTeachinGroupsExtIds = jsonStateField.ImportTeachingGroupExtIds == null ? new List<string>() :
                          new List<string>(jsonStateField.ImportTeachingGroupExtIds.Split(','))
                          .Where(x => !string.IsNullOrEmpty(x)).ToList();
                            var currentTeachingGroupExtids = jsonState.CurrentTeachingGroupExtIds == null ? new List<string>() :
                                new List<string>(jsonState.CurrentTeachingGroupExtIds.Split(','))
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();
                            var newTeachingGroupExtids = new List<string>();

                            foreach (var importId in importTeachinGroupsExtIds)
                            {
                                if (!currentTeachingGroupExtids.Contains(importId))
                                {
                                    newTeachingGroupExtids.Add(importId);
                                }
                            }
                            if (newTeachingGroupExtids.Any())
                                updateCommand.Data.NewTeachingGroupExtIds = string.Join(",", newTeachingGroupExtids);
                            var removeTeachingGroupExtIds = new List<string>();
                            foreach (var currenId in currentTeachingGroupExtids)
                            {
                                if (!importTeachinGroupsExtIds.Contains(currenId))
                                {
                                    removeTeachingGroupExtIds.Add(currenId);
                                }
                            }
                            //updateCommand.Data.classExtIds = jsonStateField.classExtIds;
                            updateCommand.Data.CurrentClassId = jsonState.CurrentClassId;
                            if (removeTeachingGroupExtIds.Any())
                                updateCommand.Data.RemoveTeachingGroupExtIds = string.Join(",", removeTeachingGroupExtIds);
                            updateCommand.Data.Remove("ImportTeachingGroupExtIds");

                            commands.Add(updateCommand);
                        }
                        break;
                }
            }

            return commands;
        }
        private dynamic ExecuteEmployeeCommand(dynamic jsonCommand)
        {
            dynamic commandInfo = new ExpandoObject();
            //commandInfo.Data = jsonCommand.Data;
            commandInfo.CommandType = jsonCommand.commandType;
            commandInfo.Identity = jsonCommand.Identity;
            commandInfo.ExecuteDate = DateTime.Now;
            try
            {
                //Execute
                CommandType commandType = CommandType.Insert;
                Enum.TryParse(jsonCommand.commandtype, out commandType);
                switch (commandType)
                {
                    case CommandType.Insert:
                        commandInfo.Identity = InsertEmployee(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Update:
                        UpdateEmployee(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Delete:
                        commandInfo.ExecuteResult = CommandExecuteResult.NotSupported;
                        break;
                }
            }
            catch (Exception ex)
            {
                commandInfo.ExecuteResult = CommandExecuteResult.Failed;
                commandInfo.ErrorMessage = ex.Message;
            }
            return commandInfo;
        }

        private void UpdateEmployee(dynamic jsonCommand)
        {
            var updateData = jsonCommand.data;

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
              .ValidateDepartment((int)updateData.EmployerDepartmentId, ArchetypeEnum.School)
              .WithStatus(EntityStatusEnum.All)
              .IsDirectParent()
              .Create();

            var identity = jsonCommand.Identity;

            var currentUser = _employeeService.GetUsers<EmployeeDto>(userIds: new List<int> { (int)identity.Id }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            currentUser.EntityStatus.LastExternallySynchronized = DateTime.Now;
            currentUser.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            currentUser.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource;
            currentUser.EntityStatus.ExternallyMastered = true;
            currentUser.EntityStatus.StatusId = EntityStatusEnum.Active;

            if (updateData.FirstName != null)
                currentUser.FirstName = updateData.FirstName;
            if (updateData.LastName != null)
                currentUser.LastName = updateData.LastName;
            if (updateData.SSN != null)
                currentUser.SSN = updateData.SSN;
            if (updateData.MobileNumber != null)
                currentUser.MobileNumber = updateData.MobileNumber;
            if (updateData.emailaddress != null)
                currentUser.EmailAddress = updateData.emailaddress;
            //Check bithdate has value or an empty string
            var dob = GetJsonDate(updateData.BirthDate);
            if (dob != null)
                currentUser.DateOfBirth = dob;
            //Check Gender has value or an empty string
            if (updateData.Gender != null)
                currentUser.Gender = Convert.ToInt16(EnumValueFromDynamicValue(updateData.Gender));
            _employeeService.UpdateUser(validationSpecification, currentUser);
            _departmentService.UpdateObjectMappingsEmployee(new List<string>(updateData.classExtIds.ToString().Split(',')), currentUser.Identity.Id.Value, currentUser.Identity.CustomerId);


            var newTeachingGroupExtIds = updateData.NewTeachingGroupExtIds == null ? new List<string>() : new List<string>(updateData.NewTeachingGroupExtIds.ToString().Split(','));
            var removeTeachingGroupExtIds = updateData.RemoveTeachingGroupExtIds == null ? new List<string>() : new List<string>(updateData.RemoveTeachingGroupExtIds.ToString().Split(','));
            ProcessEmployeeTeachingGroups(currentUser, newTeachingGroupExtIds, removeTeachingGroupExtIds);
        }

        private IdentityDto CreateIdentity(dynamic identity)
        {
            return new IdentityDto
            {
                Archetype = Enum.Parse(typeof(ArchetypeEnum), identity.Archetype, true),
                CustomerId = (int)identity.CustomerId,
                ExtId = identity.ExtId,
                Id = identity.Id as long?,
                OwnerId = (int)identity.OwnerId
            };
        }

        private JsonDynamicObject CreateJsonDynamicObjectIdentity(IdentityDto identity)
        {
            dynamic jsonDynamicObject = new JsonDynamicObject();
            jsonDynamicObject.Archetype = identity.Archetype.ToString();
            jsonDynamicObject.CustomerId = (int)identity.CustomerId;
            jsonDynamicObject.ExtId = identity.ExtId;
            jsonDynamicObject.Id = identity.Id;
            jsonDynamicObject.OwnerId = identity.OwnerId;
            return jsonDynamicObject;

        }

        private JsonDynamicObject InsertEmployee(dynamic jsonCommand)
        {
            var insertData = jsonCommand.data;
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
            .ValidateDepartment((int)insertData.EmployerDepartmentId, ArchetypeEnum.School)
            .WithStatus(EntityStatusEnum.All)
            .IsDirectParent()
            .Create();

            var insertDto = new EmployeeDto
            {
                Identity = CreateIdentity(jsonCommand.Identity),
                EmployerDepartmentId = (int)insertData.EmployerDepartmentId,
                FirstName = insertData.FirstName,
                LastName = insertData.LastName,
                SSN = insertData.SSN,
                MobileNumber = insertData.MobileNumber,
                Gender = insertData.Gender != null ? (short?)EnumValueFromDynamicValue(insertData.Gender) : null,
                EmailAddress = insertData.EmailAddress,
                EntityStatus = new EntityStatusDto
                {
                    LastExternallySynchronized = DateTime.Now,
                    StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource,
                    StatusId = EntityStatusEnum.Active,
                    LastUpdatedBy = _workContext.CurrentUserId,
                    ExternallyMastered = true
                }
            };
            var dob = GetJsonDate(insertData.BirthDate);
            if (dob != null)
                insertDto.DateOfBirth = dob;
            insertDto.Identity.Id = 0;
            var insertedEmp = _employeeService.InsertUser(validationSpecification, insertDto) as EmployeeDto;
            _departmentService.UpdateObjectMappingsEmployee(new List<string>(insertData.classExtIds.ToString().Split(',')), insertedEmp.Identity.Id.Value, insertedEmp.Identity.CustomerId);
            var newTeachingGroupExtids = insertData.NewTeachingGroupExtIds == null ? new List<string>() : new List<string>(insertData.NewTeachingGroupExtIds.Split(','));
            ProcessEmployeeTeachingGroups(insertedEmp, newTeachingGroupExtids, null);
            return CreateJsonDynamicObjectIdentity(insertedEmp.Identity);
        }
        #endregion

        #region Learner

        private List<ExpandoObject> GetLearnersStates(int ownerId, int customerId, int schoolId, List<LearnerDto> currentLearners, dynamic schoolImport)
        {
            List<ExpandoObject> states = new List<ExpandoObject>();

            var currentLearnerExtIds = currentLearners.Select(x => x.Identity.ExtId).Distinct().ToList();

            var teachingGroupsExtIds = GetImportTeachingGroupExtIds(schoolImport);
            var usersWithCurrentDepartmentInfo = new Dictionary<string, List<HierachyDepartmentIdentityDto>>();
            var usersWithMemberships = new Dictionary<string, List<MemberDto>>();
            if (currentLearners.Any())
            {
                usersWithCurrentDepartmentInfo = _learnerService.GetUserHierachyDepartmentIdentitiesByExtIds(currentLearnerExtIds);
                usersWithMemberships = _learnerService.GetUsersMemberships(currentLearnerExtIds, ArchetypeEnum.Learner);
            }
            foreach (dynamic importLearner in schoolImport.learners)
            {
                var importData = importLearner;
                var tag = GetProgramAreaTagForLearner(importData.externalid, schoolImport);
                string classExtId = GetLearnerClass(importData.externalid, schoolImport);
                int? level = GetLearnerLevel(importData.externalid, schoolImport);
                string learnerTeachingGroupExtIds = GetLearnerGroupExtIds(importData.externalid, schoolImport);


                var currentLearner = currentLearners.FirstOrDefault(x => x.Identity.ExtId == importData.ExternalId);
                if (currentLearner == null)
                {
                    dynamic state = new ExpandoObject();
                    state.State = DataStates.New.ToString();
                    state.CurrentSchoolId = schoolId;
                    state.Identity = new IdentityDto
                    {
                        Archetype = ArchetypeEnum.Learner,
                        CustomerId = customerId,
                        ExtId = importData.externalId,
                        OwnerId = ownerId,
                        Id = null
                    };
                    state.Field = new ExpandoObject();

                    state.Field.ImportSchoolId = schoolId;
                    state.Field.FirstName = importData.FirstName;
                    state.Field.LastName = importData.LastName;
                    state.Field.SSN = importData.SSN;
                    state.Field.MobileNumber = importData.MobileNumber;
                    state.Field.EmailAddress = importData.EmailAddress;
                    if (!string.IsNullOrEmpty(classExtId))
                        state.Field.ImportClassExtId = classExtId;
                    if (importData.Gender != null)
                        state.Field.Gender = importData.Gender;
                    if (!string.IsNullOrEmpty(tag))
                        state.Field.Tag = tag;
                    if (level.HasValue)
                        state.Field.ImportLevel = level;
                    var birthDate = GetJsonDate(importData.BirthDate);
                    if (birthDate != null)
                    {
                        state.Field.BirthDate = importData.BirthDate;
                    }
                    if (!string.IsNullOrEmpty(learnerTeachingGroupExtIds))
                        state.Field.ImportTeachingGroupExtIds = learnerTeachingGroupExtIds;
                    states.Add(state);
                }
                else
                {
                    var isUpdate = false;
                    dynamic state = new ExpandoObject();
                    //Init update field
                    state.Field = new ExpandoObject();
                    state.State = DataStates.Modified.ToString();
                    List<HierachyDepartmentIdentityDto> userDepartmentInfo = usersWithCurrentDepartmentInfo[currentLearner.Identity.ExtId];
                    var currentUserClass = userDepartmentInfo.FirstOrDefault(x => x.Identity.Archetype == ArchetypeEnum.Class);
                    List<MemberDto> userMembersShips = usersWithMemberships[currentLearner.Identity.ExtId];
                    int? currentlevelId = null;
                    var currentLevel = userMembersShips.FirstOrDefault(x => x.Identity.Archetype == ArchetypeEnum.Level);
                    //get all group in db to compare with the group in import file
                    var currentUserGroups = userMembersShips.Where(x => x.Identity.Archetype == ArchetypeEnum.TeachingGroup || x.Identity.Archetype == ArchetypeEnum.EducationProgram).ToList();
                    var lstLearnerTeachingGroupExtIds = learnerTeachingGroupExtIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    //Level state

                    if (currentLevel != null)
                    {
                        currentlevelId = (int?)currentLevel.Identity.Id;
                    }
                    state.CurrentLevelId = currentlevelId;
                    if (level != null && level != currentlevelId)
                    {
                        isUpdate = true;
                        state.Field.ImportLevel = level;
                    }
                    if (currentLearner.EntityStatus.StatusId != EntityStatusEnum.Active)
                    {
                        isUpdate = true;
                    }
                    //teaching groups states, just get the ug_u for the groups which exists on the file if learner is not move to new school
                    var currentGroups = currentUserGroups.Select(x => x.Identity.ExtId).ToList();
                    if (lstLearnerTeachingGroupExtIds.Any(x => !currentGroups.Contains(x)))
                        isUpdate = true;

                    //department states
                    if ((currentUserClass != null && currentUserClass.Identity.ExtId != classExtId && !string.IsNullOrEmpty(classExtId))
                        || (!string.IsNullOrEmpty(classExtId) && currentUserClass == null))
                    {
                        isUpdate = true;
                        state.Field.ImportClassExtId = classExtId;
                    }
                    //New school: get all current group to delete later
                    if (currentLearner.ParentDepartmentId != schoolId)
                    {
                        isUpdate = true;
                        state.CurrentTeachingGroupExtIds = string.Join(",", currentGroups);
                    }
                    else
                    {
                        //No new school:just get the ug_u for the groups which exists on the file to avoid delete not sent ones
                        HashSet<string> teachingGroupExtIdsHashSet = new HashSet<string>();
                        foreach (var extId in teachingGroupsExtIds)
                        {
                            teachingGroupExtIdsHashSet.Add(extId);
                        }
                        var currentGroupExtIds = currentGroups.Where(x => teachingGroupExtIdsHashSet.Contains(x)).ToList();
                        state.CurrentTeachingGroupExtIds = string.Join(",", currentGroupExtIds);
                    }
                    state.CurrentClassExtId = currentUserClass == null ? null : currentUserClass.Identity.ExtId;
                    //This parent departmentid is always is school for a leaner
                    state.CurrentSchoolId = currentLearner.ParentDepartmentId;
                    state.Field.ImportSchoolId = schoolId;
                    //Need assign import teaching group extids even when it empty, to analyze remove teaching group membership
                    state.Field.ImportTeachingGroupExtIds = learnerTeachingGroupExtIds;
                    state.CurrentClassId = currentUserClass != null ? currentUserClass.Identity.Id : null;
                    state.Identity = currentLearner.Identity;

                    var currentEmailAddress = currentLearner.EmailAddress ?? string.Empty;
                    var currentFirstName = currentLearner.FirstName ?? string.Empty;
                    var currentLastName = currentLearner.LastName ?? string.Empty;
                    var currentSSN = currentLearner.SSN ?? string.Empty;
                    var currentTag = currentLearner.Tag ?? string.Empty;
                    var currentMobileNumber = currentLearner.MobileNumber ?? string.Empty;

                    if (importData.EmailAddress != null && importData.EmailAddress != currentEmailAddress)
                    {
                        state.Field.EmailAddress = importData.EmailAddress;
                        isUpdate = true;
                    }
                    if (importData.FirstName != null && importData.FirstName != currentFirstName)
                    {
                        state.Field.FirstName = importData.FirstName;
                        isUpdate = true;
                    }
                    if (importData.LastName != null && importData.LastName != currentLastName)
                    {
                        state.Field.LastName = importData.LastName;
                        isUpdate = true;
                    }
                    if (importData.Ssn != null && importData.Ssn != currentSSN)
                    {
                        state.Field.Ssn = importData.Ssn;
                        isUpdate = true;
                    }
                    if (importData.MobileNumber != null && importData.MobileNumber != currentMobileNumber)
                    {
                        state.Field.MobileNumber = importData.MobileNumber;
                        isUpdate = true;
                    }
                    if (tag != null && tag != currentTag)
                    {
                        state.Field.Tag = tag;
                        isUpdate = true;
                    }
                    var birthDate = GetJsonDate(importData.BirthDate);
                    if (birthDate != null && birthDate != currentLearner.DateOfBirth)
                    {
                        state.Field.BirthDate = birthDate;
                        isUpdate = true;
                    }

                    if (importData.Gender != null)
                    {
                        int newGender = EnumValueFromDynamicValue(importData.Gender);
                        if (!(currentLearner.Gender == newGender))
                        {
                            state.Field.Gender = importData.Gender;
                            isUpdate = true;
                        }
                    }
                    if (isUpdate)
                    {
                        states.Add(state);
                    }
                }
            }
            return states;
        }


        private string GetLearnerTeachingGroups(dynamic learnerExternalId, dynamic schoolImport)
        {
            if (schoolImport.TeachingGroups == null)
                return string.Empty;
            var groups = (schoolImport.TeachingGroups as List<dynamic>)
                .Where(x => (x.learnermembers as List<dynamic>).Any(y => y.ExternalId == learnerExternalId)).Select(x => x.externalid);
            return string.Join(",", groups);
        }
        private string GetEmployeeTeachingGroups(dynamic employeeExternalId, dynamic schoolImport)
        {
            if (schoolImport.TeachingGroups == null)
                return string.Empty;
            var groups = (schoolImport.TeachingGroups as List<dynamic>)
                .Where(x => (x.employeemembers as List<dynamic>).Any(y => y.ExternalId == employeeExternalId)).Select(x => x.externalid);
            return string.Join(",", groups);
        }

        private string GetEmployeeGroupExtIds(dynamic employeeExternalId, dynamic schoolImport)
        {
            if (schoolImport.EducationPrograms == null && schoolImport.TeachingGroups == null)
                return string.Empty;
            var teachingGroups = (schoolImport.TeachingGroups as List<dynamic>)
                .Where(x => (x.employeemembers as List<dynamic>).Any(y => y.ExternalId == employeeExternalId)).Select(x => x.externalid).ToList();
            var programGroups = (schoolImport.EducationPrograms as List<dynamic>)
                .Where(x => (x.employeemembers as List<dynamic>).Any(y => y.ExternalId == employeeExternalId)).Select(x => x.externalid).ToList();
            return string.Join(",", teachingGroups) + "," + string.Join(",", programGroups);
        }
        private string GetLearnerGroupExtIds(dynamic employeeExternalId, dynamic schoolImport)
        {
            if (schoolImport.EducationPrograms == null && schoolImport.TeachingGroups == null)
                return string.Empty;
            var teachingGroups = (schoolImport.TeachingGroups as List<dynamic>)
                .Where(x => (x.learnermembers as List<dynamic>).Any(y => y.ExternalId == employeeExternalId)).Select(x => x.externalid).ToList();
            var programGroups = (schoolImport.EducationPrograms as List<dynamic>)
                .Where(x => (x.learnermembers as List<dynamic>).Any(y => y.ExternalId == employeeExternalId)).Select(x => x.externalid).ToList();
            return string.Join(",", teachingGroups) + "," + string.Join(",", programGroups);
        }
        private int? GetLearnerLevel(string learnerExternalId, dynamic schoolImport)
        {
            if (schoolImport.Levels == null)
                return null;
            var data = (schoolImport.Levels as List<dynamic>)
            .LastOrDefault(x => (x.learnermembers as List<dynamic>).Any(y => y.ExternalId == learnerExternalId));

            return data != null ? int.Parse(data.level.ToString()) : null;
        }

        private string GetLearnerClass(string learnerExternalId, dynamic schoolImport)
        {
            if (schoolImport.classes == null)
                return null;
            var data = (schoolImport.classes as List<dynamic>)
           .LastOrDefault(x => (x.learnermembers as List<dynamic>).Any(y => y.ExternalId == learnerExternalId));

            return data != null ? data.externalid : string.Empty;
        }

        private string GetEmployeeClass(string learnerExternalId, dynamic schoolImport)
        {
            if (schoolImport.classes == null)
                return null;
            var data = (schoolImport.classes as List<dynamic>)
                .Where(x => (x.employeeMembers as List<dynamic>).Any(y => y.ExternalId == learnerExternalId)).Select(x => x.externalid);

            return string.Join(",", data);
        }
        private string GetProgramAreaTagForLearner(string learnerExtId, dynamic schoolImport)
        {
            if (schoolImport.ProgramAreas == null)
                return null;
            var data = (schoolImport.ProgramAreas as List<dynamic>)
            .LastOrDefault(x => (x.learnermembers as List<dynamic>).Any(y => y.ExternalId == learnerExtId));
            return data != null ? data.ProgramAreaCode : string.Empty;
        }

        private IEnumerable<ExpandoObject> AnalyzeLearnerCommands(int ownerid, int customerId, dynamic jsonSchoolDataStates)
        {
            List<ExpandoObject> commands = new List<ExpandoObject>();

            var learnersExtIds = GetImportLearnersExtIdsFromStates(jsonSchoolDataStates);

            foreach (dynamic state in jsonSchoolDataStates.Learners)
            {
                var jsonState = state;
                var jsonStateField = jsonState.field;
                DataStates dataState = DataStates.NotModified;
                Enum.TryParse(jsonState.State, out dataState);
                switch ((dataState))
                {
                    case DataStates.New:
                        {
                            dynamic insertCommand = new ExpandoObject();
                            insertCommand.CommandType = CommandType.Insert;
                            insertCommand.Data = jsonState.field;
                            insertCommand.Identity = new IdentityDto
                            {
                                Archetype = ArchetypeEnum.Learner,
                                CustomerId = customerId,
                                OwnerId = ownerid,
                                ExtId = jsonState.Identity.ExtId,
                                Id = 0
                            };
                            string importClassExtId = jsonStateField.ImportClassExtId;
                            if (!string.IsNullOrEmpty(importClassExtId))
                            {
                                insertCommand.Data.NewClassExtId = importClassExtId;
                            }
                            insertCommand.Data.CurrentSchoolId = jsonState.CurrentSchoolId;
                            if (jsonStateField.ImportLevel != null)
                                insertCommand.Data.NewLevelId = jsonStateField.ImportLevel;
                            if (jsonStateField.ImportTeachingGroupExtIds != null)
                                insertCommand.Data.NewTeachingGroupExtIds = jsonStateField.ImportTeachingGroupExtIds;
                            insertCommand.Data.Remove("ImportClassExtId");
                            insertCommand.Data.Remove("ImportSchoolId");
                            insertCommand.Data.Remove("ImportLevel");
                            insertCommand.Data.Remove("ImportTeachingGroupExtIds");
                            commands.Add(insertCommand);
                        }
                        break;
                    case DataStates.Modified:
                        {
                            dynamic updateCommand = new ExpandoObject();
                            updateCommand.CommandType = CommandType.Update;
                            updateCommand.Data = jsonState.field;
                            updateCommand.Data.CurrentSchoolId = jsonState.CurrentSchoolId;
                            updateCommand.Identity = jsonState.Identity;

                            //Analyze move department
                            var importClassExtId = jsonStateField.ImportClassExtId;

                            if (importClassExtId != null && (string.IsNullOrEmpty(jsonState.CurrentClassExtId) || jsonState.CurrentClassExtId != importClassExtId))
                            {
                                updateCommand.Data.NewClassExtId = importClassExtId;
                            }
                            var moveToNewSchool = (jsonStateField.ImportSchoolId != jsonState.CurrentSchoolId);
                            if (moveToNewSchool)
                            {
                                updateCommand.Data.NewSchoolId = jsonStateField.ImportSchoolId;
                            }
                            //Analyze level
                            var importLevel = jsonState.field.ImportLevel;
                            var currentLevel = jsonState.CurrentLevelId;
                            if (importLevel != null && importLevel != currentLevel)
                                updateCommand.Data.NewLevelId = jsonStateField.ImportLevel;
                            //Analyze teaching groups
                            var importTeachinGroupsExtIds = jsonStateField.ImportTeachingGroupExtIds == null ? new List<string>() :
                                new List<string>(jsonStateField.ImportTeachingGroupExtIds.Split(','))
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();
                            var currentTeachingGroupExtids = jsonState.CurrentTeachingGroupExtIds == null ? new List<string>() :
                                new List<string>(jsonState.CurrentTeachingGroupExtIds.Split(','))
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();
                            var newTeachingGroupExtids = new List<string>();

                            foreach (var importId in importTeachinGroupsExtIds)
                            {
                                if (!currentTeachingGroupExtids.Contains(importId))
                                {
                                    newTeachingGroupExtids.Add(importId);
                                }
                            }
                            if (newTeachingGroupExtids.Any())
                                updateCommand.Data.NewTeachingGroupExtIds = string.Join(",", newTeachingGroupExtids);
                            var removeTeachingGroupExtIds = new List<string>();
                            foreach (var currenId in currentTeachingGroupExtids)
                            {
                                if (!importTeachinGroupsExtIds.Contains(currenId))
                                {
                                    removeTeachingGroupExtIds.Add(currenId);
                                }
                            }
                            updateCommand.Data.CurrentClassId = jsonState.CurrentClassId;
                            if (removeTeachingGroupExtIds.Any())
                                updateCommand.Data.RemoveTeachingGroupExtIds = string.Join(",", removeTeachingGroupExtIds);
                            updateCommand.Data.Remove("ImportClassExtId");
                            updateCommand.Data.Remove("ImportClassExtId");
                            updateCommand.Data.Remove("ImportSchoolId");
                            updateCommand.Data.Remove("ImportLevel");
                            updateCommand.Data.Remove("ImportTeachingGroupExtIds");
                            commands.Add(updateCommand);
                        }
                        break;
                }
            }

            return commands;
        }



        private List<string> GetImportLearnersExtIdsFromStates(dynamic jsonSchoolDataStates)
        {
            var result = new List<string>();
            foreach (dynamic item in jsonSchoolDataStates.learners)
            {
                var jsonState = item;
                var identity = jsonState.identity;
                result.Add(identity.ExtId);
            }

            return result;
        }

        private dynamic ExecuteLearnerCommand(dynamic jsonCommand, List<ExpandoObject> moveAssessmentInfo)
        {
            dynamic commandInfo = new ExpandoObject();
            commandInfo.CommandType = jsonCommand.commandType;
            commandInfo.Identity = jsonCommand.Identity;
            commandInfo.ExecuteDate = DateTime.Now;
            try
            {
                //commandInfo.Data = jsonCommand.Data;              
                //Execute
                CommandType commandType = CommandType.Insert;
                Enum.TryParse(jsonCommand.commandtype, out commandType);
                switch (commandType)
                {
                    case CommandType.Insert:
                        commandInfo.Identity = InsertLearner(jsonCommand);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Update:
                        UpdateLearner(jsonCommand, moveAssessmentInfo);
                        commandInfo.ExecuteResult = CommandExecuteResult.Success;
                        break;
                    case CommandType.Delete:
                        commandInfo.ExecuteResult = CommandExecuteResult.NotSupported;
                        break;
                }
            }
            catch (Exception ex)
            {
                commandInfo.ExecuteResult = CommandExecuteResult.Failed;
                commandInfo.ErrorMessage = ex.Message;
            }
            return commandInfo;
        }

        private void UpdateLearner(dynamic jsonCommand, List<ExpandoObject> moveAssessmentInfo)
        {
            var updateData = jsonCommand.data;

            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
              .ValidateDepartment((int)updateData.currentSchoolId, ArchetypeEnum.School)
              .WithStatus(EntityStatusEnum.All)
              .IsDirectParent()
              .Create();

            var identity = jsonCommand.Identity;

            var currentUser = _learnerService.GetUsers<LearnerDto>(userIds: new List<int> { (int)identity.Id }, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }).Items.FirstOrDefault();

            currentUser.EntityStatus.LastExternallySynchronized = DateTime.Now;
            currentUser.EntityStatus.LastUpdatedBy = _workContext.CurrentUserId;
            currentUser.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource;
            currentUser.EntityStatus.StatusId = EntityStatusEnum.Active;
            currentUser.EntityStatus.ExternallyMastered = true;
            if (updateData.FirstName != null)
                currentUser.FirstName = updateData.FirstName;
            if (updateData.LastName != null)
                currentUser.LastName = updateData.LastName;
            if (updateData.SSN != null)
                currentUser.SSN = updateData.SSN;
            if (updateData.MobileNumber != null)
                currentUser.MobileNumber = updateData.MobileNumber;
            if (updateData.Tag != null)
                currentUser.Tag = updateData.Tag;
            if (updateData.emailaddress != null)
                currentUser.EmailAddress = updateData.emailaddress;
            //Check bithdate has value or an empty string
            var dob = GetJsonDate(updateData.birthdate);
            if (dob != null)
                currentUser.DateOfBirth = dob;
            //Check Gender has value or an empty string
            if (updateData.Gender != null)
                currentUser.Gender = Convert.ToInt16(EnumValueFromDynamicValue(updateData.Gender));
            currentUser = _learnerService.UpdateUser(validationSpecification, currentUser) as LearnerDto;
            ProcessMoveLearner(updateData, currentUser, moveAssessmentInfo);
            var importlevel = updateData.NewLevelId;
            if (importlevel != null)
            {
                ProcessLearnerLevel(currentUser, (int)importlevel);
            }
            var newTeachingGroupExtIds = updateData.NewTeachingGroupExtIds == null ? new List<string>() : new List<string>(updateData.NewTeachingGroupExtIds.ToString().Split(','));
            var removeTeachingGroupExtIds = updateData.RemoveTeachingGroupExtIds == null ? new List<string>() : new List<string>(updateData.RemoveTeachingGroupExtIds.ToString().Split(','));
            ProcessLearnerTeachingGroups(currentUser, newTeachingGroupExtIds, removeTeachingGroupExtIds);
        }

        private void ProcessMoveAssessment(List<ExpandoObject> assessmentInfo, int ownerId, int customerId)
        {
            var states = _moveAssesmentClientService.
                GetAssessmentStates(JsonConvert.SerializeObject(assessmentInfo, converters: new StringEnumConverter()), ownerId, customerId);
            var commands = _moveAssesmentClientService.
                GetAssessmentCommands(states, ownerId, customerId);
            var result = _moveAssesmentClientService.
                ExecuteAssessmentCommand(commands, ownerId, customerId);
        }

        private void ProcessMoveLearner(dynamic commandData, LearnerDto currentUser, List<ExpandoObject> moveAssessmentInfo)
        {
            bool moveAssessment = false;
            var newClassExtId = commandData.NewClassExtId;

            var newSchoolId = commandData.NewSchoolId;
            //If new school id is is available, Move learner to current school then move him to the new school, and then move to the 
            //class if classextid is specified
            if (newSchoolId != null)
            {
                moveAssessment = true;
                //currentUser.ParentDepartmentId = newSchoolId;
                if (commandData.currentclassid != null)
                    RemoveLearnerFromClass(currentUser, (int)commandData.currentclassid);
                var currentUserInDb = _learnerService.GetUsers<LearnerDto>(_workContext.CurrentOwnerId, customerIds: new List<int>
                {
                    _workContext.CurrentCustomerId
                }, userIds: new List<int> { (int)currentUser.Identity.Id }).Items.FirstOrDefault();
                MoveLearnerToSchool(currentUserInDb, (int)newSchoolId);
                currentUser = currentUserInDb;
            }
            if (!string.IsNullOrEmpty(newClassExtId))
            {
                moveAssessment = true;
                var classDb = _classService.GetDepartments<ClassDto>(_workContext.CurrentOwnerId, customerIds: new List<int> { _workContext.CurrentCustomerId },
                    extIds: new List<string> { newClassExtId }).Items.FirstOrDefault();
                if (classDb == null)
                    throw new Exception("Class not found, extid: " + newClassExtId);
                commandData.NewClassId = classDb.Identity.Id;
                MoveLearnerToClass(currentUser, (int)classDb.Identity.Id);
            }

            if (moveAssessment)
            {
                dynamic assessmentInfo = new ExpandoObject();
                assessmentInfo.Identity = currentUser.Identity;
                if (commandData.NewSchoolId != null)
                    assessmentInfo.NewSchoolId = commandData.NewSchoolId;
                if (commandData.NewClassId != null)
                    assessmentInfo.NewClassId = commandData.NewClassId;
                moveAssessmentInfo.Add(assessmentInfo);
            }
        }

        private void RemoveLearnerFromClass(LearnerDto currentUser, int currentClassId)
        {
            var classMember = new MemberDto
            {
                Identity = new IdentityDto { Id = currentClassId, Archetype = ArchetypeEnum.Class, OwnerId = _workContext.CurrentOwnerId, CustomerId = _workContext.CurrentCustomerId },
                EntityStatus = new EntityStatusDto { StatusId = EntityStatusEnum.Inactive, LastExternallySynchronized = DateTime.Now }
            };
            _classMemberService.RemoveLearnerFromClass((int)currentUser.Identity.Id, classMember);
        }

        private void ProcessLearnerLevel(LearnerDto currentUser, int levelId)
        {
            var levelMember = new MemberDto
            {
                Identity = new IdentityDto { Id = levelId, Archetype = ArchetypeEnum.Level, OwnerId = _workContext.CurrentOwnerId, CustomerId = _workContext.CurrentCustomerId },
                EntityStatus = new EntityStatusDto { StatusId = EntityStatusEnum.Active, LastExternallySynchronized = DateTime.Now }
            };
            _userTypeService.UpdateOrInsertUserTypeUser((int)currentUser.Identity.Id, levelMember, isUnique: true);
        }

        private void MoveLearnerToClass(LearnerDto currentUser, int newClassId)
        {
            var classMember = new MemberDto
            {
                Identity = new IdentityDto { Id = newClassId, Archetype = ArchetypeEnum.Class, OwnerId = _workContext.CurrentOwnerId, CustomerId = _workContext.CurrentCustomerId },
                EntityStatus = new EntityStatusDto { StatusId = EntityStatusEnum.Active, LastExternallySynchronized = DateTime.Now }
            };

            _classMemberService.AddLearnerToClass((int)currentUser.Identity.Id, classMember);
        }

        private void MoveLearnerToSchool(LearnerDto currentUser, int newSchoolId)
        {
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
               .ValidateDepartment(newSchoolId, ArchetypeEnum.Unknown)
               .SkipCheckingArchetype()
               .WithStatus(EntityStatusEnum.All)
               .IsDirectParent()
               .Create();
            currentUser.ParentDepartmentId = newSchoolId;
            currentUser = _learnerService.UpdateUser(validationSpecification, currentUser) as LearnerDto;
        }

        private JsonDynamicObject InsertLearner(dynamic jsonCommand)
        {
            var insertData = jsonCommand.data;
            var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                   .ValidateDepartment((int)insertData.CurrentSchoolId, ArchetypeEnum.Unknown)
                   .SkipCheckingArchetype()
                   .WithStatus(EntityStatusEnum.All)
                   .IsNotDirectParent()
                   .Create();

            var insertDto = new LearnerDto
            {
                Identity = CreateIdentity(jsonCommand.Identity),
                ParentDepartmentId = (int)insertData.CurrentSchoolId,
                FirstName = insertData.FirstName,
                LastName = insertData.LastName,
                SSN = insertData.SSN,
                MobileNumber = !string.IsNullOrEmpty(insertData.MobileNumber) ? insertData.MobileNumber : null,
                Gender = insertData.Gender != null ? (short?)EnumValueFromDynamicValue(insertData.Gender) : null,
                EmailAddress = insertData.EmailAddress,
                Tag = insertData.Tag,
                EntityStatus = new EntityStatusDto
                {
                    LastExternallySynchronized = DateTime.Now,
                    StatusReasonId = EntityStatusReasonEnum.Active_SynchronizedFromSource,
                    StatusId = EntityStatusEnum.Active,
                    LastUpdatedBy = _workContext.CurrentUserId,
                    ExternallyMastered = true
                }
            };
            //Check bithdate has value or an empty string
            var dob = GetJsonDate(insertData.BirthDate);
            if (dob != null)
                insertDto.DateOfBirth = dob;
            insertDto.Identity.Id = 0;
            var insertedLearner = _learnerService.InsertUser(validationSpecification, insertDto) as LearnerDto;
            ProcessMoveLearner(insertData, insertedLearner, new List<ExpandoObject>());
            var levelId = insertData.NewLevelId;
            if (levelId != null)
            {
                ProcessLearnerLevel(insertedLearner, (int)levelId);
            }
            var newTeachingGroupExtids = insertData.NewTeachingGroupExtIds == null ? new List<string>() : new List<string>(insertData.NewTeachingGroupExtIds.Split(','));
            ProcessLearnerTeachingGroups(insertedLearner, newTeachingGroupExtids, null);
            return CreateJsonDynamicObjectIdentity(insertedLearner.Identity);
        }

        private void ProcessLearnerTeachingGroups(LearnerDto learnerDto, List<string> newTeachingGroupExtIds, List<string> removeTeachingGroupExtIds)
        {
            if (newTeachingGroupExtIds != null)
            {
                newTeachingGroupExtIds = newTeachingGroupExtIds.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (newTeachingGroupExtIds.Any())
                {
                    var teachingGroupToAdd = _teachingGroupService.GetUserGroups<TeachingGroupDto>(_workContext.CurrentOwnerId,
                    new List<int> { _workContext.CurrentCustomerId }, extIds: newTeachingGroupExtIds).Items.ToList();

                    var teachingGroupMembers = new List<MembershipDto>();
                    foreach (var userGroupExtId in newTeachingGroupExtIds)
                    {
                        var groupInfo = teachingGroupToAdd.FirstOrDefault(x => x.Identity.ExtId == userGroupExtId);
                        var membership = new UGMemberEntity
                        {
                            UserId = (int)learnerDto.Identity.Id,
                            UserGroupId = (int)groupInfo.Identity.Id,
                            validFrom = DateTime.Now,
                            EntityStatusId = (int)EntityStatusEnum.Active,
                            EntityStatusReasonId = (int)EntityStatusReasonEnum.Active_ManuallySetActive,
                            ExtId = learnerDto.Identity.ExtId,
                            ReferrerToken = string.Empty,
                            ReferrerResource = string.Empty,
                            DisplayName = string.Empty,
                            Created = DateTime.Now,
                            LastUpdated = DateTime.Now,
                            LastSynchronized = DateTime.Now,
                            CustomerId = learnerDto.Identity.CustomerId,
                            CreatedBy = _workContext.CurrentUserId,
                            LastUpdatedBy = _workContext.CurrentUserId,
                            MemberRoleId = (int)UGMemberRole.Learner

                        };
                        _uGMemberRepository.Insert(membership);
                    }
                }
            }

            if (removeTeachingGroupExtIds != null)
            {
                removeTeachingGroupExtIds = removeTeachingGroupExtIds.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (removeTeachingGroupExtIds.Any())
                {
                    var teachingGroupMembers = new List<MembershipDto>();

                    var ugMembers = _uGMemberRepository.GetUGMembers(userIds: new List<int> { (int)learnerDto.Identity.Id.Value },
                        userGroupExtIds: removeTeachingGroupExtIds, includeUserGroup: true);
                    foreach (var userGroupExtId in removeTeachingGroupExtIds)
                    {
                        var groupInfo = ugMembers.FirstOrDefault(x => x.UserGroup.ExtId == userGroupExtId && x.UserId == learnerDto.Identity.Id);
                        groupInfo.EntityStatusId = (int)EntityStatusEnum.Deactive;
                        groupInfo.EntityStatusReasonId = (int)EntityStatusReasonEnum.Deactive_ManuallySetDeactive;
                        groupInfo.ValidTo = DateTime.Now;
                        groupInfo.LastUpdated = DateTime.Now;
                        groupInfo.LastSynchronized = DateTime.Now;
                        _uGMemberRepository.Update(groupInfo);
                    }
                    _uGMemberService.Update(teachingGroupMembers);
                }
            }
            _organizationUnitOfWork.SaveChanges();
        }

        private void ProcessEmployeeTeachingGroups(EmployeeDto learnerDto, List<string> newTeachingGroupExtIds, List<string> removeTeachingGroupExtIds)
        {

            if (newTeachingGroupExtIds != null)
            {
                newTeachingGroupExtIds = newTeachingGroupExtIds.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (newTeachingGroupExtIds.Any())
                {
                    var teachingGroupToAdd = _teachingGroupService.GetUserGroups<TeachingGroupDto>(_workContext.CurrentOwnerId,
                    new List<int> { _workContext.CurrentCustomerId }, extIds: newTeachingGroupExtIds).Items.ToList();

                    var teachingGroupMembers = new List<MembershipDto>();
                    foreach (var userGroupExtId in newTeachingGroupExtIds)
                    {
                        var groupInfo = teachingGroupToAdd.FirstOrDefault(x => x.Identity.ExtId == userGroupExtId);
                        var membership = new UGMemberEntity
                        {
                            UserId = (int)learnerDto.Identity.Id,
                            UserGroupId = (int)groupInfo.Identity.Id,
                            validFrom = DateTime.Now,
                            EntityStatusId = (int)EntityStatusEnum.Active,
                            EntityStatusReasonId = (int)EntityStatusReasonEnum.Active_ManuallySetActive,
                            ExtId = learnerDto.Identity.ExtId,
                            ReferrerToken = string.Empty,
                            ReferrerResource = string.Empty,
                            DisplayName = string.Empty,
                            Created = DateTime.Now,
                            LastUpdated = DateTime.Now,
                            LastSynchronized = DateTime.Now,
                            CustomerId = learnerDto.Identity.CustomerId,
                            CreatedBy = _workContext.CurrentUserId,
                            LastUpdatedBy = _workContext.CurrentUserId,
                            MemberRoleId = (int)UGMemberRole.Employee

                        };
                        _uGMemberRepository.Insert(membership);
                    }
                }
            }

            if (removeTeachingGroupExtIds != null)
            {
                removeTeachingGroupExtIds = removeTeachingGroupExtIds.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (removeTeachingGroupExtIds.Any())
                {
                    var teachingGroupMembers = new List<MembershipDto>();

                    var ugMembers = _uGMemberRepository.GetUGMembers(userIds: new List<int> { (int)learnerDto.Identity.Id.Value },
                        userGroupExtIds: removeTeachingGroupExtIds, includeUserGroup: true);
                    foreach (var userGroupExtId in removeTeachingGroupExtIds)
                    {
                        var groupInfo = ugMembers.FirstOrDefault(x => x.UserGroup.ExtId == userGroupExtId && x.UserId == learnerDto.Identity.Id);
                        groupInfo.EntityStatusId = (int)EntityStatusEnum.Deactive;
                        groupInfo.EntityStatusReasonId = (int)EntityStatusReasonEnum.Deactive_ManuallySetDeactive;
                        groupInfo.ValidTo = DateTime.Now;
                        groupInfo.LastUpdated = DateTime.Now;
                        groupInfo.LastSynchronized = DateTime.Now;
                        _uGMemberRepository.Update(groupInfo);
                    }
                    _uGMemberService.Update(teachingGroupMembers);
                }
            }
            _organizationUnitOfWork.SaveChanges();
        }


        #endregion
    }
}

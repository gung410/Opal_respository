using System;
using System.Collections.Generic;
using Amazon.KeyManagementService;
using Backend.CrossCutting.HttpClientHelper;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Business.Crypto;
using cxOrganization.Domain.Business.Queries.ApprovingOfficer;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Services.ExportService;
using cxOrganization.Domain.Services.Reports;
using cxOrganization.Domain.Services.StorageServices;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Settings.MassUserCreationMessageSetting;
using cxOrganization.Domain.Validators;
using cxOrganization.Domain.Validators.UserTypes;

using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain
{
    public static class DomainConfiguration
    {
        public static void UseDomainService(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .RegisterOptions(configuration)
                .UseCryptographyService();

            RegisterCryptoService(services, configuration);
            RegisterRepositories(services);
            //mapping services
            RegisterDepartmentMappingServices(services);
            RegisterUserMappingServices(services);
            RegisterUserGroupMappingServices(services);
            RegisterUserGroupUserMappingServices(services);
            RegisterUsertypeMappingServices(services);
            RegisterCustomerMappingServices(services);

            //validators

            RegisterDepartmentValidators(services);
            RegisterCustomerValidators(services);
            RegisterUserGroupValidators(services);
            RegisterUserValidators(services);
            RegisterUsertypeValidators(services);
            RegisterUgMemberValidators(services);

            //api client
            services.UseHttpClientHelperConfiguration();
            services.AddScoped<IEventLogDomainApiClient, EventLogDomainApiClient>();

            //services
            //services.AddScoped<ICandidatePoolMemberService, CandidatePoolMemberService>();
            services.AddHttpClient<IIdentityServerClientService, IdmClientService>();
            services.AddHttpClient<ILearningCatalogClientService, LearningCatalogClientService>();
            services.AddHttpClient<ISystemRoleService, SystemRoleService>();
            services.AddHttpClient<IInternalHttpClientRequestService, InternalHttpClientRequestService>();

            services.AddScoped<IClassMemberService, ClassMemberService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDepartmentTypeService, DepartmentTypeService>();
            services.AddScoped<IHierarchyDepartmentService, HierarchyDepartmentService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<ILoginServiceUserService, LoginServiceUserService>();
            services.AddScoped<IOwnerService, OwnerService>();
            //services.AddScoped<ITeachingGroupMemberService, TeachingGroupMemberService>();
            services.AddScoped<IUserWithIdpInfoMappingService, UserInfoMappingService>();
            services.AddScoped<IUserInfoService, UserInfoService>();

            services.AddScoped<IBroadcastMessageService, BroadcastMessageService>();
            services.AddScoped<IBroadcastMessageRepository, BroadcastMessageRepository>();

            // Auth
            services.AddScoped<IPortalApiClient, PortalApiClient>();

            services.AddScoped<IFileInfoRepository, FileInfoRepository>();
            services.AddScoped<IFileInfoService, FileInfoService>();

            services.AddScoped<IApprovalGroupMemberService, ApprovalGroupMemberService>();
            services.AddScoped<ApprovalGroupValidator>();
            services.AddScoped<ApprovalGroupMappingService>();
            //services.AddScoped<ITeachingGroupMemberService, TeachingGroupMemberService>();

            services.AddScoped<IDataFileReader, CsvFileReader>();

            services.AddScoped<IUserPoolMemberService, UserPoolMemberService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IUserGroupTypeService, UserGroupTypeService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IDatahubLogger, DatahubLogger>();

            services.AddScoped<IExportService<UserGenericDto>, UserManagementExportService>();
            services.AddScoped<IExportService<UserEventLogInfo>, UserEventLogExportService>();
            services.AddScoped<IExportService<UserStatisticsDto>, UserStatisticsExportService>();
            services.AddScoped<IExportService<ApprovingOfficerInfo>, ApprovingOfficerExportService>();
            services.AddScoped<IExportService<UserAccountDetailsInfo>, UserAccountDetailsExportService>();
            services.AddScoped<IExportService<PrivilegedUserAccountInfo>, PrivilegedUserAccountExportService>();



            services.AddSingleton<ISuspendOrDeactiveUserBackgroundJob, SuspendOrDeactiveUserBackgroundJob>();
            services.AddSingleton<DeActiveUserStatusStrategy>();
            services.AddSingleton<SuspendUserStatusStrategy>();

            services.AddScoped<IFileStorageService, FileStorageS3Service>();
            services.AddScoped<IDataHubQueryApiClient, DataHubQueryApiClient>();

            services.AddScoped<IUserReportService, UserReportService>();


            //memory cache
            services.UseMemoryCacheAdapter(configuration);


            RegisterMassCreationUserService(services);
            RegisterUserService(services);
            RegisterUserGroupService(services);
            RegisterDepartmentService(services);
            RegisterUGMemberService(services);
            RegisterUsertypeService(services);

            RegisterQueries(services);

        }

        private static IServiceCollection RegisterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IntegrationSetting>(configuration.GetSection("IntegrationSetting"));
            services.Configure<MassUserCreationMessageConfiguration>(configuration.GetSection("MassUserCreationMessageConfiguration"));

            return services;
        }

        private static void RegisterCryptoService(IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            var awsKmsEncyption = configuration.GetSection("AwsKmsEncyption").Get<AwsKmsEncyption>();
            string secretboxNonce = null;
            string secretboxKey = null;

            if (appSettings.EncryptSSN)
            {
                secretboxNonce = Environment.GetEnvironmentVariable("SECRETBOX_NONCE");
                secretboxKey = Environment.GetEnvironmentVariable("SECRETBOX_KEY");
                if (string.IsNullOrEmpty(secretboxNonce) || string.IsNullOrEmpty(secretboxKey))
                {
                    throw new Exception("EncryptSSN mode is enabled but missing the Environment Variable (SECRETBOX_NONCE or SECRETBOX_KEY)");
                }
            }
            services.AddSingleton(new CryptoSetting(secretboxNonce, secretboxKey, awsKmsEncyption));
            if (awsKmsEncyption.Enabled)
            {
                services.AddSingleton(new AmazonKeyManagementServiceClient(awsKmsEncyption.AwsAccessKey, awsKmsEncyption.AwsSecretKey));
                services.AddSingleton<ICryptoService, AwsKmsCryptoService>();
                services.AddSingleton<ICryptoService, SodiumCryptoService>();
            }
            else
                services.AddSingleton<ICryptoService, SodiumCryptoService>();
            services.AddSingleton<IUserCryptoService, UserCryptoService>();
        }

        private static void RegisterQueries(IServiceCollection services)
        {
            services.AddScoped<SearchApprovingOfficersQueryHandler>();
        }

        private static void RegisterUsertypeService(IServiceCollection services)
        {
            services.AddScoped<IUserTypeService, UserTypeService>();
            services.AddScoped<Func<ArchetypeEnum, IUserTypeService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.Learner:
                        return new UserTypeService(
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<LevelValidator>(),
                            serviceProvider.GetService<LearnerValidator>(),
                            serviceProvider.GetService<IUserTypeMappingService>());
                    case ArchetypeEnum.Employee:
                        return new UserTypeService(
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<RoleValidator>(),
                            serviceProvider.GetService<EmployeeValidator>(),
                            serviceProvider.GetService<IUserTypeMappingService>());
                    case ArchetypeEnum.Candidate:
                        return new UserTypeService(
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<RoleValidator>(),
                            serviceProvider.GetService<CandidateValidator>(),
                            serviceProvider.GetService<IUserTypeMappingService>());
                    default:
                        throw new NotSupportedException();
                }
            });
        }

        private static void RegisterUsertypeValidators(IServiceCollection services)
        {
            services.AddScoped<LevelValidator>();
            services.AddScoped<UserGenericValidator>();
            services.AddScoped<RoleValidator>();
            services.AddScoped<UserTypeValidator>();
            services.AddScoped<IUserTypeValidator, UserTypeValidator>();
            services.AddScoped<Func<ArchetypeEnum, IUserTypeValidator>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.Level:
                        return serviceProvider.GetService<LevelValidator>();
                    case ArchetypeEnum.Role:
                        return serviceProvider.GetService<RoleValidator>();
                    default:
                        return serviceProvider.GetService<UserTypeValidator>();
                }
            });
        }

        private static void RegisterUGMemberService(IServiceCollection services)
        {
            services.AddScoped<IUGMemberService, UGMemberService>();
            services.AddScoped<UGMemberService>();
            services.AddScoped<Func<ArchetypeEnum, IUGMemberService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.TeachingGroup:
                        return new UGMemberService(
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<TeachingGroupValidator>(),
                            serviceProvider.GetService<LearnerValidator>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IUGMemberValidator>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<IAdvancedWorkContext>());
                    case ArchetypeEnum.CandidatePool:
                        return new UGMemberService(
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<CandidatePoolValidator>(),
                            serviceProvider.GetService<CandidateValidator>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IUGMemberValidator>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<IAdvancedWorkContext>());
                    case ArchetypeEnum.ExternalUserGroup:
                        return new UGMemberService(
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<ExternalUserGroupValidator>(),
                            serviceProvider.GetService<LearnerValidator>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IUGMemberValidator>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<IAdvancedWorkContext>());
                    default:
                        return serviceProvider.GetService<UGMemberService>();
                }
            });
        }

        private static void RegisterDepartmentService(IServiceCollection services)
        {
            services.AddScoped<IDepartmentAccessService, DepartmentAccessService>();
            services.AddScoped<IHierarchyDepartmentPermissionService, HierarchyDepartmentPermissionService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<Func<ArchetypeEnum, IDepartmentService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case (ArchetypeEnum.SchoolOwner):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<SchoolOwnerValidator>(),
                                                    serviceProvider.GetService<SchoolOwnerMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.OrganizationalUnit):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<OrganizationalUnitValidator>(),
                                                    serviceProvider.GetService<OrganizationalUnitMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.Company):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<CompanyValidator>(),
                                                    serviceProvider.GetService<CompanyMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.DataOwner):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<DataOwnerValidator>(),
                                                    serviceProvider.GetService<DataOwnerMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.Class):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<ClassValidator>(),
                                                    serviceProvider.GetService<ClassMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.Country):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<CountryValidator>(),
                                                    serviceProvider.GetService<CountryMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.School):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<SchoolValidator>(),
                                                    serviceProvider.GetService<SchoolMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    case (ArchetypeEnum.CandidateDepartment):
                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<CandidateDepartmentValidator>(),
                                                    serviceProvider.GetService<CandidateDepartmentMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                    default:

                        return new DepartmentService(serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                                                    serviceProvider.GetService<IDepartmentRepository>(),
                                                    serviceProvider.GetService<IAdvancedWorkContext>(),
                                                    serviceProvider.GetService<OrganizationDbContext>(),
                                                    //ISecurityHandler securityHandler,
                                                    serviceProvider.GetService<IUserRepository>(),
                                                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                                                    serviceProvider.GetService<ICustomerRepository>(),
                                                    serviceProvider.GetService<DepartmentValidator>(),
                                                    serviceProvider.GetService<DepartmentMappingService>(),
                                                    serviceProvider.GetService<ICommonService>(),
                                                    serviceProvider.GetService<IHierarchyDepartmentService>(),
                                                    serviceProvider.GetService<IDatahubLogger>());
                }
            });
        }
        private static void RegisterUserGroupService(IServiceCollection services)
        {
            services.AddScoped<IApprovalGroupAccessService, ApprovalGroupAccessService>();
            services.AddScoped<IUserPoolAccessService, UserPoolAccessService>();

            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<Func<ArchetypeEnum, IUserGroupService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.CandidatePool:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<CandidatePoolMappingService>(),
                            serviceProvider.GetService<CandidatePoolValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                    case ArchetypeEnum.ApprovalGroup:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<ApprovalGroupMappingService>(),
                            serviceProvider.GetService<ApprovalGroupValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                    case ArchetypeEnum.ExternalUserGroup:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<ExternalUserGroupMappingService>(),
                            serviceProvider.GetService<ExternalUserGroupValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                    case ArchetypeEnum.Team:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<TeamMappingService>(),
                            serviceProvider.GetService<UserGroupValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                    case ArchetypeEnum.TeachingGroup:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<TeachingGroupMappingService>(),
                            serviceProvider.GetService<TeachingGroupValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                    case ArchetypeEnum.UserPool:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<UserPoolMappingService>(),
                            serviceProvider.GetService<UserPoolValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                    default:
                        return new UserGroupService(
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<UserGroupMappingService>(),
                            serviceProvider.GetService<UserGroupValidator>(),
                            serviceProvider.GetService<IDepartmentValidator>(),
                            serviceProvider.GetService<IDatahubLogger>());
                }
            });
        }

        private static void RegisterMassCreationUserService(IServiceCollection services)
        {
            services.AddScoped<IMassCreationUserService>(service => new MassCreationUserService(
                service.GetService<IDataFileReader>(),
                service.GetService<IOptions<IntegrationSetting>>(),
                service.GetService<IOptions<MassUserCreationMessageConfiguration>>(),
                service.GetService<IUserRepository>(),
                service.GetService<IDepartmentService>(),
                service.GetService<IUserTypeService>(),
                service.GetService<IHierarchyDepartmentPermissionService>(),
                service.GetService<IDepartmentAccessService>(),
                service.GetService<IAdvancedWorkContext>(),
                service.GetService<IUserAccessService>()
                ));
        }
        private static void RegisterUserService(IServiceCollection services)
        {
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<Func<ArchetypeEnum, IUserService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.Candidate:
                        return new UserService(
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IDepartmentRepository>(),
                            serviceProvider.GetService<ICommonService>(),
                            serviceProvider.GetService<CandidateValidator>(),
                            serviceProvider.GetService<CandidateMappingService>(),
                            serviceProvider.GetService<IDepartmentMappingService>(),
                            serviceProvider.GetService<IUserGroupMappingService>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<Func<string, ICryptographyService>>(),
                            serviceProvider.GetService<IOwnerRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IObjectMappingRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentService>(),
                            serviceProvider.GetService<IDatahubLogger>(),
                            serviceProvider.GetService<IUserWithIdpInfoMappingService>(),
                            serviceProvider.GetService<ILogger<UserService>>(),
                            serviceProvider.GetService<IOptions<EmailTemplates>>(),
                            serviceProvider.GetService<IOptions<AppSettings>>(),
                            serviceProvider.GetService<IOptions<EntityStatusReasonTexts>>(),
                            serviceProvider.GetService<IUserAccessService>(),
                            serviceProvider.GetService<Func<ArchetypeEnum, IUserGroupService>>(),
                            serviceProvider.GetService<IDTDEntityRepository>(),
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentPermissionService>(),
                            serviceProvider.GetService<IIdentityServerClientService>(),
                            serviceProvider.GetService<IUserGroupService>(),
                            serviceProvider.GetService<IUGMemberService>(),
                            serviceProvider.GetService<IServiceScopeFactory>(),
                            serviceProvider.GetService<IInternalHttpClientRequestService>());
                    case ArchetypeEnum.Learner:
                        return new UserService(
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IDepartmentRepository>(),
                            serviceProvider.GetService<ICommonService>(),
                            serviceProvider.GetService<LearnerValidator>(),
                            serviceProvider.GetService<LearnerMappingService>(),
                            serviceProvider.GetService<IDepartmentMappingService>(),
                            serviceProvider.GetService<IUserGroupMappingService>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<Func<string, ICryptographyService>>(),
                            serviceProvider.GetService<IOwnerRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IObjectMappingRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentService>(),
                            serviceProvider.GetService<IDatahubLogger>(),
                            serviceProvider.GetService<IUserWithIdpInfoMappingService>(),
                            serviceProvider.GetService<ILogger<UserService>>(),
                            serviceProvider.GetService<IOptions<EmailTemplates>>(),
                            serviceProvider.GetService<IOptions<AppSettings>>(),
                            serviceProvider.GetService<IOptions<EntityStatusReasonTexts>>(),
                            serviceProvider.GetService<IUserAccessService>(),
                            serviceProvider.GetService<Func<ArchetypeEnum, IUserGroupService>>(),
                            serviceProvider.GetService<IDTDEntityRepository>(),
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentPermissionService>(),
                            serviceProvider.GetService<IIdentityServerClientService>(),
                            serviceProvider.GetService<IUserGroupService>(),
                            serviceProvider.GetService<IUGMemberService>(),
                            serviceProvider.GetService<IServiceScopeFactory>(),
                            serviceProvider.GetService<IInternalHttpClientRequestService>());
                    case ArchetypeEnum.Employee:
                        return new UserService(
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IDepartmentRepository>(),
                            serviceProvider.GetService<ICommonService>(),
                            serviceProvider.GetService<EmployeeValidator>(),
                            serviceProvider.GetService<EmployeeMappingService>(),
                            serviceProvider.GetService<IDepartmentMappingService>(),
                            serviceProvider.GetService<IUserGroupMappingService>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<Func<string, ICryptographyService>>(),
                            serviceProvider.GetService<IOwnerRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IObjectMappingRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentService>(),
                            serviceProvider.GetService<IDatahubLogger>(),
                            serviceProvider.GetService<IUserWithIdpInfoMappingService>(),
                            serviceProvider.GetService<ILogger<UserService>>(),
                            serviceProvider.GetService<IOptions<EmailTemplates>>(),
                            serviceProvider.GetService<IOptions<AppSettings>>(),
                            serviceProvider.GetService<IOptions<EntityStatusReasonTexts>>(),
                            serviceProvider.GetService<IUserAccessService>(),
                            serviceProvider.GetService<Func<ArchetypeEnum, IUserGroupService>>(),
                            serviceProvider.GetService<IDTDEntityRepository>(),
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentPermissionService>(),
                            serviceProvider.GetService<IIdentityServerClientService>(),
                            serviceProvider.GetService<IUserGroupService>(),
                            serviceProvider.GetService<IUGMemberService>(),
                            serviceProvider.GetService<IServiceScopeFactory>(),
                            serviceProvider.GetService<IInternalHttpClientRequestService>());
                    case ArchetypeEnum.Unknown:
                        return new UserService(
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IDepartmentRepository>(),
                            serviceProvider.GetService<ICommonService>(),
                            serviceProvider.GetService<UserGenericValidator>(),
                            serviceProvider.GetService<UserGenericMappingService>(),
                            serviceProvider.GetService<IDepartmentMappingService>(),
                            serviceProvider.GetService<IUserGroupMappingService>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<Func<string, ICryptographyService>>(),
                            serviceProvider.GetService<IOwnerRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IObjectMappingRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentService>(),
                            serviceProvider.GetService<IDatahubLogger>(),
                            serviceProvider.GetService<IUserWithIdpInfoMappingService>(),
                            serviceProvider.GetService<ILogger<UserService>>(),
                            serviceProvider.GetService<IOptions<EmailTemplates>>(),
                            serviceProvider.GetService<IOptions<AppSettings>>(),
                            serviceProvider.GetService<IOptions<EntityStatusReasonTexts>>(),
                            serviceProvider.GetService<IUserAccessService>(),
                            serviceProvider.GetService<Func<ArchetypeEnum, IUserGroupService>>(),
                            serviceProvider.GetService<IDTDEntityRepository>(),
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentPermissionService>(),
                            serviceProvider.GetService<IIdentityServerClientService>(),
                            serviceProvider.GetService<IUserGroupService>(),
                            serviceProvider.GetService<IUGMemberService>(),
                            serviceProvider.GetService<IServiceScopeFactory>(),
                            serviceProvider.GetService<IInternalHttpClientRequestService>());
                    default:
                        return new UserService(
                            serviceProvider.GetService<IUserRepository>(),
                            serviceProvider.GetService<IAdvancedWorkContext>(),
                            serviceProvider.GetService<OrganizationDbContext>(),
                            serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                            serviceProvider.GetService<IUserTypeRepository>(),
                            serviceProvider.GetService<IDepartmentRepository>(),
                            serviceProvider.GetService<ICommonService>(),
                            serviceProvider.GetService<UserValidator>(),
                            serviceProvider.GetService<UserMappingService>(),
                            serviceProvider.GetService<IDepartmentMappingService>(),
                            serviceProvider.GetService<IUserGroupMappingService>(),
                            serviceProvider.GetService<IEventLogDomainApiClient>(),
                            serviceProvider.GetService<Func<string, ICryptographyService>>(),
                            serviceProvider.GetService<IOwnerRepository>(),
                            serviceProvider.GetService<IUserGroupUserMappingService>(),
                            serviceProvider.GetService<IObjectMappingRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentService>(),
                            serviceProvider.GetService<IDatahubLogger>(),
                            serviceProvider.GetService<IUserWithIdpInfoMappingService>(),
                            serviceProvider.GetService<ILogger<UserService>>(),
                            serviceProvider.GetService<IOptions<EmailTemplates>>(),
                            serviceProvider.GetService<IOptions<AppSettings>>(),
                            serviceProvider.GetService<IOptions<EntityStatusReasonTexts>>(),
                            serviceProvider.GetService<IUserAccessService>(),
                            serviceProvider.GetService<Func<ArchetypeEnum, IUserGroupService>>(),
                            serviceProvider.GetService<IDTDEntityRepository>(),
                            serviceProvider.GetService<IUserGroupRepository>(),
                            serviceProvider.GetService<IUGMemberRepository>(),
                            serviceProvider.GetService<IHierarchyDepartmentPermissionService>(),
                            serviceProvider.GetService<IIdentityServerClientService>(),
                            serviceProvider.GetService<IUserGroupService>(),
                            serviceProvider.GetService<IUGMemberService>(),
                            serviceProvider.GetService<IServiceScopeFactory>(),
                            serviceProvider.GetService<IInternalHttpClientRequestService>());
                }
            });
        }
        private static void RegisterUgMemberValidators(IServiceCollection services)
        {
            services.AddScoped<IUGMemberValidator, UGMemberValidator>();
        }
        private static void RegisterCustomerValidators(IServiceCollection services)
        {
            services.AddScoped<ICustomerValidator, CustomerValidator>();
        }
        private static void RegisterDepartmentValidators(IServiceCollection services)
        {
            services.AddScoped<IDepartmentValidator, DepartmentValidator>();
            services.AddScoped<DepartmentValidator>();
            services.AddScoped<SchoolOwnerValidator>();
            services.AddScoped<OrganizationalUnitValidator>();
            services.AddScoped<CompanyValidator>();
            services.AddScoped<SchoolValidator>();
            services.AddScoped<CandidateDepartmentValidator>();
            services.AddScoped<CountryValidator>();
            services.AddScoped<DataOwnerValidator>();
            services.AddScoped<ClassValidator>();
            services.AddScoped<Func<ArchetypeEnum, IDepartmentValidator>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.School:
                        return serviceProvider.GetService<SchoolValidator>();
                    case ArchetypeEnum.Class:
                        return serviceProvider.GetService<ClassValidator>();
                    case ArchetypeEnum.SchoolOwner:
                        return serviceProvider.GetService<SchoolOwnerValidator>();
                    case ArchetypeEnum.OrganizationalUnit:
                        return serviceProvider.GetService<OrganizationalUnitValidator>();
                    case ArchetypeEnum.Country:
                        return serviceProvider.GetService<CountryValidator>();
                    case ArchetypeEnum.CandidateDepartment:
                        return serviceProvider.GetService<CandidateDepartmentValidator>();
                    case ArchetypeEnum.Company:
                        return serviceProvider.GetService<CompanyValidator>();
                    case ArchetypeEnum.DataOwner:
                        return serviceProvider.GetService<DataOwnerValidator>();
                    default:
                        return serviceProvider.GetService<DepartmentValidator>(); // or maybe return null, up to you
                }
            });
        }
        private static void RegisterUserValidators(IServiceCollection services)
        {
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<UserValidator>();
            services.AddScoped<EmployeeValidator>();
            services.AddScoped<LearnerValidator>();
            services.AddScoped<CandidateValidator>();
            services.AddScoped<CandidateValidator>();
            services.AddScoped<Func<ArchetypeEnum, IUserValidator>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.Employee:
                        return serviceProvider.GetService<EmployeeValidator>();
                    case ArchetypeEnum.Learner:
                        return serviceProvider.GetService<LearnerValidator>();
                    case ArchetypeEnum.Candidate:
                        return serviceProvider.GetService<CandidateValidator>();
                    default:
                        return serviceProvider.GetService<UserValidator>(); // or maybe return null, up to you
                }
            });
        }
        private static void RegisterUserGroupValidators(IServiceCollection services)
        {
            services.AddScoped<IUserGroupValidator, UserGroupValidator>();
            services.AddScoped<UserGroupValidator>();
            services.AddScoped<ExternalUserGroupValidator>();
            services.AddScoped<TeachingGroupValidator>();
            services.AddScoped<CandidatePoolValidator>();
            services.AddScoped<UserPoolValidator>();
            services.AddScoped<Func<ArchetypeEnum, IUserGroupValidator>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.School:
                        return serviceProvider.GetService<ExternalUserGroupValidator>();
                    case ArchetypeEnum.Class:
                        return serviceProvider.GetService<TeachingGroupValidator>();
                    case ArchetypeEnum.SchoolOwner:
                        return serviceProvider.GetService<CandidatePoolValidator>();
                    default:
                        return serviceProvider.GetService<UserGroupValidator>(); // or maybe return null, up to you
                }
            });
        }
        private static void RegisterUserGroupMappingServices(IServiceCollection services)
        {
            services.AddScoped<IUserGroupMappingService, UserGroupMappingService>();
            services.AddScoped<UserGroupMappingService>();
            services.AddScoped<ExternalUserGroupMappingService>();
            services.AddScoped<TeamMappingService>();
            services.AddScoped<CandidatePoolMappingService>();
            services.AddScoped<TeachingGroupMappingService>();
            services.AddScoped<UserPoolMappingService>();
            services.AddScoped<Func<ArchetypeEnum, IUserGroupMappingService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.ExternalUser:
                        return serviceProvider.GetService<ExternalUserGroupMappingService>();
                    case ArchetypeEnum.Team:
                        return serviceProvider.GetService<TeamMappingService>();
                    case ArchetypeEnum.CandidatePool:
                        return serviceProvider.GetService<CandidatePoolMappingService>();
                    case ArchetypeEnum.TeachingGroup:
                        return serviceProvider.GetService<TeachingGroupMappingService>();
                    case ArchetypeEnum.UserPool:
                        return serviceProvider.GetService<UserPoolMappingService>();
                    default:
                        return serviceProvider.GetService<UserGroupMappingService>(); // or maybe return null, up to you
                }
            });
        }
        private static void RegisterUserGroupUserMappingServices(IServiceCollection services)
        {
            services.AddScoped<IUserGroupUserMappingService, UserGroupUserMappingService>();
        }
        private static void RegisterUsertypeMappingServices(IServiceCollection services)
        {
            services.AddScoped<IUserTypeMappingService, UserTypeMappingService>();
        }
        private static void RegisterCustomerMappingServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerMappingService, CustomerMappingService>();
        }
        private static void RegisterUserMappingServices(IServiceCollection services)
        {
            services.AddScoped<IUserMappingService, UserMappingService>();

            services.AddScoped<EmployeeMappingService>(serviceProvider =>
            {
                return new EmployeeMappingService(serviceProvider.GetService<IUserTypeRepository>(),
                    75,
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<IDepartmentRepository>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<IUserRepository>(),
                    serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                    serviceProvider.GetService<IOwnerRepository>(),
                    serviceProvider.GetService<IDepartmentService>(),
                    serviceProvider.GetService<IUserTypeMappingService>(),
                    serviceProvider.GetService<IUserCryptoService>(),
                    serviceProvider.GetService<IOptions<AppSettings>>());
            });
            services.AddScoped<CandidateMappingService>(serviceProvider =>
            {
                return new CandidateMappingService(serviceProvider.GetService<IUserTypeRepository>(),
                    39,
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<IOwnerRepository>(),
                    serviceProvider.GetService<IUserTypeMappingService>(),
                    serviceProvider.GetService<IUserCryptoService>(),
                    serviceProvider.GetService<IOptions<AppSettings>>());
            });
            services.AddScoped<LearnerMappingService>(serviceProvider =>
            {
                return new LearnerMappingService(serviceProvider.GetService<IUserTypeRepository>(),
                    75,
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<IDepartmentService>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                    serviceProvider.GetService<IOwnerRepository>(),
                    serviceProvider.GetService<IUserTypeMappingService>(),
                    serviceProvider.GetService<IUserCryptoService>(),
                    serviceProvider.GetService<IOptions<AppSettings>>());
            });
            services.AddScoped<ActorMappingService>(serviceProvider =>
            {
                return new ActorMappingService(serviceProvider.GetService<IUserTypeRepository>(),
                    75,
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<IDepartmentRepository>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<IUserRepository>(),
                    serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                    serviceProvider.GetService<IOwnerRepository>(),
                    serviceProvider.GetService<IUserTypeMappingService>(),
                    serviceProvider.GetService<IUserCryptoService>(),
                    serviceProvider.GetService<IOptions<AppSettings>>());
            });
            services.AddScoped<UserGenericMappingService>(serviceProvider =>
            {
                return new UserGenericMappingService(serviceProvider.GetService<IUserTypeRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<IDepartmentRepository>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<IUserRepository>(),
                    serviceProvider.GetService<IHierarchyDepartmentRepository>(),
                    serviceProvider.GetService<IOwnerRepository>(),
                    serviceProvider.GetService<IUserTypeMappingService>(),
                    serviceProvider.GetService<ILogger<UserGenericMappingService>>(),
                    serviceProvider.GetService<IOptions<AppSettings>>(),
                    serviceProvider.GetService<IUserCryptoService>());
            });
            services.AddScoped<Func<ArchetypeEnum, IUserMappingService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.Employee:
                        return serviceProvider.GetService<EmployeeMappingService>();
                    case ArchetypeEnum.Candidate:
                        return serviceProvider.GetService<CandidateMappingService>();
                    case ArchetypeEnum.Learner:
                        return serviceProvider.GetService<LearnerMappingService>();
                    case ArchetypeEnum.Actor:
                        return serviceProvider.GetService<ActorMappingService>();
                    case ArchetypeEnum.Unknown:
                        return serviceProvider.GetService<UserGenericMappingService>();
                    default:
                        return serviceProvider.GetService<UserMappingService>(); // or maybe return null, up to you
                }
            });
        }
        private static void RegisterDepartmentMappingServices(IServiceCollection services)
        {
            services.AddScoped<IDepartmentMappingService, DepartmentMappingService>();
            services.AddScoped<IDepartmentTypeMappingService, DepartmentTypeMappingService>();

            services.AddScoped<IHierarchyDepartmentMappingService, HierarchyDepartmentMappingService>();
            services.AddScoped<DepartmentMappingService>();
            services.AddScoped<OrganizationalUnitMappingService>();
            services.AddScoped<SchoolOwnerMappingService>(serviceProvider =>
            {
                return new SchoolOwnerMappingService(
                    new List<int> { 35, 34 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>()
                    );
            });
            services.AddScoped<CountryMappingService>(serviceProvider =>
            {
                return new CountryMappingService(
                    new List<int> { 3 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IPropertyService>()
                    );
            });
            services.AddScoped<CandidateDepartmentMappingService>(serviceProvider =>
            {
                return new CandidateDepartmentMappingService(
                    new List<int> { 4 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IPropertyService>()
                    );
            });
            services.AddScoped<CompanyMappingService>(serviceProvider =>
            {
                return new CompanyMappingService(
                    new List<int> { 2 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IPropertyService>()
                    );
            });
            services.AddScoped<ClassMappingService>(serviceProvider =>
            {
                return new ClassMappingService(
                    new List<int> { 37 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IPropertyService>()
                    );
            });
            services.AddScoped<DataOwnerMappingService>(serviceProvider =>
            {
                return new DataOwnerMappingService(
                    new List<int> { 1 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IPropertyService>()
                    );
            });
            services.AddScoped<SchoolMappingService>(serviceProvider =>
            {
                return new SchoolMappingService(
                    new List<int> { 36 },
                    serviceProvider.GetService<IDepartmentTypeRepository>(),
                    serviceProvider.GetService<IPropertyService>(),
                    serviceProvider.GetService<ILanguageRepository>(),
                    serviceProvider.GetService<IAdvancedWorkContext>()
                    );
            });
            services.AddScoped<Func<ArchetypeEnum, IDepartmentMappingService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case ArchetypeEnum.School:
                        return serviceProvider.GetService<SchoolMappingService>();
                    case ArchetypeEnum.Class:
                        return serviceProvider.GetService<ClassMappingService>();
                    case ArchetypeEnum.SchoolOwner:
                        return serviceProvider.GetService<SchoolOwnerMappingService>();
                    case ArchetypeEnum.OrganizationalUnit:
                        return serviceProvider.GetService<OrganizationalUnitMappingService>();
                    case ArchetypeEnum.Country:
                        return serviceProvider.GetService<CountryMappingService>();
                    case ArchetypeEnum.CandidateDepartment:
                        return serviceProvider.GetService<CandidateDepartmentMappingService>();
                    case ArchetypeEnum.Company:
                        return serviceProvider.GetService<CompanyMappingService>();
                    case ArchetypeEnum.DataOwner:
                        return serviceProvider.GetService<DataOwnerMappingService>();
                    default:
                        return serviceProvider.GetService<DepartmentMappingService>(); // or maybe return null, up to you
                }
            });
        }
        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentTypeRepository, DepartmentTypeRepository>();
            services.AddScoped<IHierarchyDepartmentRepository, HierarchyDepartmentRepository>();
            services.AddScoped<IHierarchyRepository, HierarchyRepository>();
            services.AddScoped<IDTDEntityRepository, DTDEntityRepository>();

            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILoginServiceRepository, LoginServiceRepository>();
            services.AddScoped<ILoginServiceUserRepository, LoginServiceUserRepository>();
            services.AddScoped<IObjectMappingRepository, ObjectMappingRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();


            services.AddScoped<IUGMemberRepository, UGMemberRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IUserGroupTypeRepository, UserGroupTypeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTypeRepository, UserTypeRepository>();

            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IPropOptionRepository, PropOptionRepository>();
            services.AddScoped<IPropPageRepository, PropPageRepository>();
            services.AddScoped<IPropValueRepository, PropValueRepository>();
        }
    }
}

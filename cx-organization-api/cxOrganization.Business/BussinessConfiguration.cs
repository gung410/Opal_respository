using cxOrganization.Adapter.Assessment;
using cxOrganization.Business.CandidateList;
using cxOrganization.Business.Connection;
using cxOrganization.Business.CQRSClientServices;
using cxOrganization.Business.DeactivateOrganization.DeactivateDepartment;
using cxOrganization.Business.DeactivateOrganization.DeactivateUser;
using cxOrganization.Business.MoveOrganization.MoveDepartment;
using cxOrganization.Business.Notification;
using cxOrganization.Business.PDPlanner;
using cxOrganization.Business.PDPlanner.EmployeeList;
using cxOrganization.Domain.Dtos.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cxOrganization.Business
{
    public static class BussinessConfiguration
    {
        public static void UseBussinessConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeactivateDepartmentConfig>(configuration.GetSection("BusinessConfigs:DeactivateDepartmentConfig"));
            services.Configure<DeactivateUserConfig>(configuration.GetSection("BusinessConfigs:DeactivateUserConfig"));
            services.Configure<MoveDepartmentConfig>(configuration.GetSection("BusinessConfigs:MoveDepartmentConfig"));
            services.Configure<CandidateListConfig>(configuration.GetSection("BusinessConfigs:CandidateListConfig"));
            services.Configure<ConnectionConfig>(configuration.GetSection("BusinessConfigs:ConnectionConfig"));
            services.Configure<UserNotification>(configuration.GetSection("BusinessConfigs:UserNotification"));
            services.Configure<LearningNeedsAnalysisConfig>(configuration.GetSection("BusinessConfigs:LearningNeedsAnalysisConfig"));

            services.AddScoped<ICandidateListService, CandidateListService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IMoveAssesmentClientService, MoveAssesmentClientService>();

            services.AddScoped<IDeactivateDepartmentService, DeactivateDepartmentService>();
            services.AddScoped<IDeactivateUserService<CandidateDto>, DeactivateUserService<CandidateDto>>();
            services.AddScoped<IDeactivateUserService<EmployeeDto>, DeactivateUserService<EmployeeDto>>();
            services.AddScoped<IDeactivateUserService<LearnerDto>, DeactivateUserService<LearnerDto>>();
            services.AddScoped<IMoveDepartmentService, MoveDepartmentService>();

            services.AddScoped<IAssessmentAdapter, AssessmentAdapter>();
            services.AddScoped<ILearningNeedsAnalysisService, LearningNeedsAnalysisService>();
            services.AddScoped<IEmployeeListService, EmployeeListService>();

        }
    }
}

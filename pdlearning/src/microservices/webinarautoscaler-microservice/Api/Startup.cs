using Microservice.WebinarAutoscaler.Common.Extensions;
using Microservice.WebinarAutoscaler.Configuration;
using Microservice.WebinarAutoscaler.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.AspNetCore;
using Thunder.Platform.AspNetCore.Cors;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Service.Authentication;

namespace Microservice.WebinarAutoscaler
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - Webinar Autoscaler API";

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.Filters.Add<AuthorizationFilter>();
                })
                .AddControllersAsServices()
                .AddThunderJsonOptions()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .AddThunderCors()
                .AddThunderSwagger(ApiName, "v1")
                .AddAwsClient(_configuration);

            services
                .Configure<MvcOptions>(mvcOptions =>
                {
                    mvcOptions.AddThunderMvcOptions();
                })
                .AddOptions<AWSOptions>().Bind(_configuration.GetSection(nameof(AWSOptions)));
        }

        public void Configure(
            IApplicationBuilder app,
            IDbContextResolver dbContextResolver,
            IConnectionStringResolver connectionStringResolver)
        {
            dbContextResolver.InitDatabase<WebinarAutoscalerDbContext>(
                connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs()));

            app.UseThunderExceptionHandler();
            app.UserThunderRequestIdGenerator();
            app.UserThunderSecurityHeaders();

            app.UseMiddleware<ThunderAuthenticationMiddleware>();
            app.UseRouting();
            app.UseThunderCors();
            app.UseThunderUnitOfWork();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseThunderSwagger(ApiName, "v1");
        }
    }
}

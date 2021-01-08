using System;
using System.IO;
using Microservice.WebinarProxy.Configurations;
using Microservice.WebinarProxy.Infrastructure.Extensions;
using Microservice.WebinarProxy.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Thunder.Platform.Caching.Redis;
using ProxyOptions = Microservice.WebinarProxy.Configurations.ProxyOptions;

namespace Microservice.WebinarProxy
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDataProtection()
                .SetApplicationName($"{_hostingEnvironment.EnvironmentName}-webinarproxy-app")
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "DataProtectionKey")))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(36524));

            services
                .AddOptions<ProxyOptions>().Bind(_configuration.GetSection(nameof(ProxyOptions)))
                .Services
                .AddOptions<BigBlueButtonOptions>().Bind(_configuration.GetSection(nameof(BigBlueButtonOptions)))
                .Services
                .AddOptions<AuthenticationOptions>().Bind(_configuration.GetSection(nameof(AuthenticationOptions)))
                .Services
                .ConfigureOidc(_configuration)
                .AddHealthChecks()
                .Services
                .AddOptions<ThunderRedisCacheOptions>().Bind(_configuration.GetSection(nameof(ThunderRedisCacheOptions)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMiddleware<ProxyAuthenticationMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();
            app.UseMiddleware<JoinWebinarProcessMiddleware>();
        }
    }
}

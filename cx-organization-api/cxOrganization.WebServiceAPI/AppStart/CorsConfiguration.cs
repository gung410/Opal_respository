using cxPlatform.Core.Extentions.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace cxOrganization.WebServiceAPI.AppStart
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(
                "DevelopmentCorsPolicy",
                builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders(HeaderKeys.RequestId)));

            services.AddCors(options => options.AddPolicy(
                "CorsPolicy",
                builder =>
                    builder.WithOrigins()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders(HeaderKeys.RequestId)));

            return services;
        }

        public static IApplicationBuilder UseCors(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null)
            {
                throw new ArgumentException(string.Format("{0} is null", typeof(IApplicationBuilder)));
            }

            var corsPolicyName = env.IsDevelopment() ? "DevelopmentCorsPolicy" : "CorsPolicy";
            app.UseCors(corsPolicyName);

            return app;
        }
    }
}

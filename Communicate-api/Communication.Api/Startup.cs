using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

using Communication.Api.Exceptions;
using Communication.Api.Extensions;
using Communication.Api.Hangfire;
using Communication.Business.HttpClients;
using Communication.Business.MailLog;
using Communication.Business.Models.Email;
using Communication.Business.Models.FirebaseCloudMessage;
using Communication.Business.Services;
using Communication.Business.Services.Email;
using Communication.Business.Services.FirebaseCloudMessage;
using Communication.Business.Services.Mapping;
using Communication.Business.Services.NotificationSetting;
using Communication.Business.Services.Template;
using Communication.DataAccess;
using Communication.DataAccess.Notification;
using cx.datahub.scheduling.jobs.shared;
using Datahub.Processor.Base;
using Datahub.Processor.Base.ProcessorRegister;
using Datahub.Processor.Base.Settings;
using HandlebarsDotNet;
using Hangfire;
using Hangfire.SqlServer;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MongoDB.Bson.Serialization.Conventions;

using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace Communication.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            services.Configure<EmailConfig>(Configuration.GetSection("Email"));
            services.Configure<FireBaseConfig>(Configuration.GetSection("FireBaseCloudMessage"));
            services.AddScoped<IEmailSmtpService, EmailSmtpService>();
            services.AddScoped<IMappingService, MappingService>();
            services.AddHttpClient<IFirebaseCloudMessageHttpClient, FirebaseCloudMessageHttpClient>();
            services.AddHttpClient<IGoogleAppInstanceServerHttpClient, GoogleAppInstanceServerHttpClient>();
            services.AddHttpClient<IIdentityServerClientService, IdmClientService>();
            services.AddHttpClient<IOrganizationClientService, OrganizationClientService>();
            services.RegisterDataAccess(Configuration);
            services.AddScoped<ICommunicationTemplateService, CommunicationTemplateService>();
            services.AddScoped<ICommunicationService, EmailService>();
            services.AddScoped<ICommunicationService, FcmPushNotificationService>();
            services.AddScoped<IUserNotificationSettingService, UserNotificationSettingService>();
            services.AddScoped<INotificationPullHistoryRepository, NotificationPullHistoryRepository>();
            services.AddSingleton<IMailLogger, MailLogger>();
            services.AddScoped<IDigestEmailJob, DigestEmailJob>();
            services.AddProcessorCoreServices(Configuration, Assembly.GetExecutingAssembly());
            services.AddRabbitMQ(Configuration);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Bearer";
            })
            .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            {
                var authority = Configuration["IDM_BASEURL"];
                var internalURL = Configuration["IDM_INTERNAL_URL"];
                if (string.IsNullOrEmpty(internalURL))
                    internalURL = authority;
                options.Authority = internalURL;
                options.IntrospectionDiscoveryPolicy = new DiscoveryPolicy
                {
                    Authority = internalURL,
                    ValidateIssuerName = false
                };
                options.RequireHttpsMetadata = false;
                options.ApiName = Configuration["Idm_ApiName"];
                options.ApiSecret = Configuration["Idm_ApiSecret"];
                options.SupportedTokens = SupportedTokens.Both;
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerDocument(settings =>
            {
                settings.Description = "Datahub.Communication.Api is a web service api that support for communication actions.";
                settings.Title = "Datahub Communication API";
                // Add operation security scope processor
                settings.DocumentName = "Datahub Communication Api";
                // Post process the generated document
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
                settings.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Description = "Communication Api Authentication",
                    Flows = new OpenApiOAuthFlows()
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = $"https://idm.{environmentName}.opal2.conexus.net",
                            TokenUrl = $"https://idm.{environmentName}.opal2.conexus.net/connect/token",
                            Scopes = new Dictionary<string, string>
                            {
                                { "userManagement", "Read access to protected resources" },
                                { "cxDomainInternalApi", "Write access to protected resources" }
                            }
                        }

                    }
                });

                settings.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
            });

            //Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"),
                    new SqlServerStorageOptions()));
            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "communication_api" };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IRecurringJobManager recurringJobManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }



            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new JsonExceptionMiddleware(env, loggerFactory).Invoke
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseSwaggerUi3(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "cxDomainInternalApi",
                    ClientSecret = "AiOiJKV1QiLCJ4NXQiOiJhM",
                };
            });
            QueueSetting queueConfiguration = new QueueSetting()
            {
                QueueName = Environment.GetEnvironmentVariable("QUEUE_NAME"),
                Bindings = new List<BindingInfo>
                {
                    new BindingInfo
                    {
                        ExchangeName = Environment.GetEnvironmentVariable("COMMAND_EXCHANGE_NAME"),
                        RoutingKeys = new string[]
                        {
                            "*.communication.send.message.success", "communication_api.communication.send.notification_by_date.requested"
                        }
                    },
                    new BindingInfo
                    {
                        ExchangeName = Environment.GetEnvironmentVariable("EVENT_EXCHANGE_NAME"),
                        RoutingKeys = new string[]
                        {
                            "*.communication.register.firebase.success", "*.communication.logout.firebase.requested", "*.communication.mark.notification.read",
                            "*.communication.cancel.notification.cancelled"
                        }
                    }
                }
            };
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.UseHangfireDashboard();
            app.UseRabbitMQListener(queueConfiguration, loggerFactory.CreateLogger<Startup>());
            app.UseRecurringJobs(recurringJobManager);
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        
    }
}

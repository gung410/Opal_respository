using Amazon;
using Amazon.S3;
using Backend.CrossCutting.HttpClientHelper;
using cxOrganization.Adapter;
using cxOrganization.Business;
using cxOrganization.Domain;
using cxOrganization.Domain.Dtos;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.ActionFilters;
using cxOrganization.WebServiceAPI.Background;
using cxOrganization.WebServiceAPI.DbMigration;
using cxOrganization.WebServiceAPI.Middlewares;
using cxOrganization.WebServiceAPI.Processor;
using cxPlatform.Core;
using cxPlatform.Core.Cache;
using cxPlatform.Core.DatahubLog;
using cxPlatform.Core.RabbitMQ;
using Datahub.Processor.Base;
using Datahub.Processor.Base.ProcessorRegister;
using Datahub.Processor.Base.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace cxOrganization.WebServiceAPI
{
    public class Startup
    {
        Microsoft.Extensions.Logging.ILogger _logger;
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        readonly string AllowSpecificOrigins = "_allowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(cxTokenFilter));
                options.Filters.Add(typeof(LanguageFilter));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                //Return as UTC DateTime Format
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContextPool<OrganizationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sql => sql.MigrationsAssembly(migrationsAssembly));
                //if (bool.TryParse(Configuration["NOLOCK_ENABLE"], out var enableNolock) && enableNolock)
                //    options.UseCustomSqlServerQuerySqlGenerator();
            });

            AppSettings.ProjectName = Configuration["PROJECT_NAME"];
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<EventDomainLogSettings>(Configuration.GetSection("EventDomainLogSettings"));
            services.Configure<DatahubLogSettings>(Configuration.GetSection("DatahubLog"));
            services.Configure<List<MappingDto>>(Configuration.GetSection("IdmRoleMapping"));
            services.Configure<HierarchyDepartmentPermissionSettings>(Configuration.GetSection("HierarchyDepartmentPermissionSettings"));
            services.Configure<EmailTemplates>(Configuration.GetSection("EmailTemplates"));
            services.Configure<EntityStatusReasonTexts>(Configuration.GetSection("EntityStatusReasonTexts"));
            services.Configure<AccessSettings>(Configuration.GetSection("accessSettings"));
            services.Configure<DataHubQueryAPISettings>(Configuration.GetSection("DataHubQueryAPISettings"));  
            services.Configure<LearningCatalogAPISettings>(Configuration.GetSection("LearningCatalogAPISettings"));
            services.Configure<AwsSettings>(Configuration.GetSection("AwsSettings"));
            services.UseDomainService(Configuration);
            services.UseAdapterConfiguration(Configuration);
            services.UseBussinessConfiguration(Configuration);
            services.UseCache(Configuration);
            services.AddScoped<IWorkContext, WorkContext>();
            services.UseHttpClientHelperConfiguration();
            services.AddHttpContextAccessor();
            services.AddRabbitMQ(Configuration);
            services.UseAuthenticationConfig(Configuration);
            services.UseSwaggerConfig();
            services.AddProcessorCoreServices(Configuration, Assembly.GetExecutingAssembly());
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            var changeUserStatusSettings = Configuration.GetSection("ChangeUserStatusSettings").Get<ChangeUserStatusSettings>();

            services.AddChangeUserStatusSetting(changeUserStatusSettings);
            services.UseRecurringJobs(Configuration, _logger);

            UseAmazonService(services);

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseDatabaseInitializer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(AllowSpecificOrigins);
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerConfig();

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestContextMiddleware>()
               .UseMiddleware<ErrorHandlingMiddleware>();


            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            QueueSetting queueConfiguration = new QueueSetting()
            {
                QueueName = Environment.GetEnvironmentVariable("QUEUE_NAME"),
                Bindings = new List<BindingInfo>
                {
                    new BindingInfo
                    {
                        ExchangeName = Configuration["EVENT_EXCHANGE_NAME"],
                        RoutingKeys = new string[]
                        {
                            SyncIdpUserLockedInfoEventHandler.AcceptedAction,
                            SyncIdpUserLoginInfoEventHandler.AcceptedAction
                        }
                    }
                }
            };
            app.UseRabbitMQListener(queueConfiguration, loggerFactory.CreateLogger<Startup>());
        }
        private void UseAmazonService(IServiceCollection services)
        {
            services.AddSingleton<IAmazonS3>(s =>
            {
                var awsConfiguration = s.GetService<IOptions<AwsSettings>>().Value;
                return new AmazonS3Client(awsConfiguration.AccessKey, awsConfiguration.SecretKey,
                    RegionEndpoint.GetBySystemName(awsConfiguration.Region));
            });
        }

    }
}

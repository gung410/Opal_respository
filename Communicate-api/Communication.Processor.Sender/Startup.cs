using System.Collections.Generic;
using System.Reflection;
using Communication.Processor.Sender.Exceptions;
using Communication.Processor.Sender.Extensions;
using Communication.Business.HttpClients;
using Communication.Business.Models.Email;
using Communication.Business.Models.FirebaseCloudMessage;
using Communication.Business.Services;
using Communication.Business.Services.Email;
using Communication.Business.Services.FirebaseCloudMessage;
using Communication.Business.Services.Mapping;
using Communication.DataAccess.Notification;
using Datahub.Processor.Base;
using Datahub.Processor.Base.ProcessorRegister;
using Datahub.Processor.Base.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using NSwag;
using NSwag.AspNetCore;
using Serilog;
using System;
using HandlebarsDotNet;
using Microsoft.Extensions.Hosting;
using Communication.Business.MailLog;
using Communication.DataAccess;

namespace Communication.Processor.Sender
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
            services.Configure<EmailConfig>(Configuration.GetSection("Email"));
            services.Configure<FireBaseConfig>(Configuration.GetSection("FireBaseCloudMessage"));
            services.Configure<MailLogConfiguration>(Configuration.GetSection("MailLogConfiguration"));
            services.AddScoped<IEmailSmtpService, EmailSmtpService>();
            services.AddScoped<IMappingService, MappingService>();
            services.AddHttpClient<IFirebaseCloudMessageHttpClient, FirebaseCloudMessageHttpClient>();
            services.AddHttpClient<IGoogleAppInstanceServerHttpClient, GoogleAppInstanceServerHttpClient>();
            services.AddHttpClient<IIdentityServerClientService, IdmClientService>();
            services.AddHttpClient<IOrganizationClientService, OrganizationClientService>();
            services.RegisterDataAccess(Configuration, false);
            services.AddScoped<ICommunicationService, EmailService>();
            services.AddScoped<ICommunicationService, FcmPushNotificationService>();
            services.AddSingleton<IMailLogger, MailLogger>();
            services.AddProcessorCoreServices(Configuration, Assembly.GetExecutingAssembly());
            services.AddControllers();
            services.AddRabbitMQ(Configuration);
            services.AddSwaggerDocument(settings =>
            {
                settings.Description =
                    "Datahub.Communication.Processor.Sender is a web service api that support for communication sending message.";
                settings.Title = "Datahub.Communication.Processor.Sender";
                // Add operation security scope processor
                settings.DocumentName = "Datahub.Communication.Processor.Sender";
                // Post process the generated document
            });
            Handlebars.RegisterHelper("add", (writer, context, parameters) =>
            {
                writer.WriteSafeString(int.Parse(parameters[0].ToString()) + int.Parse((parameters[1]).ToString()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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
            app.UseOpenApi();
            app.UseSwaggerUi3();
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
                            "communication_api.communication.send.notification.requested",
                            "communication_api.communication.send.email.requested"
                        }
                    }
                }
            };
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseRabbitMQListener(queueConfiguration, loggerFactory.CreateLogger<Startup>());
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
using System;
using Datahub.Queue.Manager.Data.MongoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using Swashbuckle.AspNetCore.Swagger;

namespace Datahub.Queue.Manager
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddApplicationServices(Configuration, _logger);
            services.Configure<MongoDbSettings>(options =>
            {
                var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING");
                _logger.LogInformation($"MONGO_CONNECTIONSTRING: {connectionString}");
                options.ConnectionString = connectionString;
                var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASENAME");
                _logger.LogInformation($"MONGO_DATABASENAME: {databaseName}");
                options.DatabaseName = databaseName;
                bool.TryParse(Environment.GetEnvironmentVariable("MONGO_SSL_ENABLED"), out var sslEnabled);
                options.SslEnabled = sslEnabled;
            });
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Queue Manager", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseMvc();
            app.UseRabbitMQ(loggerFactory.CreateLogger("RabbitMQ"));
            
        }
    }
}

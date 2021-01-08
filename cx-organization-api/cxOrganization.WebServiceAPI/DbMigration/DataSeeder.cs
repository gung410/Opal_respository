using cxOrganization.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace cxOrganization.WebServiceAPI.DbMigration
{
    public static class DataSeeder
    {
        public static void UseDatabaseInitializer(this IApplicationBuilder app)
        {

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var serviceProvider = serviceScope.ServiceProvider;
                if (bool.Parse(configuration["AppSettings:EnableMigration"]))
                {
                    var applicationDbcontext = serviceProvider.GetRequiredService<OrganizationDbContext>();
                    applicationDbcontext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
                    applicationDbcontext.Database.Migrate();
                    applicationDbcontext.Database.SetCommandTimeout(null);
                }
                var enableSeedDummyData = bool.Parse(configuration["AppSettings:EnableSeedDummyData"]);
                if (enableSeedDummyData)
                {
                    var projectName = configuration["PROJECT_NAME"] ?? configuration["ProjectName"];

                    IDataInitializer dataInitializer = serviceProvider.GetServices<IDataInitializer>().FindServiceByName(projectName);
                    if (dataInitializer != null)
                    {
                        dataInitializer.Run();
                    }
                }


            }
        }
    }
}

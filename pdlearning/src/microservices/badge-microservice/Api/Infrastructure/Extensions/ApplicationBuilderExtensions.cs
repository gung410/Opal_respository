using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Badge.Infrastructure.Extensions
{
    public static class MongoDbApplicationBuilderExtensions
    {
        public static void UseSeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var badgeSeeder = scope.ServiceProvider.GetRequiredService<BadgeSeeder>();
                badgeSeeder.Seed();
            }
        }
    }
}

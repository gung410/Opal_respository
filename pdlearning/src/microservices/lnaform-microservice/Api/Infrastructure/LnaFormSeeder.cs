using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.LnaForm.Infrastructure
{
    public class LnaFormSeeder : IDbContextSeeder
    {
        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
        }
    }
}

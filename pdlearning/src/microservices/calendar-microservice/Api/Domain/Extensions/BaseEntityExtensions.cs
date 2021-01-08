using System;
using System.Linq;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class BaseEntityExtensions
    {
        public static IQueryable<T> HasId<T>(this IQueryable<T> query, Guid id) where T : Entity
        {
            return query.Where(e => e.Id == id);
        }
    }
}

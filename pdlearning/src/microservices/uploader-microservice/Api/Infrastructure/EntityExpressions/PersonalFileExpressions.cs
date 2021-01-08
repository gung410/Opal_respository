using System;
using System.Linq.Expressions;
using Microservice.Uploader.Domain.Entities;

namespace Microservice.Uploader.Infrastructure
{
    public class PersonalFileExpressions
    {
        public static Expression<Func<PersonalFile, bool>> HasPermissionToSeePersonalFile(Guid userId)
        {
            return dc => dc.UserId == userId;
        }
    }
}

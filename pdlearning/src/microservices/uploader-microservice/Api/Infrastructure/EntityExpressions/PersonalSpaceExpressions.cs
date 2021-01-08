using System;
using System.Linq.Expressions;
using Microservice.Uploader.Domain.Entities;

namespace Microservice.Uploader.Infrastructure
{
    public class PersonalSpaceExpressions
    {
        public static Expression<Func<PersonalSpace, bool>> HasPermissionToSeePersonalSpace(Guid userId)
        {
            return dc => dc.UserId == userId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Course.Application.DomainExtensions
{
    public static class BlockoutDateExtensions
    {
        public static Expression<Func<BlockoutDate, bool>> IsMatchServiceSchemesQueryExpr(IEnumerable<string> serviceSchemeIds)
        {
            Expression<Func<BlockoutDate, bool>> initialExpr = x => true;
            return serviceSchemeIds.Aggregate(initialExpr, (expr, serviceSchemeId) => expr.Or(x =>
                EF.Functions.Contains(x.ServiceSchemesFullTextSearch, serviceSchemeId.ToString())));
        }
    }
}

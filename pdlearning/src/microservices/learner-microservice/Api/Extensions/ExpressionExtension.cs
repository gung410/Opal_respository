using System;
using System.Linq.Expressions;

namespace Microservice.Learner.Extensions
{
    internal static class ExpressionExtension
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> firstExpr,
            Expression<Func<T, bool>> secondExpr)
        {
            var invokedExpr =
                Expression.Invoke(secondExpr, firstExpr.Parameters);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(firstExpr.Body, invokedExpr),
                firstExpr.Parameters);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> firstExpr,
            Expression<Func<T, bool>> secondExpr)
        {
            var invokedExpr =
                Expression.Invoke(secondExpr, firstExpr.Parameters);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(firstExpr.Body, invokedExpr),
                firstExpr.Parameters);
        }

        public static Expression<Func<T, bool>> Not<T>(
            this Expression<Func<T, bool>> one)
        {
            var candidateExpr = one.Parameters[0];
            var body = Expression.Not(one.Body);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }
    }
}

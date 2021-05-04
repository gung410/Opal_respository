using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace cxOrganization.Domain.Extensions
{
    public static class QueryExtension
    {
        public static Expression<Func<T, object>>[] CreateIncludeProperties<T>(params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            return includeProperties;
        }
        public static List<Expression<Func<T, object>>> CreateEmptyIncludeProperties<T>() where T : class
        {
            return new List<Expression<Func<T, object>>>();
        }

        public static IQueryable<TEntity> WhereContainsBy<TEntity, TProperty>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> propertyExpression, ICollection<TProperty> collection) where TEntity : class
        {
            return source.ApplyContainsExpression(propertyExpression, collection, ContainsMode.In);
        }

        public static IQueryable<TEntity> WhereNotContainsBy<TEntity, TProperty>(this IQueryable<TEntity> source,
           Expression<Func<TEntity, TProperty>> propertyExpression, ICollection<TProperty> collection) where TEntity : class
        {
            return source.ApplyContainsExpression(propertyExpression, collection, ContainsMode.NotIn);
        }

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }

    enum ContainsMode
    {
        In,
        NotIn
    }

    static class ContainsExpressionBuilder
    {
        public static IQueryable<TEntity> ApplyContainsExpression<TEntity, TProperty>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> propertyExpression, ICollection<TProperty> collection, ContainsMode containsMode, int cachableLevel = 20)
            where TEntity : class
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (!collection.Any())
                return source;

            Expression<Func<TEntity, bool>> expression = BuildContainsExpression(propertyExpression, collection, containsMode, cachableLevel);
            return source.Where(expression);
        }

        private static Expression<Func<TEntity, bool>> BuildContainsExpression<TEntity, TProperty>(
           Expression<Func<TEntity, TProperty>> propertyExpression, ICollection<TProperty> collection, ContainsMode containsMode, int cachableLevel = 20)
           where TEntity : class
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            if (collection == null)
                throw new ArgumentNullException("collection");

            if (!(propertyExpression.Body is MemberExpression))
                throw new ArgumentException(
                    "Property expression should be correct and type of property and collection should be equals. When property types is nullable, collection type also shoud be nullable.",
                    "propertyExpression");

            ParameterExpression parameterExpression = propertyExpression.Parameters[0];
            var memberExpression = (MemberExpression)propertyExpression.Body;

            if (collection.Count > cachableLevel)
            {
                return containsMode == ContainsMode.In
                    ? CreateContainsExpression<TEntity, TProperty>(parameterExpression, memberExpression, collection)
                    : CreateNotContainsExpression<TEntity, TProperty>(parameterExpression, memberExpression, collection);
            }

            return containsMode == ContainsMode.In
                ? CreateOrExpression<TEntity, TProperty>(parameterExpression, memberExpression, collection)
                : CreateAndExpression<TEntity, TProperty>(parameterExpression, memberExpression, collection);
        }

        public static Expression<Func<TEntity, IEnumerable<TItem>>> BuildWhereContainsExpression<TEntity, TItem, TProperty>(
            Expression<Func<TEntity, IEnumerable<TItem>>> collectionAccessorExpression,
            Expression<Func<TItem, TProperty>> propertyExpression,
            ICollection<TProperty> collection,
            ContainsMode containsMode,
            int cachableLevel = 20)
            where TItem : class
        {
            if (collectionAccessorExpression == null)
                throw new ArgumentNullException("collectionAccessorExpression");

            // x.values
            Expression collectionAccessor = collectionAccessorExpression.Body;

            // y => y.property == value op y.property == value2 ...
            Expression<Func<TItem, bool>> innerExpression = BuildContainsExpression(propertyExpression, collection, containsMode, cachableLevel);

            // x.values.Where(y => y.property == value op y.property == value2 ..)
            MethodCallExpression memberCall = Expression.Call(null, GetGenericMethodInfo<TItem>("Where", 2), collectionAccessor, innerExpression);

            // x => x.values.Where(y => y.property == value op y.property == value2 ..)
            return Expression.Lambda<Func<TEntity, IEnumerable<TItem>>>(memberCall, collectionAccessorExpression.Parameters);
        }

        private static Expression<Func<TEntity, bool>> CreateOrExpression<TEntity, TProperty>(ParameterExpression parameterExpression, MemberExpression memberExpression, ICollection<TProperty> collection)
        {
            Expression orExpression = null;

            foreach (TProperty item in collection)
            {
                Expression<Func<TProperty>> idLambda = () => item;

                BinaryExpression equalExpression = Expression.Equal(memberExpression, idLambda.Body);

                orExpression = orExpression != null ? Expression.OrElse(orExpression, equalExpression) : equalExpression;
            }

            return Expression.Lambda<Func<TEntity, bool>>(orExpression, parameterExpression);
        }

        private static Expression<Func<TEntity, bool>> CreateAndExpression<TEntity, TProperty>(ParameterExpression parameterExpression, MemberExpression memberExpression, ICollection<TProperty> collection)
        {
            Expression andExpression = null;

            foreach (TProperty item in collection)
            {
                Expression<Func<TProperty>> idLambda = () => item;

                BinaryExpression notEqualExpression = Expression.NotEqual(memberExpression, idLambda.Body);

                andExpression = andExpression != null ? Expression.And(andExpression, notEqualExpression) : notEqualExpression;
            }

            return Expression.Lambda<Func<TEntity, bool>>(andExpression, parameterExpression);
        }

        private static Expression<Func<TEntity, bool>> CreateContainsExpression<TEntity, TProperty>(
            ParameterExpression parameterExpression, MemberExpression memberExpression, ICollection<TProperty> collection)
        {
            ConstantExpression constantExpression = Expression.Constant(collection);
            MethodInfo containsMethodInfo = typeof(ICollection<TProperty>).GetMethod(("Contains"));

            MethodCallExpression callExpression = Expression.Call(constantExpression, containsMethodInfo, memberExpression);

            return Expression.Lambda<Func<TEntity, bool>>(callExpression, parameterExpression);
        }

        private static Expression<Func<TEntity, bool>> CreateNotContainsExpression<TEntity, TProperty>(
          ParameterExpression parameterExpression, MemberExpression memberExpression, ICollection<TProperty> collection)
        {
            ConstantExpression constantExpression = Expression.Constant(collection);
            MethodInfo containsMethodInfo = typeof(ICollection<TProperty>).GetMethod("Contains");

            MethodCallExpression callExpression = Expression.Call(constantExpression, containsMethodInfo, memberExpression);
            Expression notContainsExpression = Expression.Not(callExpression);

            return Expression.Lambda<Func<TEntity, bool>>(notContainsExpression, parameterExpression);
        }

        private static MethodInfo GetGenericMethodInfo<TEntity>(string methodName, int paramNumber)
        {
            var methods = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var method = methods.First(m => m.Name == methodName && m.GetParameters().Count() == paramNumber);
            return method.MakeGenericMethod(typeof(TEntity));
        }


        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");
        private static readonly FieldInfo QueryModelGeneratorField = typeof(QueryCompiler).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryModelGenerator");
        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");
        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Microsoft.EntityFrameworkCore.Storage.Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        //public static string ToSql<TEntity>(this IQueryable<TEntity> query)
        //{
        //    var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
        //    var queryModelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
        //    var queryModel = queryModelGenerator.ParseQuery(query.Expression);
        //    var database = DataBaseField.GetValue(queryCompiler);
        //    var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
        //    var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
        //    var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
        //    modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
        //    var sql = modelVisitor.Queries.First().ToString();

        //    return sql;
        //}

    }
}


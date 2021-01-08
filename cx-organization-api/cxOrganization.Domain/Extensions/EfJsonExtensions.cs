using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cxOrganization.Domain.Extensions
{
    //https://github.com/aspnet/EntityFrameworkCore/issues/11295
    public static class EfJsonExtensions
    {

        public static string JsonValue(string column, [NotParameterized] string path)
        {
            throw new NotSupportedException();
        }

        public static void AddJsonValue(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(EfJsonExtensions).GetMethod(nameof(JsonValue)))
                .HasName("JSON_VALUE")
                .HasSchema("")
                .HasTranslation(args => SqlFunctionExpression.Create("JSON_VALUE", args, typeof(string), null)); ;
        }
    }
    public static class EfJsonQueryExtensions
    {

        public static string JsonQuery(string column, [NotParameterized] string path)
        {
            throw new NotSupportedException();
        }

        public static void AddJsonQuery(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(EfJsonQueryExtensions).GetMethod(nameof(JsonQuery)))
                .HasName("JSON_QUERY")
                .HasSchema("")
                .HasTranslation(args => SqlFunctionExpression.Create("JSON_QUERY", args, typeof(string), null)); ; ;
        }
    }

}

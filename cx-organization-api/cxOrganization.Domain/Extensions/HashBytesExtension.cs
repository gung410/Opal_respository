using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cxOrganization.Domain.Extensions
{
    //https://github.com/aspnet/EntityFrameworkCore/issues/11295
    public static class ConvertHashBytesExtension
    {
        public static string Value(string hashValue)
            => throw new InvalidOperationException($"{nameof(Value)}cannot be called client side");
        public static bool ValueIn(string hashValues, string hashValue)
            => throw new InvalidOperationException($"{nameof(Value)}cannot be called client side");

        public static void AddConvertHashBytes(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(ConvertHashBytesExtension).GetMethod(nameof(Value)))
                .HasName("CUSTOM_CONVERT")
                .HasSchema("")
                .HasTranslation(args =>
                {
                    var paramValue = args.First() as SqlConstantExpression;
                    var sqlFrag = $"NVARCHAR(64),HashBytes('SHA2_256', '{paramValue.Value}' ),2";
                    return SqlFunctionExpression.Create("CONVERT", new[] { 
                        new SqlFragmentExpression(sqlFrag)}, typeof(string), null);
                });
        }

        public static void AddConvertCollectionHashBytes(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(ConvertHashBytesExtension).GetMethod(nameof(ValueIn)))
                .HasName("CUSTOM_CONVERT_2")
                .HasSchema("")
                .HasTranslation(args =>
                {
                    var paramValue = args.First() as SqlConstantExpression;
                    var sqlFrag = $"select CONVERT(VARCHAR(64),HashBytes('SHA2_256', value + u.FirstName ),2) from string_split('1,2,3', ','";
                    return SqlFunctionExpression.Create("IN", new[] {
                        new SqlFragmentExpression(sqlFrag)}, typeof(bool), null);
                });
        }
    }


}

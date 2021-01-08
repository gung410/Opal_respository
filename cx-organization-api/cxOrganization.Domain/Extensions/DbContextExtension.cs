using cxPlatform.Core.EFUtil;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Extensions
{
    public static class DbContextExtension
    {
        public static async Task<List<T>> ExecSQLAsync<T>(
            this DbContext context,
            string query,
            ILogger logger,
            params object[] sqlParams)
        {
            using (DbCommand command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                if (sqlParams != null)
                    command.Parameters.AddRange((Array) sqlParams);
                context.Database.OpenConnection();
                using (DbDataReader dbDataReader = await command.ExecuteReaderAsync())
                {
                    List<T> objList = new List<T>();
                    DataReaderMapper<T> dataReaderMapper = new DataReaderMapper<T>((IDataReader) dbDataReader);
                    while (dbDataReader.Read())
                        objList.Add(dataReaderMapper.MapFrom((IDataRecord) dbDataReader));
                    return objList;
                }
            }
        }

        public static async Task<List<T>> ExecStoreStoredProcedureAsync<T>(
            this DbContext context,
            string storeStoredProcedure,
            params object[] sqlParams)
        {
            var connection = context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = storeStoredProcedure;
                command.CommandType = CommandType.StoredProcedure;
                if (sqlParams != null)
                    command.Parameters.AddRange(sqlParams);
                using (DbDataReader dbDataReader = await command.ExecuteReaderAsync())
                {
                    List<T> objList = new List<T>();
                    DataReaderMapper<T> dataReaderMapper = new DataReaderMapper<T>(dbDataReader);
                    while (dbDataReader.Read())
                        objList.Add(dataReaderMapper.MapFrom(dbDataReader));
                    return objList;
                }
            }
        }
        public static async Task<T> ExecuteScalarAsync<T>(
            this DbContext context,
            string query,
            ILogger logger,
            params object[] sqlParams)
        {
            using (DbCommand command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                if (sqlParams != null)
                    command.Parameters.AddRange((Array)sqlParams);
                context.Database.OpenConnection();
                return (T)(await command.ExecuteScalarAsync());

            }
        }
    }
}

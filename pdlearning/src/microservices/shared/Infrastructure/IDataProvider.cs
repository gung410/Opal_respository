using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Conexus.Opal.Microservice.Infrastructure
{
    public interface IDataProvider
    {
        IDbConnection DbConnection { get; }

        DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType);

        SqlParameter GetParameter(string parameter, object value);

        SqlParameter GetParameterOut(
            string parameter,
            SqlDbType type,
            object value = null,
            ParameterDirection parameterDirection = ParameterDirection.InputOutput);

        int ExecuteNonQuery(
            string procedureName,
            List<DbParameter> parameters,
            CommandType commandType = CommandType.StoredProcedure);

        object ExecuteScalar(string procedureName, List<SqlParameter> parameters);

        DbDataReader GetDataReader(
            string procedureName,
            List<DbParameter> parameters,
            CommandType commandType = CommandType.StoredProcedure);
    }
}

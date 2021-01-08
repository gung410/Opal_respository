using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
namespace Conexus.Opal.Microservice.Infrastructure
{
    public abstract class BaseDataProvider : IDataProvider, IDisposable
    {
        private bool _disposed;

        protected BaseDataProvider(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public IDbConnection DbConnection { get; }

        public DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            return new SqlCommand(commandText, connection as SqlConnection) { CommandType = commandType };
        }

        public SqlParameter GetParameter(string parameter, object value)
        {
            return new SqlParameter(parameter, value ?? DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };
        }

        public SqlParameter GetParameterOut(
            string parameter,
            SqlDbType type,
            object value = null,
            ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            var parameterObject = new SqlParameter(parameter, type);

            if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
            {
                parameterObject.Size = -1;
            }

            parameterObject.Direction = parameterDirection;
            parameterObject.Value = value ?? DBNull.Value;

            return parameterObject;
        }

        public int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            var connection = GetConnection();
            DbCommand cmd = GetCommand(connection, procedureName, commandType);

            if (parameters != null && parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            return cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string procedureName, List<SqlParameter> parameters)
        {
            var connection = GetConnection();
            DbCommand cmd = GetCommand(connection, procedureName, CommandType.StoredProcedure);

            if (parameters != null && parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            return cmd.ExecuteScalar();
        }

        public DbDataReader GetDataReader(
            string procedureName,
            List<DbParameter> parameters,
            CommandType commandType = CommandType.StoredProcedure)
        {
            var connection = GetConnection();
            DbCommand cmd = GetCommand(connection, procedureName, commandType);
            if (parameters != null && parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            var ds = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return ds;
        }

        public DbDataReader ExecuteSqlReader(string sql)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            return command.ExecuteReader();
        }

        public async Task<DbDataReader> ExecuteSqlReaderAsync(string sql)
        {
            var connection = GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            return await command.ExecuteReaderAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }

            DbConnection.Dispose();
        }

        private SqlConnection GetConnection()
        {
            if (DbConnection.State != ConnectionState.Open)
            {
                DbConnection.Open();
            }

            return (SqlConnection)DbConnection;
        }
    }
}
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspnetCoreExtensions.SqlBasies
{
    public static class SqlConnectionExtensions
    {
        /// <summary>
        /// Asynchronously Create and Open new instance of SqlConnection using specified connectionString.
        /// </summary>
        /// <param name="connectionString">connection string to access database server.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>Opened instance of SqlConnection</returns>
        public static async Task<SqlConnection> OpenConnectionAsync(string connectionString, CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            return connection;
        }

        public static async Task<object> ExecuteScalerAsync<T>(this SqlConnection connection, SqlTransaction transaction, string commandText, T entity, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            List<SqlParameter> sqlParameters = CreateSqlParameters(commandText, entity);

            using var command = new SqlCommand(commandText, connection, transaction);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteScalarAsync(cancellationToken);
        }

        public static async Task<int> ExecuteNonQueryAsync<T>(this SqlConnection connection, SqlTransaction transaction, string commandText, T entity, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            List<SqlParameter> sqlParameters = CreateSqlParameters(commandText, entity);

            using var command = new SqlCommand(commandText, connection, transaction);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public static async Task<int> ExecuteNonQueryAsync<T>(string connectionString, string commandText, T entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            List<SqlParameter> sqlParameters = CreateSqlParameters(commandText, entity);

            using var connection = await OpenConnectionAsync(connectionString, cancellationToken);
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }



        private static List<SqlParameter> CreateSqlParameters<T>(string commandText, T entity)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            var regex = new Regex(@"(?<param>[:@]\w*)");
            var parameterNames = regex.Matches(commandText).Cast<Match>().Select(x => x.Groups["param"].Value);
            foreach (var parameterName in parameterNames)
            {
                var propertyInfo = PropertyInfoExtensions.GetProperty<T>(parameterName.Substring(1));
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(entity) ?? DBNull.Value;
                    sqlParameters.Add(new SqlParameter(parameterName, value));
                }
                else
                    Debug.Assert(propertyInfo == null, string.Format("Parameter[{0}] has no Property in Entity[{1}]", parameterName, typeof(T).Name));
            }

            return sqlParameters;
        }

        public static SqlParameter CreateSqlParameter(string commandText, object singleParameter)
        {
            var regex = new Regex(@"(?<param>[:@]\w*)");
            var parameterNames = regex.Matches(commandText).Cast<Match>().Select(x => x.Groups["param"].Value);
            if (parameterNames.Count() > 1)
                throw new InvalidOperationException($"Expect commandText to have SQL statement with only one parameter.\r\nSpecified Parameters[{string.Join(", ", parameterNames)}]");

            return new SqlParameter(parameterNames.First(), singleParameter ?? DBNull.Value);
        }

        public static async Task<SqlDataReader> ExecuteReaderAsync(string connectionString, string commandText, object singleParameter, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            if (singleParameter is null)
                throw new ArgumentNullException(nameof(singleParameter));

            SqlParameter sqlParameter = CreateSqlParameter(commandText, singleParameter);

            var connection = await OpenConnectionAsync(connectionString, cancellationToken);
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.Add(sqlParameter);
            return await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }
    }
}

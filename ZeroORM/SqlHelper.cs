using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroORM
{
    /// <summary>
    /// ZeroORM entry points, provide extensions for standards ExecuteNonQueryAsync, ExecuteScalerAsync and ExecuteReaderAsync, which take care of all parametrization efforts that you need to do when execute standard version of these methods.
    /// </summary>
    /// <example>
    /// var user = new { UserName = "morikapt@gmail.com", Email = "morikapt@gmail.com", PhoneNumber = "123456789" };
    /// var userInsertSql = "INSERT INTO Users (UserName, Email, PhoneNumber) VALUES (@UserName, @Email, @PhoneNumber); SELECT SCOPE_IDENTITY();";
    /// var cnnStr = "server=.;database=testing;persist security info=True;Integrated Security=SSPI";
    /// var newUserId = await SqlHelper.ExecuteScalerAsync(cnnStr, userInsertSql, user, default);
    /// </example>
    public static class SqlHelper
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

        /// <summary>
        /// Asynchronously Executes the query, and returns <see cref="Task"/> for the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/> that represents the connection to an instance of SQL Server.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> in which the <see cref="SqlCommand"/> executes.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation for The first column of the first row in the result set, or a null if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScalerAsync<T>(this SqlConnection connection, SqlTransaction transaction, string commandText, T value, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var command = new SqlCommand(commandText, connection, transaction);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteScalarAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously Executes the query, and returns <see cref="Task"/> for the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/> that represents the connection to an instance of SQL Server.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation for The first column of the first row in the result set, or a null if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScallerAsync<T>(this SqlConnection connection, string commandText, T value, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteScalarAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously Executes the query, and returns <see cref="Task"/> for the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation for The first column of the first row in the result set, or a null if the result set is empty. Returns a maximum of 2033 characters.</returns>
        public static async Task<object> ExecuteScalerAsync<T>(string connectionString, string commandText, T value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var connection = await OpenConnectionAsync(connectionString, cancellationToken);
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteScalarAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously Executes a Transact-SQL statement against the connection and returns <see cref="Task"/> for the number of rows affected.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/> that represents the connection to an instance of SQL Server.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> in which the <see cref="SqlCommand"/> executes.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation for The number of rows affected.</returns>
        public static async Task<int> ExecuteNonQueryAsync<T>(this SqlConnection connection, SqlTransaction transaction, string commandText, T value, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var command = new SqlCommand(commandText, connection, transaction);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously Executes a Transact-SQL statement against the connection and returns <see cref="Task"/> for the number of rows affected.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/> that represents the connection to an instance of SQL Server.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation for The number of rows affected.</returns>
        public static async Task<int> ExecuteNonQueryAsync<T>(this SqlConnection connection, string commandText, T value, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously Executes a Transact-SQL statement against the connection and returns <see cref="Task"/> for the number of rows affected.
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation for The number of rows affected.</returns>
        public static async Task<int> ExecuteNonQueryAsync<T>(string connectionString, string commandText, T value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var connection = await OpenConnectionAsync(connectionString, cancellationToken);
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        /// <summary>
        /// Sends the <paramref name="commandText"/> to the <paramref name="connection"/> and builds a <see cref="SqlDataReader"/>.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/> that represents the connection to an instance of SQL Server.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> in which the <see cref="SqlCommand"/> executes.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation. for instance of <see cref="SqlDataReader"/>.</returns>
        public static async Task<SqlDataReader> ExecuteReaderAsync<T>(this SqlConnection connection, SqlTransaction transaction, string commandText, T value, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (connection.State != System.Data.ConnectionState.Open)
                throw new InvalidOperationException("connection need to be Opened before the call.");

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var command = new SqlCommand(commandText, connection, transaction);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteReaderAsync(cancellationToken);
        }

        /// <summary>
        /// Sends the <paramref name="commandText"/> to the <paramref name="connection"/> and builds a <see cref="SqlDataReader"/>.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/> that represents the connection to an instance of SQL Server.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation. for instance of <see cref="SqlDataReader"/>.</returns>
        public static async Task<SqlDataReader> ExecuteReaderAsync<T>(this SqlConnection connection, string commandText, T value, CancellationToken cancellationToken)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteReaderAsync(cancellationToken);
        }

        /// <summary>
        /// Sends the <paramref name="commandText"/> to the <paramref name="connectionString"/> and builds a <see cref="SqlDataReader"/>.
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">Ad-hoc SQL Command.</param>
        /// <param name="value">value or values for <paramref name="commandText"/> parametrized ad-hoc statement, Could be any primitive type, object, <see cref="SqlParameter"/>, or <see cref="IEnumerable{SqlParameter}"/>.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation. for instance of <see cref="SqlDataReader"/>.</returns>
        public static async Task<SqlDataReader> ExecuteReaderAsync<T>(string connectionString, string commandText, T value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("message", nameof(commandText));

            var sqlParameters = CreateSqlParameters(commandText, value);

            var connection = await OpenConnectionAsync(connectionString, cancellationToken);
            using var command = new SqlCommand(commandText, connection);
            command.Parameters.AddRange(sqlParameters.ToArray());
            return await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }

        private static IEnumerable<SqlParameter> CreateSqlParameters<T>(string commandText, T entity)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            var regex = new Regex(@"(?<param>[:@]\w*)");
            var parameterNames = regex.Matches(commandText).Cast<Match>().Select(x => x.Groups["param"].Value).Distinct();

            var entityType = typeof(T);
            bool isPrimitiveType = entityType.IsPrimitive || entityType.IsValueType || (entityType == typeof(string));

            if (isPrimitiveType)
            {
                if (parameterNames.Count() > 1)
                    throw new InvalidOperationException($"Expect commandText to have SQL statement with only one parameter.\r\nSpecified Parameters[{string.Join(", ", parameterNames)}]");

                if (entity == null)
                    sqlParameters.Add(new SqlParameter(parameterNames.First(), DBNull.Value));
                else
                    sqlParameters.Add(new SqlParameter(parameterNames.First(), entity));

                return sqlParameters;
            }

            if (entityType == typeof(SqlParameter))
            {
                if (parameterNames.Count() > 1)
                    throw new InvalidOperationException($"Expect commandText to have SQL statement with only one parameter.\r\nSpecified Parameters[{string.Join(", ", parameterNames)}]");

                // Workaround as entity may be premitive as previously handled
                object tempSqlParam = entity;
                SqlParameter sqlParam = tempSqlParam as SqlParameter;

                if (sqlParam.ParameterName != parameterNames.First())
                    throw new InvalidOperationException($"CommandText ParameterName[{parameterNames.First()}] and SqlParameter[{sqlParam.ParameterName}] have to be same.");

                sqlParameters.Add(sqlParam);
            }

            var genericType = entityType.GetGenericArguments().FirstOrDefault();
            if (genericType != null && genericType == typeof(SqlParameter))
            {
                object tempSqlParamList = entity;
                return (IEnumerable<SqlParameter>)tempSqlParamList;
            }

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
    }
}

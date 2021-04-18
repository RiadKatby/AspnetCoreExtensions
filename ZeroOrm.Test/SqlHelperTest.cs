using NUnit.Framework;
using System.Data.SqlClient;
using System.IO;

namespace ZeroOrm.Test
{
    public class Tests
    {
        private readonly string ConnectionString = "server=.;database=ZeroOrm;Integrated Security=true";

        [SetUp]
        public void Setup()
        {
            var userTable = File.ReadAllText("UserTable.sql");
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(userTable, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }

        [Test]
        public void OpenConnection_Check_Is_Opened()
        {

            Assert.Pass();
        }
    }
}
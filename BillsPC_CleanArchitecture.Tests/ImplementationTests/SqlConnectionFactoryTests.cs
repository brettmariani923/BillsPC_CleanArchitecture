using BillsPC_CleanArchitecture.Data.Implementation;
using Final_BillsPC.Data.Tests.Helpers;

namespace BillsPC_CleanArchitecture.Data.Tests
{
    public class SqlConnectionFactoryTests
    {
        [Theory]
        [pokemonData(nameof(Hidden.ConnectionStrings), pokemonType = typeof(Hidden))]
        public void SqlConnectionFactory_Given_ConnectionString_IsValid_NewConnection_ShouldReturn_SqlConnection(string connectionString)
        {
            var connectionFactory = new SqlConnectionFactory(connectionString);

            var connection = connectionFactory.NewConnection();

            Assert.NotNull(connection);
            Assert.Equal(connection.ConnectionString, connectionString);
        }

        [Fact]
        public void SqlConnectionFactory_Given_ConnectionString_IsNotValid_NewConnection_ShouldThrow_ArgumentException()
        {
            var invalidConnectionString = "Not A Real Connection String";

            var connectionFactory = new SqlConnectionFactory(invalidConnectionString);

            Assert.Throws<ArgumentException>(() => connectionFactory.NewConnection());
        }
    }
}
using Microsoft.Data.SqlClient;
using System.Data;
using BillsPC_CleanArchitecture.Data.Interfaces;

namespace BillsPC_CleanArchitecture.Data.Implementation
{
    // <summary>
    /// This class implements IDbConnectionFactory in order to encapsulate the SqlConnection that is created and returned as an IDbConnection using this Factory class.
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        #region Private Members

        private readonly string _connectionString;

        #endregion

        #region Constructor

        public SqlConnectionFactory(string connectionString) => _connectionString = connectionString;

        #endregion

        #region Public IDbConnectionFactory Method

        public IDbConnection NewConnection() => new SqlConnection(_connectionString);

        #endregion
    }
}

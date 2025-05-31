using BillsPC_CleanArchitecture.Data.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace BillsPC_CleanArchitecture.Data.Implementation
{
    public class DataFetchList<TResponse> : IDataFetchList<TResponse>
    {
        private readonly IDbConnection _dbConnection;

        // Store SQL and parameters internally, accessible via interface methods
        private readonly string _sqlQuery;
        private readonly object? _parameters;

        public DataFetchList(IDbConnection dbConnection, string sqlQuery, object? parameters = null)
        {
            _dbConnection = dbConnection;
            _sqlQuery = sqlQuery;
            _parameters = parameters;
        }

        // Implement IDataRequest methods
        public string GetSql()
        {
            return _sqlQuery;
        }

        public object? GetParameters()
        {
            return _parameters;
        }

        // Your own method to execute the query and return the list
        public async Task<IEnumerable<TResponse>> ExecuteAsync()
        {
            var result = await _dbConnection.QueryAsync<TResponse>(GetSql(), GetParameters());
            return result;
        }
    }
}

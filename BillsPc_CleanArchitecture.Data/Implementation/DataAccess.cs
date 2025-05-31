using BillsPC_CleanArchitecture.Data.Interfaces;
using System.Data;
using Dapper;

namespace BillsPC_CleanArchitecture.Data.Implementation
{
    public class DataAccess : IDataAccess
    {
        #region Private Fields

        private readonly IDbConnectionFactory _connectionFactory;

        #endregion

        #region Constructor

        public DataAccess(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #endregion

        #region Public IDataAccess Methods

        public async Task<int> ExecuteAsync(IDataExecute request) => await HandleRequest(async _ => await _.ExecuteAsync(request.GetSql(), request.GetParameters()));

        public async Task<TResponse?> FetchAsync<TResponse>(IDataFetch<TResponse> request) => await HandleRequest(async _ => await _.QueryFirstOrDefaultAsync<TResponse>(request.GetSql(), request.GetParameters()));

        public async Task<IEnumerable<TResponse>> FetchListAsync<TResponse>(IDataFetchList<TResponse> request) => await HandleRequest(async _ => await _.QueryAsync<TResponse>(request.GetSql(), request.GetParameters()));

        #endregion

        #region Private Helper Method

        private async Task<T> HandleRequest<T>(Func<IDbConnection, Task<T>> funcHandleRequest)
        {
            using var connection = _connectionFactory.NewConnection();

            connection.Open();

            return await funcHandleRequest.Invoke(connection);
        }

        #endregion
    }
}

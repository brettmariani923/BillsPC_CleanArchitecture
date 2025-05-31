
namespace BillsPC_CleanArchitecture.Data.Interfaces
{
    public interface IDataAccess
    {
        /// <summary>
        /// Executes the DataRequest using GetSql() and GetParameters() from the IDataExecute request. 
        /// </summary>
        /// <param name="request">The type of DataRequest to Execute</param>
        /// <returns>The number of rows impacted</returns>
        public Task<int> ExecuteAsync(IDataExecute request);

        /// <summary>
        /// Fetch the FirstOrDefault result from the query provided by the DataRequest using GetSql() and GetParameters() from the IDataFetch request.
        /// </summary>
        /// <typeparam name="TResponse">The type of DTO being returned by the SQL Query</typeparam>
        /// <param name="request">The type of DataRequest to Fetch</param>
        /// <returns>Returns an object of the type TResponse associated with the IDataFetch.</returns>
        public Task<TResponse?> FetchAsync<TResponse>(IDataFetch<TResponse> request);

        /// <summary>
        /// Fetch the collection of results from the query provided by the DataRequest using GetSql() and GetParameters() from the IDataFetchList request.
        /// </summary>
        /// <typeparam name="TResponse">The type of DTO that a collection of is being returned by the SQL Query</typeparam>
        /// <param name="request">The type of DataRequest to Fetch</param>
        /// <returns>Returns an IEnumerable of objects of the type TResponse associated with the IDataFetch.</returns>
        public Task<IEnumerable<TResponse>> FetchListAsync<TResponse>(IDataFetchList<TResponse> request);

    }
}


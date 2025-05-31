namespace BillsPC_CleanArchitecture.Data.Interfaces
{
    /// <summary>
    /// This is the interface that will be implemented for any Sql Queries that will return a single record/object.
    /// </summary>
    /// <typeparam name="TResponse">The type of object being returned by the SQL Query</typeparam>
    public interface IDataFetch<TResponse> : IDataRequest { }

}

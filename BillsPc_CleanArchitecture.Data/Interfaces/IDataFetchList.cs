namespace BillsPC_CleanArchitecture.Data.Interfaces
{
    /// <summary>
    /// This is the interface that will be implemented for any Sql Queries that will return a collection of records/objects.
    /// </summary>
    /// <typeparam name="TResponse">The type of object being returned by the SQL Query</typeparam>
    public interface IDataFetchList<TResponse> : IDataRequest { }
}

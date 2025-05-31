namespace BillsPC_CleanArchitecture.Data.Interfaces
{
    /// <summary>
    /// This is the interface that will be implemented for any Sql Commands such as Insert, Update, and Delete which do not return a response object.
    /// </summary>
    public interface IDataExecute : IDataRequest { }
}

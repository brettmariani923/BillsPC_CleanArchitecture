using System.Data;

namespace BillsPC_CleanArchitecture.Data.Interfaces
{
    /// <summary>
    /// This interface defines the factory that will return the IDbConnection used to connect to the database.
    /// </summary>
    public interface IDbConnectionFactory
    {
        public IDbConnection NewConnection();
    }
}

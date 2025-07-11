using Microsoft.Data.SqlClient;
using System.Data;
using BillsPC_CleanArchitecture.Data.Interfaces;

namespace BillsPC_CleanArchitecture.Data.Implementation
{
    // <summary>
    /// This class implements IDbConnectionFactory in order to encapsulate the SqlConnection that is created and returned as an IDbConnection using this Factory class.
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory
    //This class implements IDbConnectionFactory, meaning it must define a method called NewConnection() that returns an IDbConnection.
    {
        #region Private pokemons

        private readonly string _connectionString;
        //Stores the database connection string needed to create a SqlConnection.

        //readonly means it can only be set once, in the constructor.

        #endregion

        #region Constructor

        public SqlConnectionFactory(string connectionString) => _connectionString = connectionString;
        //Takes a connection string as a parameter and saves it to _connectionString.

        //This is typically injected via Dependency Injection(DI), usually configured in Program.cs or Startup.cs.
        #endregion

        #region Public IDbConnectionFactory Method

        public IDbConnection NewConnection() => new SqlConnection(_connectionString);
        //Implements the method defined in IDbConnectionFactory.

        //Returns a new SqlConnection object initialized with the saved _connectionString.

        //The return type is the interface IDbConnection, not SqlConnection, which promotes abstraction and testability.
        #endregion
    }
}

//The job of this class is to create and return a new SQL database connection when requested. It's designed to encapsulate the creation logic and hide the SqlConnection details from other parts of the app.
//This class is a factory pattern and its useful in clean architecture for several reasons:
//Encapsulation, The actual SqlConnection logic is hidden behind the interface
//Flexibility, if you later switch to another database, you only change this class
//Single Responsibility, this class is only responsible for creating sql conntections, nothing else
using BillsPC_CleanArchitecture.Data.Interfaces;
using System.Data;
using Dapper;

namespace BillsPC_CleanArchitecture.Data.Implementation
{
    // The DataAccess class is responsible for actually executing the SQL commands using Dapper. It doesn't define the queries itself, it gets those from the request objects(in this case ReturnAllPokemonRequest)
    // that implement interfaces like IDataFetchList<T>, IDataFetch<T>, or IDataExecute.
    //
    // In the example we've been using, the ReturnAllPokemonRequest class implements IDataFetchList<Pokemon_DTO>, which provides the GetSql() and GetParameters() methods. The definitions for the SQL query and parameters(SELECT * FROM dbo.pokemon, null) are what that will be used in this step.
    // Now that they've been defined, the FetchListAsync method below (last in Public IdataAcess methods below) takes that request, retrieves the SQL and parameters via the interface, and uses Dapper's QueryAsync<T>() to actually execute it and return the pokemon info
    // we asked for. So to reiterate where we are in the process,
    // the "View All Pokémon" button is pressed, get request --> controller (ReturnAllPokemon()) --> service layer (GetAllPokemonWithImagesAsync())--> data request (ReturnAllPokemonRequest) --> data access layer (You are here, FetchListAsync<TResponse>)
    // --> Dapper executes the query and returns the results.
    //
    // This might seem overly complicated or like a huge pain, but by setting it up like this it separates the query definition (ReturnAllPokemonRequest) from the query execution (DataAccess). By doing it like this, we have a separation of concerns that we dont get in other patterns.
    // it ends up being a lot more flexible, maintainable, and most of all its a lot safer. Because the SQL statement is separated from the execution logic, it keeps responsibilities clearly divided and makes it easier to maintain.
    // Only known request types are allowed to use the DataAccess class, which is a lot safer than just having it all in one place where any code could execute any SQL command. 
    //
    //Breifly I want to go over the Pokemon_DTO class, I dont think it warrants an entire visit. Its basically just the data structure of a Pokémon in the application. It contains properties like Id, Name, Type, and ImageUrl, that are populated by the database, and used to transfer that data
    //from the database to our website. If you have any questions about it, feel free to ask me about it in on discord and we can go over it more.
    // Now that we have safely recieved the data we need, we can return to the PokemonService class in the Application layer to see how it uses this data.

    //DataAccess is a generic, reusable class that performs database operations using SQL strings and parameters provided by request objects. It implements an interface called IDataAccess, ensuring it exposes a common set of methods for executing queries.
    public class DataAccess : IDataAccess
    {
        #region Private Fields

        private readonly IDbConnectionFactory _connectionFactory;

        #endregion

       // This field stores an object responsible for creating database connections.

        //IDbConnectionFactory is likely a custom interface you wrote to return a DbConnection(e.g., SQL Server connection) — allowing dependency injection and easier unit testing.

        #region Constructor

        public DataAccess(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        //When this class is created, the constructor requires an IDbConnectionFactory to be passed in. That factory is stored and used to open database connections later.

        #endregion

        #region Public IDataAccess Methods

        public async Task<int> ExecuteAsync(IDataExecute request) => await HandleRequest(async _ => await _.ExecuteAsync(request.GetSql(), request.GetParameters()));
        //Used to run INSERT, UPDATE, or DELETE SQL statements.

        //Accepts an IDataExecute request object.

        //Calls request.GetSql() and request.GetParameters() to get the SQL query and parameters.

        //Internally runs the command using Dapper’s ExecuteAsync() method and returns how many rows were affected.
        public async Task<TResponse?> FetchAsync<TResponse>(IDataFetch<TResponse> request) => await HandleRequest(async _ => await _.QueryFirstOrDefaultAsync<TResponse>(request.GetSql(), request.GetParameters()));
        //Used to fetch a single record from the database.

        //The generic type TResponse is the type of object to map the result to.

        //Calls Dapper's QueryFirstOrDefaultAsync<T>() — this returns one object or null if not found.
        public async Task<IEnumerable<TResponse>> FetchListAsync<TResponse>(IDataFetchList<TResponse> request) => await HandleRequest(async _ => await _.QueryAsync<TResponse>(request.GetSql(), request.GetParameters()));
        //Used to fetch multiple records from the database.

        //Returns a list of TResponse objects.

        //Uses Dapper's QueryAsync<T>() to run the SQL and return a collection.

        #endregion

        //These are the methods your services use to run SQL commands.

        #region Private Helper Method

        private async Task<T> HandleRequest<T>(Func<IDbConnection, Task<T>> funcHandleRequest)
        {
            using var connection = _connectionFactory.NewConnection();

            connection.Open();

            return await funcHandleRequest.Invoke(connection);
        }
        //This is a reusable wrapper for opening the connection, executing the Dapper query, and returning the result.

        //Steps:

        //Calls _connectionFactory.NewConnection() to get a DbConnection.

        //Opens the connection.

        //Runs the lambda funcHandleRequest, which contains the actual database call logic (e.g., Dapper query).

        //Returns the result.

        //This method:

        //Avoids repeating using var connection = ... in every method.

        //Centralizes connection management.

        //Supports clean async execution with minimal duplication.
        #endregion
    }
}


//Clean separation of concerns: The DataAccess class doesn't know or care what SQL is being run. It only executes it.

//Reusability: You can plug in any SQL command via IDataExecute, IDataFetch, etc

//Asynchronicity: All methods are async, which avoids blocking threads and supports scalable web applications.
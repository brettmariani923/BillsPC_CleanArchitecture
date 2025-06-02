using BillsPC_CleanArchitecture.Data.Interfaces;
using System.Data;
using Dapper;
using BillsPC_CleanArchitecture.Data.DTO;
using System.Collections.Generic;

namespace BillsPC_CleanArchitecture.Data.Implementation
{
    // The DataAccess class is responsible for actually executing the SQL commands using Dapper. It doesn't define the queries itself, it gets those from the request objects(in this case ReturnAllPokemonRequest)
    // that implement interfaces like IDataFetchList<T>, IDataFetch<T>, or IDataExecute.
    //
    // In the example we've been using, the ReturnAllPokemonRequest class implements IDataFetchList<Pokemon_DTO>, which provides the GetSql() and GetParameters() methods. These definitions for SQL query and parameters(SELECT * FROM dbo.pokemon, null) are what that will be used in this step.
    // Now that they've been defined, the FetchListAsync method below (last in Public IdataAcess methods below) takes that request, retrieves the SQL and parameters via the interface, and uses Dapper's QueryAsync<T>() to actually execute it and return the pokemon info
    // we asked for. So to reiterate where we are in the process,
    // the "View All Pokémon" button is pressed, get request --> controller (ReturnAllPokemon()) --> service layer (GetAllPokemonWithImagesAsync())--> data request (ReturnAllPokemonRequest) --> data access layer (You are here, FetchListAsync<TResponse>)
    // --> Dapper executes the query and returns the results.
    //
    // This might seem overly complicated or like a huge pain, but by setting it up like this it separates the query definition (ReturnAllPokemonRequest) from the query execution (DataAccess). By doing it like this, we have a separation of concerns that we dont get in other patterns.
    // it ends up being a lot more flexible, maintainable, and most of all its a lot safer. Because the SQL statement is separated from execution logic, it keeps responsibilities clearly divided and makes it easier to maintain.
    // Only known request types are allowed to use the DataAccess class, which is a lot safer than just having it all in one place where any code could execute any SQL command. 
    //
    //Breifly I want to go over the Pokemon_DTO class, I dont think it warrants an entire visit. Its basically just the data structure of a Pokémon in the application. It contains properties like Id, Name, Type, and ImageUrl, that are populated by the database, and used to transfer that data
    //from the database to our website. If you have any questions about it, feel free to ask me about it in on discord and we can go over it more.
    // Now that we have safely recieved the data we need, we can return to the PokemonService class in the Application layer to see how it uses this data.

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

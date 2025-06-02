using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Data.Requests.Pokemon
{
    // This class holds the a SQL request object we are going to use to retrieve the data we need from the database. 
    // It doesnt execute the query itself, it just provides the SQL command and any parameters needed for the query.
    // It has to go the data access layer first to execute the query and return the results.

    // So to reiterate where we are in the process, the "View All Pokémon" button is pressed, get request --> controller --> service layer --> data request (You are here)
    // and then after this request object (ReturnAllPokemonRequest) is created, it then gets passed to the data access class to actually return the results.
    // Inside DataAccess, it gets handled by the FetchListAsync<T> method, which executes the query using Dapper and returns it as a list of results.

    // If you're wondering how or why this connects to the DataAccess class: the methods GetSql() and GetParameters() are defined 
    // by the IDataFetchList<T> interface. Since ReturnAllPokemonRequest implements this interface, the DataAccess class knows how to retrieve the SQL and parameters from it and execute the query
    // which we will go over more in the next step.

    // Go to the DataAccess class in the Implementation folder next to see how this request is executed.


    public class ReturnAllPokemonRequest : IDataFetchList<Pokemon_DTO>
    {
        public string GetSql()
        {
            return "SELECT * FROM dbo.Pokemon";
        }

        public object? GetParameters()
        {
            return null;
        }
    }
}

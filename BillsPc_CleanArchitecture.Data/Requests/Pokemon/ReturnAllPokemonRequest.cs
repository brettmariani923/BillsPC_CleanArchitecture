using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Data.Requests.Pokemon
{
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

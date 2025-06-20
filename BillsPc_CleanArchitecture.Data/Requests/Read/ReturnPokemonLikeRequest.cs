using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Data.Requests.Pokemon
{
    public class ReturnPokemonLikeRequest : IDataFetchList<Pokemon_DTO>
    {
        private readonly string _namePattern;
        private readonly int _limit;

        public ReturnPokemonLikeRequest(string name, int limit = 250)
        {
            _namePattern = $"%{name}%";
            _limit = limit;
        }

        public string GetSql() => @"
            SELECT TOP (@Limit) * 
            FROM dbo.Pokemon 
            WHERE Name LIKE @NamePattern";

        public object? GetParameters() => new
        {
            NamePattern = _namePattern,
            Limit = _limit
        };
    }
}

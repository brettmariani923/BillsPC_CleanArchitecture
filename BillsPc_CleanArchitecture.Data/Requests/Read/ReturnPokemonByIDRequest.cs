using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Interfaces;

namespace BillsPC_CleanArchitecture.Data.Requests
{
    public class ReturnPokemonByIdRequest : IDataFetch<Pokemon_DTO>
    {
        private readonly int _pokemonId;

        public ReturnPokemonByIdRequest(int pokemonId)
        {
            _pokemonId = pokemonId;
        }

        public string GetSql()
        {
            return @"SELECT * FROM Pokemon WHERE PokemonID = @PokemonID";
        }

        public object GetParameters()
        {
            return new { PokemonID = _pokemonId };
        }
    }
}

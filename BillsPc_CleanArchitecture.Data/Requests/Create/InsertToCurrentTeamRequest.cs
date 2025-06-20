using BillsPC_CleanArchitecture.Data.Interfaces;

namespace BillsPC_CleanArchitecture.Data.Requests
{
    public class InsertToCurrentTeamRequest : IDataExecute
    {
        private readonly int _slot;
        private readonly int _pokemonID;

        public InsertToCurrentTeamRequest(int slot, int pokemonId)
        {
            _slot = slot;
            _pokemonID = pokemonId;
        }

        public string GetSql()
        {
            return @"
            IF EXISTS (SELECT 1 FROM CurrentTeam WHERE Slot = @Slot)
            UPDATE CurrentTeam SET PokemonID = @PokemonID WHERE Slot = @Slot;
            ELSE
            INSERT INTO CurrentTeam (Slot, PokemonID) VALUES (@Slot, @PokemonID);";
        }

        public object GetParameters()
        {
            return new { Slot = _slot, PokemonID = _pokemonID };
        }
    }
}

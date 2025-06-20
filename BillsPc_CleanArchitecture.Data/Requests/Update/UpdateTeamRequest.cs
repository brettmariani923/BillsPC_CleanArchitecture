using BillsPC_CleanArchitecture.Data.Interfaces;

namespace BillsPC_CleanArchitecture.Data.Requests
{
    public class UpdateCurrentTeamRequest : IDataExecute
    {
        private readonly int _slot;
        private readonly int _pokemonId;

        public UpdateCurrentTeamRequest(int slot, int pokemonId)
        {
            _slot = slot;
            _pokemonId = pokemonId;
        }

        public string GetSql()
        {
            return @"
                MERGE INTO CurrentTeam AS target
                USING (SELECT @Slot AS Slot, @PokemonID AS PokemonID) AS source
                ON target.Slot = source.Slot
                WHEN MATCHED THEN 
                    UPDATE SET PokemonID = source.PokemonID
                WHEN NOT MATCHED THEN
                    INSERT (Slot, PokemonID) VALUES (source.Slot, source.PokemonID);
            ";
        }

        public object? GetParameters()
        {
            return new { Slot = _slot, PokemonID = _pokemonId };
        }
    }
}

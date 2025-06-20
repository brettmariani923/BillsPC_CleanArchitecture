using BillsPC_CleanArchitecture.Data.DTO;

public interface IPokemonService
{
    Task<List<Pokemon_DTO>> GetAllPokemonAsync();
    Task<List<Pokemon_DTO>> SearchPokemonByNameAsync(string name);
    Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync();
    Task<List<Pokemon_DTO>> SearchPokemonByNameWithImagesAsync(string name);
    Task AddImagesToCurrentTeamListAsync(List<CurrentTeam_DTO> pokemonList);
    Task<List<CurrentTeam_DTO>> GetTeamAsync();
    Task AddPokemonToTeamAsync(int slot, int pokemonId);
    Task RemovePokemonFromTeamAsync(int slot);
    Task UpdatePokemonInTeamAsync(int slot, int pokemonId);

}

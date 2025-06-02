using BillsPC_CleanArchitecture.Data.DTO;
public interface IPokemonService
{
    Task<List<Pokemon_DTO>> GetAllPokemonAsync();
    Task<List<Pokemon_DTO>> SearchPokemonByNameAsync(string name);
    Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync();
    Task<List<Pokemon_DTO>> SearchPokemonByNameWithImagesAsync(string name);
}

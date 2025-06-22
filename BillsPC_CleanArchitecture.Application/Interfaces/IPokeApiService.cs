using BillsPC_CleanArchitecture.Data.DTO;
public interface IPokeApiService
{
    Task<string?> GetPokemonImageUrlAsync(string name);
    Task<string> GetPokemonSpriteUrlAsync(string name);
    Task<List<MoveInfo_DTO>> GetPokemonMovesAsync(string pokemonName);


}

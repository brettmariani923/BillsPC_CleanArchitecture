public interface IPokeApiService
{
    Task<string?> GetPokemonImageUrlAsync(string name);
    Task<string> GetPokemonSpriteUrlAsync(string name);

}

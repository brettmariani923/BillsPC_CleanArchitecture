public interface IPokeApiService
{
    Task<string?> GetPokemonImageUrlAsync(string name);
}

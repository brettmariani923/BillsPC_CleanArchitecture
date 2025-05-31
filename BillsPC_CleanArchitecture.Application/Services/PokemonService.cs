using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Requests.Pokemon;

public class PokemonService : IPokemonService
{
    private readonly IDataAccess _dataAccess;
    private readonly IPokeApiService _pokeApiService;

    public PokemonService(IDataAccess dataAccess, IPokeApiService pokeApiService)
    {
        _dataAccess = dataAccess;
        _pokeApiService = pokeApiService;
    }

    public async Task<List<Pokemon_DTO>> GetAllPokemonAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var result = await _dataAccess.FetchListAsync<Pokemon_DTO>(request);
        return result.ToList();
    }

    public async Task<List<Pokemon_DTO>> SearchPokemonByNameAsync(string name)
    {
        var request = new ReturnPokemonLikeRequest(name);
        var result = await _dataAccess.FetchListAsync<Pokemon_DTO>(request);
        return result.ToList();
    }

    public async Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var pokemonList = (await _dataAccess.FetchListAsync<Pokemon_DTO>(request)).ToList();

        var tasks = pokemonList.Select(async p =>
        {
            p.ImageUrl = await _pokeApiService.GetPokemonImageUrlAsync(p.Name);
        });

        await Task.WhenAll(tasks);

        return pokemonList;
    }
}

using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Requests.Pokemon;
using BillsPC_CleanArchitecture.Data.Requests;

//PART 1: This is where the controller facciltates getting the data it needs. For example, using the GetAllPokemonWithImagesAsync method (last method down below),
//once the user has pressed The "View All Pokemon" button on the webpage and caused a get request, it causes the controller to call to the service layer(here).
//The service layer will then call the data layer(to ReturnAllPokemonRequest in the requests folder) to make a request to get the data it needs from the database. So to sum it all up so far,
//the View All Pokemon button will call the controller, which calls the services layer(you are here). The next step will then be be to access the data layer to load all the pokemon,
//and call the PokeApiService to get the images for each pokemon (using the AddImagesToPokemonListAsync method below), and then return the list of pokemon with images to the controller,
//which will then return it to a new webpage(view) for the user. We will cover this more in the next steps, if it seems like a lot right now it will become clearer as you go.
//go to ReturnAllPokemonRequest.cs in the Data layer next to see how the data access layer is called to get the data we need.

//PART 2(***read after DataAccess.cs section***): Hello again! For our example method GetAllPokemonWithImagesAsync, we left this class to to go to the ReturnAllPokemonRequest() class, which took us to the dataaccess class, which returned the info we needed.
// So to reiterate, we went through * var request = new ReturnAllPokemonRequest(); *, and that took us to the DataAccess class, which executed the SQL query and returned the results finishing * var result = await _dataAccess.FetchListAsync<Pokemon_DTO>(request); *.
// so the for the last step, we just need to get the images for each pokemon. This is done using the AddImagesToPokemonListAsync() method here, which goes to the PokeApiService class, which will fetch the images from the PokeAPI and save them to our local server.
// for the next step, go to the PokeApiService class directly above this one to see how it fetches the images for each pokemon.

//Part 3(***read after PokeApiService.cs section***): Welcome back! Now the AddImagesToPokemonListAsync method will take the list of Pokemon_DTO objects and fetch the images for each one using the PokeApiService.
// once that is finsished, we will have completed the final step of our method *  await AddImagesToPokemonListAsync(pokemon);  *
// all that is left now to do is the *  return pokemon;  * line, which will return the list of Pokemon_DTO objects with their images to the controller, which will then return it to the view(webpage) for the user to see.
// go back to PokemonController.cs in the Api layer for the final steps, to see how the controller handles the results and returns them to the view for the user to see.

public class PokemonService : IPokemonService
{
    private readonly IDataAccess _dataAccess;
    private readonly IPokeApiService _pokeApiService;

    public PokemonService(IDataAccess dataAccess, IPokeApiService pokeApiService)
    {
        _dataAccess = dataAccess;
        _pokeApiService = pokeApiService;
    }

    private async Task AddImagesToPokemonListAsync(List<Pokemon_DTO> pokemonList)
    {
        var semaphore = new SemaphoreSlim(50, 50);

        var tasks = pokemonList.Select(async p =>
        {
            await semaphore.WaitAsync();
            try
            {
                p.ImageUrl = await _pokeApiService.GetPokemonImageUrlAsync(p.Name);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task AddImagesToCurrentTeamListAsync(List<CurrentTeam_DTO> pokemonList)
    {
        var semaphore = new SemaphoreSlim(50, 50);

        var tasks = pokemonList.Select(async p =>
        {
            await semaphore.WaitAsync();
            try
            {
                p.ImageUrl = await _pokeApiService.GetPokemonImageUrlAsync(p.Name);     // regular image
                p.SpriteUrl = await _pokeApiService.GetPokemonSpriteUrlAsync(p.Name);   // animated sprite
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    public async Task<List<Pokemon_DTO>> GetAllPokemonAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var result = await _dataAccess.FetchListAsync<Pokemon_DTO>(request);
        return result.ToList();
    }

    public async Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var pokemon = (await _dataAccess.FetchListAsync<Pokemon_DTO>(request)).ToList();
        await AddImagesToPokemonListAsync(pokemon);
        return pokemon;
    }

    public async Task<List<Pokemon_DTO>> SearchPokemonByNameAsync(string name)
    {
        var request = new ReturnPokemonLikeRequest(name);
        var result = await _dataAccess.FetchListAsync<Pokemon_DTO>(request);
        return result.ToList();
    }

    public async Task<List<Pokemon_DTO>> SearchPokemonByNameWithImagesAsync(string name)
    {
        var request = new ReturnPokemonLikeRequest(name);
        var results = (await _dataAccess.FetchListAsync<Pokemon_DTO>(request)).ToList();
        await AddImagesToPokemonListAsync(results);
        return results;
    }

    public async Task<List<CurrentTeam_DTO>> GetTeamAsync()
    {
        var request = new ReturnCurrentTeamRequest();
        var pokemon = (await _dataAccess.FetchListAsync<CurrentTeam_DTO>(request)).ToList();
        await AddImagesToCurrentTeamListAsync(pokemon);
        return pokemon;
    }

    public async Task AddPokemonToTeamAsync(int slot, int pokemonId)
    {
        var request = new InsertToCurrentTeamRequest(slot, pokemonId);
        await _dataAccess.ExecuteAsync(request);
    }

    public async Task RemovePokemonFromTeamAsync(int slot)
    {
        var request = new RemoveFromTeamRequest(slot);
        await _dataAccess.ExecuteAsync(request);
    }

    public async Task UpdatePokemonInTeamAsync(int slot, int pokemonId)
    {
        var request = new UpdateCurrentTeamRequest(slot, pokemonId);
        await _dataAccess.ExecuteAsync(request);
    }
}

using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
    //Part 1 (very beginning): This is where it starts. We start by making our call to PokemonService.
    //A user on one of the view pages will press a search button or the info tab, and this is where the process of getting them the info they request begins.
    //A good thing to remember is that views move through controllers, and controllers move through services. The controller 
    //is your entrypoint, it accepts requests and returns ressponses. Go to PokemonService.cs in the Application layer next for the next step in the process.

    //Part 2 (dont read until after completing the steps): This is where the results are returned to the view for the user to see. It takes that information we worked so hard to get from different layers and returns it to the view for the user to see.
    // So to break it down *  var allPokemon = await _pokemonService.GetAllPokemonWithImagesAsync();  * gets the data from the service layer, which is where we are now after the previous steps.
    // Once it has the information it needs, it returns it to the view using *  return View("ReturnAllPokemon", allPokemon);  *. This directs the information to the "ReturnAllPokemon" view, which is where the user will see the results of their request.
    // Great job! You've made it through the entire process of how the data flows from the user request to the final view. If you have any questions about this or any other part of the code, feel free to ask me on discord and we can go over it together.
    // If you are curious, look at the ReturnAllPokemon.cshtml file in the Views/Pokemon folder to see how the data is displayed on the webpage.

    public class PokemonController : Controller
    {
        private readonly IPokemonService _pokemonService;
        private readonly IPokeApiService _pokeApiService;

        public PokemonController(IPokemonService pokemonService, IPokeApiService pokeApiService)
        {
            _pokemonService = pokemonService;
            _pokeApiService = pokeApiService;
        }

        public async Task<IActionResult> ReturnAllPokemon()
        {
            var allPokemon = await _pokemonService.GetAllPokemonWithImagesAsync();
            return View("ReturnAllPokemon", allPokemon);
        }


        [HttpGet]
        public IActionResult EnterName()
        {
            return View(new Pokemon_DTO());
        }

        [HttpPost]
        public async Task<IActionResult> EnterName(Pokemon_DTO model)
        {
            var results = await _pokemonService.SearchPokemonByNameWithImagesAsync(model.Name);
            return View("SearchResults", results);
        }

        public IActionResult About()
        {
            ViewBag.Title = "About This Pokémon Website";
            return View();
        }

    }
}

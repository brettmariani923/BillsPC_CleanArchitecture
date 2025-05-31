using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Application.Services;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
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
            var allPokemon = await _pokemonService.GetAllPokemonAsync();

            await AddImagesToPokemonListAsync(allPokemon);

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
            var results = await _pokemonService.SearchPokemonByNameAsync(model.Name);

            await AddImagesToPokemonListAsync(results);

            return View("SearchResults", results);
        }

        public IActionResult About()
        {
            ViewBag.Title = "About This Pokémon Website";
            return View();
        }

        // Extracted common image-fetching logic
        private async Task AddImagesToPokemonListAsync(List<Pokemon_DTO> pokemonList)
        {
            var semaphore = new SemaphoreSlim(50);

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
    }
}

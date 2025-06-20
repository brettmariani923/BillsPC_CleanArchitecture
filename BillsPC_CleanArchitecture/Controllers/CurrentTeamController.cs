using Microsoft.AspNetCore.Mvc;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
    public class CurrentTeamController : Controller
    {
        private readonly IPokemonService _pokemonService;

        public CurrentTeamController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var team = await _pokemonService.GetTeamAsync();
            return View(team);
        }

        [HttpPost]
        public async Task<IActionResult> Insert(int slot, string pokemonName)
        {
            var matchingPokemon = await _pokemonService.SearchPokemonByNameAsync(pokemonName);

            var pokemonId = matchingPokemon.FirstOrDefault()?.PokemonID;

            if (pokemonId == null)
            {
                // Handle no match found gracefully
                ModelState.AddModelError("", "Pokemon not found");
                return RedirectToAction("Index");
            }

            await _pokemonService.AddPokemonToTeamAsync(slot, pokemonId.Value);
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Remove(int slot)
        {
            await _pokemonService.RemovePokemonFromTeamAsync(slot);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int slot, string pokemonName)
        {
            // Lookup pokemonId by name via your service
            var pokemon = await _pokemonService.SearchPokemonByNameAsync(pokemonName);
            var pokemonId = pokemon.FirstOrDefault()?.PokemonID;

            if (pokemonId == null)
            {
                // Handle not found case - maybe redirect with error message or return error
                ModelState.AddModelError("", "Pokemon not found");
                return RedirectToAction("Index");
            }

            await _pokemonService.UpdatePokemonInTeamAsync(slot, pokemonId.Value);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> GetTeam()
        {
            var team = await _pokemonService.GetTeamAsync();
            return View("CurrentTeam", team);
        }
    }
}

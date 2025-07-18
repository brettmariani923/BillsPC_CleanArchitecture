﻿using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> TeamBuilder()
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
                ModelState.AddModelError("", "Pokemon not found");
                return RedirectToAction("TeamBuilder");
            }

            await _pokemonService.AddPokemonToTeamAsync(slot, pokemonId.Value);
            return RedirectToAction("TeamBuilder");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int slot)
        {
            await _pokemonService.RemovePokemonFromTeamAsync(slot);
            return RedirectToAction("TeamBuilder");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int slot, string pokemonName)
        {
            var matchingPokemon = await _pokemonService.SearchPokemonByNameAsync(pokemonName);

            var pokemonId = matchingPokemon.FirstOrDefault()?.PokemonID;

            if (pokemonId == null)
            {
                ModelState.AddModelError("", "Pokemon not found");
                return RedirectToAction("TeamBuilder");
            }

            await _pokemonService.AddPokemonToTeamAsync(slot, pokemonId.Value);
            return RedirectToAction("TeamBuilder");
        }

        [HttpGet]
        public async Task<IActionResult> GetTeam()
        {
            var team = await _pokemonService.GetTeamAsync();
            return View("CurrentTeam", team);
        }
    }
}

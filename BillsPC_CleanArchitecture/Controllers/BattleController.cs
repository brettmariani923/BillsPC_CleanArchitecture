using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Requests;
using System.Text;
using System.Text.Json;
using BillsPC_CleanArchitecture.Application.Interfaces;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
    public class BattleController : Controller
    {
        private readonly IDataAccess _data;
        private readonly IPokeApiService _pokeApiService;
        private readonly IPokemonService _pokemonService;
        private readonly IBattleService _battleService;

        public BattleController(
            IDataAccess data,
            IPokeApiService pokeApiService,
            IPokemonService pokemonService,
            IBattleService battleService)  
        {
            _data = data;
            _pokeApiService = pokeApiService;
            _pokemonService = pokemonService;
            _battleService = battleService;  
        }

        public async Task<IActionResult> Index()
        {
            var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
            return View("SelectPokemon", allPokemon.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> StartTeamBattle()
        {
            var vm = await _battleService.StartTeamBattleAsync();
            if (vm == null)
            {
                var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
                return View("SelectPokemon", allPokemon.ToList());
            }
            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> UsePlayerMove(
            int activeSlot1, int activeSlot2,
            int pokemon1CurrentHP, int pokemon2CurrentHP,
            string moveName,
            string pokemon1Status, string pokemon2Status,
            int pokemon1SleepCounter, int pokemon2SleepCounter,
            string battleLog,
            string playerTeamJson, string aiTeamJson,
            int? switchTo,
            bool requireSwitch,
            int previousPlayerHP,
            int previousAIHP)
        {
            var vm = await _battleService.UsePlayerMoveAsync(
                activeSlot1, activeSlot2,
                pokemon1CurrentHP, pokemon2CurrentHP,
                moveName,
                pokemon1Status, pokemon2Status,
                pokemon1SleepCounter, pokemon2SleepCounter,
                battleLog,
                playerTeamJson, aiTeamJson,
                switchTo,
                requireSwitch,
                previousPlayerHP,
                previousAIHP);

            return View("_BattleView", vm);
        }

        [HttpPost]
        public async Task<IActionResult> UseAIMove(
            int activeSlot1, int activeSlot2,
            int pokemon1CurrentHP, int pokemon2CurrentHP,
            string pokemon1Status, string pokemon2Status,
            int pokemon1SleepCounter, int pokemon2SleepCounter,
            string battleLog,
            string playerTeamJson, string aiTeamJson,
            bool requireSwitch,
            int previousPlayerHP,
            int previousAIHP)
        {
            var vm = await _battleService.UseAIMoveAsync(
                activeSlot1, activeSlot2,
                pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status,
                pokemon1SleepCounter, pokemon2SleepCounter,
                battleLog,
                playerTeamJson, aiTeamJson,
                requireSwitch,
                previousPlayerHP,
                previousAIHP);

            return View("_BattleView", vm);
        }


    }
}

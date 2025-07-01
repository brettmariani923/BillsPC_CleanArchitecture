using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Application.Models;
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
        private readonly ISinglesService _singlesService;

        public BattleController(
            IDataAccess data,
            IPokeApiService pokeApiService,
            ISinglesService SinglesService,
            IPokemonService pokemonService,
            IBattleService battleService)  
        {
            _data = data;
            _pokeApiService = pokeApiService;
            _pokemonService = pokemonService;
            _singlesService = SinglesService;
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

        [HttpGet]
        public async Task<IActionResult> SelectPokemon()
        {
            var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
            return View("SelectPokemon", allPokemon.ToList());
        }


        [HttpPost]
        public async Task<IActionResult> StartBattle(int pokemon1Id, int pokemon2Id)
        {
            var model = await _singlesService.StartBattleAsync(pokemon1Id, pokemon2Id);
            return View("Singles", model);
        }

        [HttpPost]
        public async Task<IActionResult> UsePlayerMoveSingle(
        int pokemon1Id, int pokemon2Id,
        int pokemon1CurrentHP, int pokemon2CurrentHP,
        string moveName,
        string pokemon1Status, string pokemon2Status,
        int pokemon1SleepCounter, int pokemon2SleepCounter,
        string battleLog)
        {
            var model = await _singlesService.UsePlayerMoveSinglesAsync(
                pokemon1Id, pokemon2Id,
                pokemon1CurrentHP, pokemon2CurrentHP,
                moveName,
                pokemon1Status, pokemon2Status,
                pokemon1SleepCounter, pokemon2SleepCounter,
                battleLog);

            // Only let AI attack if it’s still alive and battle isn't over
            if (model.AICurrentHP > 0 && !model.BattleOver)
            {
                model = await _singlesService.UseAIMoveSinglesAsync(
                    pokemon1Id, pokemon2Id,
                    model.PlayerCurrentHP, model.AICurrentHP,
                    model.PlayerStatus, model.AIStatus,
                    model.PlayerSleepCounter, model.AISleepCounter,
                    model.BattleLog);

                // TEMP DEBUG
            }

            return View("Singles", model);
        }

    }


}


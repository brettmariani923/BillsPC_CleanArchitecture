using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Application.Models;
using BillsPC_CleanArchitecture.Data.Requests;
using BillsPC_CleanArchitecture.Application.Interfaces;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
    public class BattleController : Controller
    {
        private readonly IDataAccess _data;
        private readonly IBattleService _battleService;
        private readonly ISinglesService _singlesService;

        public BattleController(
            IDataAccess data,
            ISinglesService SinglesService,
            IBattleService battleService)  
        {
            _data = data;
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
        public async Task<IActionResult> UsePlayerMove([FromForm] BattleMoveRequest request)
        {
            var vm = await _battleService.UsePlayerMoveAsync(request);
            return View("_BattleView", vm);
        }

        [HttpPost]
        public async Task<IActionResult> UseAIMove([FromForm] BattleMoveRequest request)
        {
            var vm = await _battleService.UseAIMoveAsync(request);
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
        public async Task<IActionResult> UsePlayerMoveSingle([FromForm] SingleBattleMoveRequest request)
        {
            var model = await _singlesService.UsePlayerMoveSinglesAsync(request);
            return View("Singles", model);
        }

        [HttpPost]
        public async Task<IActionResult> UseAIMoveSingle([FromForm] SingleBattleMoveRequest request)
        {
            var model = await _singlesService.UseAIMoveSinglesAsync(request);
            return View("Singles", model);
        }

    }
}


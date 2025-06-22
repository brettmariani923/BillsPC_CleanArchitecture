using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Requests;
using BillsPC_CleanArchitecture.Application.Services;
using BillsPC_CleanArchitecture.Api.Models;
using System.Text;
using System.Linq;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
    public class BattleController : Controller
    {
        private readonly IDataAccess _data;
        private readonly IPokeApiService _pokeApiService;

        public BattleController(IDataAccess data, IPokeApiService pokeApiService)
        {
            _data = data;
            _pokeApiService = pokeApiService;
        }

        public async Task<IActionResult> Index()
        {
            var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
            return View("SelectPokemon", allPokemon.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle(int pokemon1Id, int pokemon2Id)
        {
            var p1 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon1Id));
            var p2 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon2Id));

            if (p1 == null || p2 == null)
            {
                ModelState.AddModelError("", "One or both Pokémon could not be found.");
                var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
                return View("SelectPokemon", allPokemon.ToList());
            }

            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            var vm = new BattleViewModel
            {
                Pokemon1 = p1,
                Pokemon2 = p2,
                Pokemon1CurrentHP = p1.HP,
                Pokemon2CurrentHP = p2.HP,
                Pokemon1Moves = p1Moves,
                Pokemon2Moves = p2Moves,
                BattleLog = $"The battle between {p1.Name} and {p2.Name} begins!\n",
                IsPlayerOneTurn = true,
                BattleOver = false
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> UsePlayerMove(
            int pokemon1Id,
            int pokemon2Id,
            int pokemon1CurrentHP,
            int pokemon2CurrentHP,
            string moveName,
            string battleLog)
        {
            var p1 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon1Id));
            var p2 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon2Id));
            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            var logBuilder = new StringBuilder(battleLog);

            // Case-insensitive move matching
            var selectedMove = p1Moves.FirstOrDefault(m => string.Equals(m.Name, moveName, StringComparison.OrdinalIgnoreCase))
                ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

            int damage = CalculateDamage(p1, p2, selectedMove.Power, selectedMove.Type, selectedMove.IsSpecial);
            pokemon2CurrentHP -= damage;
            if (pokemon2CurrentHP < 0) pokemon2CurrentHP = 0;

            logBuilder.AppendLine($"{p1.Name} used {selectedMove.Name} and dealt {damage} damage!");
            logBuilder.AppendLine($"{p2.Name} has {pokemon2CurrentHP} HP left.");

            return View("Index", new BattleViewModel
            {
                Pokemon1 = p1,
                Pokemon2 = p2,
                Pokemon1CurrentHP = pokemon1CurrentHP,
                Pokemon2CurrentHP = pokemon2CurrentHP,
                Pokemon1Moves = p1Moves,
                Pokemon2Moves = p2Moves,
                BattleLog = logBuilder.ToString(),

                // After player move, it becomes AI's turn
                IsPlayerOneTurn = false,

                BattleOver = pokemon2CurrentHP == 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> UseAIMove(
            int pokemon1Id,
            int pokemon2Id,
            int pokemon1CurrentHP,
            int pokemon2CurrentHP,
            string battleLog)
        {
            var p1 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon1Id));
            var p2 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon2Id));
            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            var logBuilder = new StringBuilder(battleLog);

            var aiMove = p2Moves.OrderBy(_ => Guid.NewGuid()).FirstOrDefault()
                ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

            int damage = CalculateDamage(p2, p1, aiMove.Power, aiMove.Type, aiMove.IsSpecial);
            pokemon1CurrentHP -= damage;
            if (pokemon1CurrentHP < 0) pokemon1CurrentHP = 0;

            logBuilder.AppendLine($"{p2.Name} used {aiMove.Name} and dealt {damage} damage!");
            logBuilder.AppendLine($"{p1.Name} has {pokemon1CurrentHP} HP left.");

            return View("Index", new BattleViewModel
            {
                Pokemon1 = p1,
                Pokemon2 = p2,
                Pokemon1CurrentHP = pokemon1CurrentHP,
                Pokemon2CurrentHP = pokemon2CurrentHP,
                Pokemon1Moves = p1Moves,
                Pokemon2Moves = p2Moves,
                BattleLog = logBuilder.ToString(),

                // After AI move, it becomes player's turn if player is alive
                IsPlayerOneTurn = pokemon1CurrentHP > 0,
                BattleOver = pokemon1CurrentHP == 0
            });
        }

        private int CalculateDamage(Pokemon_DTO attacker, Pokemon_DTO defender, int movePower, string moveType, bool isSpecial)
        {
            const int level = 50;
            double attackStat = isSpecial ? attacker.SpecialAttack : attacker.Attack;
            double defenseStat = isSpecial ? defender.SpecialDefense : defender.Defense;

            double stab = (attacker.TypeOne?.Equals(moveType, StringComparison.OrdinalIgnoreCase) == true ||
                           attacker.TypeTwo?.Equals(moveType, StringComparison.OrdinalIgnoreCase) == true) ? 1.5 : 1.0;

            double typeEffectiveness = GetTypeEffectiveness(moveType, defender.TypeOne, defender.TypeTwo);

            double modifier = stab * typeEffectiveness * (Random.Shared.NextDouble() * 0.15 + 0.85);

            double damage = (((2 * level / 5.0 + 2) * attackStat * movePower / defenseStat) / 50 + 2) * modifier;

            return (int)Math.Round(damage);
        }

        private double GetTypeEffectiveness(string moveType, string defenderType1, string defenderType2)
        {
            // Add real type chart logic later; for now, return 1.0 (neutral)
            return 1.0;
        }
    }
}

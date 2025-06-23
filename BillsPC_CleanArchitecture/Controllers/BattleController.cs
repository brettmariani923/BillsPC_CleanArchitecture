using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Requests;
using BillsPC_CleanArchitecture.Api.Models;
using System.Text;

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
                BattleOver = false,
                Pokemon1Status = "None",
                Pokemon2Status = "None",
                Pokemon1SleepCounter = 0,
                Pokemon2SleepCounter = 0
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
            string battleLog,
            string pokemon1Status,
            string pokemon2Status,
            int pokemon1SleepCounter,
            int pokemon2SleepCounter)
        {
            var p1 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon1Id));
            var p2 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon2Id));
            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            var logBuilder = new StringBuilder(battleLog);

            // Corrected sleep logic: Skip turn if sleeping, decrement AFTER skipping
            if (pokemon1Status == "Sleep")
            {
                if (pokemon1SleepCounter > 0)
                {
                    logBuilder.AppendLine($"{p1.Name} is fast asleep and can't move!");
                    pokemon1SleepCounter--;
                    return CreateBattleView(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                        pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter,
                        logBuilder.ToString(), false);
                }
                else
                {
                    pokemon1Status = "None";
                    pokemon1SleepCounter = 0;
                    logBuilder.AppendLine($"{p1.Name} woke up!");
                }
            }

            // Check paralysis status
            if (pokemon1Status == "Paralyzed" && Random.Shared.Next(0, 100) < 25)
            {
                logBuilder.AppendLine($"{p1.Name} is paralyzed! It can't move!");
                return CreateBattleView(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                    pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter,
                    logBuilder.ToString(), false);
            }

            // Proceed with attack
            var selectedMove = p1Moves.FirstOrDefault(m => string.Equals(m.Name, moveName, StringComparison.OrdinalIgnoreCase))
                ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

            int damage = CalculateDamage(p1, p2, selectedMove.Power, selectedMove.Type, selectedMove.IsSpecial);
            pokemon2CurrentHP = Math.Max(0, pokemon2CurrentHP - damage);
            logBuilder.AppendLine($"{p1.Name} used {selectedMove.Name} and dealt {damage} damage!");

            ApplyStatusEffects(selectedMove.Name, p2.Name, ref pokemon2Status, ref pokemon2SleepCounter, logBuilder);

            return CreateBattleView(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter,
                logBuilder.ToString(), false);
        }

        [HttpPost]
        public async Task<IActionResult> UseAIMove(
            int pokemon1Id,
            int pokemon2Id,
            int pokemon1CurrentHP,
            int pokemon2CurrentHP,
            string battleLog,
            string pokemon1Status,
            string pokemon2Status,
            int pokemon1SleepCounter,
            int pokemon2SleepCounter)
        {
            var p1 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon1Id));
            var p2 = await _data.FetchAsync(new ReturnPokemonByIdRequest(pokemon2Id));
            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            var logBuilder = new StringBuilder(battleLog);

            if (pokemon2Status == "Burned")
            {
                int burnDamage = Math.Max(1, p2.HP / 16);
                pokemon2CurrentHP = Math.Max(0, pokemon2CurrentHP - burnDamage);
                logBuilder.AppendLine($"{p2.Name} is hurt by its burn!");
            }

            // Corrected sleep logic for AI
            if (pokemon2Status == "Sleep")
            {
                if (pokemon2SleepCounter > 0)
                {
                    logBuilder.AppendLine($"{p2.Name} is fast asleep and can't move!");
                    pokemon2SleepCounter--;
                    return CreateBattleView(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                        pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter,
                        logBuilder.ToString(), true);
                }
                else
                {
                    pokemon2Status = "None";
                    pokemon2SleepCounter = 0;
                    logBuilder.AppendLine($"{p2.Name} woke up!");
                }
            }

            // Check paralysis
            if (pokemon2Status == "Paralyzed" && Random.Shared.Next(0, 100) < 25)
            {
                logBuilder.AppendLine($"{p2.Name} is paralyzed! It can't move!");
                return CreateBattleView(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                    pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter,
                    logBuilder.ToString(), true);
            }

            // Proceed with attack
            var aiMove = p2Moves.OrderBy(_ => Guid.NewGuid()).FirstOrDefault()
                ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

            int damage = CalculateDamage(p2, p1, aiMove.Power, aiMove.Type, aiMove.IsSpecial);
            pokemon1CurrentHP = Math.Max(0, pokemon1CurrentHP - damage);
            logBuilder.AppendLine($"{p2.Name} used {aiMove.Name} and dealt {damage} damage!");

            ApplyStatusEffects(aiMove.Name, p1.Name, ref pokemon1Status, ref pokemon1SleepCounter, logBuilder);

            return CreateBattleView(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter,
                logBuilder.ToString(), true);
        }

        private void ApplyStatusEffects(string moveName, string targetName, ref string status, ref int sleepCounter, StringBuilder logBuilder)
        {
            string lower = moveName.ToLower();
            if (lower == "ember" && status == "None" && Random.Shared.NextDouble() < 0.2)
            {
                status = "Burned";
                logBuilder.AppendLine($"{targetName} was burned!");
            }
            else if (lower == "thunder shock" && status == "None" && Random.Shared.NextDouble() < 0.2)
            {
                status = "Paralyzed";
                logBuilder.AppendLine($"{targetName} was paralyzed!");
            }
            else if (lower == "hypnosis" && status == "None" && Random.Shared.NextDouble() < 0.6)
            {
                status = "Sleep";
                sleepCounter = Random.Shared.Next(2, 6);
                logBuilder.AppendLine($"{targetName} fell asleep!");
            }
        }

        private IActionResult CreateBattleView(Pokemon_DTO p1, Pokemon_DTO p2, List<MoveInfo_DTO> p1Moves, List<MoveInfo_DTO> p2Moves,
            int p1HP, int p2HP, string p1Status, string p2Status, int p1Sleep, int p2Sleep, string log, bool isPlayerTurn)
        {
            return View("Index", new BattleViewModel
            {
                Pokemon1 = p1,
                Pokemon2 = p2,
                Pokemon1CurrentHP = p1HP,
                Pokemon2CurrentHP = p2HP,
                Pokemon1Moves = p1Moves,
                Pokemon2Moves = p2Moves,
                Pokemon1Status = p1Status,
                Pokemon2Status = p2Status,
                Pokemon1SleepCounter = p1Sleep,
                Pokemon2SleepCounter = p2Sleep,
                BattleLog = log,
                IsPlayerOneTurn = isPlayerTurn,
                BattleOver = p1HP == 0 || p2HP == 0
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
            return 1.0;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Requests;
using BillsPC_CleanArchitecture.Api.Models;
using System.Text;
using System.Text.Json;

namespace BillsPC_CleanArchitecture.Api.Controllers
{
    public class BattleController : Controller
    {
        private readonly IDataAccess _data;
        private readonly IPokeApiService _pokeApiService;
        private readonly IPokemonService _pokemonService;

        public BattleController(IDataAccess data, IPokeApiService pokeApiService, IPokemonService pokemonService)
        {
            _data = data;
            _pokeApiService = pokeApiService;
            _pokemonService = pokemonService;
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

            return View("Singles");
        }

        [HttpPost]
        public async Task<IActionResult> StartTeamBattle()
        {
            var playerTeam = await _pokemonService.GetTeamAsync();
            var aiTeam = await GenerateAITeam();

            if (playerTeam.Count == 0 || aiTeam.Count == 0)
            {
                ModelState.AddModelError("", "Player or AI team is empty.");
                var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
                return View("SelectPokemon", allPokemon.ToList());
            }

            foreach (var p in playerTeam)
                p.CurrentHP = p.HP;
            foreach (var p in aiTeam)
                p.CurrentHP = p.HP;

            var playerActive = playerTeam[0];
            var aiActive = aiTeam[0];

            var playerMoves = await _pokeApiService.GetPokemonMovesAsync(playerActive.Name);
            var aiMoves = await _pokeApiService.GetPokemonMovesAsync(aiActive.Name);

            var vm = new BattleViewModel
            {
                PlayerTeam = playerTeam,
                AITeam = aiTeam,
                PlayerActiveIndex = 0,
                AIActiveIndex = 0,
                PlayerCurrentHP = playerActive.CurrentHP,
                AICurrentHP = aiActive.CurrentHP,
                PlayerMoves = playerMoves,
                AIMoves = aiMoves,
                BattleLog = $"The battle begins! {playerActive.Name} vs {aiActive.Name}\n",
                IsPlayerTurn = true,
                PlayerStatus = "None",
                AIStatus = "None",
                PlayerSleepCounter = 0,
                AISleepCounter = 0,
                BattleOver = false,
                RequireSwitch = false
            };

            return View("Index", vm);
        }

        private async Task<List<CurrentTeam_DTO>> GenerateAITeam()
        {
            var allPokemon = await _data.FetchListAsync(new ReturnAllPokemonRequest());
            var aiTeam = allPokemon.OrderBy(_ => Guid.NewGuid()).Take(6)
                .Select(p => new CurrentTeam_DTO
                {
                    Slot = 0,
                    PokemonID = p.PokemonID,
                    Name = p.Name,
                    HP = p.HP,
                    CurrentHP = p.HP,
                    Attack = p.Attack,
                    Defense = p.Defense,
                    SpecialAttack = p.SpecialAttack,
                    SpecialDefense = p.SpecialDefense,
                    Speed = p.Speed,
                    Ability = p.Ability,
                    Legendary = p.Legendary,
                    Region = p.Region
                }).ToList();

            return aiTeam;
        }

        private bool IsTeamDefeated(List<CurrentTeam_DTO> team)
        {
            return team.All(p => p.CurrentHP <= 0);
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
            var playerJson = Encoding.UTF8.GetString(Convert.FromBase64String(playerTeamJson));
            var aiJson = Encoding.UTF8.GetString(Convert.FromBase64String(aiTeamJson));

            var playerTeam = JsonSerializer.Deserialize<List<CurrentTeam_DTO>>(playerJson);
            var aiTeam = JsonSerializer.Deserialize<List<CurrentTeam_DTO>>(aiJson);

            var logBuilder = new StringBuilder(battleLog);

            // Block move if requireSwitch true and no switch chosen
            if (requireSwitch && !switchTo.HasValue)
            {
                ModelState.AddModelError("", "You must switch to a new Pokémon before continuing.");
                var vm = new BattleViewModel
                {
                    PlayerTeam = playerTeam,
                    AITeam = aiTeam,
                    PlayerActiveIndex = activeSlot1,
                    AIActiveIndex = activeSlot2,
                    PlayerCurrentHP = pokemon1CurrentHP,
                    AICurrentHP = pokemon2CurrentHP,
                    PlayerPreviousHP = previousPlayerHP,
                    AIPreviousHP = previousAIHP,
                    PlayerMoves = await _pokeApiService.GetPokemonMovesAsync(playerTeam[activeSlot1].Name),
                    AIMoves = await _pokeApiService.GetPokemonMovesAsync(aiTeam[activeSlot2].Name),
                    PlayerStatus = pokemon1Status,
                    AIStatus = pokemon2Status,
                    PlayerSleepCounter = pokemon1SleepCounter,
                    AISleepCounter = pokemon2SleepCounter,
                    BattleLog = logBuilder.ToString(),
                    IsPlayerTurn = false,
                    BattleOver = false,
                    RequireSwitch = true
                };
                return View("Index", vm);
            }

            // Handle switch
            if (switchTo.HasValue)
            {
                int newActiveIndex = switchTo.Value;

                if (newActiveIndex >= 0 && newActiveIndex < playerTeam.Count && playerTeam[newActiveIndex].CurrentHP > 0)
                {
                    var oldActiveName = playerTeam[activeSlot1].Name;
                    var newActive = playerTeam[newActiveIndex];

                    logBuilder.AppendLine($"{oldActiveName} was switched out for {newActive.Name}!");

                    var pTwo = aiTeam[activeSlot2];
                    var pTwoMoves = await _pokeApiService.GetPokemonMovesAsync(pTwo.Name);

                    var modelSwitch = new BattleViewModel
                    {
                        PlayerTeam = playerTeam,
                        AITeam = aiTeam,
                        PlayerActiveIndex = newActiveIndex,
                        AIActiveIndex = activeSlot2,
                        PlayerCurrentHP = newActive.CurrentHP,
                        AICurrentHP = pokemon2CurrentHP,
                        PlayerPreviousHP = newActive.CurrentHP,
                        AIPreviousHP = pokemon2CurrentHP,
                        PlayerMoves = await _pokeApiService.GetPokemonMovesAsync(newActive.Name),
                        AIMoves = pTwoMoves,
                        PlayerStatus = "None",
                        AIStatus = pokemon2Status,
                        PlayerSleepCounter = 0,
                        AISleepCounter = pokemon2SleepCounter,
                        BattleLog = logBuilder.ToString(),
                        IsPlayerTurn = false,
                        BattleOver = false,
                        RequireSwitch = false
                    };
                    return View("Index", modelSwitch);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid switch request or Pokémon has fainted.");
                }
            }

            var p1 = playerTeam[activeSlot1];
            var p2 = aiTeam[activeSlot2];

            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            // Save previous HP before damage
            int p1PrevHP = pokemon1CurrentHP;
            int p2PrevHP = pokemon2CurrentHP;

            // Calculate damage
            var selectedMove = p1Moves.FirstOrDefault(m => string.Equals(m.Name, moveName, StringComparison.OrdinalIgnoreCase))
                               ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

            int damage = CalculateDamage(p1, p2, selectedMove.Power, selectedMove.Type, selectedMove.IsSpecial);
            int updatedAIHP = Math.Max(0, pokemon2CurrentHP - damage);

            logBuilder.AppendLine($"{p1.Name} used {selectedMove.Name} and dealt {damage} damage!");
            aiTeam[activeSlot2].CurrentHP = updatedAIHP;

            // Create model
            var model = new BattleViewModel
            {
                PlayerTeam = playerTeam,
                AITeam = aiTeam,
                PlayerActiveIndex = activeSlot1,
                AIActiveIndex = activeSlot2,
                PlayerCurrentHP = pokemon1CurrentHP,
                AICurrentHP = updatedAIHP,
                PlayerPreviousHP = p1PrevHP,
                AIPreviousHP = p2PrevHP,
                PlayerMoves = p1Moves,
                AIMoves = p2Moves,
                PlayerStatus = pokemon1Status,
                AIStatus = pokemon2Status,
                PlayerSleepCounter = pokemon1SleepCounter,
                AISleepCounter = pokemon2SleepCounter,
                BattleLog = logBuilder.ToString(),
                IsPlayerTurn = true,
                BattleOver = false,
                RequireSwitch = false
            };

            // Apply status effects
            string aiStatus = model.AIStatus;
            int aiSleepCounter = model.AISleepCounter;
            ApplyStatusEffects(selectedMove.Name, p2.Name, ref aiStatus, ref aiSleepCounter, logBuilder);
            model.AIStatus = aiStatus;
            model.AISleepCounter = aiSleepCounter;

            // AI faint check
            if (model.AICurrentHP <= 0)
            {
                logBuilder.AppendLine($"{p2.Name} fainted!");
                model.AITeam[model.AIActiveIndex].CurrentHP = 0;

                if (IsTeamDefeated(model.AITeam))
                {
                    logBuilder.AppendLine("Player wins the battle!");
                    model.BattleOver = true;
                    model.IsPlayerTurn = false;
                }
                else
                {
                    int nextAIIndex = model.AITeam.FindIndex(i => i.CurrentHP > 0 && model.AITeam.IndexOf(i) != model.AIActiveIndex);
                    if (nextAIIndex == -1)
                    {
                        logBuilder.AppendLine("Player wins the battle!");
                        model.BattleOver = true;
                        model.IsPlayerTurn = false;
                    }
                    else
                    {
                        model.AIActiveIndex = nextAIIndex;
                        model.AICurrentHP = model.AITeam[nextAIIndex].CurrentHP;
                        model.AIPreviousHP = model.AICurrentHP;
                        model.AIMoves = await _pokeApiService.GetPokemonMovesAsync(model.AITeam[nextAIIndex].Name);
                        model.AIStatus = "None";
                        model.AISleepCounter = 0;
                        logBuilder.AppendLine($"{model.AITeam[nextAIIndex].Name} was sent out!");
                    }
                }
            }

            model.BattleLog = logBuilder.ToString();
            model.IsPlayerTurn = (!model.BattleOver && model.AICurrentHP > 0 && !model.RequireSwitch) ? false : true;

            return View("Index", model);
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
            var playerJson = Encoding.UTF8.GetString(Convert.FromBase64String(playerTeamJson));
            var aiJson = Encoding.UTF8.GetString(Convert.FromBase64String(aiTeamJson));

            var playerTeam = JsonSerializer.Deserialize<List<CurrentTeam_DTO>>(playerJson);
            var aiTeam = JsonSerializer.Deserialize<List<CurrentTeam_DTO>>(aiJson);

            if (requireSwitch)
            {
                var vmWaitSwitch = new BattleViewModel
                {
                    PlayerTeam = playerTeam,
                    AITeam = aiTeam,
                    PlayerActiveIndex = activeSlot1,
                    AIActiveIndex = activeSlot2,
                    PlayerCurrentHP = pokemon1CurrentHP,
                    AICurrentHP = pokemon2CurrentHP,
                    PlayerPreviousHP = previousPlayerHP,
                    AIPreviousHP = previousAIHP,
                    PlayerMoves = await _pokeApiService.GetPokemonMovesAsync(playerTeam[activeSlot1].Name),
                    AIMoves = await _pokeApiService.GetPokemonMovesAsync(aiTeam[activeSlot2].Name),
                    PlayerStatus = pokemon1Status,
                    AIStatus = pokemon2Status,
                    PlayerSleepCounter = pokemon1SleepCounter,
                    AISleepCounter = pokemon2SleepCounter,
                    BattleLog = battleLog,
                    IsPlayerTurn = false,
                    BattleOver = false,
                    RequireSwitch = true
                };

                return View("Index", vmWaitSwitch);
            }

            var p1 = playerTeam[activeSlot1];
            var p2 = aiTeam[activeSlot2];

            var p1Moves = await _pokeApiService.GetPokemonMovesAsync(p1.Name);
            var p2Moves = await _pokeApiService.GetPokemonMovesAsync(p2.Name);

            // Save previous HP before damage
            int p1PrevHP = pokemon1CurrentHP;
            int p2PrevHP = pokemon2CurrentHP;

            var model = new BattleViewModel
            {
                PlayerTeam = playerTeam,
                AITeam = aiTeam,
                PlayerActiveIndex = activeSlot1,
                AIActiveIndex = activeSlot2,
                PlayerCurrentHP = pokemon1CurrentHP,
                AICurrentHP = pokemon2CurrentHP,
                PlayerPreviousHP = p1PrevHP,
                AIPreviousHP = p2PrevHP,
                PlayerMoves = p1Moves,
                AIMoves = p2Moves,
                PlayerStatus = pokemon1Status,
                AIStatus = pokemon2Status,
                PlayerSleepCounter = pokemon1SleepCounter,
                AISleepCounter = pokemon2SleepCounter,
                BattleLog = battleLog,
                IsPlayerTurn = false,
                BattleOver = false,
                RequireSwitch = false
            };

            var logBuilder = new StringBuilder(model.BattleLog);

            // AI Sleep check
            if (model.AIStatus == "Sleep" && model.AISleepCounter > 0)
            {
                logBuilder.AppendLine($"{p2.Name} is fast asleep and can't move!");
                model.AISleepCounter--;
                model.IsPlayerTurn = true;
                model.BattleLog = logBuilder.ToString();
                return View("Index", model);
            }
            else if (model.AIStatus == "Sleep")
            {
                model.AIStatus = "None";
                model.AISleepCounter = 0;
                logBuilder.AppendLine($"{p2.Name} woke up!");
            }

            // AI Paralysis check
            if (model.AIStatus == "Paralyzed" && Random.Shared.Next(0, 100) < 25)
            {
                logBuilder.AppendLine($"{p2.Name} is paralyzed and can't move!");
                model.IsPlayerTurn = true;
                model.BattleLog = logBuilder.ToString();
                return View("Index", model);
            }

            // AI selects random move
            var random = new Random();
            var selectedMove = p2Moves.Count > 0
                ? p2Moves[random.Next(p2Moves.Count)]
                : new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

            int damage = CalculateDamage(p2, p1, selectedMove.Power, selectedMove.Type, selectedMove.IsSpecial);
            model.PlayerCurrentHP = Math.Max(0, model.PlayerCurrentHP - damage);
            logBuilder.AppendLine($"{p2.Name} used {selectedMove.Name} and dealt {damage} damage!");
            model.PlayerTeam[model.PlayerActiveIndex].CurrentHP = model.PlayerCurrentHP;

            // Apply status effects to player
            string playerStatus = model.PlayerStatus;
            int playerSleep = model.PlayerSleepCounter;
            ApplyStatusEffects(selectedMove.Name, p1.Name, ref playerStatus, ref playerSleep, logBuilder);
            model.PlayerStatus = playerStatus;
            model.PlayerSleepCounter = playerSleep;

            // Player faint check
            if (model.PlayerCurrentHP <= 0)
            {
                logBuilder.AppendLine($"{p1.Name} fainted!");
                model.PlayerTeam[model.PlayerActiveIndex].CurrentHP = 0;

                if (IsTeamDefeated(playerTeam))
                {
                    logBuilder.AppendLine("AI wins the battle!");
                    model.BattleOver = true;
                    model.IsPlayerTurn = false;
                    model.RequireSwitch = false;
                }
                else
                {
                    model.RequireSwitch = true;
                    logBuilder.AppendLine("Your Pokémon fainted! Please choose a new Pokémon to send out.");
                    model.IsPlayerTurn = false;
                }
            }
            else
            {
                model.RequireSwitch = false;
            }

            model.BattleLog = logBuilder.ToString();
            model.IsPlayerTurn = (!model.BattleOver && !model.RequireSwitch);

            return View("Index", model);
        }


        private void ApplyStatusEffects(string moveName, string targetName, ref string status, ref int sleepCounter, StringBuilder logBuilder)
        {
            string lower = moveName.ToLower();
            if (lower == "ember" && status == "None" && Random.Shared.NextDouble() < 0.2)
            {
                status = "Burned";
                logBuilder.AppendLine($"{targetName} was burned!");
            }
            else if (lower == "thunder shock" || lower == "thunder wave" && status == "None" && Random.Shared.NextDouble() < 0.2)
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

        private int CalculateDamage(CurrentTeam_DTO attacker, CurrentTeam_DTO defender, int movePower, string moveType, bool isSpecial)
        {
            const int level = 50;
            double attackStat = isSpecial ? attacker.SpecialAttack : attacker.Attack;
            double defenseStat = isSpecial ? defender.SpecialDefense : defender.Defense;
            double stab = 1.0;  // Extend for type matching
            double typeEffectiveness = 1.0;  // Extend for type effectiveness
            double modifier = stab * typeEffectiveness * (Random.Shared.NextDouble() * 0.15 + 0.85);
            double damage = (((2 * level / 5.0 + 2) * attackStat * movePower / defenseStat) / 50 + 2) * modifier;
            return (int)Math.Round(damage);
        }
    }
}

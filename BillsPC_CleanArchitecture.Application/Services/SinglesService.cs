using BillsPC_CleanArchitecture.Application.Interfaces;
using BillsPC_CleanArchitecture.Application.Models;
using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.Requests;
using System.Text;

public class SinglesService : ISinglesService
{
    private readonly IDataAccess _data;
    private readonly IPokeApiService _pokeApiService;

    public SinglesService(IDataAccess data, IPokeApiService pokeApiService)
    {
        _data = data;
        _pokeApiService = pokeApiService;
    }

    public async Task<BattleViewModel> StartBattleAsync(int playerId, int opponentId)
    {
        var (player, playerMoves) = await GetPokemonWithMovesAsync(playerId);
        var (opponent, opponentMoves) = await GetPokemonWithMovesAsync(opponentId);

        return new BattleViewModel
        {
            Pokemon1 = player,
            Pokemon2 = opponent,
            Pokemon1CurrentHP = player.HP,
            Pokemon2CurrentHP = opponent.HP,
            Pokemon1Moves = playerMoves,
            Pokemon2Moves = opponentMoves,
            IsPlayerOneTurn = true,
            BattleOver = false,
            Pokemon1Status = "None",
            Pokemon2Status = "None",
            Pokemon1SleepCounter = 0,
            Pokemon2SleepCounter = 0,
            BattleLog = $"The battle between {player.Name} and {opponent.Name} begins!\n"
        };
    }

    public async Task<BattleViewModel> UsePlayerMoveSinglesAsync(
        int pokemon1Id, int pokemon2Id,
        int pokemon1CurrentHP, int pokemon2CurrentHP,
        string moveName,
        string pokemon1Status, string pokemon2Status,
        int pokemon1SleepCounter, int pokemon2SleepCounter,
        string battleLog)
    {
        var (p1, p1Moves) = await GetPokemonWithMovesAsync(pokemon1Id);
        var (p2, p2Moves) = await GetPokemonWithMovesAsync(pokemon2Id);

        var log = new StringBuilder(battleLog);

        if (HandleSleep(ref pokemon1Status, ref pokemon1SleepCounter, p1.Name, log))
        {
            return CreateModel(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter, log.ToString(), false);
        }

        if (pokemon1Status == "Paralyzed" && Random.Shared.Next(100) < 25)
        {
            log.AppendLine($"{p1.Name} is paralyzed and can't move!");
            return CreateModel(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter, log.ToString(), false);
        }

        if (string.IsNullOrWhiteSpace(moveName))
            moveName = "Tackle";

        var move = p1Moves.FirstOrDefault(m => string.Equals(m.Name, moveName, StringComparison.OrdinalIgnoreCase))
                   ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

        int damage = CalculateDamage(p1, p2, move.Power, move.Type, move.IsSpecial);
        pokemon2CurrentHP = Math.Max(0, pokemon2CurrentHP - damage);
        log.AppendLine($"{p1.Name} used {move.Name} and dealt {damage} damage!");

        ApplyStatusEffects(move.Name, p2.Name, ref pokemon2Status, ref pokemon2SleepCounter, log);

        bool battleOver = pokemon2CurrentHP <= 0;
        if (battleOver)
            log.AppendLine($"{p2.Name} fainted!");

        return CreateModel(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
            pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter, log.ToString(), false, battleOver);
    }

    public async Task<BattleViewModel> UseAIMoveSinglesAsync(
        int pokemon1Id, int pokemon2Id,
        int pokemon1CurrentHP, int pokemon2CurrentHP,
        string pokemon1Status, string pokemon2Status,
        int pokemon1SleepCounter, int pokemon2SleepCounter,
        string battleLog)
    {
        var (p1, p1Moves) = await GetPokemonWithMovesAsync(pokemon1Id);
        var (p2, p2Moves) = await GetPokemonWithMovesAsync(pokemon2Id);

        var log = new StringBuilder(battleLog);

        if (pokemon2Status == "Burned")
        {
            int burnDamage = Math.Max(1, p2.HP / 16);
            pokemon2CurrentHP = Math.Max(0, pokemon2CurrentHP - burnDamage);
            log.AppendLine($"{p2.Name} is hurt by its burn!");
        }

        if (HandleSleep(ref pokemon2Status, ref pokemon2SleepCounter, p2.Name, log))
        {
            return CreateModel(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter, log.ToString(), true);
        }

        if (pokemon2Status == "Paralyzed" && Random.Shared.Next(100) < 25)
        {
            log.AppendLine($"{p2.Name} is paralyzed and can't move!");
            return CreateModel(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
                pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter, log.ToString(), true);
        }

        var move = p2Moves.Count > 0
            ? p2Moves[Random.Shared.Next(p2Moves.Count)]
            : new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

        int damage = CalculateDamage(p2, p1, move.Power, move.Type, move.IsSpecial);
        pokemon1CurrentHP = Math.Max(0, pokemon1CurrentHP - damage);
        log.AppendLine($"{p2.Name} used {move.Name} and dealt {damage} damage!");

        ApplyStatusEffects(move.Name, p1.Name, ref pokemon1Status, ref pokemon1SleepCounter, log);

        bool battleOver = pokemon1CurrentHP <= 0;
        if (battleOver)
            log.AppendLine($"{p1.Name} fainted!");

        return CreateModel(p1, p2, p1Moves, p2Moves, pokemon1CurrentHP, pokemon2CurrentHP,
            pokemon1Status, pokemon2Status, pokemon1SleepCounter, pokemon2SleepCounter, log.ToString(), true, battleOver);
    }

    private async Task<(Pokemon_DTO, List<MoveInfo_DTO>)> GetPokemonWithMovesAsync(int id)
    {
        var pokemon = await _data.FetchAsync(new ReturnPokemonByIdRequest(id));
        var moves = await _pokeApiService.GetPokemonMovesAsync(pokemon.Name);
        return (pokemon, moves);
    }

    private BattleViewModel CreateModel(
        Pokemon_DTO p1, Pokemon_DTO p2,
        List<MoveInfo_DTO> p1Moves, List<MoveInfo_DTO> p2Moves,
        int hp1, int hp2,
        string status1, string status2,
        int sleep1, int sleep2,
        string log, bool isPlayerTurn, bool battleOver = false)
    {
        return new BattleViewModel
        {
            Pokemon1 = p1,
            Pokemon2 = p2,
            Pokemon1CurrentHP = hp1,
            Pokemon2CurrentHP = hp2,
            Pokemon1Moves = p1Moves,
            Pokemon2Moves = p2Moves,
            Pokemon1Status = status1,
            Pokemon2Status = status2,
            Pokemon1SleepCounter = sleep1,
            Pokemon2SleepCounter = sleep2,
            BattleLog = log,
            IsPlayerOneTurn = isPlayerTurn,
            BattleOver = battleOver
        };
    }

    private bool HandleSleep(ref string status, ref int counter, string name, StringBuilder log)
    {
        if (status == "Sleep")
        {
            if (counter > 0)
            {
                log.AppendLine($"{name} is fast asleep and can't move!");
                counter--;
                return true;
            }
            else
            {
                status = "None";
                counter = 0;
                log.AppendLine($"{name} woke up!");
            }
        }
        return false;
    }

    private int CalculateDamage(Pokemon_DTO attacker, Pokemon_DTO defender, int power, string type, bool isSpecial)
    {
        const int level = 50;
        double attack = isSpecial ? attacker.SpecialAttack : attacker.Attack;
        double defense = isSpecial ? defender.SpecialDefense : defender.Defense;
        double modifier = 1.0 * (Random.Shared.NextDouble() * 0.15 + 0.85);
        double damage = (((2 * level / 5.0 + 2) * power * (attack / defense)) / 50 + 2) * modifier;
        return Math.Max(1, (int)Math.Round(damage));
    }

    private void ApplyStatusEffects(string moveName, string targetName, ref string status, ref int sleepCounter, StringBuilder logBuilder)
    {
        if (string.IsNullOrWhiteSpace(moveName)) return;

        if (string.Equals(moveName, "Ember", StringComparison.OrdinalIgnoreCase) && status == "None" && Random.Shared.NextDouble() < 0.2)
        {
            status = "Burned";
            logBuilder.AppendLine($"{targetName} was burned!");
        }
        else if ((string.Equals(moveName, "Thunder Shock", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(moveName, "Thunder Wave", StringComparison.OrdinalIgnoreCase)) &&
                 status == "None" && Random.Shared.NextDouble() < 0.2)
        {
            status = "Paralyzed";
            logBuilder.AppendLine($"{targetName} was paralyzed!");
        }
        else if (string.Equals(moveName, "Hypnosis", StringComparison.OrdinalIgnoreCase) &&
                 status == "None" && Random.Shared.NextDouble() < 0.6)
        {
            status = "Sleep";
            sleepCounter = Random.Shared.Next(2, 6);
            logBuilder.AppendLine($"{targetName} fell asleep!");
        }
    }
}

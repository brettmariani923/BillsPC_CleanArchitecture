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

    public async Task<BattleViewModel> UsePlayerMoveSinglesAsync(SingleBattleMoveRequest request)
    {
        var (p1, p1Moves) = await GetPokemonWithMovesAsync(request.Pokemon1Id);
        var (p2, p2Moves) = await GetPokemonWithMovesAsync(request.Pokemon2Id);

        var log = new StringBuilder(request.BattleLog);

        var status1 = request.Pokemon1Status;
        var sleep1 = request.Pokemon1SleepCounter;

        if (HandleSleep(ref status1, ref sleep1, p1.Name, log))
        {
            request.Pokemon1Status = status1;
            request.Pokemon1SleepCounter = sleep1;

            return CreateModel(p1, p2, p1Moves, p2Moves, request.Pokemon1CurrentHP, request.Pokemon2CurrentHP,
                status1, request.Pokemon2Status, sleep1, request.Pokemon2SleepCounter, log.ToString(), false);
        }

        if (status1 == "Paralyzed" && Random.Shared.Next(100) < 25)
        {
            log.AppendLine($"{p1.Name} is paralyzed and can't move!");

            request.Pokemon1Status = status1; // sync updated status
            request.Pokemon1SleepCounter = sleep1;

            return CreateModel(p1, p2, p1Moves, p2Moves, request.Pokemon1CurrentHP, request.Pokemon2CurrentHP,
                status1, request.Pokemon2Status, sleep1, request.Pokemon2SleepCounter, log.ToString(), false);
        }

        if (string.IsNullOrWhiteSpace(request.MoveName))
            request.MoveName = "Tackle";

        var move = p1Moves.FirstOrDefault(m => string.Equals(m.Name, request.MoveName, StringComparison.OrdinalIgnoreCase))
                   ?? new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

        int damage = CalculateDamage(p1, p2, move.Power, move.Type, move.IsSpecial);
        request.Pokemon2CurrentHP = Math.Max(0, request.Pokemon2CurrentHP - damage);
        log.AppendLine($"{p1.Name} used {move.Name} and dealt {damage} damage!");

        var status2 = request.Pokemon2Status;
        var sleep2 = request.Pokemon2SleepCounter;

        ApplyStatusEffects(move.Name, p2.Name, ref status2, ref sleep2, log);

        request.Pokemon2Status = status2;
        request.Pokemon2SleepCounter = sleep2;

        bool battleOver = request.Pokemon2CurrentHP <= 0;
        if (battleOver)
            log.AppendLine($"{p2.Name} fainted!");

        return CreateModel(p1, p2, p1Moves, p2Moves, request.Pokemon1CurrentHP, request.Pokemon2CurrentHP,
            status1, status2, sleep1, sleep2, log.ToString(), false, battleOver);
    }

    public async Task<BattleViewModel> UseAIMoveSinglesAsync(SingleBattleMoveRequest request)
    {
        var (p1, p1Moves) = await GetPokemonWithMovesAsync(request.Pokemon1Id);
        var (p2, p2Moves) = await GetPokemonWithMovesAsync(request.Pokemon2Id);

        var log = new StringBuilder(request.BattleLog);

        var status1 = request.Pokemon1Status;
        var status2 = request.Pokemon2Status;
        var sleep1 = request.Pokemon1SleepCounter;
        var sleep2 = request.Pokemon2SleepCounter;
        var hp1 = request.Pokemon1CurrentHP;
        var hp2 = request.Pokemon2CurrentHP;

        if (status2 == "Burned")
        {
            int burnDamage = Math.Max(1, p2.HP / 16);
            hp2 = Math.Max(0, hp2 - burnDamage);
            log.AppendLine($"{p2.Name} is hurt by its burn!");
        }

        if (HandleSleep(ref status2, ref sleep2, p2.Name, log))
        {
            return CreateModel(p1, p2, p1Moves, p2Moves, hp1, hp2,
                status1, status2, sleep1, sleep2, log.ToString(), true);
        }

        if (status2 == "Paralyzed" && Random.Shared.Next(100) < 25)
        {
            log.AppendLine($"{p2.Name} is paralyzed and can't move!");
            return CreateModel(p1, p2, p1Moves, p2Moves, hp1, hp2,
                status1, status2, sleep1, sleep2, log.ToString(), true);
        }

        var move = p2Moves.Count > 0
            ? p2Moves[Random.Shared.Next(p2Moves.Count)]
            : new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal", IsSpecial = false };

        int damage = CalculateDamage(p2, p1, move.Power, move.Type, move.IsSpecial);
        hp1 = Math.Max(0, hp1 - damage);
        log.AppendLine($"{p2.Name} used {move.Name} and dealt {damage} damage!");

        ApplyStatusEffects(move.Name, p1.Name, ref status1, ref sleep1, log);

        bool battleOver = hp1 <= 0;
        if (battleOver)
            log.AppendLine($"{p1.Name} fainted!");

        return CreateModel(p1, p2, p1Moves, p2Moves, hp1, hp2,
            status1, status2, sleep1, sleep2, log.ToString(), true, battleOver);
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

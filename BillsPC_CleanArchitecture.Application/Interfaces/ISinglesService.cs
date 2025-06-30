using BillsPC_CleanArchitecture.Application.Models;

using System.Threading.Tasks;

namespace BillsPC_CleanArchitecture.Application.Interfaces;
public interface ISinglesService
{
    Task<BattleViewModel> StartBattleAsync(int playerId, int opponentId);

    Task<BattleViewModel> UsePlayerMoveSinglesAsync(
        int pokemon1Id, int pokemon2Id,
        int pokemon1CurrentHP, int pokemon2CurrentHP,
        string moveName,
        string pokemon1Status, string pokemon2Status,
        int pokemon1SleepCounter, int pokemon2SleepCounter,
        string battleLog);

    Task<BattleViewModel> UseAIMoveSinglesAsync(
        int pokemon1Id, int pokemon2Id,
        int pokemon1CurrentHP, int pokemon2CurrentHP,
        string pokemon1Status, string pokemon2Status,
        int pokemon1SleepCounter, int pokemon2SleepCounter,
        string battleLog);
}



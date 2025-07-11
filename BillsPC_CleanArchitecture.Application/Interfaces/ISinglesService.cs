using BillsPC_CleanArchitecture.Application.Models;

using System.Threading.Tasks;

namespace BillsPC_CleanArchitecture.Application.Interfaces;
public interface ISinglesService
{
    Task<BattleViewModel> StartBattleAsync(int playerId, int opponentId);
    Task<BattleViewModel> UsePlayerMoveSinglesAsync(SingleBattleMoveRequest request);
    Task<BattleViewModel> UseAIMoveSinglesAsync(SingleBattleMoveRequest request);

}



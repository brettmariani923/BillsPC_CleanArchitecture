using BillsPC_CleanArchitecture.Application.Models;

public interface ISinglesService
{
    Task<BattleViewModel> StartBattleAsync(int playerId, int opponentId);
    Task<BattleViewModel> UsePlayerMoveSinglesAsync(SingleBattleMoveRequest request);
    Task<BattleViewModel> UseAIMoveSinglesAsync(SingleBattleMoveRequest request);

}



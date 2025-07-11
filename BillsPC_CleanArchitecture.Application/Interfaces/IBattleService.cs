using BillsPC_CleanArchitecture.Application.Models;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Application.Interfaces
{
    public interface IBattleService
    {
        Task<BattleViewModel> StartTeamBattleAsync();
        Task<List<CurrentTeam_DTO>> GenerateAITeamAsync();
        Task<BattleViewModel> UsePlayerMoveAsync(BattleMoveRequest request);
        Task<BattleViewModel> UseAIMoveAsync(BattleMoveRequest request);

    }
}

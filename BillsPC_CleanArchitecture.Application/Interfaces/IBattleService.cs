using BillsPC_CleanArchitecture.Application.Models;
using BillsPC_CleanArchitecture.Data.DTO;
using System.Threading.Tasks;

namespace BillsPC_CleanArchitecture.Application.Interfaces
{
    public interface IBattleService
    {
        Task<List<CurrentTeam_DTO>> GenerateAITeamAsync();

        Task<BattleViewModel> StartTeamBattleAsync();

        Task<BattleViewModel> UsePlayerMoveAsync(BattleMoveRequest request);
        Task<BattleViewModel> UseAIMoveAsync(BattleMoveRequest request);

    }
}
//i am serializing and deserializing these moves to make the form submissions go quicker. because of that these variables have all of the values written out in the parameters,
//  because I find when i try to put them all into a single value, it creates issues where it struggles to update them
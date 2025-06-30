using BillsPC_CleanArchitecture.Application.Models;
using BillsPC_CleanArchitecture.Data.DTO;
using System.Threading.Tasks;

namespace BillsPC_CleanArchitecture.Application.Interfaces
{
    public interface IBattleService
    {
        Task<List<CurrentTeam_DTO>> GenerateAITeamAsync();

        Task<BattleViewModel> StartTeamBattleAsync();

        Task<BattleViewModel> UsePlayerMoveAsync(
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
            int previousAIHP);

        Task<BattleViewModel> UseAIMoveAsync(
            int activeSlot1, int activeSlot2,
            int pokemon1CurrentHP, int pokemon2CurrentHP,
            string pokemon1Status, string pokemon2Status,
            int pokemon1SleepCounter, int pokemon2SleepCounter,
            string battleLog,
            string playerTeamJson, string aiTeamJson,
            bool requireSwitch,
            int previousPlayerHP,
            int previousAIHP);

    }
}

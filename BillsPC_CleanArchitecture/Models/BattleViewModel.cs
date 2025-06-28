using System.Collections.Generic;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Api.Models
{
    public class BattleViewModel
    {
        public Pokemon_DTO Pokemon1 { get; set; }
        public Pokemon_DTO Pokemon2 { get; set; }

        public int Pokemon1CurrentHP { get; set; }
        public int Pokemon2CurrentHP { get; set; }

        public List<MoveInfo_DTO> Pokemon1Moves { get; set; } = new();
        public List<MoveInfo_DTO> Pokemon2Moves { get; set; } = new();

        public string Pokemon1Status { get; set; } = "";
        public string Pokemon2Status { get; set; } = "";

        public string BattleLog { get; set; } = "";
        public bool IsPlayerOneTurn { get; set; }
        public bool BattleOver { get; set; }

        public int Pokemon1SleepCounter { get; set; } = 0;
        public int Pokemon2SleepCounter { get; set; } = 0;

        public List<CurrentTeam_DTO> PlayerTeam { get; set; }
        public List<CurrentTeam_DTO> AITeam { get; set; }

        public int PlayerActiveIndex { get; set; }
        public int AIActiveIndex { get; set; }

        public List<MoveInfo_DTO> PlayerMoves { get; set; }
        public List<MoveInfo_DTO> AIMoves { get; set; }

        public int PlayerCurrentHP { get; set; }
        public int AICurrentHP { get; set; }

        public string PlayerStatus { get; set; }
        public string AIStatus { get; set; }

        public int PlayerSleepCounter { get; set; }
        public int AISleepCounter { get; set; }

        public bool IsPlayerTurn { get; set; }

        public CurrentTeam_DTO PlayerActive => PlayerTeam?.Count > PlayerActiveIndex ? PlayerTeam[PlayerActiveIndex] : null;
        public CurrentTeam_DTO AIActive => AITeam?.Count > AIActiveIndex ? AITeam[AIActiveIndex] : null;

        // ✅ Add missing aliases
        public CurrentTeam_DTO ActiveSlot1 => PlayerActive;
        public CurrentTeam_DTO ActiveSlot2 => AIActive;
    }
}

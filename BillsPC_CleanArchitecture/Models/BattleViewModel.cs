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

        public string ImageUrl { get; set; }
        public string BattleLog { get; set; } = "";
        public bool IsPlayerOneTurn { get; set; }
        public bool BattleOver { get; set; }

    }
}

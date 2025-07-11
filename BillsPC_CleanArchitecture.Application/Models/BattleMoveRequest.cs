namespace BillsPC_CleanArchitecture.Application.Models
{
    public class BattleMoveRequest
    {
        public int ActiveSlot1 { get; set; }
        public int ActiveSlot2 { get; set; }

        public int Pokemon1CurrentHP { get; set; }
        public int Pokemon2CurrentHP { get; set; }

        public string MoveName { get; set; } = string.Empty;

        public string Pokemon1Status { get; set; } = "None";
        public string Pokemon2Status { get; set; } = "None";

        public int Pokemon1SleepCounter { get; set; }
        public int Pokemon2SleepCounter { get; set; }

        public string BattleLog { get; set; } = string.Empty;

        public string PlayerTeamJson { get; set; } = string.Empty;
        public string AITeamJson { get; set; } = string.Empty;

        public int? SwitchTo { get; set; } = null;
        public bool RequireSwitch { get; set; } = false;

        public int PreviousPlayerHP { get; set; }
        public int PreviousAIHP { get; set; }
    }
}

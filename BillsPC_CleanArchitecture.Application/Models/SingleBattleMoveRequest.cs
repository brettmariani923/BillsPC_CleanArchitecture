namespace BillsPC_CleanArchitecture.Application.Models
{
    public class SingleBattleMoveRequest
    {
        public int Pokemon1Id { get; set; }
        public int Pokemon2Id { get; set; }

        public int Pokemon1CurrentHP { get; set; }
        public int Pokemon2CurrentHP { get; set; }

        public string MoveName { get; set; } = string.Empty;

        public string Pokemon1Status { get; set; } = "None";
        public string Pokemon2Status { get; set; } = "None";

        public int Pokemon1SleepCounter { get; set; }
        public int Pokemon2SleepCounter { get; set; }

        public string BattleLog { get; set; } = string.Empty;
    }
}

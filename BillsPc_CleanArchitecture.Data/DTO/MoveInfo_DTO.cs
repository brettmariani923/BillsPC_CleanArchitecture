namespace BillsPC_CleanArchitecture.Data.DTO
{
    public class MoveInfo_DTO
    {
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public bool IsSpecial { get; set; } = false; // NEW: Determines if move uses SpAtk/SpDef
    }
}

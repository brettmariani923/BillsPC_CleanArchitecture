namespace BillsPC_CleanArchitecture.Data.DTO
{
    public class CurrentTeam_DTO
    {
        public int Slot { get; set; }         // 1 to 6
        public int PokemonID { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public string Ability { get; set; }
        public bool Legendary { get; set; }
        public string Region { get; set; }
        public string ImageUrl { get; set; }
        public string SpriteUrl { get; set; }
        public int CurrentHP { get; set; }    // Add this to track HP during battle
    }
}

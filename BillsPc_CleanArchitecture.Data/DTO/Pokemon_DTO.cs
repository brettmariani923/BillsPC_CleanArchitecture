namespace BillsPC_CleanArchitecture.Data.DTO
{
    public class Pokemon_DTO
    {
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
    }
}

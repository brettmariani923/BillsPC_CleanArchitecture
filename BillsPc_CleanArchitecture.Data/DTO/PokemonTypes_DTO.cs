using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillsPC_CleanArchitecture.Data.DTO
{
    internal class PokemonTypes_DTO
    {
        // From Pokemon table
        public int PokemonID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public string Ability { get; set; } = string.Empty;
        public bool Legendary { get; set; }
        public string Region { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        // From PokemonTypes and Types tables
        public int TypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public byte Slot { get; set; }
    }
}
﻿using System.Text.Json.Serialization;

namespace BillsPC_CleanArchitecture.Data.DTO
{
    public class PokeApiResponse
    {
        public PokeApiResponse_DTO Sprites { get; set; }
    }

    public class PokeApiResponse_DTO
    {
        public OtherSprites Other { get; set; }
    }

    public class OtherSprites
    {
        [JsonPropertyName("official-artwork")]
        public OfficialArtwork OfficialArtwork { get; set; }
    }

    public class OfficialArtwork
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }
    }

}

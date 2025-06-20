using System.Text.Json.Serialization;

namespace BillsPC_CleanArchitecture.Data.DTO
{
    public class PokeApiResponse
    {
        public PokeApiResponse_DTO Sprites { get; set; }
    }

    public class PokeApiResponse_DTO
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }

        public OtherSprites Other { get; set; }

        public Versions Versions { get; set; }
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

    public class Versions
    {
        [JsonPropertyName("generation-v")]
        public GenerationV GenerationV { get; set; }
    }

    public class GenerationV
    {
        [JsonPropertyName("black-white")]
        public BlackWhite BlackWhite { get; set; }
    }

    public class BlackWhite
    {
        public Animated Animated { get; set; }
    }

    public class Animated
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }
    }
}

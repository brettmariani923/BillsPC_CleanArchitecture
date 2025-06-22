using System.Text.Json;
using BillsPC_CleanArchitecture.Data.DTO;

namespace BillsPC_CleanArchitecture.Application.Services
{
    //We are almost at the end! In this phase, the technicals aren't really relevant for what we're trying to accomplish, but I just wanted to show you how the api gets implemented with the database. 
    //This isn't important and I dont want to overload you will things that dont matter, but just know that the connection to the api is made in the method below, which fetches the image from the PokeAPI.
    //For the next step, go back to PokemonService.cs
    public class PokeApiService : IPokeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _imageCacheFolder;

        public PokeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _imageCacheFolder = Path.Combine(webRootPath, "images", "pokemon");

            if (!Directory.Exists(_imageCacheFolder))
                Directory.CreateDirectory(_imageCacheFolder);
        }

        private static readonly SemaphoreSlim _imageWriteLock = new(1, 1);

        public async Task<string?> GetPokemonImageUrlAsync(string name)
        {
            var fileName = $"{name.ToLower()}.png";
            var localFilePath = Path.Combine(_imageCacheFolder, fileName);

            // If it already exists, no need to fetch again
            if (File.Exists(localFilePath))
                return $"/images/pokemon/{fileName}";

            // Acquire lock to prevent multiple threads writing the same file
            await _imageWriteLock.WaitAsync();
            try
            {
                if (File.Exists(localFilePath))
                    return $"/images/pokemon/{fileName}";

                var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<PokeApiResponse>(json, options);

                var imageUrl = data?.Sprites?.Other?.OfficialArtwork?.FrontDefault;
                if (string.IsNullOrEmpty(imageUrl))
                    return null;

                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(localFilePath, imageBytes);
            }
            finally
            {
                _imageWriteLock.Release();
            }

            return $"/images/pokemon/{fileName}";
        }

        public async Task<string?> GetPokemonSpriteUrlAsync(string name)
        {
            var fileName = $"{name.ToLower()}-sprite.png";
            var localFilePath = Path.Combine(_imageCacheFolder, fileName);

            // Return if already downloaded
            if (File.Exists(localFilePath))
                return $"/images/pokemon/{fileName}";

            await _imageWriteLock.WaitAsync();
            try
            {
                if (File.Exists(localFilePath))
                    return $"/images/pokemon/{fileName}";

                var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<PokeApiResponse>(json, options);

                var imageUrl = data?.Sprites?.FrontDefault;
                if (string.IsNullOrEmpty(imageUrl))
                    return null;

                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(localFilePath, imageBytes);
            }
            finally
            {
                _imageWriteLock.Release();
            }

            return $"/images/pokemon/{fileName}";
        }

        public async Task<string?> GetAnimatedSpriteUrlAsync(string name)
        {
            var fileName = $"{name.ToLower()}-sprite.gif"; // many animated sprites are .gif
            var localFilePath = Path.Combine(_imageCacheFolder, fileName);

            if (File.Exists(localFilePath))
                return $"/images/pokemon/{fileName}";

            await _imageWriteLock.WaitAsync();
            try
            {
                if (File.Exists(localFilePath))
                    return $"/images/pokemon/{fileName}";

                var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<PokeApiResponse>(json, options);

                // Pull animated gif from generation-v
                var spriteUrl = data?.Sprites?.Versions?.GenerationV?.BlackWhite?.Animated?.FrontDefault;
                if (string.IsNullOrEmpty(spriteUrl))
                    return null;

                var imageBytes = await _httpClient.GetByteArrayAsync(spriteUrl);
                await File.WriteAllBytesAsync(localFilePath, imageBytes);
            }
            finally
            {
                _imageWriteLock.Release();
            }

            return $"/images/pokemon/{fileName}";
        }

        public async Task<List<MoveInfo_DTO>> GetPokemonMovesAsync(string pokemonName)
        {
            using var http = new HttpClient();
            string url = $"https://pokeapi.co/api/v2/pokemon/{pokemonName.ToLower()}";
            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<MoveInfo_DTO> {
            new MoveInfo_DTO { Name = "Tackle", Power = 40, Type = "normal" }
        };

            using var stream = await response.Content.ReadAsStreamAsync();
            var doc = await JsonDocument.ParseAsync(stream);

            var moves = new List<MoveInfo_DTO>();

            if (doc.RootElement.TryGetProperty("moves", out var movesArray))
            {
                foreach (var moveEntry in movesArray.EnumerateArray())
                {
                    if (moveEntry.TryGetProperty("move", out var moveObj) &&
                        moveObj.TryGetProperty("name", out var moveNameProp))
                    {
                        var moveName = moveNameProp.GetString();
                        if (string.IsNullOrWhiteSpace(moveName))
                            continue;

                        string moveUrl = moveObj.GetProperty("url").GetString() ?? "";
                        var moveResponse = await http.GetAsync(moveUrl);

                        if (!moveResponse.IsSuccessStatusCode)
                            continue;

                        using var moveStream = await moveResponse.Content.ReadAsStreamAsync();
                        var moveDoc = await JsonDocument.ParseAsync(moveStream);

                        int power = 0;
                        string type = "normal";

                        if (moveDoc.RootElement.TryGetProperty("power", out var powerProp) &&
                            powerProp.ValueKind == JsonValueKind.Number)
                        {
                            power = powerProp.GetInt32();
                        }

                        if (moveDoc.RootElement.TryGetProperty("type", out var typeObj) &&
                            typeObj.TryGetProperty("name", out var typeNameProp))
                        {
                            type = typeNameProp.GetString() ?? "normal";
                        }

                        moves.Add(new MoveInfo_DTO
                        {
                            Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(moveName.Replace('-', ' ')),
                            Power = power,
                            Type = type
                        });

                        if (moves.Count >= 4)
                            break;
                    }
                }
            }

            return moves;
        }





    }

}

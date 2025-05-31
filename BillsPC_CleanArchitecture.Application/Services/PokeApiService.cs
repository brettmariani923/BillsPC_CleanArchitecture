using System.Text.Json;
using BillsPC_CleanArchitecture.Data.DTO;




namespace BillsPC_CleanArchitecture.Application.Services
{
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
                // Double-check again inside the lock (maybe another thread already wrote it)
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

    }

}

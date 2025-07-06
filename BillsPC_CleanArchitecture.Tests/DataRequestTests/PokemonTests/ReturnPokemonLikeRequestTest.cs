using BillsPC_CleanArchitecture.Tests.Helpers;
using BillsPC_CleanArchitecture.Data.Requests.Pokemon;
using Xunit;

namespace BillsPC_CleanArchitecture.Tests.DataRequestTests.PokemonTests
{
    public class ReturnPokemonLikeRequestTest : DataTest
    {
        [Fact]
        public async Task Return_SinglePokemon_ShouldReturn_NotNull_OrEmpty()
        {
            // Arrange
            var request = new ReturnPokemonLikeRequest("Pikachu");

            // Act
            var result = await _dataAccess.FetchListAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, pokemon => Assert.False(string.IsNullOrWhiteSpace(pokemon.Name)));
        }

        [Fact]
        public async Task Return_SinglePokemon_ShouldReturn_EmptyList_When_NoMatch()
        {
            // Arrange
            var request = new ReturnPokemonLikeRequest("NonExistentPokemon");

            // Act
            var result = await _dataAccess.FetchListAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); 
        }

        [Fact]
        public async Task Return_SinglePokemon_ShouldReturn_Pokemon_When_CaseInsensitiveMatch()
        {
            // Arrange
            var request = new ReturnPokemonLikeRequest("piKaCHu"); // Lowercase input
            
            // Act
            var result = await _dataAccess.FetchListAsync(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, pokemon => Assert.Equal("Pikachu", pokemon.Name, StringComparer.OrdinalIgnoreCase)); // Ensure case-insensitive match
        }

        [Fact]
        public async Task Return_SinglePokemon_ShouldReturn_Pokemon_When_ExactMatch()
        {
            // Arrange
            var request = new ReturnPokemonLikeRequest("Pikachu"); // Exact match input
            
            // Act
            var result = await _dataAccess.FetchListAsync(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, pokemon => Assert.Equal("Pikachu", pokemon.Name)); // Ensure exact match
        }

        [Fact]
        public async Task ShouldReturn_AllMatchingPokemon_When_PartialMatch()
        {
            // Arrange
            var request = new ReturnPokemonLikeRequest("chu"); // Partial match input
            
            // Act
            var result = await _dataAccess.FetchListAsync(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, pokemon => Assert.Contains("chu", pokemon.Name, StringComparison.OrdinalIgnoreCase)); // Ensure partial match
        }

        [Fact]
        public async Task ShouldReturn_EmptyList_When_Numbers_Or_Symbols_InInput()
        {
            // Arrange
            var request = new ReturnPokemonLikeRequest("123!@#"); // Input with numbers and symbols
            
            // Act
            var result = await _dataAccess.FetchListAsync(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Ensure an empty list is returned when no match is found
        }
    }
}
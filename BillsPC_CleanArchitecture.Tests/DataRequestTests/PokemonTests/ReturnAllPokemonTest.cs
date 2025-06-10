using BillsPC_CleanArchitecture.Tests.Helpers;
using BillsPC_CleanArchitecture.Data.Requests.Pokemon;
using Xunit;

namespace BillsPC_CleanArchitecture.Tests.DataRequestTests.PokemonTests
{
    public class ReturnAllPokemonTest : DataTest
    {
        [Fact]
        public async Task ReturnAllPokemon_ShouldReturn_AllPokemon()
        {
            // Arrange
            var request = new ReturnAllPokemonRequest();

            // Act
            var result = await _dataAccess.FetchListAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, pokemon => Assert.False(string.IsNullOrWhiteSpace(pokemon.Name)));
        }
    }
}

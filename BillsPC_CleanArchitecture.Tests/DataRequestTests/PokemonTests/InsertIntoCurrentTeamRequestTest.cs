using BillsPC_CleanArchitecture.Tests.Helpers;
using BillsPC_CleanArchitecture.Data.Requests;

namespace BillsPC_CleanArchitecture.Tests.DataRequestTests.PokemonTests
{//unfisnished, come back to this one
    public class InsertIntoCurrentTeamRequestTest : DataTest
    {
        [Fact]
        public async Task Insert_Into_CurrentTeam_ShouldNot_BeNull_OrEmpty()
        {
            int slot = 1;
            int pokemonNumber = 25;

            // Arrange
            var insertRequest = await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(slot, pokemonNumber));

            // Act
            var result = await _dataAccess.FetchListAsync(new ReturnCurrentTeamRequest());

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(1, 25);
        }

    
    }
}

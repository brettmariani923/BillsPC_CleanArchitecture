using BillsPC_CleanArchitecture.Tests.Helpers;
using BillsPC_CleanArchitecture.Data.Requests;
using Microsoft.Data.SqlClient;


namespace BillsPC_CleanArchitecture.Tests.DataRequestTests.PokemonTests
{//unfisnished, come back to this one
    public class InsertIntoCurrentTeamRequestTest : DataTest
    {
        int slot = 2;
        int pokemonNumber = 26;

        [Fact]
        public async Task Insert_Into_CurrentTeam_ShouldNot_Return_NullOrEmpty()
        {
            // Arrange
            var insertRequest = await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(slot, pokemonNumber));

            // Act
            var result = await _dataAccess.FetchListAsync(new ReturnCurrentTeamRequest());

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            var delete = await _dataAccess.ExecuteAsync(new RemoveFromTeamRequest(slot));

            var cleanUp = await _dataAccess.FetchListAsync(new ReturnCurrentTeamRequest());

            Assert.Empty(cleanUp);
        }

        [Fact]
        public async Task InsertPokemon_Given_BiggerThan_6_Should_ThrowError()
        {
            await Assert.ThrowsAsync<SqlException>(async () => await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(7, pokemonNumber)));

        }

        [Fact]
        public async Task InsertPokemon_Given_SmallerThan_1_Should_ThrowError()
        {
            await Assert.ThrowsAsync<SqlException>(async() => await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(0, pokemonNumber)));
        }

        [Fact]
        public async Task InsertPokemon_Given_NoMatch_Should_Throw_Error()
        {
            await Assert.ThrowsAsync<SqlException>(async () => await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(slot, 1000)));
        }


    }
}

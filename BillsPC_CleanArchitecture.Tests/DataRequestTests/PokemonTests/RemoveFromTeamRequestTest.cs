using System;
using Microsoft.Data.SqlClient;
using BillsPC_CleanArchitecture.Tests.Helpers;
using BillsPC_CleanArchitecture.Data.Requests;

namespace BillsPC_CleanArchitecture.Tests.DataRequestTests.PokemonTests
{
    public class RemoveFromTeamRequestTest : DataTest
    {
        int slot = 1;
        int pokemonNumber = 25;

        [Fact]
        public async Task DeletePokemon_Slot_ShouldDelete_Successfully()
        {
            var insertRequest = await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(slot, pokemonNumber));

            var delete = await _dataAccess.ExecuteAsync(new RemoveFromTeamRequest(slot));

            var result = await _dataAccess.FetchListAsync(new ReturnCurrentTeamRequest());
            
            Assert.Empty(result);
        }
    }
}

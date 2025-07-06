using System;
using System.Collections.Generic;
using BillsPC_CleanArchitecture.Tests.Helpers;
using BillsPC_CleanArchitecture.Data.Requests;

namespace BillsPC_CleanArchitecture.Tests.DataRequestTests.PokemonTests
{
    public class RemoveFromTeamRequestTest : DataTest
    {
        [Fact]
        public async Task DeletePokemon_Slot_ShouldDelete_Successfully()
        {
            int slot = 1;
            int pokemonNumber = 25;

            var insertRequest = await _dataAccess.ExecuteAsync(new InsertToCurrentTeamRequest(slot, pokemonNumber));

            var delete = await _dataAccess.ExecuteAsync(new RemoveFromTeamRequest(slot));

            var result = await _dataAccess.FetchListAsync(new ReturnCurrentTeamRequest());
            
            Assert.Empty(result);

        }

        [Fact]
        public async Task DeleteSlot_GivenIncorrectNumber_ShouldReturn_Zero()
        {
            var result = await _dataAccess.ExecuteAsync(new RemoveFromTeamRequest(99999));
            Assert.Equal(0, result);
        }
    }

}

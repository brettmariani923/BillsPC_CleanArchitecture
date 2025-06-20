using BillsPC_CleanArchitecture.Data.Interfaces;

namespace BillsPC_CleanArchitecture.Data.Requests
{
    public class RemoveFromTeamRequest : IDataExecute
    {
        private readonly int _slot;

        public RemoveFromTeamRequest(int slot)
        {
            _slot = slot;
        }

        public string GetSql()
        {
            return "DELETE FROM CurrentTeam WHERE Slot = @Slot";
        }

        public object? GetParameters()
        {
            return new { Slot = _slot };
        }
    }
}

using BillsPC_CleanArchitecture.Data.DTO;
using BillsPC_CleanArchitecture.Data.Interfaces;

public class ReturnCurrentTeamRequest : IDataFetchList<CurrentTeam_DTO>
{
    public string GetSql()
    {
        return @"
            SELECT 
                pt.Slot,
                p.PokemonID,
                p.Name,
                p.HP,
                p.Attack,
                p.Defense,
                p.SpecialAttack,
                p.SpecialDefense,
                p.Speed,
                p.Ability,
                p.Legendary,
                p.Region,
                '' AS ImageUrl -- placeholder; you'll populate this later in the service layer
            FROM CurrentTeam pt
            JOIN Pokemon p ON pt.PokemonID = p.PokemonID
            ORDER BY pt.Slot;
        ";
    }

    public object? GetParameters()
    {
        return null;
    }
}

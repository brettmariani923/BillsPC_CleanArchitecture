using System.Text;
using BillsPC_CleanArchitecture.Data.DTO;

public class BattleResult
{
    public string Log { get; set; }
    public string Winner { get; set; }
}

public class BattleService
{
    public BattleResult SimulateBattle(Pokemon_DTO p1, Pokemon_DTO p2)
    {
        var log = new StringBuilder();
        int hp1 = p1.HP;
        int hp2 = p2.HP;

        log.AppendLine($"⚔️ Battle Start: {p1.Name} vs {p2.Name}!\n");

        bool p1GoesFirst = p1.Speed >= p2.Speed;
        int turn = 1;

        while (hp1 > 0 && hp2 > 0)
        {
            log.AppendLine($"--- Turn {turn++} ---");

            if (p1GoesFirst)
            {
                int damageToP2 = Math.Max(1, p1.Attack - p2.Defense);
                hp2 -= damageToP2;
                log.AppendLine($"{p1.Name} attacks {p2.Name} for {damageToP2} damage! {p2.Name}'s HP: {Math.Max(0, hp2)}");

                if (hp2 <= 0) break;

                int damageToP1 = Math.Max(1, p2.Attack - p1.Defense);
                hp1 -= damageToP1;
                log.AppendLine($"{p2.Name} attacks {p1.Name} for {damageToP1} damage! {p1.Name}'s HP: {Math.Max(0, hp1)}");
            }
            else
            {
                int damageToP1 = Math.Max(1, p2.Attack - p1.Defense);
                hp1 -= damageToP1;
                log.AppendLine($"{p2.Name} attacks {p1.Name} for {damageToP1} damage! {p1.Name}'s HP: {Math.Max(0, hp1)}");

                if (hp1 <= 0) break;

                int damageToP2 = Math.Max(1, p1.Attack - p2.Defense);
                hp2 -= damageToP2;
                log.AppendLine($"{p1.Name} attacks {p2.Name} for {damageToP2} damage! {p2.Name}'s HP: {Math.Max(0, hp2)}");
            }

            log.AppendLine(); // spacing between turns
        }

        string winner = hp1 > 0 ? p1.Name : p2.Name;
        log.AppendLine($"🎉 Winner: {winner}!");

        return new BattleResult
        {
            Winner = winner,
            Log = log.ToString()
        };
    }
}


using System.Collections.Generic;

public class GameManager
{
    public static List<Unit> AllUnits = new List<Unit>();

    public static List<Unit> GetUnitsOfTeam(Team team)
    {
        List<Unit> units = new List<Unit>();
        foreach (Unit unit in AllUnits)
        {
            if (unit.Team == team)
            {
                units.Add(unit);
            }
        }
        return units;
    }

    public static bool OpposingTeams(Team team1, Team team2)
    {
        if (
            (team1 == Team.Allied && team2 == Team.Enemy) ||
            (team1 == Team.Enemy && team2 == Team.Allied) ||
            (team1 == Team.Visitor && team2 == Team.Enemy) ||
            (team1 == Team.Enemy && team2 == Team.Visitor)
            )
        {
            return true;
        }
        return false;
    }
}
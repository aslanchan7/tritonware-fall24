public class GameManager
{
    public static bool OpposingTeams(Team team1, Team team2)
    {
        if (team1 == Team.Allied && team2 == Team.Enemy) return true;
        if (team1 == Team.Neutral && team2 == Team.Enemy) { return true; }
        return false;
    }
}
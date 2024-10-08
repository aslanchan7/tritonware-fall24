public class AlliedUnit : Unit
{
    public override Team Team => ForceEnemy ? Team.Enemy : Team.Allied;

    public bool ForceEnemy = false;     // forces this unit to be considered as an enemy for testing
    public bool IsControllable()
    {
        return true; // todo
    }


}
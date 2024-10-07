public class AlliedUnit : Unit
{
    public override Team Team => Team.Allied;

    public bool IsControllable()
    {
        return true; // todo
    }


}
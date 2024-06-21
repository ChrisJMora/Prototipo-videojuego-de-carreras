namespace CarRace
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
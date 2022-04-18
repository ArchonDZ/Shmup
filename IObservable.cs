public interface IObservable
{
    public void RegisterObserver(IObserver observer);
    public void RemoveObserver(IObserver observer);
}

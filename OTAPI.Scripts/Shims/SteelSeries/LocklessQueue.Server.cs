namespace SteelSeries.GameSense
{
    public class LocklessQueue<T>
    {
        public LocklessQueue(uint size) { }

        public bool PEnqueue(T obj) => false;

        public T CDequeue() => default(T);
    }
}

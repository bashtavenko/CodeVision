namespace CodeVision.CSharp.Semantic
{
    public class Memento<T>
    {
        public Memento(T state)
        {
            this.State = state;
        }

        public T State { get; set; }
    }
}

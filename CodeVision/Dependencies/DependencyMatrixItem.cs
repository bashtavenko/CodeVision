namespace CodeVision.Dependencies
{
    // This is not Tuple<int,string> because it is better to have item.Id than item.Item1
    public class DependencyMatrixItem
    {
        public int Id { get; }
        public string Value { get; }

        public DependencyMatrixItem(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}
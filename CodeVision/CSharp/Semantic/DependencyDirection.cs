namespace CodeVision.CSharp.Semantic
{
    public enum DependencyDirection
    {
        Downstream, // Which dependencies this one has?
        Upstream    // Who depends on it?
    }
}

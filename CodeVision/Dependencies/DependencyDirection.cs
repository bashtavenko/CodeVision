namespace CodeVision.Dependencies
{
    public enum DependencyDirection
    {
        Downstream, // Which dependencies this one has?
        Upstream    // Who depends on it?
    }
}

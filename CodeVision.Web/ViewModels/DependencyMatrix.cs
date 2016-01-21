using CodeVision.Dependencies;

namespace CodeVision.Web.ViewModels
{
    public class DependencyMatrix
    {
        public DependencyMatrixItem[] Packages { get; set; }
        public DependencyMatrixItem[] Projects { get; set; }
        public bool[,] Matrix { get; set; }
    }
}
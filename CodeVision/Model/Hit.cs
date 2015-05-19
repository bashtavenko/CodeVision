using System.Collections.Generic;
using System.IO;

namespace CodeVision.Model
{
    public class Hit
    {
        public string FilePath { get; set; }
        public string FileName { get { return Path.GetFileName(FilePath);}}
        public float Score { get; set; }
        public string BestFragment { get; set; }
        public List<Offset> Offsets { get; set; }

        public Hit()
        {
            Offsets = new List<Offset>();
        }
    }
}

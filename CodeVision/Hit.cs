using System.Collections.Generic;
using System.IO;

namespace CodeVision
{
    public class Hit
    {
        public string FilePath { get; set; }
        public string FileName { get { return Path.GetFileName(FilePath);}}
        public float Score { get; set; }
        public List<Offset> Offsets { get; set; }
    }
}

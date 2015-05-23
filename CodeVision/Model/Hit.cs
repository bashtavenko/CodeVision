using System;
using System.Collections.Generic;
using System.IO;

namespace CodeVision.Model
{
    public class Hit
    {
        public string FilePath { get { return _filePath; } }
        public string FileName { get { return Path.GetFileName(FilePath);}}

        public string FriendlyFileName
        {
            get
            {
                string contentRootPathNormalized = !_contentRootPath.EndsWith(Path.DirectorySeparatorChar.ToString())
                    ? _contentRootPath + Path.DirectorySeparatorChar
                    : _contentRootPath;
                var index = FilePath.IndexOf(contentRootPathNormalized, System.StringComparison.Ordinal);
                return index == 0 ? FilePath.Substring(contentRootPathNormalized.Length) : FileName;
            }
        }

        public float Score { get; set; }
        public string BestFragment { get; set; }
        public List<Offset> Offsets { get; set; }

        private readonly string _contentRootPath;
        private readonly string _filePath;

        public Hit(string contentRootPath, string filePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                throw new NullReferenceException("Must have content root path");
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new NullReferenceException("Must have content file path");
            }
            Offsets = new List<Offset>();
            _contentRootPath = contentRootPath;
            _filePath = filePath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace CodeVision.Model
{
    public class Hit
    {
        public int DocId { get; set; }
        public string FilePath { get { return _filePath; } }
        public string ContentRootPath { get { return _contentRootPath; } }
        public string FileName { get { return Path.GetFileName(FilePath);}}
        public string Language { get { return _language; } }

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
        private readonly string _language;

        public Hit(int docId, string filePath, string contentRootPath, float score, string bestFragment, List<Offset> offsets, string language) 
            : this (docId, contentRootPath, filePath, score, language)
        {
            this.BestFragment = bestFragment;
            this.Offsets = offsets;
        }

        public Hit(int docId, string contentRootPath, string filePath, float score, string language)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                throw new NullReferenceException("Must have content root path");
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new NullReferenceException("Must have content file path");
            }
            this.Offsets = new List<Offset>();
            this.Score = score;
            this.DocId = docId;
            _contentRootPath = contentRootPath;
            _filePath = filePath;
            _language = language;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CodeVision.Model;
using Newtonsoft.Json;

namespace CodeVision.Web.ViewModels
{
    public class SearchHit
    {
        public string FilePath { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }
        
        public string FriendlyFileName { get; set; }

        public string ContentRootPath { get; set; }

        [JsonIgnore]
        public float Score { get; set; }

        [JsonIgnore]
        public string BestFragment { get; set; }

        public List<Offset> Offsets { get; set; }

        public override string ToString()
        {
            var json = JsonConvert.SerializeObject(this);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(jsonBytes);
        }
    }
}

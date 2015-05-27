using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CodeVision.Web.ViewModels
{
    public class SearchHit
    {
        [JsonProperty(PropertyName = "id")]
        public int DocId { get; set; }

        [JsonProperty(PropertyName = "fp")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "fn")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "ffn")]
        public string FriendlyFileName { get; set; }

        [JsonProperty(PropertyName = "c")]
        public string ContentRootPath { get; set; }

        [JsonProperty(PropertyName = "s")]
        public float Score { get; set; }

        [JsonIgnore]
        public string BestFragment { get; set; }

        [JsonProperty(PropertyName = "o")]
        public List<SearchOffset> Offsets { get; set; }

        public override string ToString()
        {
            var json = JsonConvert.SerializeObject(this);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(jsonBytes);
        }
    }
}

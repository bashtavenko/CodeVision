using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CodeVision.Web.ViewModels
{
    public class SearchOffset
    {
        [JsonProperty(PropertyName = "s")]
        public int StartOffset { get; set; }

        [JsonProperty(PropertyName = "e")]
        public int EndOffset { get; set; }

    }
}
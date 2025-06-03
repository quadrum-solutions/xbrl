using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class EbaDocumentation
    {
        /// <summary/>
        public string CellCode { get; set; }

        /// <summary/>
        public string DataPointVersionId { get; set; }

        /// <summary/>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary/>
        [JsonPropertyName("headerCode")]
        public string HeaderCode { get; set; }

        /// <summary/>
        [JsonPropertyName("headerDirection")]
        public string HeaderDirection { get; set; }

        /// <summary/>
        public Dictionary<string, long> AllowedValue { get; set; }

        /// <summary/>
        public string DomainCode { get { return AllowedValue?.First().Key.Split(':')[0].Split('_')[1]; } }
    }
}
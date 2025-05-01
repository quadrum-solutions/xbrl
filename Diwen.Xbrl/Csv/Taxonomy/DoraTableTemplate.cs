using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>    
    public class DoraTableTemplate
    {
        /// <summary/>
        [JsonPropertyName("columns")]
        public Dictionary<string, PropertyGroup> PropertyGroups { get; set; }
    }
}
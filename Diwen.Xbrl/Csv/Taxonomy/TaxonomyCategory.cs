using System.Collections.Generic;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class TaxonomyCategory
    {
        /// <summary/>
        public string Code { get; set; } = string.Empty;
        /// <summary/>
        public string Name { get; set; } = string.Empty;
        /// <summary/>
        public string Description { get; set; } = string.Empty;
        /// <summary/>
        public Dictionary<string, Taxonomy> Taxonomies { get; set; }
    }
}

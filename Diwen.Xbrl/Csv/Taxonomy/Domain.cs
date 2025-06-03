using System.Collections.Generic;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class Domain
    {
        /// <summary/>
        public string Code { get; set; }
        /// <summary/>
        public string Description { get; set; }
        /// <summary/>
        public bool IsType { get; set; }
        /// <summary/>
        public Dictionary<string, Member> Members { get; set; }
        /// <summary/>
        public string XbrlCode { get { return $"eba_{Code}"; } }
    }
}

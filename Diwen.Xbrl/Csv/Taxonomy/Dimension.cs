namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class Dimension
    {
        /// <summary/>
        public string Code { get; set; }
        /// <summary/>
        public string Domain { get; set; }
        /// <summary/>
        public string DefaultValue { get; set; }
        /// <summary/>
        public string Description { get; set; }
        /// <summary/>
        public string XbrlCode { get { return $"eba_{Code}"; } }
    }
}

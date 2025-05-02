namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class Member
    {
        /// <summary/>
        public string Code { get; set; }
        /// <summary/>
        public string Description { get; set; }
        /// <summary/>
        public string XbrlCode { get { return $"eba_{Code}"; } }
    }
}

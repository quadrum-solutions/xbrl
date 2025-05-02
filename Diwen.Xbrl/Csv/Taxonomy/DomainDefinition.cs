using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class DomainDefinition
    {
        /// <summary/>
        public static Dictionary<string, Domain> Domains { get; set; }

        /// <summary/>
        public static Dictionary<string, Domain> DomainDefinitions(string path = "")
        {
            Domains = [];

            var pathDim = $"{path}www.eba.europa.eu/eu/fr/xbrl/crr/dict/dom/";
            string[] files = Directory.GetFiles(pathDim, "typ.xsd", SearchOption.AllDirectories).Union(Directory.GetFiles(pathDim, "exp.xsd", SearchOption.AllDirectories)).ToArray();
            foreach (var file in files)
            {
                var reader = new StreamReader(file);
                XmlSchema schema = XmlSchema.Read(reader, ValidationCallback);

                foreach (var element in schema.Items)
                {
                    if (element is XmlSchemaElement xmlElement)
                    {
                        Domains.TryAdd(xmlElement.Id, new Domain()
                        {
                            Code = xmlElement.Name,
                            IsType = file.Contains("typ.xsd"),
                        });
                    }
                }
            }

            files = Directory.GetFiles(pathDim, "typ-lab-en.xml", SearchOption.AllDirectories).Union(Directory.GetFiles(pathDim, "exp-lab-en.xml", SearchOption.AllDirectories)).ToArray();
            foreach (var file in files)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                foreach (XmlNode node in doc.LastChild.FirstChild.ChildNodes)
                {
                    if (node.LocalName != "label")
                        continue;

                    var name = node.Attributes.GetNamedItem("xlink:label").InnerText.Replace("label_", "");

                    if (Domains.TryGetValue(name, out var domainValue))
                    {
                        domainValue.Description = node.InnerText;
                    }
                }
            }

            foreach (var domain in Domains.Values)
            {
                domain.Members = [];
                var file = $"{pathDim}/{domain.Code.ToLower()}/mem.xsd";

                if (!File.Exists(file))
                    continue;

                var reader = new StreamReader(file);
                XmlSchema schema = XmlSchema.Read(reader, ValidationCallback);

                foreach (var element in schema.Items)
                {
                    if (element is XmlSchemaElement xmlElement)
                    {
                        domain.Members.TryAdd(xmlElement.Id, new Member()
                        {
                            Code = xmlElement.Name
                        });
                    }
                }

                file = $"{pathDim}/{domain.Code.ToLower()}/mem-lab-en.xml";
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                foreach (XmlNode node in doc.LastChild.FirstChild.ChildNodes)
                {
                    if (node.LocalName != "label")
                        continue;

                    var name = node.Attributes.GetNamedItem("xlink:label").InnerText.Replace("label_", "");

                    if (domain.Members.TryGetValue(name, out var memberValue))
                    {
                        memberValue.Description = node.InnerText;
                    }
                }
            }
            return Domains;
        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }
    }
}

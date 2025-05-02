using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class DimensionDefinition
    {
        /// <summary/>
        public static Dictionary<string, Dimension> Dimensions { get; set; }

        private static string GetDimension(string path, string file)
        {
            var version = file.Replace(path, "").Replace("\\", "/").Split("/")[0].Replace("dim.xsd", "");
            return string.IsNullOrEmpty(version) ? "eba_dim" : $"eba_dim_{version}";
        } 

        /// <summary/>
        public static Dictionary<string, Dimension> DimensionDefinitions(string path = "")
        {
            Dimensions = [];

            var pathDim = $"{path}www.eba.europa.eu/eu/fr/xbrl/crr/dict/dim/";
            string[] files = Directory.GetFiles(pathDim, "dim.xsd", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var reader = new StreamReader(file);
                XmlSchema schema = XmlSchema.Read(reader, ValidationCallback);

                var dimension = GetDimension(pathDim, file);

                foreach (var element in schema.Items)
                {
                    if (element is XmlSchemaElement xmlElement)
                    {
                        var value = "";
                        var typedValue = xmlElement.UnhandledAttributes.FirstOrDefault(x => x.Name == "xbrldt:typedDomainRef");
                        if (typedValue != null)
                        {
                            value = typedValue.Value.Split('#')[1];
                        }

                        Dimensions.TryAdd($"{dimension}:{xmlElement.Name}", new Dimension()
                        {
                            Code = xmlElement.Name,
                            Domain = value
                        });
                    }
                }
            }

            files = Directory.GetFiles(pathDim, "dim-lab-en.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                var dimension = GetDimension(pathDim, file);

                foreach (XmlNode node in doc.LastChild.FirstChild.ChildNodes)
                {
                    if (node.LocalName != "label")
                        continue;

                    var name = node.Attributes.GetNamedItem("xlink:label").InnerText.Split('_').Last();

                    if (Dimensions.TryGetValue($"{dimension}:{name}", out var dimensionValue))
                    {
                        dimensionValue.Description = node.InnerText;
                    }
                }
            }

            files = Directory.GetFiles(pathDim, "dim-def.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                var dimension = GetDimension(pathDim, file);

                foreach (XmlNode node in doc.LastChild.LastChild.ChildNodes)
                {
                    if (node.LocalName != "definitionArc")
                        continue;

                    var isDomain = node.Attributes.GetNamedItem("xlink:arcrole").InnerText.EndsWith("dimension-domain");
                    var name = node.Attributes.GetNamedItem("xlink:from").InnerText.Split('_').Last();
                    var value = node.Attributes.GetNamedItem("xlink:to").InnerText;

                    if (Dimensions.TryGetValue($"{dimension}:{name}", out var dimensionValue))
                    {
                        if (isDomain)
                        {
                            dimensionValue.Domain = value.Replace("loc_dom_", "");
                        }
                        else
                        {
                            dimensionValue.DefaultValue = value.Split('_')[2];
                        }
                    }
                }
            }
            return Dimensions;
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

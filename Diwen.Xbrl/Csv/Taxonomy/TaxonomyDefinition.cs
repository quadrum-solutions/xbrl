using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using System.Xml;
using System.Linq;
using System;
using System.Text.Json;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    /// <summary/>
    public class TaxonomyDefinition
    {
        /// <summary/>
        public static Dictionary<string, TaxonomyCategory> Taxonomies { get; set; }

        /// <summary/>
        public static Dictionary<string, TaxonomyCategory> TaxonomieDefinitions(string path = "")
        {
            Taxonomies = [];

            var pathDim = $"{path}www.eba.europa.eu/eu/fr/xbrl/crr/fws/";
            string[] files = Directory.GetFiles(pathDim, "*.xsd", SearchOption.AllDirectories).Where(x => x.Contains("mod")).ToArray();

            foreach (var file in files)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                var headerVersion = "";
                using (XmlReader readerHeader = XmlReader.Create(file, settings))
                {
                    while (readerHeader.Read())
                    {
                        if (readerHeader.NodeType == XmlNodeType.ProcessingInstruction && readerHeader.Name.Equals("taxonomy-version"))
                            headerVersion = readerHeader.Value;

                        if (readerHeader.NodeType == XmlNodeType.Element && readerHeader.LocalName == "schema")
                            break;
                    }
                }

                var reader = new StreamReader(file);
                XmlSchema schema = XmlSchema.Read(reader, ValidationCallback);

                foreach (var element in schema.Items)
                {
                    if (element is XmlSchemaElement xmlElement)
                    {
                        if (xmlElement.Id.Equals("eba_COREP_ALM_Ind"))
                        {
                            var test = "";
                        }
                        Taxonomies.TryAdd(xmlElement.Id, new TaxonomyCategory()
                        {
                            Code = xmlElement.Id,
                            Name = xmlElement.Name,
                            Taxonomies = [],
                        });

                        var jsonFile = file.Replace(".xsd", ".json");
                        var entrypoint = file.Replace(path, "http://").Replace(@"\", "/");

                        if (File.Exists(jsonFile))
                        {
                            var stream = new FileStream(jsonFile, FileMode.Open, FileAccess.Read);
                            var json = JsonSerializer.Deserialize<ModuleDefinition>(stream);
                            var ebaDoc = json.DocumentInfo.EbaDocumentation;
                            Taxonomies[xmlElement.Id].Taxonomies.TryAdd($"{xmlElement.Name}-{ebaDoc.ModuleVersion}", new Taxonomy()
                            {
                                Version = ebaDoc.ModuleVersion,
                                Name = xmlElement.Name,
                                FromDate = ebaDoc.FromReferenceDate,
                                ToDate = ebaDoc.ToReferenceDate,
                                EntryPoint = entrypoint,
                            });
                        }
                        else
                        {
                            var version = xmlElement.UnhandledAttributes.FirstOrDefault(x => x.Name == "model:version")?.InnerText ?? headerVersion;
                            Taxonomies[xmlElement.Id].Taxonomies.TryAdd($"{xmlElement.Name}-{version}", new Taxonomy()
                            {
                                Version = version,
                                Name = xmlElement.Name,
                                FromDate = xmlElement.UnhandledAttributes.FirstOrDefault(x => x.Name == "model:fromDate")?.InnerText,
                                ToDate = xmlElement.UnhandledAttributes.FirstOrDefault(x => x.Name == "model:toDateDate")?.InnerText,
                                EntryPoint = entrypoint,
                            });
                        }
                    }
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(file.Replace(".xsd", "-lab-en.xml"));

                foreach (XmlNode node in doc.LastChild.FirstChild.ChildNodes)
                {
                    if (node.LocalName != "label")
                        continue;

                    var name = node.Attributes.GetNamedItem("xlink:label").InnerText.Replace("label_", "");

                    if (Taxonomies.TryGetValue(name, out var taxonomyValue))
                    {
                        taxonomyValue.Description = node.InnerText;
                    }
                }
            }
            return Taxonomies;
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

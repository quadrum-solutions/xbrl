namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class ModuleDefinition
    {
        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        /// <summary/>
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        /// <summary/>
        [JsonPropertyName("parameterURL")]
        public string ParameterURL { get; set; }

        /// <summary/>
        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary/>
        [JsonPropertyName("tables")]
        public Dictionary<string, Table> Tables { get; set; }

        /// <summary/>
        public static ModuleDefinition FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<ModuleDefinition>(stream);
        }

        private Dictionary<string, TableDefinition> tableDefinitions;

        /// <summary/>
        public Dictionary<string, TableDefinition> TableDefinitions(string path = "")
        {
            if (tableDefinitions == null)
            {
                tableDefinitions = [];
                var modfolder = Path.GetDirectoryName(DocumentInfo.Taxonomy.First().Replace("http://", path));

                foreach (var moduleTable in DocumentInfo.Extends)
                {
                    var tabfile = Path.GetFullPath(Path.Combine(modfolder, moduleTable));
                    if (File.Exists(tabfile))
                    {
                        var tablecode = Path.GetFileNameWithoutExtension(tabfile);

                        using var stream = new FileStream(tabfile, FileMode.Open, FileAccess.Read);
                        var jsonTable = JsonSerializer.Deserialize<TableDefinition>(stream);

                        using var streamDora = new FileStream(tabfile, FileMode.Open, FileAccess.Read);
                        var jsonTableDora = JsonSerializer.Deserialize<DoraTableDefinition>(streamDora);

                        if (jsonTable.TableTemplates?.FirstOrDefault().Value?.Columns?.Datapoint == null)
                        {
                            jsonTable.TableTemplates.First().Value.Columns.Datapoint = new Datapoint
                            {
                                PropertyGroups = jsonTableDora.TableTemplates.First().Value.PropertyGroups
                            };
                        }
                        else
                        {
                            foreach (var dp in jsonTableDora.Datapoints)
                            {
                                jsonTable.TableTemplates.First().Value.Columns.Datapoint.PropertyGroups.Add(dp.Key, dp.Value);
                            }
                        }

                        tableDefinitions.Add(tablecode, jsonTable);
                    }
                }
            }

            return tableDefinitions;
        }
    }
}
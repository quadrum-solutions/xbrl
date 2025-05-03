namespace Diwen.Xbrl.Tests.Inline
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Linq;
    using Diwen.Xbrl.Inline;
    using Diwen.Xbrl.Xml;
    using Xunit;

    public static class InlineXbrlTests
    {

        private static XDocument XDocumentFromZipArchiveEntry(ZipArchiveEntry entry)
        {
            using (var reportStream = entry.Open())
                return XDocument.Load(reportStream);
        }

        [Fact]
        public static void InlineXbrlMultipleSchemaRefs()
        {
            var package = Path.Combine("data", "AR-example.zip");
            XDocument reportDocument;
            using (var reportStream = File.OpenRead(package))
            using (var reportArchive = new ZipArchive(reportStream, ZipArchiveMode.Read))
            {
                var reportFile = reportArchive.Entries.FirstOrDefault(e => e.Name == "AR-example.xhtml");
                reportDocument = XDocumentFromZipArchiveEntry(reportFile);
            }

            var report = InlineXbrl.ParseXDocuments(reportDocument);
            Assert.Equal(3, report.SchemaReferences.Count);
            
            var outputfile = Path.ChangeExtension(package, "xbrl");
            report.ToFile(outputfile);
            var roundtrip = Report.FromFile(outputfile);
            Assert.Equal(3, roundtrip.SchemaReferences.Count);

        }
    }
}
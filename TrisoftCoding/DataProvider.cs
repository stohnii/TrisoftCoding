using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace TrisoftCoding
{
    public class DataProvider : IDataProvider
    {
        private readonly string _path = @"C:\temp";
        private readonly List<string> _searchPatterns = new List<string> { "*.xml", "*.xsl", "*.xslt" };

        public void CreateBuckup(string filePath)
        {
            File.Move(filePath, $"{filePath}.bac", true);
        }

        public IEnumerable<string> GetFilePathes()
        {
            return _searchPatterns.SelectMany(p => Directory.EnumerateFiles(_path, p));
        }

        public XmlDocument LoadDocument(string filePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);
            return xmlDocument;
        }

        public async Task Save(string filePath, byte[] data)
        {
            await File.WriteAllBytesAsync(filePath, data);
        }
    }
}

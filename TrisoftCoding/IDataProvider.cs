using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace TrisoftCoding
{
    public interface IDataProvider
    {
        IEnumerable<string> GetFilePathes();
        Task Save(string filePath, byte[] data);
        void CreateBuckup(string filePath);
        XmlDocument LoadDocument(string filePath);
    }
}

using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace TrisoftCoding.Tests
{
    public class Tests
    {
        private readonly DataUpdater _updater = new DataUpdater();
        private readonly Mock<IDataProvider> _provider = new Mock<IDataProvider>();

        [SetUp]
        public void Setup()
        {
            string xml1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><test title = \"This Trisoft will be updated\"><line1 title = \"This Trisoft will be updated\" >Trisoft has been renamed to Trisoft </line1></test>";
            XmlDocument doc1 = new XmlDocument();
            doc1.LoadXml(xml1);

            string xml2 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><test title = \"This SDL Trisoft will be updated\"><line1 title = \"This SDL Trisoft will be updated\" >SDL Trisoft has been renamed to SDL Trisoft </line1></test>";
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(xml2);

            _provider.Setup(x => x.GetFilePathes()).Returns(new List<string> { "path1", "path2" });
            _provider.Setup(x => x.LoadDocument(It.Is<string>(x => x.Equals("path1")))).Returns(doc1);
            _provider.Setup(x => x.LoadDocument(It.Is<string>(x => x.Equals("path2")))).Returns(doc2);

            _provider.Setup(x => x.CreateBuckup(It.IsAny<string>())).Verifiable();
            _provider.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<byte[]>())).Verifiable();

            _updater.DataProvider = _provider.Object;
        }

        [Test]
        public async Task SimpleTest()
        {
            await _updater.UpdateFiles();

            _provider.Verify(mock => mock.CreateBuckup(It.IsAny<string>()), Times.Once());
            _provider.Verify(mock => mock.Save(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once());
        }
    }
}
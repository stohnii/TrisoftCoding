using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace TrisoftCoding
{
    public class DataUpdater : IDataUpdater
    {
        private const int MaxTasks = 10;
        private const string NewValue = "SDL Trisoft";
        private const string OldValue = "Trisoft";
        private const string AttributeToCheck = "title";

        public IDataProvider DataProvider { get; set; } = new DataProvider();

        public async Task UpdateFiles()
        {
            var files = DataProvider.GetFilePathes();

            if (!files.Any())
                return;

            List<Task> tasks = new List<Task>(MaxTasks);

            foreach (string file in files)
            {
                if (tasks.Count < MaxTasks)
                    tasks.Add(UpdateFileContent(file));

                if (tasks.Count == MaxTasks)
                    await WaitAny(tasks);
            }

            while (tasks.Count > 0)
            {
                await WaitAny(tasks);
            }
        }

        private async Task UpdateFileContent(string filePath)
        {
            bool fileUpdated = false;
            XmlDocument xmlDocument = DataProvider.LoadDocument(filePath);

            XmlNode root = xmlDocument.DocumentElement;
            ProcessData(root, ref fileUpdated);

            if (fileUpdated)
            {
                DataProvider.CreateBuckup(filePath);
                using (MemoryStream ms = new MemoryStream())
                {
                    xmlDocument.Save(ms);
                    await DataProvider.Save(filePath, ms.ToArray());
                }
            }
        }

        private void ProcessData(XmlNode node, ref bool fileUpdated)
        {
            Replace(node, ref fileUpdated);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                ProcessData(childNode, ref fileUpdated);
            }
        }

        private void Replace(XmlNode node, ref bool fileUpdated)
        {
            string attributeValue = node.Attributes?[AttributeToCheck]?.Value;

            if (!String.IsNullOrEmpty(attributeValue))
            {
                string newAttrValue = ReplaceInString(attributeValue);

                if (!fileUpdated)
                    fileUpdated = !node.Attributes[AttributeToCheck].Value.Equals(newAttrValue);
                node.Attributes[AttributeToCheck].Value = newAttrValue;
            }

            if (!string.IsNullOrEmpty(node.Value))
            {
                string newValue = ReplaceInString(node.Value);

                if (!fileUpdated)
                    fileUpdated = !node.Value.Equals(newValue);

                node.Value = newValue;
            }
        }

        private string ReplaceInString(string value)
        {
            value = value.Replace(NewValue, OldValue);
            return value.Replace(OldValue, NewValue);
        }

        private async Task WaitAny(List<Task> tasks)
        {
            Task finishedTask = await Task.WhenAny(tasks);
            tasks.Remove(finishedTask);

            if (finishedTask.IsFaulted)
                Console.WriteLine(finishedTask.Exception.Message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using DirectoryTreeExplorer.Business.LockFreeQueue;
using DirectoryTreeExplorer.Business.Logs;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Represents a logic for creation a xml file. 
    /// </summary>
    public sealed class XmlCreator
    {
        private readonly ILogProvider _logProvider;


        public XmlCreator(ILogProvider logProvider = null)
        {
            _logProvider = logProvider;
        }

        public void Create(string path, IQueue<DirectoryElement> directoryElements, Func<bool> isIterationActive)
        {
            var createDocumentThread = new Thread(() => CreateInternal(path, directoryElements, isIterationActive))
            {
                IsBackground = true
            };
            createDocumentThread.Start();
        }

        private void CreateInternal(string path, IQueue<DirectoryElement> directoryElements, Func<bool> isIterationActive)
        {
            if (Directory.Exists(path))
            {
                _logProvider?.Log("Path of xml-file is invalid.");
                return;
            }

            _logProvider?.Log("XML creation has been started.");

            const int rootLevelNumber = 0;

            var cachedLastXmlElements = new Dictionary<int, XElement>();
            var document = new XDocument();

            while (!directoryElements.IsEmpty || isIterationActive())
            {
                if (directoryElements.IsEmpty)
                    continue;

                directoryElements.TryDequeue(out var directoryElement);

                var xmlElement = CreateXmlElement(directoryElement);

                if (directoryElement.Kind != DirectoryElementKind.Root)
                {
                    cachedLastXmlElements[directoryElement.Level - 1].Add(xmlElement);
                }

                UpdateDirectoriesSize(directoryElement, cachedLastXmlElements);

                cachedLastXmlElements[directoryElement.Level] = xmlElement;
            }

            cachedLastXmlElements.TryGetValue(rootLevelNumber, out var lastUsedXmlElement);

            document.Add(lastUsedXmlElement);

            document.Save(path);

            _logProvider?.Log($"XML creation has been finished. (File path: '{path}').");
        }

        private void UpdateDirectoriesSize(DirectoryElement directoryElement, Dictionary<int, XElement> cachedLastXmlElements)
        {
            if (directoryElement.Kind == DirectoryElementKind.File)
            {
                foreach (var lastXmlElement in cachedLastXmlElements)
                {
                    if (lastXmlElement.Key < directoryElement.Level)
                    {
                        var sizeXmlAttribute = lastXmlElement.Value.Attribute("Size");

                        if (sizeXmlAttribute != null)
                        {
                            sizeXmlAttribute.Value = (Convert.ToInt64(sizeXmlAttribute.Value) + directoryElement.Size).ToString();
                        }
                    }
                }
            }
        }

        private XElement CreateXmlElement(DirectoryElement directoryElement)
        {
            return
                new XElement(directoryElement.Kind.ToString(),
                    new XAttribute("Name", directoryElement.Name),
                    new XAttribute("CreationTime", directoryElement.CreationTime.ToString("yyyy-MM-dd")),
                    new XAttribute("ModificationTime", directoryElement.ModificationTime.ToString("yyyy-MM-dd")),
                    new XAttribute("LastAccessTime", directoryElement.LastAccessTime.ToString("yyyy-MM-dd")),
                    new XAttribute("Attributes", directoryElement.Attributes),
                    new XAttribute("Owner", directoryElement.Owner),
                    new XAttribute("Size", directoryElement.Size));
        }
    }
}

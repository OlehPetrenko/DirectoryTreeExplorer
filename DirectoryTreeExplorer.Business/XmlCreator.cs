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
        private Dictionary<int, XmlDirectoryItem> _cachedLastXmlElements;


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
            if (!File.Exists(path))
            {
                _logProvider?.Log("Path of XML-file is invalid.");
                return;
            }

            _logProvider?.Log("XML creation has been started.");

            const int rootLevelNumber = 0;

            _cachedLastXmlElements = new Dictionary<int, XmlDirectoryItem>();
            var document = new XDocument();

            while (!directoryElements.IsEmpty || isIterationActive())
            {
                if (directoryElements.IsEmpty)
                    continue;

                var directoryElement = directoryElements.Dequeue();

                var xmlElement = CreateXmlElement(directoryElement);

                if (directoryElement.Kind != DirectoryElementKind.Root)
                {
                    _cachedLastXmlElements[directoryElement.Level - 1].XmlElement.Add(xmlElement);
                }

                UpdateDirectoriesSize(directoryElement);
                AddDirectoryToCache(directoryElement, xmlElement);
            }

            _cachedLastXmlElements.TryGetValue(rootLevelNumber, out var rootXmlElement);
            SetDirectorySize(rootXmlElement);
            document.Add(rootXmlElement?.XmlElement);

            try
            {
                document.Save(path);
            }
            catch (Exception exception)
            {
                _logProvider?.Log(exception.Message);
                return;
            }

            _logProvider?.Log($"XML creation has been finished. (File path: '{path}').");
        }

        private void AddDirectoryToCache(DirectoryElement directoryElement, XElement xmlElement)
        {
            if (directoryElement.Kind != DirectoryElementKind.Directory && directoryElement.Kind != DirectoryElementKind.Root)
                return;

            _cachedLastXmlElements.TryGetValue(directoryElement.Level, out var cacheItem);
            SetDirectorySize(cacheItem);

            _cachedLastXmlElements[directoryElement.Level] = new XmlDirectoryItem(xmlElement);
        }

        private void SetDirectorySize(XmlDirectoryItem cacheItem)
        {
            var sizeAttribute = cacheItem?.XmlElement.Attribute("Size");

            if (sizeAttribute != null)
                sizeAttribute.Value = cacheItem.Size.ToString();
        }

        private void UpdateDirectoriesSize(DirectoryElement directoryElement)
        {
            if (directoryElement.Kind != DirectoryElementKind.File)
                return;

            foreach (var lastXmlElement in _cachedLastXmlElements)
            {
                if (lastXmlElement.Key >= directoryElement.Level)
                    continue;

                lastXmlElement.Value.Size += directoryElement.Size;
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

    public class XmlDirectoryItem
    {
        public XElement XmlElement { get; }
        public long Size { get; set; }

        public XmlDirectoryItem(XElement xmlElement, long size = 0)
        {
            XmlElement = xmlElement;
            Size = size;
        }
    }
}

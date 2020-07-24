using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Albion_RMT_Empire_Tool_Beta
{
    class CustomXMLReader
    {
        private XDocument itemsXml;
        private XDocument dataXml;
        public CustomXMLReader()
        {
            itemsXml = XDocument.Load(XmlReader.Create(Constants.URLStringItems));
            dataXml = XDocument.Load(XmlReader.Create(Constants.URLStringData));
        }

        public XmlReader GetItemsXml()
        {
            XmlReader xmlReader = XmlReader.Create(new StringReader((itemsXml.ToString())));
            return xmlReader;
        }

        public XmlReader GetDataXml()
        {
            XmlReader xmlReader = XmlReader.Create(new StringReader((dataXml.ToString())));
            return xmlReader;
        }
    }
}

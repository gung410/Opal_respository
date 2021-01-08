using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Conexus.Opal.Microservice.Metadata.Common
{
    /// <summary>
    /// To create XMLData for sql query. This class is cloned from old project so please don't confused about this class.
    /// Just use it!.
    /// </summary>
    public static class SqlXmlDataTypeHelper
    {
        public static string SerializeToXml(params string[] values)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(string[]));
            StringBuilder stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder))
            {
                xmlSerializer.Serialize(xmlWriter, values);
            }

            return stringBuilder.ToString();
        }

        public static string SerializeObjectToXml<T>(T obj)
        {
            string xml = null;
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, obj);
                xml = sw.ToString();
            }

            return xml;
        }

        public static string SerializeListOfObjectToXml<T>(IEnumerable<T> listOfObj)
        {
            return SerializeToXml(listOfObj.Select(p => SerializeObjectToXml(p)).ToArray());
        }
    }
}

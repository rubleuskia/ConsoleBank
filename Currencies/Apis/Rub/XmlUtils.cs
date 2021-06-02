using System.IO;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub
{
    public static class XmlUtils
    {
        public static T ParseXml<T>(string xml)
            where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using TextReader reader = new StringReader(xml);
            return serializer.Deserialize(reader) as T;
        }
    }
}

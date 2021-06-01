using System.IO;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub
{
    public static class XmlUtils
    {
        public static T Parse<T>(string xml)
            where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using TextReader reader = new StringReader(xml);
            var response = serializer.Deserialize(reader) as T;
            return response;
        }
    }
}

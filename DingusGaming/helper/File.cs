using System.IO;
using Rocket.Core.Logging;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DingusGaming.DingusGaming.helper
{
    class File
    {
        public static void writeToXml(object obj, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), "");
            using(XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                serializer.Serialize(xmlTextWriter, obj);
            }   
        }

        public static T readFromXml<T>(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using(XmlTextReader xmlTextReader = new XmlTextReader(fileName))
            {
                if (serializer.CanDeserialize(xmlTextReader))
                    return (T) serializer.Deserialize(xmlTextReader);
                return default(T);
            }
        }

        public static string readFromFile(string fileName)
        {
            try
            {
                return System.IO.File.ReadAllText(fileName);
            }
            catch (FileNotFoundException e)
            {
                Logger.LogError(fileName + " not found!");
                Logger.LogException(e);
                return null;
            }
        }
    }
}

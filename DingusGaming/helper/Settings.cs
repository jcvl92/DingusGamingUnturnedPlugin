using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DingusGaming.DingusGaming.helper
{
    class Settings
    {
        private const string SETTINGS_FILE = "settings.yml";
        private static Dictionary<string, string> settings = null;

        public static Dictionary<string, string> getSettings()
        {
            if (settings == null)
            {
                readSettings();
            }
            return settings;
        }

        private static void readSettings()
        {
            var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
            var input = new StringReader(File.readFromFile(SETTINGS_FILE));

            settings = deserializer.Deserialize<Dictionary<string, string>>(input);
        }
    }
}

using DingusGaming.DingusGaming.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DingusGaming.DingusGaming.data
{
    class DataAccessFactory
    {
        private static string DB_SETTING = Settings.getSettings()["db.type"];
        public static DataAccess getDataAccessor()
        {
            switch (DB_SETTING)
            {
                case "dynamo": return new Dynamo();
                case "xml": return new Xml();  
                default: throw new ArgumentException("Invalid database setting (db.type): " + DB_SETTING);
            }
        }
    }
}

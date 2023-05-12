using System;
using System.Collections.Generic;
using System.Configuration;
using apiTicket.Models;


namespace apiTicket.Utils
{

    public class Util
    {

        public static string[] ObtainConfig(string value)
        {
            ExeConfigurationFileMap customConfigFileMap = new ExeConfigurationFileMap();
            customConfigFileMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + "Messages.xml";
            Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(customConfigFileMap, ConfigurationUserLevel.None);
            AppSettingsSection appSettings = (customConfig.GetSection("appSettings") as AppSettingsSection);
            return appSettings.Settings[value].Value.Split(";");
        }
        public static string ObtainConfigAWS(string value)
        {
            ExeConfigurationFileMap customConfigFileMap = new ExeConfigurationFileMap();
            customConfigFileMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + "Messages.xml";
            Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(customConfigFileMap, ConfigurationUserLevel.None);
            AppSettingsSection appSettings = (customConfig.GetSection("appSettings") as AppSettingsSection);
            return appSettings.Settings[value].Value.ToString();
        }
        public bool CheckString(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckDecimal(Decimal? value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckOperacion(int? value)
        {
            if (value == 1 || value == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckList<T>(List<T> lista)
        {
            if (lista == null || lista.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<List<TicketSGC>> SplitList(List<TicketSGC> locations, int nSize = 30)
        {
            var list = new List<List<TicketSGC>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Discord.Commands;
using System.Threading.Tasks;


namespace DiscordDestrucoBot
{
    class DataStorage
    {
        private static Dictionary<string, string> pairs = new Dictionary<string, string>();

        public static void AddPair(string key, string value)
        {
            pairs[key] = value;
            SaveData();
        }

        public static void RemoveKey(string key)
        {
            pairs.Remove(key);
            SaveData();
        }

        public static string GetPrefixValue(string key)
        {
            return pairs.GetValueOrDefault(key, Config.bot.defaultcmdPrefix);
        }

        public static string GetKeyValue(string key)
        {
            return pairs.GetValueOrDefault(key);
        }

        public static bool KeyExists(string key)
        {
            return pairs.ContainsKey(key);
        }

        public static int GetPairsCount()
        {
            return pairs.Count;
        }

        static DataStorage()
        {
            if (!ValidateStorageFile("Resources/DataStorage.json"))
                return;

            string json = File.ReadAllText("Resources/DataStorage.json");
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText("Resources/DataStorage.json", json);
        }

        private static bool ValidateStorageFile(string file)
        {
            if(!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;


        }

        
    }
}

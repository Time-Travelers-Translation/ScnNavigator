using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.Configuration.DataClasses;
using CrossCutting.Core.Serialization.JsonAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CrossCutting.Core.Configuration.File
{
    public class FileConfigurationRepository : IConfigurationRepository
    {
        public IEnumerable<ConfigCategory> Load()
        {
            var categories = GetConfigurationCategories();
            if (categories == null)
                yield break;

            var userCategories = GetUserConfigurationCategories();

            foreach (ConfigCategory category in categories)
            {
                var userCategory = userCategories?.FirstOrDefault(x => x.Name == category.Name);

                foreach (ConfigEntry entry in category.Entries)
                {
                    var userEntry = userCategory?.Entries.FirstOrDefault(x => x.Key == entry.Key);
                    if (userEntry != null)
                        entry.Value = userEntry.Value;

                    entry.Category = category;
                }

                yield return category;
            }
        }

        private List<ConfigCategory>? GetConfigurationCategories()
        {
            string cfgPath = GetConfigPath();
            if (!System.IO.File.Exists(cfgPath))
                return null;

            string json = System.IO.File.ReadAllText(cfgPath);
            if (string.IsNullOrEmpty(json))
                return null;

            JsonSerializer serializer = new();
            return serializer.Deserialize<List<ConfigCategory>>(json);
        }

        private List<ConfigCategory>? GetUserConfigurationCategories()
        {
            string cfgPath = GetUserConfigPath();
            if (!System.IO.File.Exists(cfgPath))
                return null;

            string json = System.IO.File.ReadAllText(cfgPath);
            if (string.IsNullOrEmpty(json))
                return null;

            JsonSerializer serializer = new();
            return serializer.Deserialize<List<ConfigCategory>>(json);
        }

        private string GetConfigPath()
        {
            return Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "config.json");
        }

        private string GetUserConfigPath()
        {
            return Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "config.json.user");
        }
    }
}

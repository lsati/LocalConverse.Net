using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LocalConverseClient.Net.Settings
{
    internal class LocalConverseSettings
    {
        public LocalConverseSettings()
        {
        }

        public string ModelDirectory { get; set; }

        public string LastSelectedModelName { get; set; }

        [JsonIgnore]
        public List<string> LoadedModels { get; set; }

        internal static LocalConverseSettings GetDefault()
        {
            var settings = new LocalConverseSettings()
            {
                ModelDirectory = ".\\Models\\",
                LastSelectedModelName = String.Empty
            };

            return settings;
        }

        public static List<string> LoadModels(string modelDirectory)
        {
            // Load models
            if (!Directory.Exists(modelDirectory))
                Directory.CreateDirectory(modelDirectory);

            var files = Directory.GetFiles(modelDirectory, "*.gguf");
            return files.ToList();
        }
    }

    internal static class LocalConverseSettingsManager
    {
        private static LocalConverseSettings settings;

        public static LocalConverseSettings GetInstance()
        {
            if (settings == null)
            {
                Trace.TraceInformation("loading default settings");
                settings = LocalConverseSettings.GetDefault();
            }

            settings.LoadedModels = LocalConverseSettings.LoadModels(settings.ModelDirectory);
            return settings;
        }
    }
}

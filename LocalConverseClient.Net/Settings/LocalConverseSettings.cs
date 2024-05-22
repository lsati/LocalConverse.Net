using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace LocalConverseClient.Net.Settings
{
    public class LocalConverseSettings
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
        private const string SettingsFile = "LocalConverse.settings";

        private static LocalConverseSettings settings;

        public static LocalConverseSettings GetInstance()
        {
            // try to load from disk
            if (File.Exists(SettingsFile))
            {
                settings = JsonConvert.DeserializeObject<LocalConverseSettings>(File.ReadAllText(SettingsFile));
            }

            if (settings == null)
            {
                Trace.TraceInformation("loading default settings");
                settings = LocalConverseSettings.GetDefault();
            }

            settings.LoadedModels = LocalConverseSettings.LoadModels(settings.ModelDirectory);
            return settings;
        }

        public static void SaveInstance()
        {
            var text = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFile, text);
        }
    }
}

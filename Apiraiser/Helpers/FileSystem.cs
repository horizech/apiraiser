using System;
using System.IO;

using System.Text.Json;

using Apiraiser.Services;
using Apiraiser.Constants;

namespace Apiraiser.Helpers
{
    public static class FileSystem
    {

        public static string GetCurrentDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetFullPath(string subPath)
        {
            return Path.Combine(GetCurrentDirectory(), subPath);
        }

        public static string GetPathInConfigurations(string name)
        {
            return Path.Combine(GetFullPath(Config.ConfigurationDirectory), name);
        }

        public static string ReadFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                ServiceManager
                    .Instance
                    .GetService<LogService>()
                    .Print(e.Message, Enums.LoggingLevel.Errors);

                return null;
            }
        }

        public static T ReadJsonString<T>(string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (Exception e)
            {
                ServiceManager
                    .Instance
                    .GetService<LogService>()
                    .Print(e.Message, Enums.LoggingLevel.Errors);

                return default(T);
            }
        }
    }
}

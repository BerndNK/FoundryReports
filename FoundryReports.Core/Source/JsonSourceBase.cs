using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoundryReports.Core.Source
{
    public class JsonSourceBase
    {
        protected static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
            {TypeNameHandling = TypeNameHandling.Objects, Formatting = Formatting.Indented};

        protected async Task<T> Deserialize<T>(string filePath) where T : class, new()
        {
            var content = await GetOrCreateFile(filePath);
            T? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<T>(content, SerializerSettings);
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
            }

            if (result == null)
            {
                result = new T();
                await WriteFile(filePath, JsonConvert.SerializeObject(result, SerializerSettings));
            }

            return result;
        }

        protected async Task WriteFile(string path, string content)
        {
            try
            {
                await File.WriteAllTextAsync(path, content);
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
            }
        }

        private async Task<string> GetOrCreateFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    return await File.ReadAllTextAsync(path);

                File.Create(path);

                return string.Empty;
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
                return string.Empty;
            }
        }
    }
}
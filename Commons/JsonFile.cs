using Newtonsoft.Json;
using System.IO;

namespace Commons
{
    public static class JsonFile
    {
        public static TResult Read<TResult>(string filePath)
        {
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(content))
                {
                    return JsonConvert.DeserializeObject<TResult>(content);
                }
            }

            return default(TResult);
        }

        public static void Write(string filePath, object content)
        {
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);

            File.WriteAllText(filePath, json);
        }
    }
}

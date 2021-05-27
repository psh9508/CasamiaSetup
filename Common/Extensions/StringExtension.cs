using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class StringExtension
    {
        private static void Save<T>(this T dataModel, string destPath)
        {
            string jsonText = JsonConvert.SerializeObject(dataModel);

            File.WriteAllText(destPath, jsonText);
        }

        public static T Deserialize<T>(this string path)
        {
            var jsonText = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(jsonText);
        }
    }
}

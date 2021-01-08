using System.IO;
using System.Reflection;

namespace LearnerApp.Helper
{
    public static class HtmlResourceHelper
    {
        public static readonly string HtmlCommonResourcesBasePath = "LearnerApp.Resources";

        public static string ReadFile(string resourcePath)
        {
            Assembly assembly = typeof(HtmlResourceHelper).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            string data = string.Empty;
            using (StreamReader reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }
    }
}

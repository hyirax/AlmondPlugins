using System.IO;
using System.Reflection;

namespace AlmondHousing.Util
{
    public static class EmbeddedResource
    {
        /// <summary>
        /// Reads an embedded resource as a string. Falls back to reading from disk.
        /// </summary>
        public static string ReadAllText(string resourceName, string diskPath = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            // Fallback: read from disk (for development)
            if (!string.IsNullOrEmpty(diskPath) && File.Exists(diskPath))
            {
                return File.ReadAllText(diskPath);
            }

            return null;
        }
    }
}
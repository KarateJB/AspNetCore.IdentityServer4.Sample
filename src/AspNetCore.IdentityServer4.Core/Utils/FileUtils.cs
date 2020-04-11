using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.IdentityServer4.Core.Utils
{
    /// <summary>
    /// File utility
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Read text from file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Text</returns>
        public static async Task<string> ReadFileAsync(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                using (var sr = new StreamReader(path))
                {
                    string text = await sr.ReadToEndAsync();
                    return text;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Save text to file
        /// </summary>
        /// <param name="path">File's path</param>
        /// <param name="text">Text</param>
        /// <param name="isAppend">Is append or overwrite</param>
        public static async Task SaveFileAsync(string path, string text, bool isAppend = false)
        {
            using (var sw = new StreamWriter(path, isAppend, Encoding.UTF8))
            {
                await sw.WriteLineAsync(text);
            }
        }
    }
}

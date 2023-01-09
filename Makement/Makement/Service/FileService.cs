using System.IO;
using System.Reflection;
using System.Text;

namespace Makement.Service
{
    public static class FileService
    {
        public static void Set(string filename, string content)
        {
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "/" + filename;

            if (File.Exists(directory))
            {
                File.SetAttributes(directory, FileAttributes.Normal);
            }

            using (StreamWriter writer = new StreamWriter(directory, false, Encoding.UTF8))
            {
                string crypt = CryptService.EncryptString(content);

                writer.WriteLine(crypt);
            }
            File.SetAttributes(directory, FileAttributes.Hidden);
        }

        public static string Get(string filename)
        {

            string content;
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "/" + filename;

            if (!File.Exists(directory))
                return null;

            File.SetAttributes(directory, FileAttributes.Normal);
            using (StreamReader writer = new StreamReader(directory))
            {
                content = writer.ReadToEnd();
            }
            File.SetAttributes(directory, FileAttributes.Hidden);

            return CryptService.DecryptString(content);
        }

        public static void Remove(string filename)
        {
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "/" + filename;

            File.SetAttributes(directory, FileAttributes.Normal);
            File.Delete(directory);
        }
    }
}

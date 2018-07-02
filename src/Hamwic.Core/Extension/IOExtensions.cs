using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Hamwic.Core.Extension
{
    public static class IOExtensions
    {
        public static void DeleteDirectory(this DirectoryInfo directory)
        {
            if (directory == null || !directory.Exists)
                return;

            foreach (var file in Directory.EnumerateFiles(directory.FullName))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directory.FullName))
                DeleteDirectory(new DirectoryInfo(subDirectory));

            directory.Delete(false);
        }

        public static void DeleteDirectory(this string directoryPath)
        {
            DeleteDirectory(new DirectoryInfo(directoryPath));
        }

        public static string GetLastBytes(this FileInfo file, long numberOfBytes)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var bytesToRead = Math.Min(reader.BaseStream.Length, numberOfBytes) * -1;
                reader.BaseStream.Seek(bytesToRead, SeekOrigin.End);
                return reader.ReadToEnd();
            }
        }

        public static string AssemblyDirectory(this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static Stream GetEmbeddedResourceAsStream(this Type rootType, string name, params string[] folders)
        {
            var resourcePath = string.Format("{0}.{1}{2}",
                rootType.Namespace,
                folders.Length == 0 ? "" : string.Join(".", folders) + ".",
                name);

            var stream = rootType.Assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
                throw new MissingManifestResourceException(
                    string.Format("Unable to locate embedded resource at the path '{0}'", resourcePath));

            return stream;
        }
    }
}
using System.IO;
using System.Text;

namespace Hamwic.Cif.Core.Extension
{
    public static class StreamExtensions
    {
        public static string ReadText(this Stream stream)
        {
            if (!PrepareStream(stream))
                return string.Empty;

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public static string ReadText(this Stream stream, Encoding encoding)
        {
            if (!PrepareStream(stream))
                return string.Empty;

            using (var reader = new StreamReader(stream, encoding))
                return reader.ReadToEnd();
        }

        private static bool PrepareStream(Stream stream)
        {
            if (stream == null || !stream.CanRead || stream.Length == 0)
                return false;

            if (stream.CanSeek && stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);
            else
                stream.Position = 0;

            return true;
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[bufferSize];
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
    }
}
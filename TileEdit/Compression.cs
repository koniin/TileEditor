using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileEdit
{
    public static class Compression
    {
        public static void Compress(string file, string content)
        {
            using (FileStream compressedFileStream = File.Create(file + ".gz"))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    using (var sw = new StreamWriter(compressionStream))
                    {
                        sw.Write(content);
                        sw.Flush();
                    }
                }
            }
        }

        public static IEnumerable<string> Decompress(string file)
        {
            using (FileStream originalFileStream = File.OpenRead(file))
            {
                List<string> result = new List<string>();
                using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress)) {
                    using (var sr = new StreamReader(decompressionStream)) {
                        while (!sr.EndOfStream) {
                            result.Add(sr.ReadLine());
                        }
                    }
                }
                return result;
            }
        }
    }
}

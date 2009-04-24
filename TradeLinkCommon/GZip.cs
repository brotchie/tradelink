using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace TradeLink.Common
{
    public class GZip
    {
        public static string Compress(string text)
        {
            // convert text to bytes
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            // get a stream
            MemoryStream ms = new MemoryStream();
            // get ready to zip up our stream
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                // compress the data into our buffer
                zip.Write(buffer, 0, buffer.Length);
            }
            // reset our position in compressed stream to the start
            ms.Position = 0;
            // get the compressed data
            byte[] compressed = ms.ToArray();
            ms.Read(compressed, 0, compressed.Length);
            // prepare final data with header that indicates length
            byte[] gzBuffer = new byte[compressed.Length + 4];
            //copy compressed data 4 bytes from start of final header
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            // copy header to first 4 bytes
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            // convert back to string and return
            return Convert.ToBase64String(gzBuffer);
        }

        public static string Uncompress(string compressedText)
        {
            // get string as bytes
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            // prepare stream to do uncompression
            MemoryStream ms = new MemoryStream();
            // get the length of compressed data
            int msgLength = BitConverter.ToInt32(gzBuffer, 0);
            // uncompress everything besides the header
            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
            // prepare final buffer for just uncompressed data
            byte[] buffer = new byte[msgLength];
            // reset our position in stream since we're starting over
            ms.Position = 0;
            // unzip the data through stream
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
            // do the unzip
            zip.Read(buffer, 0, buffer.Length);
            // convert back to string and return
            return Encoding.UTF8.GetString(buffer);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AppAzureBlob.Helper
{
    public static class Converters
    {
        public async static Task<byte[]> FileToByteArray(string fp)
        {
            if (string.IsNullOrEmpty(fp))
            {
                return null;
            }
            FileStream stream = File.Open(fp, FileMode.Open);
            byte[] bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            return bytes;
        }
    }
}

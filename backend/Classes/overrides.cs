using System.Net.Sockets;
using Assistant;
namespace overrides
{
    public static class extensionclass
    {
        public static async Task WriteAsync(this NetworkStream stream, byte[] msg)
        {
            try
            {
                await stream.WriteAsync(msg);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static string ArrayToString(this byte[] array)
        {
            string result = "{";
            foreach (var item in array)
            {
                result += item + ",";
            }
            result = result.Substring(0, result.Length - 1);
            result += "}";
            return result;
        }
        public static async Task<byte[]> ReadAsync(this NetworkStream stream)
        {
            byte[] data = new byte[Program.config.Buffersize];
            try
            {
                await stream.ReadAsync(data, 0, data.Length);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}

using System.Net.Sockets;
using System.Threading.Tasks;
namespace overrides
{
    public static class Extentions
    {
        public static byte booltobyte(this bool input)
        {
            return input ? (byte)1 : (byte)0;
        }
        public static string ArrayToString(this byte[] array)
        {
            if (array == null)
                return "";
            string result = "{";
            foreach (var item in array)
            {
                result += item + ",";
            }
            result = result.Substring(0, result.Length - 1);
            result += "}";
            return result;
        }
        public static async Task WriteAsync(this NetworkStream stream, byte[] msg)
        {
            try
            {
                await stream.WriteAsync(msg, 0, msg.Length);
            }
            catch (System.Exception e)
            {
                Godot.GD.Print(e);
                throw e;
            }
        }
        public static async Task<byte[]> ReadAsync(this NetworkStream stream, int buffer)
        {
            byte[] data = new byte[buffer];
            try
            {
                await stream.ReadAsync(data, 0, data.Length);
                return data;
            }
            catch (System.Exception e)
            {
                Godot.GD.Print(e);
                throw e;

            }
        }
    }
}

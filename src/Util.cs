using System.Text;

namespace SerdesNet
{
    static class Util
    {
        const string HexChars = "0123456789ABCDEF";
        public static string ConvertToHexString(byte[] bytes)
        {
            var result = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                result.Append(HexChars[b >> 4]);
                result.Append(HexChars[b & 0xf]);
            }

            return result.ToString();
        }
    }
}

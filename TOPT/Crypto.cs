using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TOTP
{
    internal static class Crypto
    {
        public static string GenerateTotp(string secret, long timeStep)
        {
            byte[] key = Base32Decode(secret);

            // 將時間步驟轉換為字節數組
            byte[] timeBytes = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeBytes);

            // 使用 HMAC-SHA1 算法計算
            using (var hmac = new HMACSHA1(key))
            {
                byte[] hash = hmac.ComputeHash(timeBytes);

                // 獲取動態截斷
                int offset = hash[hash.Length - 1] & 0xf;
                int binary =
                    ((hash[offset] & 0x7f) << 24) |
                    ((hash[offset + 1] & 0xff) << 16) |
                    ((hash[offset + 2] & 0xff) << 8) |
                    (hash[offset + 3] & 0xff);

                // 生成 6 位數 TOTP
                int totp = binary % 1000000;
                return totp.ToString("D6");
            }
        }

        private static byte[] Base32Decode(string base32)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            byte[] result = new byte[base32.Length * 5 / 8];
            int buffer = 0;
            int bitsLeft = 0;
            int resultIndex = 0;

            foreach (char c in base32.ToUpper())
            {
                int val = base32Chars.IndexOf(c);
                if (val < 0)
                    throw new ArgumentException("Invalid Base32 character");

                buffer <<= 5;
                buffer |= val & 31;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    result[resultIndex++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }
            return result;
        }
    }
}

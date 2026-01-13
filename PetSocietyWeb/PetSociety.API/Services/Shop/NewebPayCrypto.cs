using System.Security.Cryptography;
using System.Text;
using System.Linq;

public static class NewebPayCrypto
{
    // 1. 將參數轉為 Query String 格式 (Key=Value&Key2=Value2...) => 整理行李 (打包)
    public static string ToQueryString(Dictionary<string, string> source)
    {
        return string.Join("&", source.Select(x => $"{x.Key}={x.Value}"));
    }

    // 2. AES 加密 (核心鑰匙！) => 鎖進保險箱 (AES 加密)
    public static string EncryptAESHex(string source, string cryptoKey, string cryptoIV)
    {
        byte[] sourceBytes = Encoding.UTF8.GetBytes(source);

        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;             // 藍新規定使用 CBC 模式
            aes.Padding = PaddingMode.PKCS7;       // 藍新規定使用 PKCS7 填充
            aes.Key = Encoding.UTF8.GetBytes(cryptoKey);
            aes.IV = Encoding.UTF8.GetBytes(cryptoIV);

            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                byte[] encryptedData = encryptor.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
                // 轉成 Hex String (藍新要求大寫)
                return BitConverter.ToString(encryptedData).Replace("-", "").ToUpper();
            }
        }
    }

    // 3. SHA256 加密 (驗證簽章) => 貼上防偽封條 (數位簽章)
    public static string EncryptSHA256(string source)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] hashBytes = sha256.ComputeHash(sourceBytes);
            // 轉成 Hex String (藍新要求大寫)
            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
        }
    }

    // 4. AES 解密
    public static string DecryptAES256(string source, string key, string iv)
    {
        try
        {
            byte[] sourceBytes = StringToByteArray(source);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes).Trim();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解密失敗: {ex.Message}");
            return "";
        }
    }

    // 5. 輔助小工具
    private static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }
}

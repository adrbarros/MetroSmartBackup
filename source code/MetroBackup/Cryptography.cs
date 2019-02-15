using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MetroSmartBackup
{
    public class Cryptography
    {
        static string passPhrase = "<P@s5pHr@s&*_";
        static string saltValue = "$@LyV@lu3";
        static string hashAlgorithm = "MD5";
        static int passwordIterations = 1;
        static string initVector = ">>@1B*2C3D*4e5F67*";
        static int keySize = 256;

        static void Initialize(out byte[] initVectorBytes, out byte[] saltValueBytes)
        {
            initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
        }
        static byte[] GetKeyBytes(byte[] saltValueBytes)
        {
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            return password.GetBytes(keySize / 8);
        }

        public static string Encrypt(string plainText)
        {
            byte[] initVectorBytes, saltValueBytes;
            Initialize(out initVectorBytes, out saltValueBytes);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = GetKeyBytes(saltValueBytes);

            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }
        public static string Decrypt(string cipherText)
        {
            byte[] initVectorBytes, saltValueBytes;
            Initialize(out initVectorBytes, out saltValueBytes);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = GetKeyBytes(saltValueBytes);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }

        public static string GetMD5(string value)
        {
            var md5 = MD5.Create();
            byte[] valueBytes = Encoding.ASCII.GetBytes(value);
            byte[] computeHash = md5.ComputeHash(valueBytes);

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < computeHash.Length; i++)
                stringBuilder.Append(computeHash[i].ToString("X2"));

            return stringBuilder.ToString();
        }
    }
}
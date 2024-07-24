using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Models
{
    public class AesOperation
    {
        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Helpers.AesKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Helpers.AesKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }



        public static byte[] DecryptAes1(byte[] encryptedBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Helpers.AesKey);

                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Decrypt the bytes
                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (MemoryStream msPlainText = new MemoryStream())
                {
                    csDecrypt.CopyTo(msPlainText);
                    return msPlainText.ToArray();
                }


            }
        }

        public static byte[] DecryptAes1(byte[] encryptedBytes, byte[] encryptionBytesKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                //aesAlg.Key = Encoding.UTF8.GetBytes(Helpers.AesKey);
                //aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                //byte[] keyBytes = Convert.FromBase64String(encryptionKey);

                aesAlg.Key = encryptionBytesKey;

                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Decrypt the bytes
                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (MemoryStream msPlainText = new MemoryStream())
                {
                    csDecrypt.CopyTo(msPlainText);
                    return msPlainText.ToArray();
                }
            }
        }

        public static string DecryptAes(byte[] dataToDecrypt, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key.ToString());
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecrypt = new MemoryStream(dataToDecrypt))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }



        public static byte[] DecryptBytes(string encryptedData, string encryptionKey, string encryptionIV)
        {


            byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
            byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionIV);


            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;
                aesAlg.KeySize = 128;
                aesAlg.Mode = CipherMode.CBC; // Use the same mode as during encryption
                                              //aesAlg.Padding = PaddingMode.PKCS7; // Use the same padding as during encryption

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    }
                }


            }
        }

        public static string DecryptBytes1(string encryptedData, string encryptionKey, string encryptionIV)
        {
            var keyBytes = Encoding.UTF8.GetBytes("1234567890789456");
            var ivBytes = Encoding.UTF8.GetBytes("7894561230123456");
            var encryptedBytes = Convert.FromBase64String(encryptedData);
            string PlainTxt = string.Empty;

            using (var rMapped = new RijndaelManaged())
            {
                rMapped.Key = keyBytes;
                rMapped.IV = ivBytes;
                rMapped.KeySize = 128;
                rMapped.Padding = PaddingMode.PKCS7;
                rMapped.Mode = CipherMode.CBC; // Use the same mode as during encryption
                                               //aesAlg.Padding = PaddingMode.PKCS7; // Use the same padding as during encryption

                var decryptor = rMapped.CreateDecryptor(rMapped.Key, rMapped.IV);
                using (var memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var resultStream = new StreamReader(cryptoStream))
                        {
                            PlainTxt = resultStream.ReadToEnd();
                        }
                    }
                }


            }

            return PlainTxt;
        }


        public static string DecryptString123(string cipherText)
        {
            //byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("1234567890789456");
                aes.IV = Encoding.UTF8.GetBytes("7894561230123456");
                //aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }

        }




        public static string DecryptStringAES(string cipherText)
        {

            var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            string plaintext = null;
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

    }
}
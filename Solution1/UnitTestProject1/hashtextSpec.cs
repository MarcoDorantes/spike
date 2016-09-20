using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;

namespace UnitTestProject1
{
    [TestClass]
    public class hashtextSpec
    {
        [TestMethod]
        public void basic_text_hash()
        {
            var plaintext = "some words";
            var plaintext2 = TextHasher.DecryptString(TextHasher.EncryptString(plaintext));
            Assert.AreEqual<string>(plaintext, plaintext2);
        }
    }

    public static class TextHasher
    {
        private const string customPassPhrase = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.";

        public static string EncryptString(string Message)
        {
            byte[] results;

            var hashProvider = new MD5CryptoServiceProvider();
            var tripleDESAlgorithm = new TripleDESCryptoServiceProvider() { Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            tripleDESAlgorithm.Key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(customPassPhrase));

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(Message);

            try
            {
                ICryptoTransform encryptor = tripleDESAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                encryptor.Dispose();
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                tripleDESAlgorithm.Clear();
                hashProvider.Clear();
            }

            return Convert.ToBase64String(results);
        }
        public static string DecryptString(string message)
        {
            byte[] results;

            // Step 1. We hash the customPassPhrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(customPassPhrase));

            // Step 3. Setup the decoder
            var tripleDESAlgorithm = new TripleDESCryptoServiceProvider() { Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            tripleDESAlgorithm.Key = TDESKey;

            // Step 4. Convert the input string to a byte[]
            byte[] dataToDecrypt = Convert.FromBase64String(message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform decryptor = tripleDESAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                decryptor.Dispose();
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                tripleDESAlgorithm.Clear();
                hashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return Encoding.UTF8.GetString(results);
        }
    }
}
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace LicenseKeyGenerator.Core
{
    internal static class LicenseKeyManager
    {
        static byte[] SaltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        static LicenseKeyManager()
        {

        }

        public static void CreateKeyFile(string productKey, DateTime expireDate, string password)
        {
            try
            {
                if (expireDate <= DateTime.Now)
                    throw new Exception("Fecha inválida");
                if (string.IsNullOrEmpty(productKey))
                    throw new Exception("Debe ingresar una clave de producto");
                if (string.IsNullOrEmpty(password))
                    throw new Exception("Debe ingresar un contraseña");
                string content = string.Format("PRODUCTKEY={0}\r\nEXPIRYDATE={1}", productKey, expireDate);
                string encrypt = Encrypt(content, password);
                byte[] bytes = Encoding.UTF8.GetBytes(encrypt);
                string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "LicenseKey.key";
                var fileStream = File.Create(path);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new Exception("No se encontró contenido para encriptar");
            if (string.IsNullOrEmpty(password))
                throw new Exception("No se suministró una clave para realizar la encriptación");

            byte[] encryptedBytes = null;

            try
            {
                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        var key = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, 1000);

                        AES.KeySize = 256;
                        AES.BlockSize = 128;
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }

                        encryptedBytes = ms.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al Encriptar", e);
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}

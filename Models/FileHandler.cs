using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace PasteBin.Models
{
    public class FileHandler
    {
        private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("expireTime")]
        public DateTime ExpireTime { get; set; }

        [JsonPropertyName("isEncrypted")]
        public bool IsEncrypted { get; set; }

        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        public FileHandler(string name, DateTime expire, bool encrypt) 
        {
            FileName = name;
            ExpireTime = expire;
            IsEncrypted = encrypt;
        }

        public async void WriteFile(string filePath, string text, DateTime expire, bool encrypt)
        {
            await File.WriteAllTextAsync(filePath, text);

            string fileName = Path.GetFileName(filePath);
            FileHandler fileHandler = new(fileName, expire, encrypt);
            string jsonString = JsonSerializer.Serialize(fileHandler);
            string jsonFile = $"{fileName}.json";

            await File.WriteAllTextAsync(Path.Combine(Locations.JsonLocation, jsonFile), jsonString);
        }

        public async void WriteFileEncrypted(string filePath, string text, DateTime expire, bool encrypt, string password)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            string fileName = Path.GetFileName(filePath);
            

            FileHandler fileHandler = new(fileName, expire, encrypt);
            string jsonString = JsonSerializer.Serialize(fileHandler);
            string jsonFile = $"{fileName}.json";

            await File.WriteAllTextAsync(Path.Combine(Locations.JsonLocation, jsonFile), jsonString);


            using FileStream fileStream = File.Create(filePath);

            Rijndael rij = Rijndael.Create();
            
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
            rij.Key = pdb.GetBytes(32);
            rij.IV = pdb.GetBytes(16);

            using (CryptoStream CryptoStream = new CryptoStream(fileStream, rij.CreateEncryptor(), CryptoStreamMode.Write))
            {
                CryptoStream.Write(data, 0, data.Length);
            }
        }

        public string DecryptFile(string fileName, byte[] data, string password)
        {
            Rijndael rij = Rijndael.Create();

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password,SALT);
            rij.Key = pdb.GetBytes(32);
            rij.IV = pdb.GetBytes(16);
            
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rij.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                }
                return memoryStream.ToArray().ToString();
            }
        }
    }
}
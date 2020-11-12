using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;

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

        [JsonPropertyName("isImage")]
        public bool IsImage { get; set; }

        public static bool IfImage(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            if (fileExtension == ".jpg" || fileExtension == ".webp" || fileExtension == ".jpeg" )
            {
                return true;
            }
            return false;
        }
        public async void WriteFile(string filePath, string text, DateTime expire)
        {
            await File.WriteAllTextAsync(filePath, text);

            string fileName = Path.GetFileName(filePath);
            FileHandler fileHandler = new();
            fileHandler.FileName = filePath;
            fileHandler.ExpireTime = expire;
            fileHandler.IsEncrypted = false;
            fileHandler.IsImage = false;
            string jsonString = JsonSerializer.Serialize(fileHandler);
            string jsonFile = $"{fileName}.json";

            await File.WriteAllTextAsync(Path.Combine(Locations.JsonLocation, jsonFile), jsonString);
        }

        public async void UploadFile(IFormFile file)
        {
            using var fileStream = new FileStream(Path.Combine(Locations.FileLocation, file.FileName), FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(fileStream);
            
            FileHandler fileHandler = new();
            fileHandler.FileName = file.FileName;
            fileHandler.ExpireTime = DateTime.UtcNow.Add(TimeSpan.FromDays(365));
            fileHandler.IsEncrypted = false;
            fileHandler.IsImage = IfImage(file.FileName);

            string jsonString = JsonSerializer.Serialize(fileHandler);
            string jsonFile = $"{file.FileName}.json";

            await File.WriteAllTextAsync(Path.Combine(Locations.JsonLocation, jsonFile), jsonString);
        }

        public async void WriteFileEncrypted(string filePath, string text, DateTime expire, string password)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            string fileName = Path.GetFileName(filePath);
            

            FileHandler fileHandler = new();
            fileHandler.FileName = filePath;
            fileHandler.ExpireTime = expire;
            fileHandler.IsEncrypted = true;
            fileHandler.IsImage = false;
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
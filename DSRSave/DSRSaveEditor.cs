using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DSRSave
{
    public static class DSRSaveEditor
    {
        private const int SaveFileSize = 0x4204D0;
        private const int SaveSlotSize = 0x060030;
        private const int BaseSlotOffset = 0x02C0;
        private const int UserDataSize = 0x060020;
        private const int UserDataFileCount = 11;

        private static readonly byte[] Key = new byte[16] {
            0x01, 0x23, 0x45, 0x67,
            0x89, 0xAB, 0xCD, 0xEF,
            0xFE, 0xDC, 0xBA, 0x98,
            0x76, 0x54, 0x32, 0x10
        };

        public static IEnumerable<Character> ReadSave(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Save file not found", path);

            var saveData = File.ReadAllBytes(path);

            if (saveData.Length < SaveFileSize)
                throw new InvalidDataException("Invalid save file size.");

            var characters = new List<Character>();

            for (int i = 0; i < UserDataFileCount; i++)
            {
                int offset = BaseSlotOffset + i * SaveSlotSize;

                byte[] iv = new byte[16];
                Array.Copy(saveData, offset, iv, 0, 16);

                byte[] encrypted = new byte[UserDataSize];
                Array.Copy(saveData, offset + 16, encrypted, 0, UserDataSize);

                byte[] decrypted = DecryptAesCbc(encrypted, Key, iv);
                characters.Add(new Character(decrypted, i));
            }

            return characters;
        }

        public static void WriteSave(this IEnumerable<Character> characters, string originalPath)
        {
            var saveData = File.ReadAllBytes(originalPath);
            if (saveData.Length < SaveFileSize)
                throw new InvalidDataException("Invalid save file size.");

            foreach (var character in characters)
            {
                int offset = BaseSlotOffset + character.SlotNumber * SaveSlotSize;

                byte[] iv = new byte[16];
                Array.Copy(saveData, offset, iv, 0, 16);

                byte[] encrypted = EncryptAesCbc(character.GetRawData(), Key, iv);
                byte[] checksum = CalculateMD5(encrypted);

                Array.Copy(checksum, 0, saveData, offset, 16);
                Array.Copy(encrypted, 0, saveData, offset + 16, encrypted.Length);
            }

            File.WriteAllBytes(originalPath, saveData);
        }

        private static byte[] DecryptAesCbc(byte[] cipherData, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(cipherData, 0, cipherData.Length);
        }

        private static byte[] EncryptAesCbc(byte[] plainData, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
        }

        private static byte[] CalculateMD5(byte[] data)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(data);
        }
    }
}

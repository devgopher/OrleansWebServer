using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Utils.Crypto
{
    public static class HashUtils
    {
        private static readonly HashAlgorithm hashAlgorithm = MD5.Create();
        private static BinaryFormatter binaryFormatter = new BinaryFormatter();

        public static byte[] HashForFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"Can't find file {filePath}!");

            var fileContents = File.ReadAllBytes(filePath);
            return hashAlgorithm.ComputeHash(fileContents);
        }

        public static byte[] HashForBytes(byte[] input)
        {
            if (input?.Length == 0)
                throw new InvalidOperationException($"input is  null or empty!");

            return hashAlgorithm.ComputeHash(input);
        }

        public static byte[] HashForAssembly(Assembly input)
        {
            if (input == null)
                throw new InvalidOperationException($"input is null!");

            using (MemoryStream stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, input);
                var bytes = stream.ToArray();
                return hashAlgorithm.ComputeHash(bytes);
            }
        }

        public static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1 == default || hash2 == default)
                return false;
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; ++i)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }
    }
}

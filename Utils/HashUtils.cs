﻿using System;
using System.Text;

namespace CustomSettingsAndLayouts.Utils
{
    public static class HashUtils
    {
        private static System.Security.Cryptography.SHA1 hash = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        public static int GetID(string strText)
        {
            if (string.IsNullOrEmpty(strText)) return 0;

            byte[] byteContents = Encoding.Unicode.GetBytes(strText);
            byte[] hashText = hash.ComputeHash(byteContents);
            uint hashCodeStart = BitConverter.ToUInt32(hashText, 0);
            uint hashCodeMedium = BitConverter.ToUInt32(hashText, 8);
            uint hashCodeEnd = BitConverter.ToUInt32(hashText, 16);
            var hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            return BitConverter.ToInt32(BitConverter.GetBytes(uint.MaxValue - hashCode), 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Testflow.Utility.I18nUtil
{
    internal class I18NOptionComparer : IEqualityComparer<I18NOption>
    {
        public bool Equals(I18NOption elem1, I18NOption elem2)
        {
            return elem1.FirstLanguageFile.Equals(elem2.FirstLanguageFile) && elem1.SecondLanguageFile.Equals(elem2.SecondLanguageFile) &&
                elem1.FirstLanguage.Equals(elem2.FirstLanguage) && elem1.SecondLanguage.Equals(elem2.SecondLanguage);
        }

        public int GetHashCode(I18NOption option)
        {
            SHA1 sha1 = SHA1.Create(option.Name);
            byte[] hashBytes = sha1.Hash;
            int size = hashBytes.Length;
            sha1.Dispose();
            if (size > sizeof(int))
            {
                size = sizeof(int);
            }
            int[] hashValue = new int[1];
            Buffer.BlockCopy(hashBytes, 0, hashValue, 0, size);
            return hashValue[0];
        }
    }
}
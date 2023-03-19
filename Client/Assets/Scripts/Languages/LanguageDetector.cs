using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Languages
{
    public static class LanguageDetector
    {
        public static LanguageCode DetectLanguage(string text)
        {
            if (IsSimplifiedChinese(text))
                return LanguageCode.cn;
            else if (IsFrench(text))
                return LanguageCode.fr;
            else
                return LanguageCode.en;
        }

        public static bool IsSimplifiedChinese(string str)
        {
            foreach (char c in str)
                if (c >= 0x4e00 && c <= 0x9fff)
                    return true; // The character is in the range of Simplified Chinese characters

            // None of the characters are in the range of Simplified Chinese characters
            return false;
        }

        public static bool IsFrench(string str)
        {
            foreach (char c in str)
                if (c >= 'À' && c <= 'ö') // French letters are in this range
                    return true;

            return false;
        }

    }
}

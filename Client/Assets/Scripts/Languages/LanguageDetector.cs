using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Languages
{
    public class LanguageDetector
    {
        public string DetectLanguage(string text)
        {
            if (IsSimplifiedChinese(text))
                return "cn";
            else if (IsFrench(text))
                return "fr";
            else
                return "en";
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

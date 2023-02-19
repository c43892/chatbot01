using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Languages
{
    public class LanguageManager
    {
        readonly Dictionary<string, LanguageInfo> languages;
        public string DefaultLangauge { get; private set; }

        public LanguageManager(string defaultLang, Dictionary<string, LanguageInfo> langs)
        {
            languages = langs;
            DefaultLangauge = defaultLang;
        }

        public LanguageInfo DefaultLanguageInfo {
            get => languages[DefaultLangauge];
        }

        public LanguageInfo this[string langCode]
        {
            get => languages[langCode];
        }

        public IEnumerable<string> AllLanguages
        {
            get => languages.Keys;
        }
    }
}

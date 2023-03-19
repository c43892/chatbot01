using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Languages
{
    public enum LanguageCode
    {
        en,
        cn,
        fr,
        ja,
    }

    public class LanguageManager
    {
        public LanguageCode[] Languages { get; private set; }

        public LanguageManager(IEnumerable<LanguageCode> allSupported)
        {
            Languages = allSupported.ToArray();
        }
    }
}

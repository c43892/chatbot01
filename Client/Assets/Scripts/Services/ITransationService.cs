using Assets.Scripts.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface ITransationService
    {
        void Translate(string srcText, LanguageCode srcLang, LanguageCode dstLang, Action<string> onResponse, Action<string> onError);
    }
}

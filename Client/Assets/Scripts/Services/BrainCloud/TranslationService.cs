using Assets.Scripts.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    class TranslationService : BrainCloudServiceService, ITransationService
    {
        internal TranslationService(BrainCloudWrapper wrapper) : base(wrapper) { }

        string GetLanguageName(LanguageCode code)
        {
            switch (code)
            {
                case LanguageCode.en: return "English";
                case LanguageCode.cn: return "Simplified Chinese";
                case LanguageCode.fr: return "French";
                default: return "English";
            }
        }

        public void Translate(string srcText, LanguageCode srcLang, LanguageCode dstLang, Action<string> onResponse, Action<string> onError)
        {
            var jsonArgs = @"{""messages"":[{""role"":""system"", ""content"":""you're a prefect translator.""},";
            jsonArgs += @"{""role"":""user"", ""content"":""Translate this text from " + GetLanguageName(srcLang) + @" to " + GetLanguageName(dstLang) + @":" + srcText + @"""}]}";

            RunScript("chatgpt_chat", jsonArgs, (response) =>
            {
                if (response["status"] == "succeed")
                    onResponse?.Invoke(response["answer"]);
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }
    }
}

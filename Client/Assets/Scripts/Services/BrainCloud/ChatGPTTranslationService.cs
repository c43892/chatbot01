using Assets.Scripts.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    class ChatGPTTranslationService : BrainCloudServiceService, ITransationService
    {
        internal ChatGPTTranslationService(BrainCloudWrapper wrapper) : base(wrapper) { }

        string GetLanguageName(LanguageCode code)
        {
            switch (code)
            {
                case LanguageCode.en: return "English";
                case LanguageCode.cn: return "Simplified Chinese";
                case LanguageCode.fr: return "French";
                case LanguageCode.ja: return "Japanese";
                default: throw new Exception(code + " is not supported yet");
            }
        }

        public void Translate(string srcText, LanguageCode srcLang, LanguageCode dstLang, Action<string> onResponse, Action<string> onError)
        {
            var jsonArgs = @"{""messages"":[{""role"":""system"", ""content"":""you're a prefect translator.""},";
            jsonArgs += @"{""role"":""user"", ""content"":""Translate this text from " + GetLanguageName(srcLang) + @" to " + GetLanguageName(dstLang) + @":" + srcText + @"""}]}";

            RunScript("chatgpt_chat", jsonArgs, (response) =>
            {
                if (response["status"] == "succeed")
                    onResponse?.Invoke(response["answer"].Trim('"'));
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }
    }
}

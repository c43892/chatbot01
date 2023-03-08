using Assets.Scripts.Languages;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services.BrainCloud
{
    public class BrainCloudServiceProvider : IServiceProvider
    {
        BrainCloudWrapper bcw;
        readonly Dictionary<string, IText2SpeechService> text2SpeechServices = new Dictionary<string, IText2SpeechService>();
        readonly Dictionary<string, ISpeech2TextService> speech2TextServices = new Dictionary<string, ISpeech2TextService>();
        readonly Dictionary<string, IChatBotService> chatBotServices = new Dictionary<string, IChatBotService>();
        TranslationService translationService;

        string LanguageCode2Name(LanguageCode lang)
        {
            switch (lang)
            {
                case LanguageCode.cn: return "cmn_CN";
                case LanguageCode.en: return "en-US";
                case LanguageCode.fr: return "fr-FR";
                default: return "en-US";
            }
        }

        string LanguageCode2ModelName(LanguageCode lang)
        {
            switch (lang)
            {
                case LanguageCode.cn: return "cmn-CN-Standard-D";
                case LanguageCode.en: return "en-US-Neural2-G";
                case LanguageCode.fr: return "fr-FR-Neural2-A";
                default: return "cmn-CN-Standard-D";
            }
        }
        public void Init(BrainCloudWrapper wrapper, Action onResponse, Action<int, int> onError)
        {
            bcw = wrapper;

            bcw.Init();
            bcw.Client.SetPacketTimeouts(new List<int>() { 60, 60, 60 });

            bcw.AuthenticateAnonymous((string jsonResponse, object cbObject) =>
            {
                onResponse?.Invoke();
            }, (int status, int reasonCode, string jsonerror, object cbobject) =>
            {
                onError?.Invoke(status, reasonCode);
            });
        }

        public IText2SpeechService GetText2SpeechService(LanguageCode lang, int sampleRate)
        {
            var langName = LanguageCode2Name(lang);
            var langModel = LanguageCode2ModelName(lang);
            var key = langName + ":" + langModel;
            if (!text2SpeechServices.ContainsKey(key))
                text2SpeechServices[key] = new Text2SpeechService(bcw, langName, langModel);

            return text2SpeechServices[key];
        }

        public ISpeech2TextService GetSpeech2TextService(LanguageCode lang)
        {
            var langName = LanguageCode2Name(lang);
            if (!speech2TextServices.ContainsKey(langName))
                speech2TextServices[langName] = new Speech2TextService(bcw, langName);

            return speech2TextServices[langName];
        }

        public IChatBotService GetChatBotService(int maxPayload)
        {
            if (!chatBotServices.ContainsKey(maxPayload.ToString()))
                chatBotServices[maxPayload.ToString()] = new ChatBotService(bcw, maxPayload);

            return chatBotServices[maxPayload.ToString()];
        }

        public ITransationService GetTranslationService()
        {
            if (translationService == null)
                translationService = new TranslationService(bcw);

            return translationService;
        }
    }
}

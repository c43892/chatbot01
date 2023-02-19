using Assets.Scripts.Languages;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services.BrainCloud
{
    public class BrainCloudServiceProvider : IServiceProvider
    {
        BrainCloudWrapper bcw;
        Text2SpeechService text2SpeechService = null;
        Speech2TextService speech2TextService = null;
        ChatBotService chatBotService = null;

        public LanguageInfo LangInfo
        {
            get => langInfo;
            set
            {
                if (langInfo == value)
                    return;

                langInfo = value;
                text2SpeechService = null;
                speech2TextService = null;
                chatBotService = null;
            }
        }
        LanguageInfo langInfo;

        public void Init(BrainCloudWrapper wrapper, LanguageInfo langInfo, Action onResponse, Action<int, int> onError)
        {
            bcw = wrapper;
            LangInfo = langInfo;

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

        public IText2SpeechService GetText2SpeechService(int sampleRate)
        {
            if (text2SpeechService == null)
                text2SpeechService = new Text2SpeechService(bcw, LangInfo.Code, LangInfo.Model, sampleRate);

            return text2SpeechService;
        }

        public ISpeech2TextService GetSpeech2TextService()
        {
            if (speech2TextService == null)
                speech2TextService = new Speech2TextService(bcw, LangInfo.Code);

            return speech2TextService;
        }

        public IChatBotService GetChatBotService(int maxPayload)
        {
            if (chatBotService == null)
                chatBotService = new ChatBotService(bcw, maxPayload);

            return chatBotService;
        }
    }
}

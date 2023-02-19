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

        public IText2SpeechService GetText2SpeechService(string languageCode, string languageModel, int sampleRate)
        {
            var key = languageCode + ":" + languageModel;
            if (!text2SpeechServices.ContainsKey(key))
                text2SpeechServices[key] = new Text2SpeechService(bcw, languageCode, languageModel, sampleRate);

            return text2SpeechServices[key];
        }

        public ISpeech2TextService GetSpeech2TextService(string languageCode)
        {
            if (!speech2TextServices.ContainsKey(languageCode))
                speech2TextServices[languageCode] = new Speech2TextService(bcw, languageCode);

            return speech2TextServices[languageCode];
        }

        public IChatBotService GetChatBotService(int maxPayload)
        {
            if (!chatBotServices.ContainsKey(maxPayload.ToString()))
                chatBotServices[maxPayload.ToString()] = new ChatBotService(bcw, maxPayload);

            return chatBotServices[maxPayload.ToString()];
        }
    }
}

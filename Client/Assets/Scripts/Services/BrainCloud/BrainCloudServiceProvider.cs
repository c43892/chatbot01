using BrainCloud;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    public class BrainCloudServiceProvider
    {
        BrainCloudWrapper bcw;
        string languageCode;
        string languageModel;

        public void Init(BrainCloudWrapper wrapper, string langCode, string langModel, Action onResponse, Action<int, int> onError)
        {
            bcw = wrapper;
            languageCode = langCode;
            languageModel = langModel;

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
            return new Text2SpeechService(bcw, languageCode, languageModel, sampleRate);
        }

        public ISpeech2TextService GetSpeech2TextService()
        {
            return new Speech2TextService(bcw, languageCode);
        }

        public IChatBotService GetChatBotService(int maxPayload)
        {
            return new ChatBotService(bcw, maxPayload);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    class Text2SpeechService : BrainCloudServiceService, IText2SpeechService
    {
        string languageCode;
        string languageModel;
        public int SampleRate { get; private set; }

        internal Text2SpeechService(BrainCloudWrapper wrapper, string langCode, string langModel, int sampleRate) : base(wrapper)
        {
            languageCode = langCode;
            languageModel = langModel;
            SampleRate = sampleRate;
        }

        public void Text2Speech(string text, Action<byte[]> onResponse, Action<string> onError)
        {
            var jsonArgs = @"
{
    ""text"": """ + text + @""",
    ""languageCode"": """ + languageCode + @""",
    ""langaugeModel"": """ + languageModel + @""",
    ""sampleRateHertz"":" + SampleRate + @"
}";

            RunScript("text2Speech", jsonArgs, (response)  =>
            {
                if (response["status"].ToString() == "success")
                {
                    byte[] audioData = Convert.FromBase64String(response["audio"]);
                    onResponse?.Invoke(audioData);
                }
                else
                    onError?.Invoke(response["status"].ToString());
            }, onError);
        }
    }
}

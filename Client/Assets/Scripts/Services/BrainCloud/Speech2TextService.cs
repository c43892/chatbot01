using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    class Speech2TextService : BrainCloudServiceService, ISpeech2TextService
    {
        string languageCode;

        internal Speech2TextService(BrainCloudWrapper wrapper, string langCode) : base(wrapper)
        {
            languageCode = langCode;
        }

        public void Speech2Text(byte[] audioData, int sampleRate, int channels, Action<string, string, float> onResponse, Action<string> onError)
        {
            string jsonArgs = @"
{
    ""languageCode"":""" + languageCode + @""",
    ""audioChannelCount"":" + channels + @",
    ""sampleRateHertz"":" + sampleRate + @",
    ""audioData"":""" + Convert.ToBase64String(audioData) + @"""
}";

            RunScript("speech2Text", jsonArgs, (response) =>
            {
                if (response["status"] == "succeed")
                {
                    var transcript = response["transcript"];
                    var languageCode = response["languageCode"];
                    var confidence = float.Parse(response["confidence"]);
                    onResponse?.Invoke(transcript, languageCode, confidence);
                }
                else
                {
                    onError?.Invoke(response["statusMessage"]);
                }
            }, onError);
        }
    }
}

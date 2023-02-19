using Assets.Scripts.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface IServiceProvider
    {
        IText2SpeechService GetText2SpeechService(string languageCode, string languageModel, int sampleRate);

        ISpeech2TextService GetSpeech2TextService(string languageCode);

        IChatBotService GetChatBotService(int maxPayload);    
    }
}

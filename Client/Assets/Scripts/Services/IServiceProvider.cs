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
        IText2SpeechService GetText2SpeechService(LanguageCode code, int sampleRate);

        ISpeech2TextService GetSpeech2TextService(LanguageCode code);

        IChatBotService GetChatBotService(int maxPayload);

        ITransationService GetTranslationService(string type);
    }
}

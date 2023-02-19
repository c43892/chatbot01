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
        LanguageInfo LangInfo { get; set; }

        IText2SpeechService GetText2SpeechService(int sampleRate);

        ISpeech2TextService GetSpeech2TextService();

        IChatBotService GetChatBotService(int maxPayload);    
    }
}

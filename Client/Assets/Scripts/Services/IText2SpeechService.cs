using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface IText2SpeechService
    {
        int SampleRate { get; }

        void Text2Speech(string text, Action<byte[]> onResponse, Action<string> onError);
    }
}

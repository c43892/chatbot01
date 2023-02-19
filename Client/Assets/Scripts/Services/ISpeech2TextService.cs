using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface ISpeech2TextService
    {
        void Speech2Text(byte[] audioData, int sampleRate, int channels, Action<string, string, float> onResponse, Action<string> onError);
    }
}

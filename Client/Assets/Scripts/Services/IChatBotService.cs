using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface IChatBotService
    {
        void Completion(string question, Action<string> onResponse, Action<string> onError);
        void Chat(IEnumerable<KeyValuePair<string, string>> messages, Action<string> onResponse, Action<string> onError);
    }
}

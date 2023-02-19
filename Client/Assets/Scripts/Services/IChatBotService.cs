using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface IChatBotService
    {
        public void Ask(string question, Action<string> onResponse, Action<string> onError);
    }
}

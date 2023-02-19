using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    class ChatBotService : BrainCloudServiceService, IChatBotService
    {
        int maxPayload = 1024;

        internal ChatBotService(BrainCloudWrapper wrapper, int payload) : base(wrapper)
        {
            maxPayload = payload;
        }

        public void Ask(string question, Action<string> onResponse, Action<string> onError)
        {
            var jsonArgs = @"{
""prompt"":""" + question + @""",
""max_tokens"":" + maxPayload + @"
}";
            RunScript("chatgpt_completions", jsonArgs, (response) =>
            {
                if (response["status"] == "succeed")
                    onResponse?.Invoke(response["answer"]);
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }
    }
}

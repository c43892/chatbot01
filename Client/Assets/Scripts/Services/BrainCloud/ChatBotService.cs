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
        int maxPayload = 2048;


        internal ChatBotService(BrainCloudWrapper wrapper, int payload) : base(wrapper)
        {
            maxPayload = payload;
        }

        public void Chat(IEnumerable<KeyValuePair<string, string>> messages, Action<string> onResponse, Action<string> onError)
        {
            var jsonMessages = @"{""role"":""system"", ""content"":""请扮演一个会撒娇粘人性格活泼的年轻女性，但只回复对话内容，不要回复任何描述文字""},";
            foreach (var kv in messages)
                jsonMessages += @"{""role"":""" + kv.Key + @""", ""content"":""" + kv.Value + @"""},";

            jsonMessages = jsonMessages.Substring(0, jsonMessages.Length - 1) + "]";

            var jsonArgs = @"{ ""messages"":[" + jsonMessages + @"}";
            RunScript("chatgpt_chat", jsonArgs, (response) =>
            {
                if (response["status"] == "succeed")
                    onResponse?.Invoke(response["answer"]);
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }

        public void Completion(string question, Action<string> onResponse, Action<string> onError)
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

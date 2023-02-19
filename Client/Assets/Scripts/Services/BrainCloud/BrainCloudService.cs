
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services.BrainCloud
{
    class BrainCloudServiceService
    {
        BrainCloudWrapper bcw;

        protected BrainCloudServiceService(BrainCloudWrapper wrapper)
        {
            bcw = wrapper;
        }

        protected void RunScript(string script, string argumentsInJson, Action<Dictionary<string, string>> onResponse, Action<string> onError)
        {
            bcw.ScriptService.RunScript(script, argumentsInJson, (string jsonResponse, object cbObject) =>
            {
                var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                if (response["status"].ToString() == "200")
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response["data"].ToString());
                    var innerResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(data["response"].ToString());
                    onResponse?.Invoke(innerResponse);
                }
                else
                    onError?.Invoke(response["status"].ToString());
            }, (int status, int reasonCode, string jsonError, object cbObject) =>
            {
                onError?.Invoke(reasonCode.ToString());
            });
        }
    }
}

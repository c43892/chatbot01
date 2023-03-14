using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assets.Scripts.Services.BrainCloud
{
    class AccountService: BrainCloudServiceService, IAccountService
    {
        public AccountService(BrainCloudWrapper bcw) : base(bcw) { }

        public void CreateOrSign(string linkID, string platform, Action<Account> onResponse, Action<string> onError)
        {
            var json = @"{""linkId"":""" + linkID + @""", ""platform"":""" + platform + @"""}";
            RunScript("create_or_sign_account", json, (response) =>
            {
                if (response["status"] == "succeed")
                {
                    var acc = JsonConvert.DeserializeObject<Account>(response["account"]);
                    onResponse?.Invoke(acc);
                }
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }

        public void UpdateAccountInfo(Account acc, Action<Account> onResponse, Action<string> onError)
        {
            var json = JsonConvert.SerializeObject(acc);
            RunScript("update_account_info", json, (response) =>
            {
                if (response["status"] == "succeed")
                {
                    acc = JsonConvert.DeserializeObject<Account>(response["account"]);
                    onResponse?.Invoke(acc);
                }
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }
    }
}

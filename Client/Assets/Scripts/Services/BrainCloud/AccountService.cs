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
        public Account Me { get; private set; } = null;

        public AccountService(BrainCloudWrapper bcw) : base(bcw) { }

        public bool Signed { get => Me != null; }

        public void CreateOrSign(string linkID, string platform, Action<Account> onResponse, Action<string> onError)
        {
            var json = @"{""linkId"":""" + linkID + @""", ""platform"":""" + platform + @"""}";
            RunScript("create_or_sign_account", json, (response) =>
            {
                if (response["status"] == "succeed")
                {
                    Me = JsonConvert.DeserializeObject<Account>(response["account"]);
                    onResponse?.Invoke(Me);
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
                    Me = JsonConvert.DeserializeObject<Account>(response["account"]);
                    onResponse?.Invoke(Me);
                }
                else
                    onError?.Invoke(response["statusMessage"]);
            }, onError);
        }
    }
}

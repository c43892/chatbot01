using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public interface IAccountService
    {
        Account Me { get; }

        bool Signed { get; }

        void CreateOrSign(string linkID, string platform, Action<Account> onResponse, Action<string> onError);

        void UpdateAccountInfo(Account acc, Action<Account> onResponse, Action<string> onError);
    }
}

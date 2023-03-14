using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account
{
    [JsonProperty("linkId")]
    public string ID { get; private set; }

    [JsonProperty("info")]
    public AccountInfo Info { get; private set; }

    public Account(string linkId, AccountInfo info)
    {
        ID = linkId;
        Info = info;
    }
}

public class AccountInfo
{
    [JsonProperty("displayingName")]
    public string DisplayingName { get; set; }

    [JsonProperty("credits")]
    public int Credits { get; set; }

    [JsonProperty("platform")]
    public string Platform { get; set; }
}

using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CacheController
{

    private static Dictionary<string, string> Cache = new Dictionary<string, string>()
    {
        { "Кто ты".ToWord(), "$label WhoAreYou_Alex1" },
        { "Привет".ToWord(), "$label Hello_Alex1" },
	};

    public static string GetCachedOrNull(string message)
    {
        string word = message.ToWord();
        string answer;

        bool isSuccess = Cache.TryGetValue(word, out answer);

        if(isSuccess) 
            return answer;
        return null;
    }

}
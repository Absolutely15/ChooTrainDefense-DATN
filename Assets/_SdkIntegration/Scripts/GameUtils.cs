using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

//  ---------------------------------------------
//  Author:     DatDz <steven@atsoft.io> 
//  Copyright (c) 2022 AT Soft
// ----------------------------------------------
namespace ATSoft
{
    public class GameUtils
    {
        public static void RaiseMessage(object msg) {
            var router = ServiceFactory.Instance.Resolve<MessageRouter>();
            router.RaiseMessage(msg);
        }

        public static void AddHandler<T>(Action<T> handler)
        {
            var router = ServiceFactory.Instance.Resolve<MessageRouter>();
            router.AddHandler(handler);
        }

        public static void RemoveHandler<T>(Action<T> handler)
        {
            var router = ServiceFactory.Instance.Resolve<MessageRouter>();
            router.RemoveHandler(handler);
        }

        public static IObservable<T> AsObservable<T>() {
            return Observable.FromEvent<T>(AddHandler, RemoveHandler);
        }
        
        private static readonly Dictionary<SystemLanguage,string> COUTRY_CODES = new Dictionary<SystemLanguage, string>()
        {
            { SystemLanguage.Afrikaans, "ZA" },
            { SystemLanguage.Arabic    , "SA" },
            { SystemLanguage.Basque    , "US" },
            { SystemLanguage.Belarusian    , "BY" },
            { SystemLanguage.Bulgarian    , "BJ" },
            { SystemLanguage.Catalan    , "ES" },
            { SystemLanguage.Chinese    , "CN" },
            { SystemLanguage.Czech    , "HK" },
            { SystemLanguage.Danish    , "DK" },
            { SystemLanguage.Dutch    , "BE" },
            { SystemLanguage.English    , "US" },
            { SystemLanguage.Estonian    , "EE" },
            { SystemLanguage.Faroese    , "FU" },
            { SystemLanguage.Finnish    , "FI" },
            { SystemLanguage.French    , "FR" },
            { SystemLanguage.German    , "DE" },
            { SystemLanguage.Greek    , "JR" },
            { SystemLanguage.Hebrew    , "IL" },
            { SystemLanguage.Icelandic    , "IS" },
            { SystemLanguage.Indonesian    , "ID" },
            { SystemLanguage.Italian    , "IT" },
            { SystemLanguage.Japanese    , "JP" },
            { SystemLanguage.Korean    , "KR" },
            { SystemLanguage.Latvian    , "LV" },
            { SystemLanguage.Lithuanian    , "LT" },
            { SystemLanguage.Norwegian    , "NO" },
            { SystemLanguage.Polish    , "PL" },
            { SystemLanguage.Portuguese    , "PT" },
            { SystemLanguage.Romanian    , "RO" },
            { SystemLanguage.Russian    , "RU" },
            { SystemLanguage.SerboCroatian    , "SP" },
            { SystemLanguage.Slovak    , "SK" },
            { SystemLanguage.Slovenian    , "SI" },
            { SystemLanguage.Spanish    , "ES" },
            { SystemLanguage.Swedish    , "SE" },
            { SystemLanguage.Thai    , "TH" },
            { SystemLanguage.Turkish    , "TR" },
            { SystemLanguage.Ukrainian    , "UA" },
            { SystemLanguage.Vietnamese    , "VN" },
            { SystemLanguage.ChineseSimplified    , "CN" },
            { SystemLanguage.ChineseTraditional    , "CN" },
            { SystemLanguage.Unknown    , "US" },
            { SystemLanguage.Hungarian    , "HU" },
        };
 
        /// <summary>
        /// Returns approximate country code of the language.
        /// </summary>
        /// <returns>Approximated country code.</returns>
        /// <param name="language">Language which should be converted to country code.</param>
        public static string ToCountryCode(SystemLanguage language)
        {
            return COUTRY_CODES.TryGetValue (language, out var result) ? result : COUTRY_CODES[SystemLanguage.Unknown];
        }
    }
}
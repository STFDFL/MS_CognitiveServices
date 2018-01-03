/*

	Copyright (c) 2016 Keizo Nagamine

	Released under the MIT license

	http://opensource.org/licenses/mit-license.php

*/



using UnityEngine;

using UnityEngine.Networking;

using System.Collections;

using System.Xml.Serialization;

using System.IO;

using System.Text;



/// <summary>

/// Microsoft Translator Text API 実行クラス

/// インスペクターでclientIDとclientSecretを設定した状態でTranslateメソッドで翻訳を実行

/// 結果は引数として渡すコールバックメソッドを通じて受け取る

/// </summary>

public class TranslateV2 : MonoBehaviour

{

    /// <summary>

    /// 翻訳前、翻訳後の言語種別

    /// とりあえず英語と日本語しか用意していない

    /// APIが対応しているものは使用可能なはず

    /// </summary>

    public enum Language

    {

        en,

        ja,

    }



    /// <summary>

    /// Jsonで返却されるアクセストークンを保持する

    /// </summary>

    [System.Serializable]

    private class AccessToken

    {

        public string access_token = string.Empty;

        public string token_type = string.Empty;

        public string expires_in = string.Empty;

        public string scope = string.Empty;

    }



    /// <summary>

    /// MicrosoftDataMarketで登録したアプリケーションのクライアントID

    /// インスペクターで設定する

    /// </summary>

    [SerializeField]

    private string clientID;



    /// <summary>

    /// MicrosoftDataMarketで登録したアプリケーションのクライアントシークレット

    /// インスペクターで設定する

    /// </summary>

    [SerializeField]

    private string clientSecret;



    /// <summary>

    /// 翻訳を実行する

    /// </summary>

    /// <param name="text">翻訳したい文字列</param>

    /// <param name="from">翻訳前の言語</param>

    /// <param name="to">翻訳後の言語</param>

    /// <param name="callback">翻訳終了時のコールバック</param>

    public void Translate(string text, Language from, Language to, System.Action<string> callback)

    {

        StartCoroutine(_Translate(text, from, to, callback));

    }



    private IEnumerator _Translate(string text, Language from, Language to, System.Action<string> callback)

    {

        // アクセストークン取得

        string authUrl = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";



        var authForm = new WWWForm();

        authForm.AddField("client_id", clientID);

        authForm.AddField("client_secret", clientSecret);

        authForm.AddField("scope", "http://api.microsofttranslator.com");

        authForm.AddField("grant_type", "client_credentials");



        AccessToken token = null;



        using (var request = UnityWebRequest.Post(authUrl, authForm))

        {

            yield return request.Send();



            if (request.isNetworkError)

            {

                Debug.LogError(request.error);

                if (callback != null) { callback(null); }

                yield break;

            }



            token = JsonUtility.FromJson<AccessToken>(request.downloadHandler.text);

        }



        // 翻訳API実行

        string translateUrl = string.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}", text, from, to);



        using (var request = UnityWebRequest.Get(translateUrl))

        {

            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", token.access_token));



            yield return request.Send();



            if (request.isNetworkError)

            {

                Debug.LogError(request.error);

                if (callback != null) { callback(null); }

                yield break;

            }



            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(request.downloadHandler.text)))

            {

                var xmlSerializer = new XmlSerializer(typeof(string), "http://schemas.microsoft.com/2003/10/Serialization/");

                var result = (string)xmlSerializer.Deserialize(ms);

                if (callback != null) { callback(result); }

            }

        }

    }



}
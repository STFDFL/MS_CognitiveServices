using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Translate2 : MonoBehaviour {


    /// Header name used to pass the subscription key to translation service
    private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

    /// Url template to make translate call
    private const string TranslateUrlTemplate = "http://api.microsofttranslator.com/v2/http.svc/translate?text={0}&from={1}&to={2}&category={3}";

    private const string AzureSubscriptionKey = "c44d929eebef40f586c238c985f74a69";   //Enter here the Key from your Microsoft Translator Text subscription on http://portal.azure.com

    private string _authorizationKey = "c44d929eebef40f586c238c985f74a69";
    private string _authorizationToken;
    private DateTime _timestampWhenTokenExpires;

    private void RefreshToken()
    {
        HttpWebRequest request;

        if (string.IsNullOrEmpty(_authorizationKey))
        {
            throw new InvalidOperationException("Authorization key not set.");
        }

        request = WebRequest.CreateHttp("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
        request.Method = WebRequestMethods.Http.Post;
        request.Headers.Add("Ocp-Apim-Subscription-Key", _authorizationKey);
        request.ContentLength = 0; // Must be set to avoid 411 response
        try
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //_authorizationToken = this.GetResponseString(response);
                _authorizationToken = response.GetResponseStream().ToString();
                _timestampWhenTokenExpires = DateTime.UtcNow.AddMinutes(8);
            }
        }
        catch (Exception e)
        {
            Debug.Log( e.Message);
            Debug.Log(e.Message);
        }
        
    }

    // Use this for initialization
    void Start () {
        RefreshToken();
	}

    public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain,
        // look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    break;
                }
            }
        }
        return isOk;
    }
}

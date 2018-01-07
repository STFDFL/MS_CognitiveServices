using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class Languages : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public string[] GetLanguages()
    {
        HttpWebRequest request;
        string[] results;

        this.CheckToken();

        request = WebRequest.CreateHttp("https://api.microsofttranslator.com/v2/http.svc/GetLanguagesForTranslate");
        request.Headers.Add("Authorization", "Bearer " + _authorizationToken);
        request.Accept = "application/xml";

        using (WebResponse response = request.GetResponse())
        {
            using (Stream stream = response.GetResponseStream())
            {
                results = ((List<string>)new DataContractSerializer(typeof(List<string>)).ReadObject(stream)).ToArray();
            }
        }

        return results;
    }
}

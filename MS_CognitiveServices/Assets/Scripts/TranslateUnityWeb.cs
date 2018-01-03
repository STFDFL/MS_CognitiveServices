using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TranslateUnityWeb : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(GetText());

    }

    public GameObject TempText;
    static string TempValue;
    IEnumerator GetText()
    {
        Debug.Log("Inside Coroutine");
        while (true)
        {
            yield return new WaitForSeconds(5f);
            string url = "http://Administrator:ZZh7y6dn@*IP Address*:8080/Thingworx/Things/SimulationData/Properties/OvenTemperature/";

            Debug.Log("Before UnityWebRequest");
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            Debug.Log("After UnityWebRequest");
            if (www.isNetworkError)
            {
                Debug.Log("Error while Receiving: " + www.error);
            }
            else
            {
                Debug.Log("Success. Received: " + www.downloadHandler.text);
                string result = www.downloadHandler.text;
                Char delimiter = '>';

                String[] substrings = result.Split(delimiter);
                foreach (var substring in substrings)
                {
                    if (substring.Contains("</TD"))
                    {
                        String[] Substrings1 = substring.Split('<');
                        Debug.Log(Substrings1[0].ToString() + "Temp Value");
                        TempValue = Substrings1[0].ToString();
                        TempText.GetComponent<TextMesh>().text = TempValue + "'C";
                    }
                }
            }

        }

    }

}

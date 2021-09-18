using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ClassCubeScript : MonoBehaviour
{
    public GameObject Magic8BallText;
    public GameObject WeatherText;
    public GameObject TempText;
    private string weatherAPIUrl = "https://api.openweathermap.org/data/2.5/weather?q=Paris&appid=1d6c65e03049c8519e63f5519a02a606&units=metric";
    // Magic 8 Ball List of Texts
    private string[] texts = {
        "It is Certain",
        "It is decidedly so",
        "Without a doubt",
        "Yes definitely",
        "You may rely on it",
        "As I see it, yes.",
        "Most likely",
        "Outlook good",
        "Yes",
        "Signs point to yes",
        "Reply hazy, try again",
        "Ask again later",
        "Better not tell you now",
        "Cannot predict now",
        "Concentrate and ask again",
        "Don't count on it",
        "My reply is no",
        "My sources say no",
        "Outlook not so good",
        "Very doubtful"
    };
    private LocationInfo locationInfo;
    // Start is called before the first frame update
    void Start()
    {   
        // Updates Magic Eight Ball Text
        UpdateMEBText();

        // wait a couple seconds to start and then refresh every 900 seconds
        InvokeRepeating("GetData", 2f, 900f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetData() {
        StartCoroutine(GetRequest(weatherAPIUrl));
    }

    IEnumerator GetRequest(string uri) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            if (webRequest.isNetworkError) {
                Debug.Log(": Error: " + webRequest.error);
            }
            else {
                // print out the weather data to make sure it makes sense
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);

                locationInfo = JsonUtility.FromJson<LocationInfo>(webRequest.downloadHandler.text);
                UpdateWeatherData(locationInfo);
                
            }
        }
    }

    void UpdateWeatherData(LocationInfo locationDetails) {
        WeatherText.GetComponent<TextMeshPro>().text = locationDetails.weather[0].main;
        TempText.GetComponent<TextMeshPro>().text = locationDetails.main.temp + " C";
    }

    void UpdateMEBText() {
        Magic8BallText.GetComponent<TextMeshPro>().text = GetRandomExpression();
    }

    string GetRandomExpression() {
        System.Random r = new System.Random();
        int rInt = r.Next(0, texts.Length); //for ints
        return texts[rInt];
    }
}

[System.Serializable]
public class LocationInfo {
    public MainObject main;
    public WeatherObject[] weather; 
}

[System.Serializable]
public class MainObject {
    public float temp;
}

[System.Serializable]
public class WeatherObject {
    public string main;
}






using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class MergeCubeScript : MonoBehaviour
{
    public GameObject ModernMEBText;
    public GameObject WeatherTextMerge;
    public GameObject TempTextMerge;
    public GameObject NightSignBlue;
    public GameObject Pokemon;
    private bool isNightSignActive = true;
    private string weatherAPIUrl = "https://api.openweathermap.org/data/2.5/weather?q=Tokyo&appid=1d6c65e03049c8519e63f5519a02a606&units=imperial";

    // Classic magic 8 ball sayings
    private string[] ballTexts = {
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
        InvokeRepeating("ToggleNightSign", 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        Pokemon.transform.Rotate(0, 1f, 0, Space.Self);
    }

    void ToggleNightSign() {
        isNightSignActive = !isNightSignActive;
        NightSignBlue.SetActive(isNightSignActive);
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
        WeatherTextMerge.GetComponent<TextMeshPro>().text = locationDetails.weather[0].main;
        TempTextMerge.GetComponent<TextMeshPro>().text = locationDetails.main.temp + " F";
    }

    void UpdateMEBText() {
        ModernMEBText.GetComponent<TextMeshPro>().text = GetRandomExpression();
    }

    string GetRandomExpression() {
        System.Random r = new System.Random();
        int rInt = r.Next(0, ballTexts.Length); //for ints
        return ballTexts[rInt];
    }
}
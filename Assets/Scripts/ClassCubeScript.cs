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
    public AudioSource BallSound;
    private float xAngle;
    private float zAngle;
    private float lowThreshold = 30f;
    private float highThreshold = 180f;
    private bool isInRotationLoop = false;

    enum RotationStatus {Started, Flipped, Reset};
    private RotationStatus rotationStatus = RotationStatus.Reset;

    // Weather API URL
    private string weatherAPIUrl = "https://api.openweathermap.org/data/2.5/weather?q=Paris&appid=1d6c65e03049c8519e63f5519a02a606&units=metric";

    // Custom Magic 8 Ball Sayings
    private string[] ballTexts = {
        "Absolutely",
        "very much possible",
        "Attempt Again",
        "Improbable",
        "Unsure about it",
        "Under no Circumstance",
        "Can't Forsee it",
        "It is not gonna happen",
        "Confident about this",
        "Nah"
    };
    private LocationInfo locationInfo;
    // Start is called before the first frame update
    void Start()
    {   
        // Updates Magic Eight Ball Text
        UpdateMEBText();
        // Set angles
        SetAnglesForRotationDetection();
        // wait a couple seconds to start and then refresh every 900 seconds
        InvokeRepeating("GetData", 2f, 900f);
    }

    void SetAnglesForRotationDetection() {
        xAngle = Mathf.Abs(UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).x);
        zAngle = Mathf.Abs(UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z);
    }

    void DetectRotationStart() {
        if ((xAngle + zAngle) > lowThreshold) {
            rotationStatus = RotationStatus.Started;
        }
    }

    void DetectObjectFlipped() {
        if((xAngle + zAngle) > highThreshold) {
            rotationStatus = RotationStatus.Flipped;
        }
    }

    void DetectObjectReset() {
        if((xAngle + zAngle) < lowThreshold) {
            UpdateMEBText();
            PlaySound();
            rotationStatus = RotationStatus.Reset;
        }
    }

    void PlaySound() {
        BallSound.Play();
    }

    void CheckRotation() {
        UpdateMEBText();
    }

    // Update is called once per frame
    void Update()
    {   
        SetAnglesForRotationDetection();
        if(rotationStatus == RotationStatus.Reset) {
            DetectRotationStart();
        }
        if(rotationStatus == RotationStatus.Started) {
            DetectObjectFlipped();
        }
        if(rotationStatus == RotationStatus.Flipped) {
            DetectObjectReset();
        }
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
        System.Random randomObject = new System.Random();
        int randomValue = r.Next(0, ballTexts.Length); //for ints
        return ballTexts[randomValue];
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






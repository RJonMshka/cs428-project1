using System.Collections;
using UnityEditor;
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
    public AudioSource BallSound;
    private float xAngle;
    private float zAngle;

    // Min value of angle that is required to start the rotation detection process
    private float lowThreshold = 30f;
    private float highThreshold = 180f;

    // declaring an enum represnting states of rotation of the cube
    enum RotationStatus {Started, Flipped, Reset};
    // Initially set to reset state
    private RotationStatus rotationStatus = RotationStatus.Reset;

    // Weather API URL
    private bool isNightSignActive = true;

    // Weather API URL
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

    // Stores location info after get request for weather is resolved
    private LocationInfo locationInfo;
    // Start is called before the first frame update
    void Start()
    {   
        // Updates Magic Eight Ball Text
        UpdateMEBText();

        // wait a couple seconds to start and then refresh every 900 seconds
        InvokeRepeating("GetData", 2f, 900f);

        // Toggle Night Sign's active property
        InvokeRepeating("ToggleNightSign", 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {   // Sets angles first
        SetAnglesForRotationDetection();

        // Checks for tart of the rotation cycle if already in reset state
        if(rotationStatus == RotationStatus.Reset) {
            DetectRotationStart();
        }
        // Checks for flipped state
        if(rotationStatus == RotationStatus.Started) {
            DetectObjectFlipped();
        }
        // Checks for reset state when object is flipped back up
        if(rotationStatus == RotationStatus.Flipped) {
            DetectObjectReset();
        }

        // Rotate the pokemon
        Pokemon.transform.Rotate(0, 1f, 0, Space.Self);
    }

    // This method sets the value of xAngle and zAngle to values from the inspector in Unity
    void SetAnglesForRotationDetection() {
        xAngle = Clamp0360(transform.localEulerAngles.x);
        zAngle = Clamp0360(transform.localEulerAngles.z);
        if(xAngle > 180f) {
            xAngle = 360f - xAngle;
        }
        if(zAngle > 180f) {
            zAngle = 360f - zAngle;
        }
    }

    // This method checks if rotation has started
    void DetectRotationStart() {
        if ((xAngle + zAngle) > lowThreshold) {
            rotationStatus = RotationStatus.Started;
        }
    }

    // This method checks if the object is completely flipped
    void DetectObjectFlipped() {
        if((xAngle + zAngle) > highThreshold) {
            rotationStatus = RotationStatus.Flipped;
        }
    }

    // This method checks if the the object is flipped back up
    void DetectObjectReset() {
        if((xAngle + zAngle) < lowThreshold) {
            UpdateMEBText();
            PlaySound();
            rotationStatus = RotationStatus.Reset;
        }
    }

    // This method plays the sound if magic 8 ball has completed the rotation cycle successfully
    void PlaySound() {
        BallSound.Play();
    }

    // This method toggle active state of night sign
    void ToggleNightSign() {
        isNightSignActive = !isNightSignActive;
        NightSignBlue.SetActive(isNightSignActive);
    }

    // This method get the weather data by calling the weatherAPI
    void GetData() {
        StartCoroutine(GetRequest(weatherAPIUrl));
    }

    // This block represents the functioning of get request
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

    // This method populates the weather text components with weather data
    void UpdateWeatherData(LocationInfo locationDetails) {
        WeatherTextMerge.GetComponent<TextMeshPro>().text = locationDetails.weather[0].main;
        TempTextMerge.GetComponent<TextMeshPro>().text = locationDetails.main.temp + " F";
    }

    // This methods updates and populates the text component of Magic Eight Ball
    void UpdateMEBText() {
        ModernMEBText.GetComponent<TextMeshPro>().text = GetRandomExpression();
    }

    // This method returns a random number between 0 and an amount one less than length of magic 8 ball sayings array
    string GetRandomExpression() {
        System.Random randomObject = new System.Random();
        int randomValue = randomObject.Next(0, ballTexts.Length - 1); //for ints
        return ballTexts[randomValue];
    }

    // This method converts local eulerAngles to match inspector angles
    float Clamp0360(float eulerAngles) { 
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0) {
            result += 360f;
        }
        return result;
    }
}
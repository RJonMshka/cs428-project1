using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClassCubeScript : MonoBehaviour
{
    public GameObject Magic8BallText;
    private string[] texts = {"1", "2", "3", "4", "5"};
    // Start is called before the first frame update
    void Start()
    {   
        updateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updateText() {
        Magic8BallText.GetComponent<TextMeshPro>().text = getRandomString();
    }

    string getRandomString() {
        System.Random r = new System.Random();
        int rInt = r.Next(0, texts.Length); //for ints
        return texts[rInt];
    }
}

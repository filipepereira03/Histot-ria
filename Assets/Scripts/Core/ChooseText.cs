using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseText : MonoBehaviour
{ 
    public GameObject gObject;
    public string text;
    public string command;
    public string key;
    public List<GameObject> allQuestions;
    private TextMeshProUGUI thisText;
    private int index;

    void Start()
    {
        index = Convert.ToInt16(key) -1;
        gObject = this.gameObject;
        thisText = this.gameObject.GetComponent<TextMeshProUGUI>();
        thisText.text = key+". "+text;
        thisText.rectTransform.anchoredPosition = new Vector2(-133, (25 - (index * 25)));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            InputDecoder.atQuestion = false;
            InputDecoder.ParseInputLine(command);
            foreach (GameObject g in allQuestions)
            {
                Destroy(g);
            }
        }
    }
    void OnMouseDown()
    {
        InputDecoder.atQuestion = false;
        InputDecoder.ParseInputLine(command);
        Debug.Log(allQuestions.Count);
        foreach (GameObject g in allQuestions)
        {
            Destroy(g);
        }
    }

}

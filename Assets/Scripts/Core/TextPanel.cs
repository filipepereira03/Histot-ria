using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPanel : MonoBehaviour
{

    public TextMeshProUGUI textPanel;
    public string[] lines;
    public float textSpeed;
    private int index;


    void Start()
    {
        textPanel.text = string.Empty;
        StartDialogue();
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textPanel.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textPanel.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
    }

    IEnumerator typeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textPanel.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textPanel.text = string.Empty;
            StartCoroutine(typeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

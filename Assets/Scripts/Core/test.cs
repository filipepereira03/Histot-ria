using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Button ContinueButton;
    private bool canClick = true; //lol

    void Start()
    {
        FindObjectOfType<ScreenOrientationController>().SetHorizontalOrientation();

        string cenario = PlayerPrefs.GetString("Cenario");
        InputDecoder.load();
        ContinueButton.onClick.AddListener(OnContinueButtonClicked);
        InputDecoder.ReadScript("Script/cenario" + cenario);
        if (InputDecoder.QuestionBox != null)
        {
            InputDecoder.QuestionBox.SetActive(false);
        }
        else
        {
            Debug.Log("question null");
        }
    }

    void Update()
    {
        canClick = !InputDecoder.isTyping;

        if (Input.GetKeyDown("h"))
        {
            if (InputDecoder.InterfaceElements.activeInHierarchy)
            {
                InputDecoder.InterfaceElements.SetActive(false);
            }
            else
            {
                InputDecoder.InterfaceElements.SetActive(true);
            }
        }

        if (!InputDecoder.PausedHere && InputDecoder.CommandLine < InputDecoder.Commands.Count - 1 && !InputDecoder.atQuestion)
        {
            //Debug.Log("AQUI " + InputDecoder.lastCommand);
            InputDecoder.lastCommand = InputDecoder.Commands[InputDecoder.CommandLine];
            InputDecoder.PausedHere = false;
            InputDecoder.ParseInputLine(InputDecoder.Commands[InputDecoder.CommandLine]);
            InputDecoder.CommandLine++;
        }

        // Botao continue some ou aparece se puder ser clicado
        if (canClick && !InputDecoder.atQuestion)
        {
            ContinueButton.gameObject.SetActive(true);
        }
        else
        {
            ContinueButton.gameObject.SetActive(false);
        }
    }

    void OnContinueButtonClicked()
    {
        if (canClick)  // Só avança se o botão puder ser clicado
        {
            HandleNextCommand();
        }
    }

    void HandleNextCommand()
    {
        ContinueButton.gameObject.SetActive(false);

        //Debug.Log("PausedHere: " + InputDecoder.PausedHere + "commandLine: " + InputDecoder.CommandLine + "atquestion: " + InputDecoder.atQuestion);
        if (InputDecoder.PausedHere && InputDecoder.CommandLine < InputDecoder.Commands.Count - 1 && !InputDecoder.atQuestion)
        {
            InputDecoder.lastCommand = InputDecoder.Commands[InputDecoder.CommandLine];
            InputDecoder.PausedHere = false;
            InputDecoder.ParseInputLine(InputDecoder.Commands[InputDecoder.CommandLine]);
            InputDecoder.CommandLine++;
        }

        // some o botão
        ContinueButton.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Properties;

public class InputDecoder
{
    #region Variabels
    public static bool PausedHere  = false;
    public static bool atQuestion = false;

    //Character
    public static List<Character> CharacterList = new List<Character>();
    private static GameObject ImageCharacter = Resources.Load("Prefabs/CharacterSideImage") as GameObject;
    private static GameObject characterSide = GameObject.Find("CharacterImage");


    //Command Storage
    [NonSerialized]
    public static List<string> Commands = new List<string>();
    public static List<Label> LabelList = new List<Label>();
    public static int CommandLine = 0;
    public static string lastCommand = "";

    //find and define UIElements
    public static GameObject InterfaceElements = GameObject.Find("UI_Elements");

    //find and define the background image
    private static GameObject canvas = GameObject.Find("ImageLayers");
    private static GameObject ImageBackground = Resources.Load("Prefabs/Background") as GameObject;

    //find and define text fields
    public static GameObject DialogText = GameObject.Find("DialogueText");
    public static GameObject NameText = GameObject.Find("NameText");
    public static GameObject QuestionBox = GameObject.Find("QuestionBox");
    private static GameObject ChooseText = Resources.Load("Prefabs/ChooseText") as GameObject;

    //find question
    public static Button QuestionButton1 = GameObject.Find("Resposta1").GetComponent<Button>();
    public static Button QuestionButton2 = GameObject.Find("Resposta2").GetComponent<Button>();
    public static Button QuestionButton3 = GameObject.Find("Resposta3").GetComponent<Button>();
    public static Button QuestionButton4 = GameObject.Find("Resposta4").GetComponent<Button>();

    //text verify
    public static bool isTyping = false;
    private static string fullText = ""; // Armazena o texto completo atual
    #endregion

    public static void load()
    {
        //CoroutineManager.Instance.StopAllCoroutines();

        InterfaceElements = GameObject.Find("UI_Elements");
        DialogText = GameObject.Find("DialogueText");
        NameText = GameObject.Find("NameText");
        QuestionBox = GameObject.Find("QuestionBox");
        ChooseText = Resources.Load("Prefabs/ChooseText") as GameObject;

        canvas = GameObject.Find("ImageLayers");
        ImageBackground = Resources.Load("Prefabs/Background") as GameObject;

        QuestionButton1 = GameObject.Find("Resposta1").GetComponent<Button>();
        QuestionButton2 = GameObject.Find("Resposta2").GetComponent<Button>();
        QuestionButton3 = GameObject.Find("Resposta3").GetComponent<Button>();
        QuestionButton4 = GameObject.Find("Resposta4").GetComponent<Button>();

        ImageCharacter = Resources.Load("Prefabs/CharacterSideImage") as GameObject;
        characterSide = GameObject.Find("CharacterImage");

        // Resetar valores estáticos
        Commands.Clear();
        LabelList.Clear();
        CommandLine = 0;
        lastCommand = "";
        PausedHere = false;
        atQuestion = false;
        isTyping = false; // Reinicia flag de digitação
        fullText = "";    // Garante que o texto esteja vazio

    }


    public static void ParseInputLine(string command) //TODO restructure
    {
        command = command.Replace("\t", "").Trim();
        if (command.StartsWith("\""))
        {
            Say(command);
        }
        else if(command == "")
        {
            
        }
        else
        {
            string[] SeperatingString = { " ", "\'", "\"", "(", ")", ":" };
            string[] args = command.Split(SeperatingString, StringSplitOptions.RemoveEmptyEntries);

            string nameAux = args[0].Replace("_", " ");
            foreach (Character character in CharacterList)
            {
                if (nameAux == character.name)
                {
                    Say(character.name, SplitToSay(command, character), character.color);
                }
            }
            if (args[0] == "show")
            {
                showImage(command);
            }
            else if (args[0] == "clrscr")
            {
                clearScreen(null, true);
            }
            else if (args[0] == "character")
            {
                createNewCharacter(command);
            }
            else if (args[0] == "jump")
            {
                JumpTo(command);
            }
            else if (args[0] == "appear")
            {
                AppearCharacter(command);
            }
            else if (args[0] == "disappear")
            {
                DisappearCharacter(command);
            }
            else if (args[0] == "end")
            {
                PlayerPrefs.SetString("respostas", "a,a,a,a");
                CoroutineManager.Destroy(CoroutineManager.Instance.gameObject);
                SceneManager.LoadScene("Aluno");
                //Application.Quit();
                //UnityEditor.EditorApplication.isPlaying = false;
            }
            else if (args[0] == "question")
            {
                Question(command);
            }
            else if (args[0] == "edit")
            {
                editCharacterImage(command);
            }
        }
    }


    #region Say

    public static string SplitToSay(string say, Character character)
    {
        int startQuote = say.IndexOf("\"") + 1;
        int endQuote = say.Length - 1;

        return say.Substring(startQuote, endQuote - startQuote);
    }

    public static void Say(string say)
    {
        if (!InterfaceElements.activeInHierarchy)
            InterfaceElements.SetActive(true);

        // Verifique se está atualmente exibindo texto
        if (isTyping)
        {
            // Se já está digitando, exibe todo o texto imediatamente
            CompleteText(DialogText.GetComponent<TextMeshProUGUI>());
        }
        else
        {
            fullText = say.Substring(1, say.Length - 2); // Armazena o texto completo
            CoroutineManager.Instance.StartCoroutine(TypeText(fullText, DialogText.GetComponent<TextMeshProUGUI>()));
        }

        NameText.GetComponent<TextMeshProUGUI>().text = "";
        PausedHere = true;
    }

    public static void Say(string who, string what, Color color)
    {
        if (!InterfaceElements.activeInHierarchy)
            InterfaceElements.SetActive(true);

        NameText.GetComponent<TextMeshProUGUI>().text = who.Replace("\"", "");
        NameText.GetComponent<TextMeshProUGUI>().color = color;

        Debug.Log(who + "lol");
        Debug.Log(what + "lol");

        // Verifique se está atualmente exibindo texto
        if (isTyping)
        {
            CompleteText(DialogText.GetComponent<TextMeshProUGUI>());
        }
        else
        {
            CoroutineManager.Instance.StopAllCoroutines(); // Interrompe qualquer execução anterior
            fullText = what;
            CoroutineManager.Instance.StartCoroutine(TypeText(fullText, DialogText.GetComponent<TextMeshProUGUI>()));
        }



        PausedHere = true;
    }


    public static void Question(string command)
    {
        UnityEngine.Debug.Log(command);
        QuestionBox.SetActive(true);
        atQuestion=true;

        // Divide a linha da pergunta no formato: question "Qual a capital da França?" : "Paris" ; "Londres" ; "Brasília" ; "Berlim"
        string[] parts = command.Split(':');

        if (parts.Length == 2)
        {
            // Parte antes dos ':' contém a pergunta, removemos o "question" e as aspas
            string questionText = parts[0].Replace("question", "").Trim().Replace("\"", "");
            // Parte depois dos ':' contém as opções de resposta, separadas por ';'
            string[] options = parts[1].Split(';');

            // Remove espaços em branco extras e aspas das opções
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = options[i].Trim().Replace("\"", "");
            }

            QuestionButton1.GetComponentInChildren<TMP_Text>().text = options[0];
            QuestionButton2.GetComponentInChildren<TMP_Text>().text = options[1];
            QuestionButton3.GetComponentInChildren<TMP_Text>().text = options[2];
            QuestionButton4.GetComponentInChildren<TMP_Text>().text = options[3];

            DialogText.GetComponent<TMP_Text>().text = questionText;

            QuestionButton1.GetComponent<Button>().onClick.AddListener(() => OnAnswerSelected(1));
            QuestionButton2.GetComponent<Button>().onClick.AddListener(() => OnAnswerSelected(2));
            QuestionButton3.GetComponent<Button>().onClick.AddListener(() => OnAnswerSelected(3));
            QuestionButton4.GetComponent<Button>().onClick.AddListener(() => OnAnswerSelected(4));
        }
        else
        {
            UnityEngine.Debug.LogError("Formato de pergunta inválido: " + command);
        }
    }

    // Método para lidar com a seleção de resposta
    private static void OnAnswerSelected(int buttonNumber)
    {
        atQuestion = false;
        QuestionBox.SetActive(false);

        // Salve o botão pressionado (pode usar uma variável global ou de classe)
        string selectedButton = buttonNumber switch
        {
            1 => "a",
            2 => "b",
            3 => "c",
            4 => "d",
            _ => "n/a"
        };

        UnityEngine.Debug.Log("Botão selecionado: " + selectedButton);
        // Aqui você pode adicionar lógica adicional com base na resposta selecionada
    }

    private static IEnumerator TypeText(string text, TextMeshProUGUI textMeshPro)
    {
        textMeshPro.text = ""; // Limpa o texto anterior
        float textSpeed = 0.04f; // Velocidade de exibição dos caracteres
        isTyping = true; // Seta a flag de que o texto está sendo exibido

        foreach (char c in text)
        {
            textMeshPro.text += c; // Adiciona o caractere ao texto
            yield return new WaitForSeconds(textSpeed); // Aguarda um pouco antes de adicionar o próximo caractere

            // Se o jogador clicar durante a exibição, interrompe o loop
            if (!isTyping) yield break;
        }

        isTyping = false; // A exibição do texto terminou
    }

    // Método para completar a exibição do texto instantaneamente
    public static void CompleteText(TextMeshProUGUI textMeshPro)
    {
        textMeshPro.text = fullText; // Mostra o texto completo
        isTyping = false; // Texto já foi exibido completamente
    }

    #endregion

    #region Images

    public static void showImage(string image) //TODO restructure
    {
        string ImageToShow = null;
        bool fadeEffect = false;
        bool doClear = false;
        var ImageToUse = new Regex(@"show (?<ImageFileName>[^.]+)");
        var ImageToUseTransition = new Regex(@"show (?<ImageFileName>[^.]+) with (?<TransitionName>[^.]+)");
        var ImageToUseTransitionToDoScreenClear = new Regex(@"show (?<ImageFileName>[^.]+) with (?<TransitionName>[^.]+) do (?<ScreenName>[^.]+)");

        var matches = ImageToUse.Match(image);
        var altMatches = ImageToUseTransition.Match(image);
        var firstMatches = ImageToUseTransitionToDoScreenClear.Match(image);
        if (firstMatches.Success)
        {
            ImageToShow = altMatches.Groups["ImageFileName"].ToString();
            fadeEffect = true;
            doClear= true;
        }
        else if (altMatches.Success) 
        { 
            ImageToShow = altMatches.Groups["ImageFileName"].ToString();
            fadeEffect= true;
        }
        else if(matches.Success)
        {
            ImageToShow = matches.Groups["ImageFileName"].ToString();
        }
        if (ImageToShow == null)
        {
            Debug.LogError("Nome da imagem não capturado: " + image);
            return;
        }

        GameObject PictureInstance = GameObject.Instantiate(ImageBackground);
        if (PictureInstance == null)
        {
            Debug.LogError("ImageBackground não foi instanciado.");
            return;
        }
        PictureInstance.transform.SetParent(canvas.transform, false);
        PictureInstance.GetComponent<ImageInstance>().FadeIn = fadeEffect;
        PictureInstance.GetComponent<Image>().color = Color.white;
        PictureInstance.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Backgrounds/" + ImageToShow);
        if (doClear)
        {
            clearScreen(PictureInstance, false);
        }
    }

    public static void clearScreen(GameObject image, bool removeAll)
    {
        if (removeAll == true)
        {
            foreach (Transform t in canvas.transform)
            {
                MonoBehaviour.Destroy(t.gameObject);
            }
            foreach (Transform t in characterSide.transform)
            {
                MonoBehaviour.Destroy(t.gameObject); 
            }
            InterfaceElements.SetActive(false);
        }
        else
        {
            foreach (Transform t in canvas.transform)
            {
                if (image != null && t != image.transform)
                {
                    MonoBehaviour.Destroy(t.gameObject, 1f);
                }
            }
        }
    }

    #endregion

    #region Character

    public static void createNewCharacter(string command)
    {
        string name = null;
        Color color = Color.white;
        string sideImage = null;

        // Regex atualizado para capturar nomes com ou sem aspas e espaços
        var character = new Regex(@"character\((?:""(?<name>[^""]+)""|(?<name>[a-zA-Z0-9_]+)), color=(?<color>[a-zA-Z0-9_]+), image=(?<sideImage>[a-zA-Z0-9_]+)\)");
        var characterA = new Regex(@"character\((?:""(?<name>[^""]+)""|(?<name>[a-zA-Z0-9_]+)), color=(?<color>[a-zA-Z0-9_]+)\)");
        var characterB = new Regex(@"character\((?:""(?<name>[^""]+)""|(?<name>[a-zA-Z0-9_]+)), image=(?<sideImage>[a-zA-Z0-9_]+)\)");
        var characterC = new Regex(@"character\((?:""(?<name>[^""]+)""|(?<name>[a-zA-Z0-9_]+))\)");

        if (character.IsMatch(command))
        {
            var matches = character.Match(command);
            name = matches.Groups["name"].Value;
            ColorUtility.TryParseHtmlString(matches.Groups["color"].Value, out color);
            sideImage = matches.Groups["sideImage"].Value;
        }
        else if (characterA.IsMatch(command))
        {
            var matches = characterA.Match(command);
            name = matches.Groups["name"].Value.Replace("\"", "");
            ColorUtility.TryParseHtmlString(matches.Groups["color"].Value, out color);
        }
        else if (characterB.IsMatch(command))
        {
            var matches = characterB.Match(command);
            name = matches.Groups["name"].Value;
            sideImage = matches.Groups["sideImage"].Value;
        }
        else if (characterC.IsMatch(command))
        {
            var matches = characterC.Match(command);
            name = matches.Groups["name"].Value;
        }

        CharacterList.Add(new Character(name, color, sideImage));
        Debug.Log("Nome do personagem criado: " + name);

    }

    public static void editCharacterImage(string command)
    {
        command = command.Replace("edit ","");
        string[] parts = command.Split(',');
        parts[0] = parts[0].Trim();
        parts[1] = parts[1].Trim();

        foreach (Character c in CharacterList)
        {
            if (c.name == parts[0])
            {
                c.sideImage = parts[1];
                break;
            }
        }
    }



    public static void AppearCharacter(string command)
    {
        var characterCheck = new Regex(@"appear (?:""(?<Name>[^""]+)""|(?<Name>[a-zA-Z0-9_]+))");
        var characterCheckA = new Regex(@"appear (?:""(?<Name>[^""]+)""|(?<Name>[a-zA-Z0-9_]+)), (?<Position>[a-zA-Z]+)");

        if (characterCheckA.IsMatch(command))
        {
            var match = characterCheckA.Match(command);
            foreach (Character c in CharacterList)
            {
                if (c.name == match.Groups["Name"].Value)
                {
                    GameObject PictureInstance = GameObject.Instantiate(ImageCharacter);
                    PictureInstance.transform.SetParent(characterSide.transform, false);
                    PictureInstance.GetComponent<Image>().color = c.color;
                    PictureInstance.GetComponent<Image>().name = c.name;
                    if (c.sideImage != null)
                    {
                        PictureInstance.GetComponent<Image>().color = Color.white;
                        Sprite sideSprite = Resources.Load<Sprite>("Images/Personagens/" + c.sideImage);
                        Debug.Log("Carregando imagem " + c.sideImage);

                        if (sideSprite == null)
                        {
                            Debug.LogError("Imagem do personagem não encontrada" + c.sideImage);
                            return;
                        }
                        PictureInstance.GetComponent<CharacterSideImage>().size = new Vector2(sideSprite.rect.width, sideSprite.rect.height);
                        PictureInstance.GetComponent<Image>().sprite = sideSprite;
                        PictureInstance.GetComponent<CharacterSideImage>().position = match.Groups["Position"].Value;
                    }
                }
            }
        }
        else if (characterCheck.IsMatch(command))
        {
            var match = characterCheck.Match(command);
            foreach (Character c in CharacterList)
            {
                if (c.name == match.Groups["Name"].Value)
                {
                    GameObject PictureInstance = GameObject.Instantiate(ImageCharacter);
                    PictureInstance.transform.SetParent(characterSide.transform, false);
                    PictureInstance.GetComponent<Image>().color = c.color;
                    PictureInstance.GetComponent<Image>().name = c.name;
                    if (c.sideImage != null)
                    {
                        PictureInstance.GetComponent<Image>().color = Color.white;
                        Sprite sideSprite = Resources.Load<Sprite>("Images/Personagens/" + c.sideImage);
                        Debug.Log("Carregando imagem " + c.sideImage);

                        if (sideSprite == null)
                        {
                            Debug.LogError("Imagem do personagem não encontrada" + c.sideImage);
                            return;
                        }
                        PictureInstance.GetComponent<CharacterSideImage>().size = new Vector2(sideSprite.rect.width, sideSprite.rect.height);
                        PictureInstance.GetComponent<Image>().sprite = sideSprite;
                    }
                }
            }
        }
    }



    public static void DisappearCharacter(string command)
    {
        var character = new Regex(@"disappear (?:""(?<Name>[^""]+)""|(?<Name>[a-zA-Z0-9_]+))");
        if (character.IsMatch(command))
        {
            var match = character.Match(command);
            foreach (Transform t in characterSide.transform)
            {
                if (t.gameObject.name == match.Groups["Name"].Value)
                {
                    Debug.Log($"Desaparecendo o personagem: {t.gameObject.name}");
                    t.GetComponent<CharacterSideImage>().KillMe();
                    MonoBehaviour.Destroy(t.gameObject);
                    break;
                }
            }
        }
    }



    #endregion

    #region Labels

    public static void ReadScript(string file)
    {
        TextAsset commandFile = Resources.Load(file) as TextAsset;
        var commandArray = commandFile.text.Replace("\r", "").Split("\n");

        foreach(string line in commandArray)
        {
            Commands.Add(line);
        }

        for ( int x=0; x< Commands.Count; x++ )
        {
            if (Commands[x].StartsWith("label"))
            {
                var labelSplit = Commands[x].Split(' ');
                LabelList.Add(new Label(labelSplit[1], x));
            }
        }
    }

    public static void JumpTo(string line) 
    {
        var tempString = line.Split(' ');
        foreach(Label d in LabelList) 
        {
            if(d.LabelName == tempString[1])
            {
                CommandLine = d.LabelIndex;
                PausedHere = false;
            }
        }
    }

    #endregion
}

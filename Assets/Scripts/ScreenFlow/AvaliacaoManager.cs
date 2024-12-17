using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AvaliacaoManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform panelAvaliacoes;

    void Start()
    {
        FindObjectOfType<ScreenOrientationController>().SetPortraitOrientation();
        CarregarCenarios();
    }

    void CarregarCenarios()
    {
        StartCoroutine(FazerRequisicaoCenarios());
    }

    IEnumerator FazerRequisicaoCenarios()
    {
        // Limpar botões existentes no painel
        foreach (Transform child in panelAvaliacoes)
        {
            Debug.Log($"Destruindo botão: {child.name}");
            Destroy(child.gameObject);
        }

        string url = $"http://3.133.149.14:8080/api/cenario?offset=0&fetch=20";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log($"JSON recebido: {json}");

            List<Cenario> cenarios = JsonUtility.FromJson<CenarioList>($"{{\"cenarios\":{json}}}").cenarios;

            foreach (Cenario cenario in cenarios)
            {
                if (cenario.ativo == true)
                {
                    CriarBotaoParaCenario(cenario);
                }
            }
        }
        else
        {
            Debug.LogError("Erro ao carregar cenários: " + request.error);
        }
        Canvas.ForceUpdateCanvases();

    }

    void CriarBotaoParaCenario(Cenario cenario)
    {
        GameObject botaoObj = Instantiate(buttonPrefab, panelAvaliacoes);

        // Configurar o texto para o nome da turma
        TMP_Text textoTurma = botaoObj.transform.Find("texto").GetComponent<TMP_Text>();
        if (textoTurma != null)
        {
            textoTurma.text = $"{cenario.name}";
        }

        Button botaoComponente = botaoObj.GetComponent<Button>();
        botaoComponente.onClick.AddListener(() => OnClickCenario(cenario));
    }

    void OnClickCenario(Cenario cenario)
    {
        Debug.Log($"Cenário selecionado: {cenario.idCenario}");
        PlayerPrefs.SetString("Cenario", cenario.idCenario);
        SceneManager.LoadScene("SampleScene");
    }
}

[System.Serializable]
public class Pergunta
{
    public string textoPergunta;
    public List<string> opcoes;
}

[System.Serializable]
public class CenarioList
{
    public List<Cenario> cenarios;
}
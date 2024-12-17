using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class EstatisticaGeralManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform panelTurmas;
    public Button ano1Button;
    public Button ano2Button;
    public Button ano3Button;

    private string anoSelecionado = "1";

    public EstatisticaCenarioManager cenarioManager; // aqui


    void Start()
    {
        ano1Button.onClick.AddListener(() => SelecionarAno("1", ano1Button));
        ano2Button.onClick.AddListener(() => SelecionarAno("2", ano2Button));
        ano3Button.onClick.AddListener(() => SelecionarAno("3", ano3Button));
        ano1Button.GetComponentInChildren<TMP_Text>().fontStyle |= FontStyles.Underline;
        CarregarTurmas();
    }

    void SelecionarAno(string ano, Button botaoSelecionado)
    {
        anoSelecionado = ano;
        CarregarTurmas();
        AtualizarSublinhado(botaoSelecionado);
    }

    void AtualizarSublinhado(Button botaoSelecionado)
    {
        // Remover sublinhado de todos os botões
        RemoverSublinhado(ano1Button);
        RemoverSublinhado(ano2Button);
        RemoverSublinhado(ano3Button);

        // Adicionar sublinhado ao botão selecionado
        TMP_Text textoBotao = botaoSelecionado.GetComponentInChildren<TMP_Text>();
        textoBotao.fontStyle |= FontStyles.Underline; // Adiciona sublinhado
    }

    void RemoverSublinhado(Button botao)
    {
        TMP_Text textoBotao = botao.GetComponentInChildren<TMP_Text>();
        textoBotao.fontStyle &= ~FontStyles.Underline; // Remove sublinhado
    }

    void CarregarTurmas()
    {
        StartCoroutine(FazerRequisicaoTurmas(anoSelecionado));
    }

    //void SelecionarAno(string ano)
    //{
    //    anoSelecionado = ano;
    //    CarregarTurmas();
    //}

    IEnumerator FazerRequisicaoTurmas(string ano)
    {
        foreach (Transform child in panelTurmas)
        {
            Destroy(child.gameObject);
        }

        string url = $"http://3.133.149.14:8080/api/turma/{ano}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);

            // Processar JSON para obter as turmas
            TurmaList turmaList = JsonUtility.FromJson<TurmaList>("{\"turmas\":" + json + "}");

            foreach (Turma turma in turmaList.turmas)
            {
                StartCoroutine(ObterMediaNotas(turma));
            }
        }
        else
        {
            Debug.LogError("Erro ao carregar turmas: " + request.error);
        }
    }

    IEnumerator ObterMediaNotas(Turma turma)
    {
        string urlMediaNotas = $"http://3.133.149.14:8080/api/turma/{turma.id_turma}/media-notas";
        Debug.Log($"URL gerada: {urlMediaNotas}");

        UnityWebRequest requestMedia = UnityWebRequest.Get(urlMediaNotas);

        yield return requestMedia.SendWebRequest();

        if (requestMedia.result == UnityWebRequest.Result.Success)
        {
            string jsonMedia = requestMedia.downloadHandler.text;
            Debug.Log($"Resposta de média para turma {turma.id_turma}: {jsonMedia}");

            // Processar o JSON para obter a média
            MediaResponse mediaResponse = JsonUtility.FromJson<MediaResponse>(jsonMedia);
            turma.mediaNotas = mediaResponse.media;

            CriarBotaoParaTurma(turma);
        }
        else
        {
            Debug.LogError($"Erro ao carregar média das notas para a turma {turma.id_turma}: {requestMedia.error}");
        }
    }

    void CriarBotaoParaTurma(Turma turma)
    {
        // Instanciar o prefab do botão no painel
        GameObject botaoObj = Instantiate(buttonPrefab, panelTurmas);

        // Configurar o texto para o nome da turma
        TMP_Text textoTurma = botaoObj.transform.Find("turma").GetComponent<TMP_Text>();
        if (textoTurma != null)
        {
            textoTurma.text = $"Turma {turma.id_turma}";
        }

        // Configurar o texto para a média da turma
        TMP_Text textoMedia = botaoObj.transform.Find("media").GetComponent<TMP_Text>();
        if (textoMedia != null)
        {
            textoMedia.text = $"Média Geral: {turma.mediaNotas:F2}";
        }

        // Configurar o botão para realizar uma ação ao ser clicado
        Button botao = botaoObj.GetComponent<Button>();
        if (botao != null)
        {
            botao.onClick.AddListener(() => OnClickTurma(turma.id_turma.ToString()));
        }

        // Adicionar o botão ao painel (feito automaticamente com o parent definido em Instantiate)
        botaoObj.SetActive(true);
    }

    void OnClickTurma(string idTurma)
    {
        Debug.Log("Turma selecionada: " + idTurma + "ano selecionado: " + anoSelecionado);
        cenarioManager.ConfigurarTelaEstatCenario(idTurma, anoSelecionado);
    }

    // Classe para desserializar a lista de turmas
    [System.Serializable]
    public class TurmaList
    {
        public Turma[] turmas;
    }

    // Classe para representar uma turma
    [System.Serializable]
    public class Turma
    {
        public int id_turma;
        public int ano;
        public bool ativa;
        public Professor id_professor;
        public float mediaNotas; // Armazena a média da turma
    }

    [System.Serializable]
    public class Professor
    {
        public int id;
    }

    // Classe para processar a resposta da média
    [System.Serializable]
    public class MediaResponse
    {
        public float media;
    }


}
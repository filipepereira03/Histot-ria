using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class EstatisticaCenarioManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject turmaPrefab;

    public Canvas TelaQuestoes;
    public GameObject ModalQuestoes;

    public PorcentagemManager porcentagemManager;

    public Transform panelHeader;
    public Transform panelTurmas;

    private string anoSelecionado = "1";
    public string turmaSelecionada = "1";

    public Canvas telaEstatistica;
    public Canvas telaEstatCenario;

    private Button botaoTurmaSelecionado;
    private Button botaoTurmaSelecionada; // Armazena o botão sublinhado atualmente
    public Button Voltar;

    public void ConfigurarTelaEstatCenario(string idTurma, string ano)
    {
        anoSelecionado = ano;
        turmaSelecionada = idTurma;
        AlternarParaTelaEstatCenario();
        CarregarCenariosMedia();
        AlterarAnoVisual();
        CarregarTurmasDoAno();
        Voltar.onClick.AddListener(() => OnClickVoltar());
    }

    public TMP_Text anoText;
    void AlterarAnoVisual()
    {
        anoText.text = anoSelecionado+"º Ano";
    }

    void OnClickVoltar()
    {
        telaEstatCenario.gameObject.SetActive(false);
        telaEstatistica.gameObject.SetActive(true);
    }

    void AlternarParaTelaEstatCenario()
    {
        telaEstatCenario.gameObject.SetActive(true);
        telaEstatistica.gameObject.SetActive(false);
    }

    public void VoltarTelaEstatistica()
    {
        telaEstatCenario.gameObject.SetActive(false);
        telaEstatistica.gameObject.SetActive(true);
    }

    void SelecionarAno(string ano, Button botaoSelecionado)
    {
        anoSelecionado = ano;
        CarregarCenariosMedia();
        AtualizarSublinhado(botaoSelecionado);
    }

    void AtualizarSublinhado(Button botaoSelecionado)
    {
        // Remover sublinhado do botão anterior, se houver
        if (botaoTurmaSelecionado != null)
        {
            RemoverSublinhado(botaoTurmaSelecionado);
        }

        // Adicionar sublinhado ao botão selecionado
        TMP_Text textoBotao = botaoSelecionado.GetComponentInChildren<TMP_Text>();
        textoBotao.fontStyle |= FontStyles.Underline;

        // Atualizar referência do botão selecionado
        botaoTurmaSelecionado = botaoSelecionado;
    }

    void RemoverSublinhado(Button botao)
    {
        TMP_Text textoBotao = botao.GetComponentInChildren<TMP_Text>();
        textoBotao.fontStyle &= ~FontStyles.Underline; // Remove o sublinhado
    }


    void CarregarCenariosMedia()
    {
        StartCoroutine(FazerRequisicaoCenariosMedia(turmaSelecionada));
    }

    IEnumerator FazerRequisicaoCenariosMedia(string turmaVar)
    {
        foreach (Transform child in panelTurmas)
        {
            Destroy(child.gameObject);
        }

        string url = $"http://3.133.149.14:8080/api/turma/{turmaVar}/media-cenarios";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Desserializar a resposta JSON
            string jsonResponse = request.downloadHandler.text;
            CenarioMediaResponse[] cenarios = JsonHelper.FromJson<CenarioMediaResponse>(jsonResponse);

            // Criar botões para cada cenário
            foreach (var cenario in cenarios)
            {
                CriarBotaoParaCenario(cenario);
            }
        }
        else
        {
            Debug.LogError("Erro na requisição: " + request.error);
        }
    }

    void CriarBotaoParaCenario(CenarioMediaResponse cenario)
    {
        // Instanciar o prefab do botão no painel
        GameObject botaoObj = Instantiate(buttonPrefab, panelTurmas);

        // Configurar o texto para o ID do cenário
        TMP_Text textoCenario = botaoObj.transform.Find("turma").GetComponent<TMP_Text>();
        if (textoCenario != null)
        {
            textoCenario.text = $"Cenário {cenario.idCenario}";
        }

        // Configurar o texto para a média do cenário
        TMP_Text textoMedia = botaoObj.transform.Find("media").GetComponent<TMP_Text>();
        if (textoMedia != null)
        {
            textoMedia.text = $"Média: {cenario.media:F2}";
        }

        // Configurar o botão para realizar uma ação ao ser clicado
        Button botao = botaoObj.GetComponent<Button>();
        if (botao != null)
        {
            botao.onClick.AddListener(() => OnClickCenario(cenario.idCenario.ToString()));
        }

        // Adicionar o botão ao painel (feito automaticamente com o parent definido em Instantiate)
        botaoObj.SetActive(true);
    }

    void OnClickCenario(string idCenario)
    {
        Debug.Log("ID: " + idCenario);
        Debug.Log("Turma: " + turmaSelecionada);
        telaEstatCenario.gameObject.SetActive(false);
        TelaQuestoes.gameObject.SetActive(true);
        porcentagemManager.ConfigurarTelaQuestoes(turmaSelecionada, idCenario);
    }

    void CarregarTurmasDoAno()
    {
        StartCoroutine(FazerRequisicaoTurmas(anoSelecionado));
    }

    IEnumerator FazerRequisicaoTurmas(string ano)
    {
        // Limpar turmas existentes no header
        foreach (Transform child in panelHeader)
        {
            Destroy(child.gameObject);
        }

        string url = $"http://3.133.149.14:8080/api/turma/{ano}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Desserializar a resposta JSON
            string jsonResponse = request.downloadHandler.text;
            Turma[] turmas = JsonHelper.FromJson<Turma>(jsonResponse);

            // Criar botões para cada turma
            foreach (var turma in turmas)
            {
                CriarBotaoParaTurma(turma);
            }
        }
        else
        {
            Debug.LogError("Erro na requisição de turmas: " + request.error);
        }
    }

    void CriarBotaoParaTurma(Turma turma)
    {
        // Instanciar o prefab do botão no header
        GameObject botaoObj = Instantiate(turmaPrefab, panelHeader);

        // Configurar o texto para o nome da turma
        TMP_Text textoTurma = botaoObj.transform.Find("Text").GetComponent<TMP_Text>();
        if (textoTurma != null)
        {
            textoTurma.text = $"Turma {turma.id_turma}";
        }

        // Configurar o botão para realizar uma ação ao ser clicado
        Button botao = botaoObj.GetComponent<Button>();
        if (botao != null)
        {
            botao.onClick.AddListener(() => OnClickTurma(turma.id_turma.ToString(), botao));
        }

        // Se a turma atual for a mesma da `turmaSelecionada`, aplicar sublinhado
        if (turma.id_turma.ToString() == turmaSelecionada)
        {
            AtualizarSublinhado(botao);
            botaoTurmaSelecionada = botao; // Armazena o botão selecionado inicialmente
        }

        // Ativar o botão
        botaoObj.SetActive(true);
    }


    void OnClickTurma(string idTurma, Button botaoSelecionado)
    {
        Debug.Log("Turma selecionada no header: " + idTurma);
        turmaSelecionada = idTurma;

        // Atualizar sublinhado
        if (botaoTurmaSelecionada != null)
        {
            RemoverSublinhado(botaoTurmaSelecionada); // Remove sublinhado do botão anterior
        }

        AtualizarSublinhado(botaoSelecionado); // Sublinha o botão atual
        botaoTurmaSelecionada = botaoSelecionado; // Atualiza a referência do botão atual

        // Carregar cenários relacionados à nova turma
        CarregarCenariosMedia();
    }


    // Classe para desserializar a lista de cenários com suas médias
    [System.Serializable]
    public class CenarioMediaResponse
    {
        public string idCenario;
        public float media;
    }

    // Classe para processar a resposta da média
    [System.Serializable]
    public class MediaResponse
    {
        public float media;
    }

    [System.Serializable]
    public class Turma
    {
        public int id_turma;
        public int ano;
        public bool ativa;
        public Professor id_professor;
        public float mediaNotas; // Armazena a média da turma (se houver)
    }
}

// Classe auxiliar para tratar JSON com arrays
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
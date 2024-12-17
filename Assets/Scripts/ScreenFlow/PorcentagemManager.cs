using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class PorcentagemManager : MonoBehaviour
{
    public GameObject questaoPrefab; // Prefab para exibir uma quest�o com porcentagem
    public Transform panelQuestoes; // Painel onde as quest�es ser�o exibidas

    public Canvas telaQuestoes; // Tela para exibir as quest�es
    public Canvas telaEstatCenario;
    public GameObject modalQuestao; // Modal para exibir detalhes da quest�o

    public TMP_Text cenarioTitulo; // T�tulo indicando o ID do cen�rio
    public TMP_Text turmaTitulo; // Texto para o nome da turma
    public TMP_Text modalQuestaoTexto; // Texto para exibir a quest�o no modal
    public TMP_Text modalAcertosTexto; // Texto para exibir porcentagem de acertos no modal
    public TMP_Text modalErrosTexto; // Texto para exibir porcentagem de erros no modal
    public Button modalBotaoFechar; // Bot�o para fechar o modal

    public string turmaSelecionada = "1";
    public string cenarioSelecionado = "1";

    public Button Voltar;

    private string baseUrl = "http://3.133.149.14:8080/api"; // URL base do backend (AWS)

    void Start()
    {
        modalBotaoFechar.onClick.AddListener(() => FecharModal());
    }

    public void ConfigurarTelaQuestoes(string idTurma, string idCenario)
    {
        turmaSelecionada = idTurma;
        cenarioSelecionado = idCenario;
        telaQuestoes.gameObject.SetActive(true);
        // Preenchendo os textos do header
        cenarioTitulo.text = $"Cen�rio {idCenario}";
        turmaTitulo.text = $"Turma {idTurma}";
        CarregarQuestoes(idTurma, idCenario);
        Voltar.onClick.AddListener(() => OnClickVoltar());
    }
    void OnClickVoltar()
    {
        telaQuestoes.gameObject.SetActive(false);
        telaEstatCenario.gameObject.SetActive(true);
    }

    void CarregarQuestoes(string idTurma, string idCenario)
    {
        StartCoroutine(FazerRequisicaoQuestoes(idTurma, idCenario));
    }

    IEnumerator FazerRequisicaoQuestoes(string idTurma, string idCenario)
    {
        // Limpar quest�es existentes no painel
        foreach (Transform child in panelQuestoes)
        {
            Destroy(child.gameObject);
        }

        // Construindo a URL do endpoint
        string url = $"{baseUrl}/turma/{idTurma}/porcentagem-acertos-questoes";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();



        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Resposta bruta recebida do servidor: {jsonResponse}");
            Debug.Log($"JSON inicia com: {jsonResponse.TrimStart()[0]}");


            // Desserializar o JSON usando JsonHelper2
            CenarioResponse[] cenarios = JsonHelper2.DeserializeArrayOrSingle<CenarioResponse>(jsonResponse);

            if (cenarios != null && cenarios.Length > 0)
            {
                Debug.Log($"N�mero de cen�rios retornados: {cenarios.Length}");
                foreach (var cenario in cenarios)
                {
                    Debug.Log($"Processando Cen�rio: {cenario.cenario}");
                    if (cenario.cenario == idCenario && cenario.questoes != null)
                    {
                        for (int i = 0; i < cenario.questoes.Length; i++)
                        {
                            CriarQuestaoUI(cenario.questoes[i], i);
                        }
                        break; // Encontrou e processou o cen�rio correspondente
                    }

                }
            }
            else
            {
                Debug.LogWarning("Nenhum cen�rio foi retornado ou o array est� vazio.");
            }
        }
        else
        {
            Debug.LogError($"Erro na requisi��o: {request.error}");
        }
    }

    void CriarQuestaoUI(Questao questao, int index)
    {
        if (questaoPrefab == null)
        {
            Debug.LogError("questaoPrefab n�o est� configurado no inspector.");
            return;
        }

        GameObject questaoObj = Instantiate(questaoPrefab, panelQuestoes);
        if (questaoObj == null)
        {
            Debug.LogError("Erro ao instanciar questaoPrefab.");
            return;
        }

        TMP_Text textoQuestao = questaoObj.transform.Find("TextoQuestao")?.GetComponent<TMP_Text>();
        if (textoQuestao != null)
        {
            // Adiciona a numera��o e substitui ":" por "?"
            string questaoComNumero = $"{index + 1}. {questao.questao.Replace(":", "?")}";
            textoQuestao.text = questaoComNumero;
        }
        else
        {
            Debug.LogError("TextoQuestao n�o foi encontrado no prefab.");
        }

        Button botaoQuestao = questaoObj.GetComponent<Button>();
        if (botaoQuestao != null)
        {
            botaoQuestao.onClick.AddListener(() => AbrirModal(questao));
        }
        else
        {
            Debug.LogError("Bot�o n�o encontrado no questaoPrefab.");
        }
    }


    void AbrirModal(Questao questao)
    {
        // Atualizar os textos do modal
        modalQuestaoTexto.text = $"{questao.questao.Replace(":", "?")}";
        modalAcertosTexto.text = $"PORCENTAGEM DE ACERTOS: {questao.porcentagem}%";
        modalErrosTexto.text = $"PORCENTAGEM DE ERROS: {100 - questao.porcentagem}%";

        // For�ar o layout a recalcular antes de mostrar o modal
        LayoutRebuilder.ForceRebuildLayoutImmediate(modalQuestao.GetComponent<RectTransform>());

        // Ativar o modal
        modalQuestao.SetActive(true);
    }

    void FecharModal()
    {
        modalQuestao.SetActive(false);
    }

    [System.Serializable]
    public class CenarioResponse
    {
        public string cenario; // ID do cen�rio
        public Questao[] questoes; // Lista de quest�es com porcentagens
    }

    [System.Serializable]
    public class Questao
    {
        public string questao; // Texto da quest�o
        public int porcentagem; // Porcentagem de acertos
    }
}

// Classe auxiliar para desserializar JSON



public static class JsonHelper2
{
    // M�todo para tratar tanto arrays quanto objetos �nicos no JSON
    public static T[] DeserializeArrayOrSingle<T>(string json)
    {
        // Tenta desserializar diretamente como um array
        if (json.TrimStart().StartsWith("["))
        {
            return DeserializeArray<T>(json);
        }

        // Caso contr�rio, tenta desserializar como um objeto �nico
        try
        {
            T singleObject = JsonUtility.FromJson<T>(json);
            if (singleObject != null)
            {
                return new T[] { singleObject }; // Encapsula o objeto em um array
            }
        }
        catch
        {
            Debug.LogError("Falha ao desserializar JSON como objeto �nico.");
        }

        // Retorna null se ambas as tentativas falharem
        return null;
    }

    // M�todo para desserializar arrays no JSON
    public static T[] DeserializeArray<T>(string json)
    {
        // Encapsula o JSON em um objeto fict�cio para desserializar como array
        string newJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    // Classe auxiliar para encapsular arrays
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}



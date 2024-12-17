using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class TurmasManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform panelTurmas;
    public Button ano1Button;
    public Button ano2Button;
    public Button ano3Button;
    public Canvas telaAlunos;

    public AlunosManager alunosManager;

    private string anoSelecionado = "1";

    void Start()
    {
        FindObjectOfType<ScreenOrientationController>().SetPortraitOrientation();
        telaAlunos.gameObject.SetActive(false);
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

            json = json.Replace("\"$ref\"", "\"ref\"").Replace("\"$id\"", "\"id\"");

            TurmaList turmaList = JsonUtility.FromJson<TurmaList>("{\"turmas\":" + json + "}");

            foreach (Turma turma in turmaList.turmas)
            {
                CriarBotaoParaTurma(turma);
            }
        }
        else
        {
            Debug.LogError("Erro ao carregar turmas: " + request.error);
        }
    }

    void CriarBotaoParaTurma(Turma turma)
    {
        GameObject botao = Instantiate(buttonPrefab, panelTurmas);

        TMP_Text botaoTexto = botao.GetComponentInChildren<TMP_Text>();
        botaoTexto.text = $"Turma {turma.id_turma} - Professor {turma.id_professor.id}";

        Button botaoComponente = botao.GetComponent<Button>();
        botaoComponente.onClick.AddListener(() => OnClickTurma(turma.id_turma.ToString()));

        Button bookButton = botao.transform.Find("BookButton").GetComponent<Button>();
        if (bookButton != null)
        {
            bookButton.onClick.AddListener(() => OnClickBook(turma.id_turma.ToString()));
        }
    }

    void OnClickTurma(string idTurma)
    {
        Debug.Log("Turma selecionada: " + idTurma);
        alunosManager.ConfigurarTelaAlunos(idTurma); //Chama o metodo de AlunosManager para configurar a tela de alunos
    }

    void OnClickBook(string idTurma)
    {
        Debug.Log("Turma + selecionar todos os cenarios: " + idTurma);
    }

}


// Classes simplificadas para JsonUtility
[System.Serializable]
public class TurmaList
{
    public Turma[] turmas;
}

[System.Serializable]
public class Turma
{
    public TurmaId id;
    public int id_turma;
    public Professor id_professor;
    public int ano;
    public bool ativa;
    public Aluno[] alunos;
    public Avaliacao[] avaliacoes;
}

[System.Serializable]
public class TurmaId
{
    public long timestamp;
    public int machine;
    public int pid;
    public int increment;
    public string creationTime;
}

[System.Serializable]
public class Professor
{
    public string @ref;
    public int id;
}

[System.Serializable]
public class Aluno
{
    public AlunoId id_aluno;
    public string nome;
    public string matricula;
}

[System.Serializable]
public class AlunoId
{
    public string @ref;
    public int id;
}

[System.Serializable]
public class Avaliacao
{
    public string id_avaliacao;
    public string[] respostas;
    public AlunoId id_aluno;
    public Cenario id_cenario;
    public int nota;
    public string comentario;
}

[System.Serializable]
public class Cenario
{
    public string @ref;
    public string id;
    public string idCenario;
    public string name;
    public List<Pergunta> perguntas;
    public List<string> gabarito;
    public bool ativo;
}

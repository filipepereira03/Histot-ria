using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;

public class AlunosManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform painelAlunos;
    private String turmaSelecionada;
    public TMP_Text textoTurma;
    public Canvas telaTurmas;  
    public Canvas telaAlunos;
    public Canvas telaAvaliacao;

    public void ConfigurarTelaAlunos(string idTurma)
    {
        turmaSelecionada = idTurma;
        AtualizarTextoTurma();
        AlternarParaTelaAlunos();
        CarregarAlunos();
    }

    void AlternarParaTelaAlunos()
    {
        telaAlunos.gameObject.SetActive(true);
        telaTurmas.gameObject.SetActive(false);
    }

    public void VoltarTelaTurmas()
    {
        telaAlunos.gameObject.SetActive(false);
        telaTurmas.gameObject.SetActive(true);
    }

    public void IrParaAvaliacaoAluno()
    {
        telaAlunos.gameObject.SetActive(false);
        telaAvaliacao.gameObject.SetActive(true);
    }

    void AtualizarTextoTurma()
    {
        if (textoTurma != null)
        {
            textoTurma.text = $"Turma {turmaSelecionada}";
        }
    }

    void CarregarAlunos() 
    {
        StartCoroutine(FazerRequisicaoAlunos(turmaSelecionada));
    }

    public IEnumerator FazerRequisicaoAlunos(String turmaId)
    {
        foreach (Transform child in painelAlunos)
        {
            Destroy(child.gameObject);
        }

        string url = $"http://3.133.149.14:8080/api/turma/{turmaId}/alunos";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);

            json = json.Replace("\"$ref\"", "\"ref\"").Replace("\"$id\"", "\"id\"");

            AlunoList alunoList = JsonUtility.FromJson<AlunoList>("{\"alunos\":" + json + "}");

            foreach (Aluno aluno in alunoList.alunos)
            {
                CriarBotaoParaAluno(aluno);
            }
        }
        else
        {
            Debug.LogError("Erro ao carregar alunos: " + request.error);
        }
    }

    void CriarBotaoParaAluno(Aluno aluno)
    {
        GameObject botao = Instantiate(buttonPrefab, painelAlunos);

        TMP_Text botaoTexto = botao.GetComponentInChildren<TMP_Text>();
        botaoTexto.text = $"{aluno.nome}\n{aluno.matricula}";

        Button botaoComponente = botao.GetComponent<Button>();
        botaoComponente.onClick.AddListener(() => IrParaAvaliacaoAluno());

        botaoComponente.onClick.AddListener(() => OnClickAluno(aluno.id_aluno.id.ToString()));
    }

    void OnClickAluno(string idAluno)
    {
        Debug.Log("Aluno selecionado: " + idAluno);
    }

    [System.Serializable]
    public class AlunoList
    {
        public Aluno[] alunos;
    }
}


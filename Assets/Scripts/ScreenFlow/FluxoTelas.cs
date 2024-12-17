using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FluxoTelas : MonoBehaviour
{
    [SerializeField] TMP_InputField email; //variavel que recebe o email digitado pelo usuario
    [SerializeField] TMP_InputField senha; //variavel que recebe a senha digitada pelo usuario
    public static bool ehAluno; //variavel pra validar se o usuario  aluno ou professor
    public GameObject telaSelecao;
    public GameObject telaLogin;
    public Aluno aluno;
    public Button botaoOk;
    public GameObject modalErro;
    public TextMeshProUGUI mensagemErro;
    public string baseURL = "http://3.133.149.14:8080/api";

    public Button voltar;

    void Start()
    {
        FindObjectOfType<ScreenOrientationController>().SetPortraitOrientation();
        modalErro.SetActive(false);
        telaSelecao.SetActive(true); // Comea sempre ativando a tela de seleo e desativando a de login
        telaLogin.SetActive(false);
        voltar.onClick.AddListener(() => OnClickVoltar());
        botaoOk.onClick.AddListener(() => OnClickBotaoOk());
    }

    void OnClickVoltar()
    {
        telaSelecao.SetActive(true); // Desativa a tela de seleo e ativa a de Login 
        telaLogin.SetActive(false);
    }

    void OnClickBotaoOk()
    {
        modalErro.SetActive(false);
    }

    void showModalErro(string mensagem)
    {
        mensagemErro.text = mensagem;
        modalErro.SetActive(true);
    }

    public void loginProfessor()
    {
        ehAluno = false;
        fazerLogin();
    }

    public void loginAluno()
    {
        ehAluno = true;
        fazerLogin();
    }

    public void fazerLogin()
    {
        telaSelecao.SetActive(false); // Desativa a tela de seleo e ativa a de Login 
        telaLogin.SetActive(true);
    }

    public void LoadSceneProfessor(string idProfessor, string nomeProfessor)
    {
        PlayerPrefs.SetString("idProfessor", idProfessor);
        PlayerPrefs.SetString("nomeProfessor", nomeProfessor);
        SceneManager.LoadScene("TelaInicialProfessor");
    }

    public void LoadSceneAluno(string idAluno, string nomeAluno)
    {
        PlayerPrefs.SetString("idAluno", idAluno);
        PlayerPrefs.SetString("nomeAluno", nomeAluno);
        SceneManager.LoadScene("Aluno");
    }

    public IEnumerator LoginRequest(string path, string email, string senha)
    {
        // Validação de campos vazios antes de realizar qualquer requisição
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            showModalErro("Email e senha não podem estar vazios.");
            yield break; // Impede a execução do restante do código
        }

        RequisicaoLogin loginData = new RequisicaoLogin
        {
            Email = email,
            Senha = senha
        };

        Debug.Log(loginData.Email);
        Debug.Log(loginData.Senha);

        // Serializa o objeto para JSON
        string jsonBody = JsonUtility.ToJson(loginData);

        if (ehAluno) path = "/aluno" + path;
        else path = "/professor" + path;

        string remoteURL = baseURL + path;

        Debug.Log("Enviando requisio para URL: " + remoteURL);
        Debug.Log("Corpo JSON: " + jsonBody);

        // Cria a requisio POST com o corpo JSON
        UnityWebRequest request = new UnityWebRequest(remoteURL, "POST");

        // Converte o corpo JSON para bytes e adiciona ao corpo da requisio
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);

        // Define o tipo de contedo como JSON
        request.SetRequestHeader("Content-Type", "application/json");

        // Define o downloadHandler para obter a resposta da requisio
        request.downloadHandler = new DownloadHandlerBuffer();

        // Envia a requisio e espera pela resposta
        yield return request.SendWebRequest();

        // Verifica o status da requisicao
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            showModalErro("Credenciais inválidas.");
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log(json);

            if (string.IsNullOrEmpty(json) || json.Contains("erro") || json.Contains("invalid")) // Erro de resposta do servidor
            {
                showModalErro("Email ou senha incorretos.");
            }
            else
            {
                if (ehAluno)
                {
                    Aluno aluno = JsonUtility.FromJson<Aluno>(json);
                    string alunoId = aluno.id_Aluno.ToString();
                    string alunoNome = aluno.nome;
                    // Redireciona para a cena do aluno
                    LoadSceneAluno(alunoId, alunoNome);
                }
                else
                {
                    Professor professor = JsonUtility.FromJson<Professor>(json);
                    string professorId = professor.id_Professor.ToString();
                    string professorNome = professor.nome;
                    // Redireciona para a cena do professor
                    LoadSceneProfessor(professorId, professorNome);
                }
            }
        }
    }



    [SerializeField]
    public class RequisicaoLogin
    {
        public string Email;
        public string Senha;
    }

    //TODO: Criar uma pasta entitys com todos objetos serializebles do banco, ex: turma, aluno, professor etc
    [SerializeField]
    public class Aluno
    {
        public _id _id;
        public int id_Aluno;
        public string nome;
        public string email;
        public string senha;
        public string matricula;
    }

    [SerializeField]
    public class _id
    {
        public long timestamp;
        public long pid;
        public long increment;
        public string creationTime;
    }

    [SerializeField]
    public class Professor
    {
        public int id_Professor;
        public string nome;
        public string email;
        public string senha;
    }
}

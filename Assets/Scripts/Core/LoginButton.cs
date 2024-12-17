using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginButton : MonoBehaviour
{
    public TMP_InputField emailField; // Campo de entrada para o email
    public TMP_InputField senhaField; // Campo de entrada para a senha
    private Button loginButton;       // Boto de login
    private FluxoTelas fluxo;         // Script de fluxo de telas

    void Start()
    {
        Debug.Log("LoginButton script has started");
        loginButton = GameObject.Find("botao-login").GetComponent<Button>();
        emailField = GameObject.Find("email").GetComponent<TMP_InputField>();
        senhaField = GameObject.Find("senha").GetComponent<TMP_InputField>();
        fluxo = GameObject.Find("FluxoTelaSelecaoParaLogin").GetComponent<FluxoTelas>();
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    void OnLoginClicked()
    {
        Debug.Log("Botao foi clicado!");
        StartCoroutine(fluxo.LoginRequest("/login", emailField.text, senhaField.text));
        //Debug.Log("Aluno: " + aluno);
        emailField.text = "";
        senhaField.text = "";
    }
}

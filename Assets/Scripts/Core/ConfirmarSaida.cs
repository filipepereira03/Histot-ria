using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmarSaída : MonoBehaviour
{
    public GameObject painelConfirmarSaida;

    // Método para mostrar o painel de confirmação depois de clicar no botão sair
    public void MostrarPainelConfirmacao()
    {
        painelConfirmarSaida.SetActive(true);
    }

    // Metodo que faz a aplicação parar: somente quando clicar no sim, deve ser colocado apenas no botão sim
    public void ConfirmarSaida()
    {
        CoroutineManager.Destroy(CoroutineManager.Instance.gameObject);
        SceneManager.LoadScene("Aluno");
    }

    // Método que cancela a saída e esconde o painel, somente quando o usuário clicar em cancelar/não
    public void CancelarSaida()
    {
        painelConfirmarSaida.SetActive(false);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public TextPanel textPanel; // Refer�ncia ao script de di�logo
    private bool isWaiting = false; // Para verificar se o delay est� ativo
    private Button continueButton; // Refer�ncia local ao bot�o

    void Start()
    {
        continueButton = GetComponent<Button>(); // Obt�m o componente Button anexado ao GameObject
        continueButton.onClick.AddListener(OnContinueClicked); // Adiciona listener para o clique
    }

    void OnContinueClicked()
    {
        if (!isWaiting)
        {
            textPanel.NextLine(); // Chama o m�todo NextLine no TextPanel
            StartCoroutine(DelayButton()); // Inicia o delay de 5 segundos
        }
    }

    IEnumerator DelayButton()
    {
        isWaiting = true; // Desativa o bot�o temporariamente
        continueButton.interactable = false; // Desabilita o bot�o para impedir cliques
        yield return new WaitForSeconds(5); // Espera 5 segundos
        continueButton.interactable = true; // Reabilita o bot�o
        isWaiting = false; // Reinicia o estado de espera
    }
}

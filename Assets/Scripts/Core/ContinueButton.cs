using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public TextPanel textPanel; // Referência ao script de diálogo
    private bool isWaiting = false; // Para verificar se o delay está ativo
    private Button continueButton; // Referência local ao botão

    void Start()
    {
        continueButton = GetComponent<Button>(); // Obtém o componente Button anexado ao GameObject
        continueButton.onClick.AddListener(OnContinueClicked); // Adiciona listener para o clique
    }

    void OnContinueClicked()
    {
        if (!isWaiting)
        {
            textPanel.NextLine(); // Chama o método NextLine no TextPanel
            StartCoroutine(DelayButton()); // Inicia o delay de 5 segundos
        }
    }

    IEnumerator DelayButton()
    {
        isWaiting = true; // Desativa o botão temporariamente
        continueButton.interactable = false; // Desabilita o botão para impedir cliques
        yield return new WaitForSeconds(5); // Espera 5 segundos
        continueButton.interactable = true; // Reabilita o botão
        isWaiting = false; // Reinicia o estado de espera
    }
}

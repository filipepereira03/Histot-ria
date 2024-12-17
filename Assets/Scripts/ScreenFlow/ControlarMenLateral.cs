using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlarMenuLateral : MonoBehaviour
{
    [SerializeField] private GameObject telaInicial;
    [SerializeField] private GameObject menuLateral;
    public Button abrirMenu;
    public Button fecharMenu;

    private Vector3 posInicial;
    private Vector3 posFinal;
    private float tempoAnimacao = 1f;



    // deixei comentado a animao, acredito que ela esteja funcionando porm no consegui testar direito no meu computador
    void Start()
    {
        posInicial = new Vector3(Screen.width, menuLateral.transform.localPosition.y, menuLateral.transform.localPosition.z);
        posFinal = new Vector3(-15, menuLateral.transform.localPosition.y, menuLateral.transform.localPosition.z);

        menuLateral.transform.localPosition = posInicial;
        menuLateral.SetActive(false);
    }

    public void AbrirMenu()
    {
        menuLateral.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AbrirMenuAnimacao());
    }

    public void FecharMenu()
    {
        StopAllCoroutines();
        StartCoroutine(FecharMenuAnimacao());
    }


    IEnumerator AbrirMenuAnimacao()
    {
        float tempoPassado = 0f;

        while (tempoPassado < tempoAnimacao)
        {
            float t = Mathf.SmoothStep(0f, 1f, tempoPassado / tempoAnimacao);
            
            menuLateral.transform.localPosition = Vector3.Lerp(posInicial, posFinal, t);
            tempoPassado += Time.deltaTime;
            yield return null;
        }

        menuLateral.transform.localPosition = posFinal;
    }

    IEnumerator FecharMenuAnimacao()
    {
        float tempoPassado = 0f;

        while (tempoPassado < tempoAnimacao)
        {
            float t = Mathf.SmoothStep(0f, 1f, tempoPassado / tempoAnimacao);

            menuLateral.transform.localPosition = Vector3.Lerp(posFinal, posInicial, t);
            tempoPassado += Time.deltaTime;
            yield return null;
        }

        menuLateral.transform.localPosition = posInicial;
        menuLateral.SetActive(false);
    }
}

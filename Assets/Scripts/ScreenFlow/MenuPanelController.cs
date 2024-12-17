using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LateralMenuController : MonoBehaviour
{
    public Canvas Canvas;
    public Canvas telaInicio;
    public Canvas telaEstatic;

    public GameObject menuPanelPrefab;
    private GameObject menuPanelInstance;
    public Transform canvasTransform;

    public Button openMenuButton;

    private Button closeButton;
    private Button logoutButton;
    private Button statisticsButton;
    private Button homeButton;

    private bool isMenuOpen = false;
    private Vector3 closedPosition = new Vector3(600, 0, 0);
    private Vector3 openPosition = new Vector3(201, 0, 0);

    void Start()
    {
        // Instanciar o painel de menu e ocultá-lo inicialmente
        menuPanelInstance = Instantiate(menuPanelPrefab, canvasTransform);
        menuPanelInstance.transform.localPosition = closedPosition;

        // Encontrar os botões no MenuPanel
        closeButton = menuPanelInstance.transform.Find("FecharMenuButton").GetComponent<Button>();
        logoutButton = menuPanelInstance.transform.Find("SairButton").GetComponent<Button>();
        statisticsButton = menuPanelInstance.transform.Find("EstatisticasButton").GetComponent<Button>();
        homeButton = menuPanelInstance.transform.Find("HomeButton").GetComponent<Button>();

        // Adicionar listeners aos botões
        closeButton.onClick.AddListener(FecharMenu);
        logoutButton.onClick.AddListener(Sair);
        statisticsButton.onClick.AddListener(Estatisticas);
        homeButton.onClick.AddListener(Home);

        // Adicionar listener ao botão para abrir o menu
        openMenuButton.onClick.AddListener(AbrirMenu);
    }

    // Método para abrir o menu com transição
    void AbrirMenu()
    {
        if (!isMenuOpen)
        {
            StartCoroutine(MoverPainel(menuPanelInstance, openPosition, 0.5f)); // Abre o menu com transição
            isMenuOpen = true;
        }
    }

    // Método para fechar o menu com transição
    void FecharMenu()
    {
        if (isMenuOpen)
        {
            StartCoroutine(MoverPainel(menuPanelInstance, closedPosition, 0.5f)); // Fecha o menu com transição
            isMenuOpen = false;
            Debug.Log("Menu fechado");
        }
    }

    // Método temporário para a função de logout
    void Sair()
    {
        Debug.Log("Logout clicado");
    }

    // Método temporário para ir à tela de estatísticas
    void Estatisticas()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
        Canvas.gameObject.SetActive(true);
        telaEstatic.gameObject.SetActive(true);
        FecharMenu();
    }

    void Home()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
        Canvas.gameObject.SetActive(true);
        telaInicio.gameObject.SetActive(true);
        FecharMenu();
    }

    IEnumerator MoverPainel(GameObject painel, Vector3 destino, float duracao)
    {
        Vector3 posicaoInicial = painel.transform.localPosition;
        float tempo = 0;

        while (tempo < duracao)
        {
            painel.transform.localPosition = Vector3.Lerp(posicaoInicial, destino, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        painel.transform.localPosition = destino;
    }
}

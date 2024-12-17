using UnityEngine;

public class DynamicScreenManager : MonoBehaviour
{
    // Configurações do Aspect Ratio
    [Header("Aspect Ratio Settings")]
    public float targetWidth = 390f;  // Largura alvo
    public float targetHeight = 844f; // Altura alvo

    // Safe Area
    [Header("Safe Area Settings")]
    public bool adjustForSafeArea = true;

    private Vector2 lastScreenSize; // Para verificar mudanças de resolução
    private RectTransform rectTransform;
    private Camera backgroundCamera; // Câmera para barras pretas

    void Start()
    {
        // Verifica se o script está anexado a um UI com RectTransform
        rectTransform = GetComponent<RectTransform>();

        if (adjustForSafeArea && rectTransform == null)
        {
            Debug.LogWarning("Safe Area adjustment requires this script to be attached to a UI element with RectTransform.");
        }

        // Configura a câmera auxiliar para as barras pretas
        SetupBackgroundCamera();

        // Inicializa com os ajustes iniciais
        ApplyScreenAdjustments();
        lastScreenSize = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        // Verifica se a resolução ou orientação mudou
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            // Atualiza ajustes
            ApplyScreenAdjustments();
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }

    private void ApplyScreenAdjustments()
    {
        FixAspectRatio();

        if (adjustForSafeArea && rectTransform != null)
        {
            AdjustSafeArea();
        }
    }

    private void FixAspectRatio()
    {
        // Calcula o aspect ratio alvo
        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = Camera.main;

        if (camera != null)
        {
            if (scaleHeight < 1.0f)
            {
                // Adiciona letterbox (barras pretas verticais)
                Rect rect = camera.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                camera.rect = rect;
            }
            else
            {
                // Adiciona pillarbox (barras pretas horizontais)
                float scaleWidth = 1.0f / scaleHeight;

                Rect rect = camera.rect;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
                camera.rect = rect;
            }
        }
    }

    private void AdjustSafeArea()
    {
        // Ajusta a Safe Area para dispositivos com notch ou barras pretas
        Rect safeArea = Screen.safeArea;

        if (rectTransform != null)
        {
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
        else
        {
            Debug.LogWarning("Safe Area adjustment requires this script to be attached to a UI element with RectTransform.");
        }
    }

    private void SetupBackgroundCamera()
    {
        // Cria uma câmera auxiliar para o fundo preto
        GameObject bgCameraObj = new GameObject("BackgroundCamera");
        backgroundCamera = bgCameraObj.AddComponent<Camera>();

        // Configura a câmera para exibir apenas uma cor sólida
        backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
        backgroundCamera.backgroundColor = Color.black;

        // Define o plano da câmera para renderizar antes da principal
        backgroundCamera.depth = -10;
        backgroundCamera.cullingMask = 0; // Não renderiza nenhum objeto
    }
}

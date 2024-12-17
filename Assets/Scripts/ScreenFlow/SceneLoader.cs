using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Método para carregar uma cena pelo nome
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
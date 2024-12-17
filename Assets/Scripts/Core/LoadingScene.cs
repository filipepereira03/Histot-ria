using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider LoadingBarFill;
    public float progressValue = 0;
    public float loadingTime = 0;
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }
    IEnumerator LoadSceneAsync(int sceneId)
    {
        LoadingScreen.SetActive(true);
        while (loadingTime < 5)
        {
            progressValue = Mathf.Clamp01(loadingTime / 5.0f);
            loadingTime += Time.deltaTime;
            LoadingBarFill.value = progressValue;
            yield return null;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageInstance : MonoBehaviour
{
    public float FadeInSpeed = 1f;
    public bool FadeIn, FadeOut;
    public GameObject gObject;
    private Image thisImage;
    private int screenHeight = Screen.height;
    private int screenWidth = Screen.width;

    void Start()
    {
        gObject = this.gameObject;
        thisImage = this.gameObject.GetComponent<Image>();
        thisImage.rectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);

        if (FadeIn)
        {
            Color c = thisImage.color;
            c.a = 0;
            thisImage.color = c;
            StartCoroutine(FadeInRoutine(FadeInSpeed));
        }
    }

    void Update()
    {
        if(screenHeight != Screen.height || screenWidth != Screen.width)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            thisImage.rectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
        }
    }

    IEnumerator FadeInRoutine(float speed)
    {
        Color c = thisImage.color;
        for(float alpha = 0f; alpha <= 1f; alpha+=0.05f) 
        {
            c.a = alpha;
            thisImage.color = c;
            yield return new WaitForSeconds(speed / 20); //20 because 0.05f * 20 = 1 second
        }
        c.a = 1.0f;
        thisImage.color = c;
    }

    IEnumerator FadeOutRoutine(float speed)
    {
        Color c = thisImage.color;
        for (float alpha = 1f; alpha >= 0f; alpha -= 0.05f)
        {
            c.a = alpha;
            thisImage.color = c;
            yield return new WaitForSeconds(speed / 20); //20 because 0.05f * 20 = 1 second
        }
        Destroy(gObject);
    }

    public void KillMe()
    {
        StartCoroutine(FadeOutRoutine(FadeInSpeed));
    }
}

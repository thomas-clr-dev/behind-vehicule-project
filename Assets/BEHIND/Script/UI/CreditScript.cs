using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;


public class CreditScript : MonoBehaviour
{
    [SerializeField] float fadeInDuration, persistanceDuration, fadeOutDuration = 1f;
    CanvasGroup[] screens;
    int currentScreen = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screens = new CanvasGroup[transform.childCount];
        for (int i=0;i<transform.childCount;i++)
        {
            screens[i] = transform.GetChild(i).GetComponent<CanvasGroup>();
        }
        foreach (CanvasGroup group in screens) group.alpha = 0;
        StartCoroutine(ShowScreen(screens[0]));
    }



    IEnumerator ShowScreen(CanvasGroup screen)
    {
        while (screen.alpha <1f)
        {
            screen.alpha += Time.deltaTime / fadeInDuration;
            yield return null;
        }
        float chrono = 0;
        
        if (currentScreen>transform.childCount-2) yield break;

        while (chrono<persistanceDuration)
        {
            chrono += Time.deltaTime;
            yield return null;
        }
        while (screen.alpha>0)
        {
            screen.alpha -= Time.deltaTime / fadeInDuration;
            yield return null;
        }
        currentScreen++;
        if (currentScreen<screens.Length) StartCoroutine(ShowScreen(screens[currentScreen]));
    }

} // SCRIPT END

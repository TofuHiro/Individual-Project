using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeIn : MonoBehaviour
{
    [SerializeField] float fadeDelay;
    [SerializeField] float fadeTime;
    [SerializeField] bool fadeOnStart;

    CanvasGroup canvasGroup;

    bool fading = false;
    float timer = 0f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;

        if (fadeOnStart)
            fading = true;
    }

    void Update()
    {
        if (!fading)
            return;

        timer += Time.deltaTime;

        if (timer < fadeDelay)
            return;

        canvasGroup.alpha = 1 - ((timer - fadeDelay) / fadeTime);

        if (timer >= fadeTime + fadeDelay)
            enabled = false;
    }

    public void StartFade()
    {
        fading = true;
    }
}

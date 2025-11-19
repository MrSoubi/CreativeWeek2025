using System;
using UnityEngine;

public class ScreenFadeManager : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private RSE_AskFadeIn askFadeIn;
    [SerializeField] private RSE_AskFadeOut askFadeOut;
    [SerializeField] RSE_OnFadeInComplete onFadeInComplete;
    [SerializeField] RSE_OnFadeOutComplete onFadeOutComplete;
    
    private void OnEnable()
    {
        askFadeIn.Call.AddListener(FadeIn);
        askFadeOut.Call.AddListener(FadeOut);
    }
    
    private void OnDisable()
    {
        askFadeIn.Call.RemoveListener(FadeIn);
        askFadeOut.Call.RemoveListener(FadeOut);
    }
    
    private void FadeOut()
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
        }
    }

    private void FadeIn()
    {
        if (fadeAnimator)
        {
            fadeAnimator.SetTrigger("FadeIn");
        }
    }

    void OnFadeInCompleted()
    {
        onFadeInComplete.Call.Invoke();
    }
    
    void OnFadeOutCompleted()
    {
        onFadeOutComplete.Call.Invoke();
    }
}

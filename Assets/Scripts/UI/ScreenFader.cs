using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image leakyKittenScreen;

    [SerializeField]
    private float fadeTimeStep = 0.01f;

    [SerializeField]
    private float fadeRate = 0.15f;

    [SerializeField]
    private float maxAlpha = 0.65f;

    public bool screenIsVisible { get; private set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        leakyKittenScreen = GetComponent<Image>();
        //leakyKittenScreen.color = new Color(leakyKittenScreen.color.r, leakyKittenScreen.color.g, leakyKittenScreen.color.b, maxAlpha);
        StartFadeOut();
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(fadeTimeStep);

        float nextAlpha = leakyKittenScreen.color.a + fadeRate * Time.deltaTime;

        if(nextAlpha < maxAlpha)
        {            
            leakyKittenScreen.color = new Color(leakyKittenScreen.color.r, leakyKittenScreen.color.g, leakyKittenScreen.color.b, nextAlpha);
            StartCoroutine(FadeIn());
        }

        else
            screenIsVisible = true;
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeTimeStep);

        float nextAlpha = leakyKittenScreen.color.a - fadeRate * Time.deltaTime;

        if (nextAlpha > 0)
        {
            leakyKittenScreen.color = new Color(leakyKittenScreen.color.r, leakyKittenScreen.color.g, leakyKittenScreen.color.b, nextAlpha);
            StartCoroutine(FadeOut());
        }

        else
        {
            leakyKittenScreen.color = new Color(leakyKittenScreen.color.r, leakyKittenScreen.color.g, leakyKittenScreen.color.b, 0);
            screenIsVisible = false;
        }
    }

}

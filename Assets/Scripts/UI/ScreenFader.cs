using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image tinkleTigersScreen;

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
        tinkleTigersScreen = GetComponent<Image>();
        StartFadeOut();
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(fadeTimeStep);

        float nextAlpha = tinkleTigersScreen.color.a + fadeRate * Time.deltaTime;

        if(nextAlpha < maxAlpha)
        {
            tinkleTigersScreen.color = new Color(tinkleTigersScreen.color.r, tinkleTigersScreen.color.g, tinkleTigersScreen.color.b, nextAlpha);
            StartCoroutine(FadeIn());
        }

        else
        {
            screenIsVisible = true;
            StartFadeOut();
        }
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeTimeStep);

        float nextAlpha = tinkleTigersScreen.color.a - fadeRate * Time.deltaTime;

        if (nextAlpha > 0)
        {
            tinkleTigersScreen.color = new Color(tinkleTigersScreen.color.r, tinkleTigersScreen.color.g, tinkleTigersScreen.color.b, nextAlpha);
            StartCoroutine(FadeOut());
        }

        else
        {
            tinkleTigersScreen.color = new Color(tinkleTigersScreen.color.r, tinkleTigersScreen.color.g, tinkleTigersScreen.color.b, 0);
            screenIsVisible = false;
            TheGame.GameControl.gameCanStart = true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XP_Text : MonoBehaviour
{
    private TMP_Text xpText;

    [SerializeField]
    private float maxDistance = 1f;

    private float maxYPos;

    [SerializeField]
    private float fadeRate = 2.5f;

    [SerializeField]
    private float fadeTimeStep = 0.01f;

    [SerializeField]
    private float flashTextSpeed = 2.5f;

    private float lastYPosition;

    // Start is called before the first frame update
    void Awake()
    {
        xpText = GetComponent<TMP_Text>();
        lastYPosition = transform.position.y;
        maxYPos = transform.position.y + maxDistance;
    }

    public void SendFlashText(string xpFlashText)
    {
        xpText.text = xpFlashText;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeTimeStep);

        Fade();
        PopText();

        StartCoroutine(FadeOut());
    }

    private void Fade()
    {
        float colorAlpha = xpText.color.a;
        colorAlpha -= fadeRate * Time.deltaTime;
        xpText.color = new Color(xpText.color.r, xpText.color.g, xpText.color.b, colorAlpha);
    }

    private void PopText()
    {
        if(lastYPosition <= maxYPos)
        {
            float nextPosition = lastYPosition + flashTextSpeed * Time.deltaTime;
            transform.position = new Vector2(transform.position.x, nextPosition);
            lastYPosition = transform.position.y;
        }
    }
}

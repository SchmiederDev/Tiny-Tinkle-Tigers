using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopMessage : MonoBehaviour
{
    private TMP_Text PopMsgTxt;

    [SerializeField]
    private float textAlpha = 0.785f;

    [SerializeField]
    private float sizeCallRate = 0.005f;

    [SerializeField]
    private float bumpCallRate = 0.005f;

    [SerializeField]
    private float minFontSize = 0f;
    [SerializeField]
    private float maxFontSize = 60f;

    [SerializeField]
    private float sizeExpansionRate = 600f;
    [SerializeField]
    private float sizeContractionRate = 700f;

    [SerializeField]
    private float readingTime = 0.5f;

    [SerializeField]
    private float minimalCharSpacing = 0f;

    [SerializeField]
    private float maximumCharSpacing = 5f;

    [SerializeField]
    private float characterSpaceExpansionRate = 0.85f;

    [SerializeField]
    private float characterSpaceContractionRate = 0.65f;

    // Start is called before the first frame update
    void Awake()
    {
        PopMsgTxt = GetComponent<TMP_Text>();
        ResetPopMessage();
    }

    public void SendPopMessage(string message)
    {
        BlendInPopMessage(message);
        StartCoroutine(PopUp());
    }

    private IEnumerator PopUp()
    {
        yield return new WaitForSeconds(sizeCallRate);

        float nextFontSize = PopMsgTxt.fontSize + sizeExpansionRate * Time.deltaTime;

        if (nextFontSize <= maxFontSize)
        {
            PopMsgTxt.fontSize = nextFontSize;
            StartCoroutine(PopUp());
        }

        else
            StartCoroutine(BumpIn());
        
    }

    private void BlendInPopMessage(string message)
    {
        PopMsgTxt.text = message;
        PopMsgTxt.color = new Color(PopMsgTxt.color.r, PopMsgTxt.color.g, PopMsgTxt.color.b, textAlpha);
    }


    private IEnumerator BumpIn()
    {
        yield return new WaitForSeconds(bumpCallRate);

        float nextSpacing = PopMsgTxt.characterSpacing + characterSpaceExpansionRate;

        if (nextSpacing <= maximumCharSpacing)
        {
            PopMsgTxt.characterSpacing = nextSpacing;
            StartCoroutine(BumpIn());
        }

        else
            StartCoroutine(BumpOut());

    }

    private IEnumerator BumpOut()
    {
        yield return new WaitForSeconds(bumpCallRate);

        float nextSpacing = PopMsgTxt.characterSpacing - characterSpaceContractionRate;

        if (nextSpacing > minimalCharSpacing)
        {
            PopMsgTxt.characterSpacing = nextSpacing;
            StartCoroutine(BumpOut());
        }

        else
        {
            PopMsgTxt.characterSpacing = minimalCharSpacing;
            StartCoroutine(WaitReadingTime());
        }
    }

    private IEnumerator WaitReadingTime()
    {        
        yield return new WaitForSeconds(readingTime);
        StartCoroutine(Dwindle());
    }

    private IEnumerator Dwindle()
    {
        yield return new WaitForSeconds(sizeCallRate);

        float nextFontSize = PopMsgTxt.fontSize - sizeContractionRate * Time.deltaTime;

        if (nextFontSize > 0)
        {
            PopMsgTxt.fontSize = nextFontSize;
            StartCoroutine(Dwindle());
        }
        else
            ResetPopMessage();
    }

    private void ResetPopMessage()
    {
        PopMsgTxt.text = "";
        PopMsgTxt.fontSize = minFontSize;
        PopMsgTxt.characterSpacing = minimalCharSpacing;
        PopMsgTxt.color = new Color(PopMsgTxt.color.r, PopMsgTxt.color.g, PopMsgTxt.color.b, 0);
    }
}

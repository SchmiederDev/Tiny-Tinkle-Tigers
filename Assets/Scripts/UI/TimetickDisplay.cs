using UnityEngine;
using TMPro;

public class TimetickDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text TimeDisplay;

    public bool timeDisplayHalt { get; set; } = false;

    // Start is called before the first frame update
    void Awake()
    {
        TimeDisplay = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float minutes = TheGame.GameControl.Timer.minutes;
        float seconds = TheGame.GameControl.Timer.seconds;

        if (!timeDisplayHalt)
        {
            if (seconds > 0)
            {
                if (minutes < 10f)
                {
                    if (seconds < 10f)
                        TimeDisplay.text = "0" + minutes + " : 0" + seconds;
                    else
                        TimeDisplay.text = "0" + minutes + " : " + seconds;
                }
                else
                {
                    if (seconds < 10f)
                        TimeDisplay.text = minutes + " : 0" + seconds;
                    else
                        TimeDisplay.text = minutes + " : " + seconds;
                }
            }

            else
                TimeDisplay.text = "00 : 00";
        }

        else
            TimeDisplay.text = "00 : 00";
    }
}

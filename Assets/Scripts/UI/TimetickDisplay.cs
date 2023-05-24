using UnityEngine;
using TMPro;

public class TimetickDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text TimeDisplay;
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
        
        if(minutes < 10f)
            TimeDisplay.text = "0" + minutes + " : " + seconds;
        else
            TimeDisplay.text = minutes + " : " + seconds;
    }
}

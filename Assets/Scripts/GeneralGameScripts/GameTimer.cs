using System.Collections;
using UnityEngine;

public class GameTimer : MonoBehaviour
{

    public float SecondsSinceLevelStart { get; private set; }
    public float LevelGoalTimeFrame { get; set; }

    public float minutes { get; private set; }
    public float seconds { get; private set; }

    [SerializeField]
    private float timePerKitten = 30f;

    public void Init_Timer(float timeRate)
    {
        LevelGoalTimeFrame = timeRate * timePerKitten;
        SecondsSinceLevelStart = LevelGoalTimeFrame;

        StartCoroutine(ClockTick());        
    }

    void Update()
    {
        minutes = Mathf.Floor(SecondsSinceLevelStart / 60f);
        seconds = SecondsSinceLevelStart % 60f;
    }

    IEnumerator ClockTick()
    {
        yield return new WaitForSecondsRealtime(1f);
        SecondsSinceLevelStart--;
        
        if(SecondsSinceLevelStart > 0)
            StartCoroutine(ClockTick());
        else
            TheGame.GameControl.RestartOnTimeOut();
    }

    public void ResetTimer()
    {        
        SecondsSinceLevelStart = 0;
        LevelGoalTimeFrame = 0;     
    }
    
}

using System.Collections;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public bool TimerHasBeenInitialized = false;

    public float SecondsSinceLevelStart { get; private set; }
    public float LevelGoalTimeFrame { get; set; }

    public float minutes { get; private set; }
    public float seconds { get; private set; }

    [SerializeField]
    private float timePerKitten = 30f;

    int callCounter = 0;

    public void Init_Timer(float timeRate)
    {
        callCounter++;
        Debug.Log("Timer Init has been called: " + callCounter + "x times");
        TimerHasBeenInitialized = true;
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
        StartCoroutine(ClockTick());
    }

    public void ResetTimer()
    {
        SecondsSinceLevelStart = 0;
        LevelGoalTimeFrame = 0;
        StopCoroutine(ClockTick());
        TimerHasBeenInitialized = false;
    }
    
}

using System.Collections;
using UnityEngine;

public class PeePuddle : MonoBehaviour
{
    Animator CollectSparksAnimator;

    Touch PlayerTouch;
    Vector2 TouchPos;

    Vector2 startingPos;
    Vector2 direction;

    bool directionChosen = false;

    Vector2 currentDirection;
    Vector2 lastDirection;

    bool stylusOverPuddle = false;

    bool swipedBackOverPuddle = false;

    [SerializeField]
    float interactionRadius = 1.0f;

    [SerializeField]
    float destructionTimeStep = 0.25f;

    [SerializeField]
    int XPperPuddle = 100;

    [SerializeField]
    int XPPenalty = 200;

    [SerializeField]
    int XPdecreaseRate = 10;

    [SerializeField]
    int XPDecreaseTimeStep = 5;

    int secondsSinceExistence = 0;

    int nextTimeStep = 0;
    int timeStepCounter = 0;

    [SerializeField]
    private XP_Text xpFlashText;

    void Start()
    {
        CollectSparksAnimator = GetComponent<Animator>();
        nextTimeStep = XPDecreaseTimeStep;
        StartCoroutine(CountTimeSinceExistence());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            PlayerTouch = Input.GetTouch(0);            
            TouchPos = Camera.main.ScreenToWorldPoint(PlayerTouch.position);

            CheckPuddleTouchDistance();
            RecordCurrentTouchPhase();
            CheckPlayerPuddleInteractionStatusOnTouchEnd();
        }
    }

    private void RecordCurrentTouchPhase()
    {
        switch (PlayerTouch.phase)
        {
            case TouchPhase.Began:
                startingPos = PlayerTouch.position;
                directionChosen = false;
                break;

            case TouchPhase.Moved:
                
                direction = PlayerTouch.position - startingPos;
                direction.Normalize();
                currentDirection = direction;
                break;

            case TouchPhase.Ended:
                directionChosen = true;
                break;
        }
        
    }

    private void CheckPuddleTouchDistance()
    {
        float distance = Vector2.Distance(gameObject.transform.position, TouchPos);

        if (distance <= interactionRadius)
            stylusOverPuddle = true;
    }

    private void CheckPlayerPuddleInteractionStatusOnTouchEnd()
    {
        if (directionChosen)
        {

            if(stylusOverPuddle)
                CheckBackSwipe();

            lastDirection = direction;
            stylusOverPuddle = false;
        }
    }

    private void CheckBackSwipe()
    {
        if(currentDirection.x > 0 && lastDirection.x < 0 || currentDirection.x < 0 && lastDirection.x > 0)
            swipedBackOverPuddle = true;

        else if (currentDirection.y > 0 && lastDirection.y < 0 || currentDirection.y < 0 && lastDirection.y > 0)
            swipedBackOverPuddle = true;

        else
            swipedBackOverPuddle = false;
    }

    private void FixedUpdate()
    {
        if (swipedBackOverPuddle)
            StartCoroutine(RemovePuddle());

    }

    IEnumerator RemovePuddle()
    {
        int acquiredXP = CalculateXP();

        xpFlashText.SendFlashText(acquiredXP.ToString());
        CollectSparksAnimator.SetBool("SparksOn", true);

        yield return new WaitForSeconds(destructionTimeStep);
        
        TheGame.GameControl.AddXP(acquiredXP);
        TheGame.GameControl.PuddlesOnScene.Remove(gameObject);
        Destroy(gameObject);
    }

    public IEnumerator DealPenalty()
    {
        xpFlashText.SwitchToPenaltyColor();
        string penaltyMessage = "- " + XPPenalty.ToString();
        Debug.Log(penaltyMessage);
        xpFlashText.SendFlashText(penaltyMessage);
        CollectSparksAnimator.SetBool("SparksOn", true);

        yield return new WaitForSeconds(destructionTimeStep);

        TheGame.GameControl.SubstractXP(XPPenalty);
        TheGame.GameControl.PuddlesOnScene.Remove(gameObject);
        Destroy(gameObject);
    }

    private int CalculateXP()
    {
        int maxXP = XPperPuddle * TheGame.GameControl.KittensOnScene.Count;
        int xpDecrease = timeStepCounter * XPdecreaseRate;

        int effectiveXP = maxXP - xpDecrease;
        return effectiveXP;
    }

    IEnumerator CountTimeSinceExistence()
    {
        yield return new WaitForSecondsRealtime(1f);

        if (secondsSinceExistence < int.MaxValue)
            secondsSinceExistence++;

        if(secondsSinceExistence == nextTimeStep)
        {
            timeStepCounter++;
            nextTimeStep = secondsSinceExistence + XPDecreaseTimeStep;
        }

        StartCoroutine(CountTimeSinceExistence());
    }
}

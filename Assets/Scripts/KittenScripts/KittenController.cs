using System.Collections;
using UnityEngine;

public class KittenController : MonoBehaviour
{
    [SerializeField]
    private Transform KittenTransform;

    [SerializeField]
    private Rigidbody2D KittenRB;

    [SerializeField]
    private Animator KittenAnimator;

    [SerializeField]
    private float minMovementSpeed = 1f;

    [SerializeField]
    private float maxMovementSpeed = 3f;

    private float averageMovementSpeed;

    private float movementSpeed;

    [SerializeField]
    private float minSpeedThreshold = 2.5f;
    [SerializeField]
    private float maxSpeedThreshold = 1.5f;

    [SerializeField]
    private float blockControlsTimeSpan = 2f;

    [SerializeField]
    private float minRunOutTime = 1f;
    [SerializeField]
    private float maxRunOutTime = 5f;

    private float runningOutTime;

    [SerializeField]
    private float runningTimeIncreaseRate = 0.2f;

    [SerializeField]
    private float maxTouchDistance = 3.5f;

    private Touch LastFirstTouch;
    private Vector3 touchWorldPos;

    private Vector2 direction;
    private Vector2 invertedDirection;

    private bool facingRight = true;

    private bool shouldMove = false;
    public bool isMovingRandomly { get; private set; } = true;

    private bool runOutEnabled = false;

    private Vector2 randomDirection;

    private bool IsIdle = false;
    private bool CRsRunning = true;

    [SerializeField]
    private float IdlePhase = 3f;
    [SerializeField]
    private float WalkingPhase = 5f;

    private bool isFrightened = false;

    public bool controlsEnabled { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        KittenRB = GetComponent<Rigidbody2D>();
        KittenTransform = GetComponent<Transform>();
        KittenAnimator = GetComponent<Animator>();

        movementSpeed = minMovementSpeed;
        averageMovementSpeed = (minMovementSpeed + maxMovementSpeed) / 2f;
        DecideOnRandomMovement();
    }

    // Update is called once per frame
    void Update()
    {
        bool obstaclesInTheWay = TraceObstacles();

        if (obstaclesInTheWay)
        {
            if(!isFrightened)
                ActFrightened();

            controlsEnabled = false;
            
            InvertDirections();
            StartCoroutine(BlockControls());
        }

        if (controlsEnabled)
        {
            if(isFrightened)
                OvercomeFear();
            
            if (Input.touchCount > 0)
            {   
                LastFirstTouch = Input.GetTouch(0);
                ConvertToWorldPoint();

                float touchCatDistance = Vector2.Distance(KittenRB.position, touchWorldPos);
                
                if(touchCatDistance <= maxTouchDistance)
                {
                    if(CRsRunning)
                        StopCRs();

                    shouldMove = true;
                    runOutEnabled = true;

                    direction = (Vector2)touchWorldPos - KittenRB.position;
                    direction.Normalize();

                    invertedDirection = direction * -1f;
                    CheckDirection(invertedDirection);
                    
                    IncreaseRunTimeOnTouchLength();
                }
            }

            else
            {
                if (runOutEnabled)
                {
                    StartCoroutine(FinishMoving());
                    runOutEnabled = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (shouldMove)
        {
            TheGame.GameControl.GameAudio.PlaySound("MEOW_01");
            
            DetermineMovementSpeed();
            
            KittenRB.MovePosition(KittenRB.position - direction * movementSpeed * Time.deltaTime);
            KittenAnimator.SetBool("IsMoving", shouldMove);
        }

        else
            MoveRandomly();
    }

    private void ConvertToWorldPoint()
    {
        touchWorldPos = Camera.main.ScreenToWorldPoint(LastFirstTouch.position);
        touchWorldPos.z = 0;
    }

    private void CheckDirection(Vector2 direction)
    {
        if (direction.x > 0 && !facingRight)
            Flip();

        if (direction.x < 0 && facingRight)
            Flip();
    }

    private void Flip()
    {        
        Vector3 currentScale = KittenTransform.localScale;
        currentScale.x *= -1f;
        KittenTransform.localScale = currentScale;

        facingRight = !facingRight;
    }

    private void IncreaseRunTimeOnTouchLength()
    {
        
        if(runningOutTime < maxRunOutTime)
            runningOutTime += runningTimeIncreaseRate * Time.deltaTime;
        else
            runningOutTime = maxRunOutTime;
    }

    private void DetermineMovementSpeed()
    {
        float touchCatDistance = Vector2.Distance(KittenRB.position, touchWorldPos);

        if (touchCatDistance < maxSpeedThreshold)
            movementSpeed = maxMovementSpeed;
        else if (touchCatDistance >= maxSpeedThreshold && touchCatDistance < minSpeedThreshold)
            movementSpeed = averageMovementSpeed;
        else if (touchCatDistance >= minSpeedThreshold)
            movementSpeed = minMovementSpeed;

    }

    private IEnumerator FinishMoving()
    {   
        yield return new WaitForSeconds(runningOutTime);

        shouldMove = false;
        KittenAnimator.SetBool("IsMoving", shouldMove);
        
        runningOutTime = minRunOutTime;
        movementSpeed = minMovementSpeed;
        DecideOnRandomMovement();
    }

    private void DecideOnRandomMovement()
    {
        
        float randomStateRange = Random.Range(0f, 2f);

        if (randomStateRange >= 1f)
            StartCoroutine(WalkOn());
        else
            StartCoroutine(StayIdle());

        isMovingRandomly = true;
    }

    private void CalculateRandomDirection()
    {
        float randomXDir = Random.Range(-1f, 1f);
        float randomYDir = Random.Range(-1f, 1f);
        randomDirection = new Vector2(randomXDir, randomYDir);        
    }

    IEnumerator StayIdle()
    {        
        IsIdle = true;
        CRsRunning = true;
        KittenAnimator.SetBool("IsMoving", !IsIdle);

        yield return new WaitForSeconds(IdlePhase);
                
        DecideOnRandomMovement();
    }

    IEnumerator WalkOn()
    {        
        IsIdle = false;

        CalculateRandomDirection();
        CheckDirection(randomDirection);
        KittenAnimator.SetBool("IsMoving", !IsIdle);
        
        yield return new WaitForSeconds(WalkingPhase);
        DecideOnRandomMovement();
    }

    private void StopCRs()
    {
        StopAllCoroutines();
        CRsRunning = false;
        isMovingRandomly = false;
    }

    private void MoveRandomly()
    {
        if (!IsIdle)
            KittenRB.MovePosition(KittenRB.position + randomDirection * averageMovementSpeed * Time.deltaTime);
    }

    private bool TraceObstacles()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(KittenRB.position, direction);

        if (rayHit.collider != null)
        {
            if (rayHit.collider.gameObject.tag == "PeePuddle" || rayHit.collider.gameObject.tag == "CatTray")
                return true;

            else
                return false;
        }

        else
            return false;
    }

    public void InvertDirections()
    {
        direction *= -1f;
        invertedDirection *= -1f;
    }

    private IEnumerator BlockControls()
    {
        yield return new WaitForSeconds(blockControlsTimeSpan);
        controlsEnabled = true;
    }

    public void ActFrightened()
    {
        isFrightened = true;
        KittenAnimator.SetBool("IsFrightened", isFrightened);
    }

    public void OvercomeFear()
    {
        isFrightened = false;
        KittenAnimator.SetBool("IsFrightened", isFrightened);
    }
}

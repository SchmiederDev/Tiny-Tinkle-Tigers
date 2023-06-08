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

    private float movementSpeed;

    [SerializeField]
    private float minRunOutTime = 1f;
    [SerializeField]
    private float maxRunOutTime = 5f;

    private float runningOutTime;

    [SerializeField]
    private float runningTimeIncreaseRate = 0.2f;

    private Touch LastFirstTouch;
    private Vector3 touchWorldPos;

    private Vector2 Direction;

    private bool facingRight = true;

    private bool shouldMove = false;

    public bool controlsEnabled { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        KittenRB = GetComponent<Rigidbody2D>();
        KittenTransform = GetComponent<Transform>();
        KittenAnimator = GetComponent<Animator>();
        movementSpeed = minMovementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(controlsEnabled)
        {
            if (Input.touchCount > 0)
            {
                shouldMove = true;
                LastFirstTouch = Input.GetTouch(0);
                ConvertToWorldPoint();
                Direction = (Vector2)touchWorldPos - KittenRB.position;
                Direction.Normalize();
                CheckDirection();
                IncreaseRunTimeOnTouchLength();
            }

            else
                StartCoroutine(FinishMoving());
        }
    }

    private void FixedUpdate()
    {
        if(shouldMove)
        {
            TheGame.GameControl.GameAudio.PlaySound("MEOW_01");
            CheckSpeed();
            KittenAnimator.SetBool("IsMoving", shouldMove);
            KittenRB.MovePosition(KittenRB.position - Direction * movementSpeed * Time.deltaTime);            
        }
    }

    private void ConvertToWorldPoint()
    {
        touchWorldPos = Camera.main.ScreenToWorldPoint(LastFirstTouch.position);
        touchWorldPos.z = 0;
    }

    private void CheckDirection()
    {
        if (Direction.x > 0 && facingRight)
        {
            Flip();
        }

        if(Direction.x < 0 && !facingRight)
        {
            Flip();
        }
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

    private void CheckSpeed()
    {
        float touchCatDistance = Vector2.Distance(KittenRB.position, touchWorldPos);

        if (touchCatDistance < 2.5f)
            movementSpeed = maxMovementSpeed;
        else if (touchCatDistance >= 2.5f && touchCatDistance < 5f)
            movementSpeed = maxMovementSpeed - minMovementSpeed;
        else if (touchCatDistance >= 5f)
            movementSpeed = minMovementSpeed;

    }

    private IEnumerator FinishMoving()
    {
        yield return new WaitForSeconds(runningOutTime);
        shouldMove = false;
        KittenAnimator.SetBool("IsMoving", shouldMove);
        runningOutTime = minRunOutTime;
        movementSpeed = minMovementSpeed;
    }
}

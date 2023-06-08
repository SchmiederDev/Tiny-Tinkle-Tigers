using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHover : MonoBehaviour
{
    Touch playerTouch;
    bool isTouching = false;

    Vector3 touchWorldPos;

    Animator TouchAnimator;

    // Start is called before the first frame update
    void Start()
    {
        TouchAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            isTouching = true;
            playerTouch = Input.GetTouch(0);
            ConvertToWorldPoint();
            transform.position = touchWorldPos;
            TouchAnimator.SetBool("IsTouching", isTouching);
        }

        else
        {
            isTouching = false;
            TouchAnimator.SetBool("IsTouching", isTouching);
        }
    }

    private void ConvertToWorldPoint()
    {
        touchWorldPos = Camera.main.ScreenToWorldPoint(playerTouch.position);
        touchWorldPos.z = 0;
    }
}

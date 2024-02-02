using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fruit : MonoBehaviour
{

    public int fruitType;
    private Vector2 tempPosition;

    public int column;
    public int row;

    private float swipeAngle = 0;

    private Vector2 lastTouchPosition;
    public Vector2 firstTouchPosition;

    public bool fadeout = false;

    public Vector2 targetV;

    private Board board;

    public bool isClicked,isSwiped=false,isMoving=false;

    public Animator animator;

    public string damageID; 

    [HideInInspector]
    public int swipeRight = Animator.StringToHash("isSwipeRight");
    [HideInInspector]
    public int swipeLeft = Animator.StringToHash("isSwipeLeft");
    [HideInInspector]
    public int swipeUp = Animator.StringToHash("isSwipeUp");
    [HideInInspector]
    public int swipeDown = Animator.StringToHash("isSwipeDown");
    [HideInInspector]
    public int swipeFlash = Animator.StringToHash("isSwipeHintIdle");


    void Awake()
    {
        board = FindObjectOfType<Board>();
        targetV.x = transform.position.x;
        targetV.y = transform.position.y;

    }

    void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name != "LevelEditor")
        {
            board.allFruits[column, row] = this.gameObject;

          
            if (isSwiped)
            {
                // Swipe movement
                transform.position = Vector2.Lerp(transform.position, targetV, 7f * Time.deltaTime);

            }
            else
            {
                // Other movements
                transform.position = Vector2.Lerp(transform.position, targetV, 6f * Time.deltaTime);

            }

            if(Vector2.Distance(targetV, transform.position) > 0.1f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            if (isClicked)
            {
                lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(transform.position, lastTouchPosition) > 0.6f)
                {
                    CalculateAngle();
                    isClicked = false;

                }
            }            
        }      
    }

    private void CalculateAngle()
    {
        float angleInRadians = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x);
        swipeAngle = angleInRadians * Mathf.Rad2Deg;

        board.SwipeFruits(swipeAngle, column, row);
    }

}

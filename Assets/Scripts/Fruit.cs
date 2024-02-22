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

    public string colorType;

    public bool fadeout = false;

    public Vector2 targetV;

    private Board board;

    public bool isClicked,isSwiped=false,isMoving=false;

    public Animator animator;

    public string damageID;

    public float speedMultiplier=6f;

    public bool isPowerUpSoundPlayed = false;

    public bool outsideOfBoard = false;

    public int hitBorder=0;

    public bool moveToward = false;

    public GameObject attachedPowerUp=null;

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
    [HideInInspector]
    public int boomerangRotating = Animator.StringToHash("isRotating");


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
            if (!outsideOfBoard)
            {
                board.allFruits[column, row] = this.gameObject;
            }

          
            if (isSwiped)
            {
                // Swipe movement
                transform.position = Vector2.Lerp(transform.position, targetV, 15f * Time.deltaTime);

            }
            else
            {
                // Other movements
                if (moveToward)
                {
                    if(attachedPowerUp)
                    {
                        attachedPowerUp.GetComponent<Fruit>().targetV = transform.position;
                    }
                    transform.position = Vector2.MoveTowards(transform.position, targetV, speedMultiplier * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.Lerp(transform.position, targetV, speedMultiplier * Time.deltaTime);

                }


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

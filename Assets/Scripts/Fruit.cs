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

    public bool isClicked,isSwiped=false;

    public Animator animator;

    public int swipeRight = Animator.StringToHash("isSwipeRight");
    public int swipeLeft = Animator.StringToHash("isSwipeLeft");
    public int swipeUp = Animator.StringToHash("isSwipeUp");
    public int swipeDown = Animator.StringToHash("isSwipeDown");

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

            // For Moving Left or Right Sides
            if (Mathf.Abs(targetV.x - transform.position.x) > .1)
            {
                // MOVE TOWARDS THE TARGET
                tempPosition = new Vector2(targetV.x, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            }
            else
            {
                // DIRECTLY SET THE POSITION
                tempPosition = new Vector2(targetV.x, transform.position.y);
                transform.position = tempPosition;
            }

            //  For Moving Up or Down Sides
            if (Mathf.Abs(targetV.y - transform.position.y) > .1)
            {
                // MOVE TOWARDS THE TARGET
                tempPosition = new Vector2(transform.position.x, targetV.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            }
            else
            {
                // DIRECTLY SET THE POSITION
                tempPosition = new Vector2(transform.position.x, targetV.y);
                transform.position = tempPosition;
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

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(1))
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                // If so, destroy this fruit

                if (SceneManager.GetActiveScene().name == "LevelEditor")
                    NewDestroyFruit();
                else
                    board.ReplaceDestroyedFruit(column, row);
                Destroy(gameObject);
            }
        }
        */
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                // If so, destroy this fruit
                if (SceneManager.GetActiveScene().name == "LevelEditor")
                    NewDestroyObstacle();
                else
                    return;
            }
        }
        */
    }

    private void OnMouseDown()
    {     
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isClicked = true;
    }
    
    private void OnMouseUp()
    {
        isClicked = false;
        if (fruitType < 0 && board.taskController.moveCount > 0 && !isSwiped)
        {
            board.taskController.MovePlayed();
            board.ActivatePowerUp(gameObject);
        }     
    }
    
    // IT WILL BE DELETED

    /*void DestroyFruit()
    {
        // DESTROY THE CURRENT FRUIT
        Destroy(gameObject);

        // CALL THE METHOD IN THE BOARD SCRIPT TO REPLACE THE DESTROYED FRUIT
        board.ReplaceDestroyedFruit(column, row);
    }*/


    private void CalculateAngle()
    {
        float angleInRadians = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x);
        swipeAngle = angleInRadians * Mathf.Rad2Deg;

        board.SwipeFruits(swipeAngle, column, row);
    }

    /*
    private void NewDestroyFruit()
    {
        // DESTROY THE CURRENT FRUIT
        Destroy(gameObject);

        // CALL THE METHOD IN THE LevelManager SCRIPT TO REPLACE THE DESTROYED FRUIT
        FindObjectOfType<LevelManager>().ReplaceDestroyedFruit(column, row);
    }

    private void NewDestroyObstacle()
    {
        // DESTROY THE CURRENT FRUIT
        Destroy(gameObject);

        // CALL THE METHOD IN THE LevelManager SCRIPT TO REPLACE THE DESTROYED FRUIT
        FindObjectOfType<LevelManager>().ReplaceObstacle(column, row);
    }
    */
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    public int fruitType;
    private Vector2 tempPosition;

    public int column;
    public int row;

    public float swipeAngle = 0;

    private Vector2 lastTouchPosition;
    public Vector2 firstTouchPosition;

    public Vector2 targetV;

    private Board board;

    public bool isClicked;

    void Start()
    {
        board = FindObjectOfType<Board>();
        targetV.x = transform.position.x;
        targetV.y = transform.position.y;
    }

    void Update()
    {
        // For Moving Left or Right Sides
        if (Mathf.Abs(targetV.x - transform.position.x) > .1)
        {
            // MOVE TOWARDS THE TARGET
            tempPosition = new Vector2(targetV.x, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            // DIRECTLY SET THE POSITION
            tempPosition = new Vector2(targetV.x, transform.position.y);
            transform.position = tempPosition;
            board.allFruits[column, row] = this.gameObject;
        }


        //  For Moving Up or Down Sides
        if (Mathf.Abs(targetV.y - transform.position.y) > .1)
        {
            // MOVE TOWARDS THE TARGET
            tempPosition = new Vector2(transform.position.x, targetV.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            // DIRECTLY SET THE POSITION
            tempPosition = new Vector2(transform.position.x, targetV.y);
            transform.position = tempPosition;
            board.allFruits[column, row] = this.gameObject;
        }

        if(isClicked)
        {
            lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(transform.position, lastTouchPosition) > 0.6f)
            {
                CalculateAngle();
                isClicked = false;

            }
        }

        if(Input.GetMouseButtonDown(1)) 
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                // If so, destroy this fruit
                DestroyFruit();
            }
        }

        
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isClicked = true;
    }

    void DestroyFruit()
    {
        // Destroy the current fruit
        Destroy(gameObject);

        // Call the method in the Board script to replace the destroyed fruit
        board.ReplaceDestroyedFruit(column, row);
    }


    /*
    private void OnMouseDrag()
    {
      
            lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Debug.Log(Vector2.Distance(firstTouchPosition, lastTouchPosition));
            Debug.Log("Dragging " + column + "," + row);
            if (Vector2.Distance(firstTouchPosition, lastTouchPosition) > 0.5f)
            {
                CalculateAngle();
            }
        
       
    }
    */


    /*private void OnMouseUp()
    {
        
        lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(firstTouchPosition, lastTouchPosition) > 0.5f)
        {
            CalculateAngle();
        }
        
    }*/



    private void CalculateAngle()
    {
        float angleInRadians = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x);
        swipeAngle = angleInRadians * Mathf.Rad2Deg;

        board.MoveFruits(swipeAngle,column,row);

        //board.CheckAndDestroyMatches();
    }


}

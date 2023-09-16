using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    private Vector2 tempPosition;

    public float swipeAngle = 0;

    public int column;
    public int row;
    public float targetX;
    public float targetY;

    private GameObject otherFruit;

    private Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = transform.position.x;
        targetY = transform.position.y;
        row = (int)(targetY+ board.yOffset);
        column = (int)(targetX + board.xOffset);
    }

    void Update()
    {
        targetX = column - board.xOffset;
        targetY = row - board.yOffset;
        // For Moving Left or Right Sides
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // MOVE TOWARDS THE TARGET
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            // DIRECTLY SET THE POSITION
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allFruits[column, row] = this.gameObject;
        }


        //  For Moving Up or Down Sides
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // MOVE TOWARDS THE TARGET
            tempPosition = new Vector2(transform.position.x,targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            // DIRECTLY SET THE POSITION
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allFruits[column, row] = this.gameObject;
        }
        //
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        float angleInRadians = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x);
        swipeAngle = angleInRadians * Mathf.Rad2Deg;
 
        if(Vector2.Distance(firstTouchPosition, lastTouchPosition) > 0.5f)
        {
            MoveFruits();
        }
    }

    private void MoveFruits()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
        {
            // RIGHT SWIPE
            otherFruit = board.allFruits[column + 1, row];
            otherFruit.GetComponent<Fruit>().column -= 1;
            column++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            // UP SWIPE
            otherFruit = board.allFruits[column, row + 1];
            otherFruit.GetComponent<Fruit>().row -= 1;
            row++;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // LEFT SWIPE
            otherFruit = board.allFruits[column - 1, row];
            otherFruit.GetComponent<Fruit>().column += 1;
            column--;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // DOWN SWIPE
            otherFruit = board.allFruits[column, row - 1];
            otherFruit.GetComponent<Fruit>().row += 1;
            row--;
        }
    }
}

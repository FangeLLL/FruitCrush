using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    private Board board;
 
    public int tileType=0;
    public int row, column;
    public GameObject[] obstacles = new GameObject[3];
    public int indexOfVisibleOne=-1;
    public bool isCurrentObstacleBox=false;

    private Vector2 firstTouchPosition;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0) && board.specialPowerID != 0)
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos) && !board.specialSwipe)
            {
                board.ActivateSpecialPower(column, row);
            }
        }
        */
    }

    private void OnMouseDown()
    {
        if (board.specialPowerID == 0 || board.specialSwipe)
        {
            if (board.allFruits[column, row])
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                board.allFruits[column, row].GetComponent<Fruit>().firstTouchPosition = firstTouchPosition;
                board.allFruits[column, row].GetComponent<Fruit>().isClicked = true;
            }
        }
    }

    private void OnMouseUp()
    {

        if (board.specialPowerID == 0 || board.specialSwipe)
        {
            if (board.allFruits[column, row])
            {
                if (board.allFruits[column, row].GetComponent<Fruit>().isClicked&&!board.specialSwipe && board.allFruits[column, row].GetComponent<Fruit>().fruitType < 0 && board.taskController.moveCount > 0 && !board.allFruits[column, row].GetComponent<Fruit>().isSwiped && board.taskController.isBoardActive)
                {
                    if (Vector2.Distance(transform.position, firstTouchPosition) < 0.3f)
                    {
                        board.taskController.MovePlayed();
                        board.StopHint();
                        board.ActivatePowerUp(board.allFruits[column, row]);

                    }              
                }
                board.allFruits[column, row].GetComponent<Fruit>().isClicked = false;

            }
        }
        else
        {
            board.ActivateSpecialPower(column, row);

        }

    }

    public void Explosion(int column,int row, string damageID)
    {

        for(int i=-1; i<2; i+=2)
        {
            if (column + i<board.width && column + i>=0 && board.allTiles[column + i, row])
            {
                if (board.allTiles[column + i, row].GetComponent<BackgroundTile>().isCurrentObstacleBox)
                {
                    board.allTiles[column + i, row].GetComponent<BackgroundTile>().Boom(damageID);
                }
            }
            
        }

        for (int i = -1; i < 2; i += 2)
        {
            if (row + i < board.height && row + i >= 0 && board.allTiles[column, row + i])
            {
                if (board.allTiles[column, row + i].GetComponent<BackgroundTile>().isCurrentObstacleBox)
                {
                    board.allTiles[column, row + i].GetComponent<BackgroundTile>().Boom(damageID);
                }
            }      
        }

        Boom(damageID);

    }
 
    public void Boom(string damageID)
    {
        if (indexOfVisibleOne>=0 && !obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.powerUpNeed)
        {
            obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().TakeDamage(damageID);
            DetectVisibleOne();
        }
    }

    public void PowerUpBoom(string damageID)
    {
        if (indexOfVisibleOne >= 0)
        {
            obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().TakeDamage(damageID);
            DetectVisibleOne();
        }
    }

    /// <summary>
    /// If one of the obstacle of tile has been destroyed then detect the current visible one.   
    /// </summary>
    public void DetectVisibleOne()
    {
        int tempIndex=-1;

        for(int i=0;i<obstacles.Length;i++)
        {
            // Checking if obstacle exist and after if its health bigger then zero because when obstacles destroy they will destroy by fadeout function and it
            // takes a little time to disappear so system must check its health.
            if (obstacles[i] && obstacles[i].GetComponent<ObstacleScript>().health>0)
            {
                tempIndex = i;
                i = obstacles.Length;
            }
        }
       
        indexOfVisibleOne = tempIndex;
        // If there is no obstacle left then isCurrentObstacleBox variable needs to be false.
        if (indexOfVisibleOne >= 0)
        {
            isCurrentObstacleBox = obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.boxObstacle;

        }
        else
        {
            isCurrentObstacleBox = false;

        }
    }
   
}

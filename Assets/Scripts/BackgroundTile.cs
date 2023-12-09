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

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && board.specialPowerID != 0)
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                board.ActivateSpecialPower(column, row);
            }
        }
    }

    public void Explosion(int column,int row)
    {

        for(int i=-1; i<2; i+=2)
        {
            if (column + i<board.width && column + i>=0)
            {
                if (board.allTiles[column + i, row].GetComponent<BackgroundTile>().isCurrentObstacleBox)
                {
                    board.allTiles[column + i, row].GetComponent<BackgroundTile>().Boom();
                }
            }
            
        }

        for (int i = -1; i < 2; i += 2)
        {
            if (row + i < board.height && row + i >= 0)
            {
                if (board.allTiles[column, row + i].GetComponent<BackgroundTile>().isCurrentObstacleBox)
                {
                    board.allTiles[column, row + i].GetComponent<BackgroundTile>().Boom();
                }
            }      
        }

        Boom();

    }
 
    public void Boom()
    {
        if (indexOfVisibleOne>=0)
        {
            obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().TakeDamage();
            DetectVisibleOne();
        }
    }

    /// <summary>
    /// If one of the obstacle of tile has been destroyed then detect the current visible one.   
    /// </summary>
    public void DetectVisibleOne()
    {
        int tempHierarchy = -1;
        int tempIndex=-1;
        for(int i=0;i<obstacles.Length;i++)
        {
            // Checking if obstacle exist and after if its health bigger then zero because when obstacles destroy they will destroy by fadeout function and it
            // takes a little time to disappear so system must check its health.
            if (obstacles[i] && obstacles[i].GetComponent<ObstacleScript>().health>0)
            {
                int temp = obstacles[i].GetComponent<ObstacleScript>().hierarchy;
                if (temp > tempHierarchy)
                {
                    tempHierarchy = temp;
                    tempIndex = i;
                }
            }
        }
       
        indexOfVisibleOne = tempIndex;
        // If there is no obstacle left then isCurrentObstacleBox variable needs to be false.
        if (indexOfVisibleOne >= 0)
        {
            isCurrentObstacleBox = obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().boxObstacle;

        }
        else
        {
            isCurrentObstacleBox = false;

        }
    }
   
}

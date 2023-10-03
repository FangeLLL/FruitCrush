using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    public bool strawBale=false;
    private Board board;
    private TaskController taskController;
    public GameObject strawBaleObj;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        taskController = FindObjectOfType<TaskController>();

    }

    public void Explosion(int column,int row)
    {

        for(int i=-1; i<2; i+=2)
        {
            if (column + i<board.width && column + i>=0)
            {
                if (board.allTiles[column + i, row].GetComponent<BackgroundTile>().strawBale)
                {
                    board.allTiles[column + i, row].GetComponent<BackgroundTile>().Boom(column + i, row);
                }
            }
            
        }

        for (int i = -1; i < 2; i += 2)
        {
            if (row + i < board.height && row + i >= 0)
            {
                if (board.allTiles[column, row + i].GetComponent<BackgroundTile>().strawBale)
                {
                    board.allTiles[column, row + i].GetComponent<BackgroundTile>().Boom(column, row + i);
                }
            }      
        }

    }

    public void Boom(int column,int row)
    {
        if (strawBale)
        {
            strawBale = false;
            StartCoroutine(board.FadeOut(strawBaleObj));
            taskController.TaskProgress(0);
         
            
        }
    }

   
}

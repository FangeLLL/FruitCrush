using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    public bool strawBale=false, wheatFarm=false;
    private Board board;
    private TaskController taskController;
    public GameObject strawBaleObj;
    public GameObject wheatFarmObj;
    AudioManager audioManager;
    public int tileType=0;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        taskController = FindObjectOfType<TaskController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

    }

    public void Explosion(int column,int row)
    {

        for(int i=-1; i<2; i+=2)
        {
            if (column + i<board.width && column + i>=0)
            {
                if (board.allTiles[column + i, row].GetComponent<BackgroundTile>().strawBale)
                {
                    board.allTiles[column + i, row].GetComponent<BackgroundTile>().BoxExplode();
                }
            }
            
        }

        for (int i = -1; i < 2; i += 2)
        {
            if (row + i < board.height && row + i >= 0)
            {
                if (board.allTiles[column, row + i].GetComponent<BackgroundTile>().strawBale)
                {
                    board.allTiles[column, row + i].GetComponent<BackgroundTile>().BoxExplode();
                }
            }      
        }

        Boom();

    }

    public void BoxExplode()
    {
        if (strawBale)
        {
            strawBale = false;
            audioManager.StrawBaleBreak();
            StartCoroutine(board.FadeOut(strawBaleObj));
            taskController.TaskProgress(0);

        }
    }

    public void Boom()
    {
        if (strawBale)
        {
           BoxExplode();

        }else if (wheatFarm) {
            wheatFarm = false;
            audioManager.MarbleBreak();
            StartCoroutine(board.FadeOut(wheatFarmObj));
            taskController.TaskProgress(1);
        }

    }

   
}

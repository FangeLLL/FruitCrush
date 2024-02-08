using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{

   [SerializeField] public ObstacleSpecs obstacleSpecs;
    private Board board;
    private TaskController taskController;
    AudioManager audioManager;

    public int health;

    private int row, column;

    public List<string> takenDamageIDs = new List<string>();


    private void Start()
    {
        board = FindObjectOfType<Board>();
        taskController = FindObjectOfType<TaskController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if (health > 1)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = obstacleSpecs.sprites[health-1];
        }

        if(obstacleSpecs.isMovable)
        {
            row = GetComponent<Fruit>().row;
            column = GetComponent<Fruit>().column;
            StartCoroutine(LoopForMovableObstacle());
        }
    }
    public void TakeDamage(string damageID)
    {

        if (takenDamageIDs.Contains(damageID))
        {
            return;
        }
        else
        {
            takenDamageIDs.Add(damageID);
        }

        if (obstacleSpecs.indestructible)
        {
            if(obstacleSpecs.isCollectible)
            {
                audioManager.SoundController(obstacleSpecs.obstacleHitSound);
                taskController.TaskProgress(obstacleSpecs.taskID);
            }         
        }
        else
        {
            health--;
            audioManager.SoundController(obstacleSpecs.obstacleHitSound);
            if (health <= 0)
            {
                StartCoroutine(board.FadeOut(gameObject));
                taskController.TaskProgress(obstacleSpecs.taskID);
                if(obstacleSpecs.is4TimesBigger)
                {
                    board.AllTilesDetectVisibleOne();
                }
            }
            else
            {
                if (obstacleSpecs.isCollectible)
                {
                    for (int i = 0; i < obstacleSpecs.amountOfCollect[health]; i++)
                    {
                        taskController.TaskProgress(obstacleSpecs.taskID);
                    }

                }
                GetComponentInChildren<SpriteRenderer>().sprite = obstacleSpecs.sprites[health-1];
            }
        }
    }

    IEnumerator LoopForMovableObstacle()
    {
        yield return new WaitForSeconds(0.5f);

        if (obstacleSpecs.isDownward && GetComponent<Fruit>().row == 0 && !GetComponent<Fruit>().isSwiped)
        {
            StartCoroutine(board.FadeOut(gameObject));
            taskController.TaskProgress(obstacleSpecs.taskID);

        }

        if(row != GetComponent<Fruit>().row || column != GetComponent<Fruit>().column)
        {
            board.allTiles[column, row].GetComponent<BackgroundTile>().obstacles[obstacleSpecs.indexOfLayer] = null;
            board.allTiles[column, row].GetComponent<BackgroundTile>().DetectVisibleOne();
           row = GetComponent<Fruit>().row;
            column = GetComponent<Fruit>().column;
            board.allTiles[column, row].GetComponent<BackgroundTile>().obstacles[obstacleSpecs.indexOfLayer] = gameObject;
            board.allTiles[column, row].GetComponent<BackgroundTile>().DetectVisibleOne();

        }

        StartCoroutine(LoopForMovableObstacle());

    }
    /*
    private void OnDestroy()
    {
        if(obstacleSpecs.isMovable)
        {
            board.allTiles[GetComponent<Fruit>().column, GetComponent<Fruit>().row].GetComponent<BackgroundTile>().DetectVisibleOne();
        }
    }
    */
}

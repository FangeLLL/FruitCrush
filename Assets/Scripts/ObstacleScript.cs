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

    public int row, column;

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
                taskController.TaskProgress(obstacleSpecs.taskID, 1);
            }         
        }
        else
        {
            health--;
            audioManager.SoundController(obstacleSpecs.obstacleHitSound);
            if (health <= 0)
            {
                if (obstacleSpecs.isConsecutive)
                {
                    taskController.TaskProgress(obstacleSpecs.taskID, obstacleSpecs.amountOfCollect[health]);

                    health = obstacleSpecs.sprites.Length;
                    GetComponentInChildren<SpriteRenderer>().sprite = obstacleSpecs.sprites[health - 1];
                }
                else
                {
                    StartCoroutine(ObstacleBreak());
                    if (obstacleSpecs.isCollectible)
                    {
                        taskController.TaskProgress(obstacleSpecs.taskID, obstacleSpecs.amountOfCollect[health]);

                    }
                    else
                    {
                        taskController.TaskProgress(obstacleSpecs.taskID, 1);

                    }

                    if (obstacleSpecs.is4TimesBigger)
                    {              
                        StartCoroutine(SpreadWheatFarms());
                    }                  
                }               
            }
            else
            {
                GetComponent<ParticleSystem>().Play();
                if (obstacleSpecs.isCollectible)
                {

                    taskController.TaskProgress(obstacleSpecs.taskID, obstacleSpecs.amountOfCollect[health]);
                }

                GetComponentInChildren<SpriteRenderer>().sprite = obstacleSpecs.sprites[health - 1];

            }
        }
    }

    public IEnumerator SpreadWheatFarms()
    {
        if (obstacleSpecs.spreadWheatfarm)
        {
            int taskAddAmount = 0;
            // Creating 4x4 wheatfarm area
            for (int i = column - 1; i < column + 3; i++)
            {
                for (int j = row - 1; j < row + 3; j++)
                {
                    if (i >= 0 && i < board.width && j >= 0 && j < board.height && board.allTiles[i, j] && !board.allTiles[i, j].GetComponent<BackgroundTile>().obstacles[1])
                    {
                        board.allTiles[i, j].GetComponent<BackgroundTile>().obstacles[1] = Instantiate(board.obstaclePrefabs[4], board.allTiles[i, j].transform.position, Quaternion.identity);
                        taskAddAmount++;
                    }
                }
            }
            // add task wheatfarm
            taskController.TaskIncrese(1, taskAddAmount);
        }
        yield return new WaitForSeconds(0.1f);
        board.AllTilesDetectVisibleOne();

    }

    /// <summary>
    /// Object will be fade away slowly before destroye.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public IEnumerator ObstacleBreak()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.2f);
        if (obstacleSpecs.isMovable)
        {
            /*
            GetComponent<Fruit>().outsideOfBoard = true;
            GetComponent<Fruit>().enabled = false;
            board.allFruits[column, row] = null;
            */
            Destroy(gameObject);
        } else
        {
            board.allTiles[column, row].GetComponent<BackgroundTile>().obstacles[obstacleSpecs.indexOfLayer] = null;
            board.allTiles[column, row].GetComponent<BackgroundTile>().DetectVisibleOne();
        }     
        yield return new WaitForSeconds(3.8f);

        Destroy(gameObject);

    }

   
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{


    /*
    public int health;
    public int indexOfPlace;
    public bool boxObstacle=false;
    public bool indestructible = false;
        public int id;

    public string obstacleHitSound;
    */

   [SerializeField] public ObstacleSpecs obstacleSpecs;
    private Board board;
    private TaskController taskController;
    AudioManager audioManager;

    public int health;

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

        if(obstacleSpecs.isDownward)
        {
            StartCoroutine(CheckForDownward());
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

    IEnumerator CheckForDownward()
    {
        yield return new WaitForSeconds(0.5f);
        if (GetComponent<Fruit>().row==0 && !GetComponent<Fruit>().isSwiped)
        {
            StartCoroutine(board.FadeOut(gameObject));
            board.allTiles[GetComponent<Fruit>().column, GetComponent<Fruit>().row].GetComponent<BackgroundTile>().DetectVisibleOne();
            taskController.TaskProgress(obstacleSpecs.taskID);
        }
        else
        {
            StartCoroutine(CheckForDownward());
        }
       
    }

}

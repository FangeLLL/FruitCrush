using System.Collections;
using System.Collections.Generic;
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
            audioManager.SoundController(obstacleSpecs.obstacleHitSound);
            taskController.TaskProgress(obstacleSpecs.taskID);
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
}

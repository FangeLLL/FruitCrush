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



    private void Start()
    {
        board = FindObjectOfType<Board>();
        taskController = FindObjectOfType<TaskController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void TakeDamage()
    {     
        if(obstacleSpecs.indestructible)
        {
            audioManager.SoundController(obstacleSpecs.obstacleHitSound);
            taskController.TaskProgress(obstacleSpecs.id);
        }
        else
        {
            health--;
            audioManager.SoundController(obstacleSpecs.obstacleHitSound);
            if (health <= 0)
            {
                StartCoroutine(board.FadeOut(gameObject));
                taskController.TaskProgress(obstacleSpecs.id);
            }
            else
            {
                Color color = gameObject.GetComponent<SpriteRenderer>().color;
                color.g += 0.5f;
                gameObject.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }
}

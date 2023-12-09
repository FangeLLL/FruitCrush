using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    public int health;
    // indexOfPlace variable represent, this prefav preffered place of inde value in obstacles variable.
    public int indexOfPlace;
    public bool boxObstacle=false;
    public bool indestructible = false;

    private Board board;
    private TaskController taskController;
    AudioManager audioManager;

    public int id;

    /*
    
    Note: Id - Name of obstacle - Index place of obstacle

    Obstacle Ids:

    0 - Strawbale - 0 
    1 - Wheatfarm - 1
    2 - Strawbale Strong (Has two health) - 0
    3 - Apple Tree - 0
    4 - Box of fruit - 0
     
     */

    private void Start()
    {
        board = FindObjectOfType<Board>();
        taskController = FindObjectOfType<TaskController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void TakeDamage()
    {     
        if(indestructible)
        {
            // This sound code will rearrange.
            switch (id)
            {
                case 4:
                    audioManager.StrawBaleBreak();
                    break;
            }
            // taskController.TaskProgress(taskIndex);
            Debug.Log("Player got apple");
        }
        else
        {
            health--;
            // This sound code will rearrange.
            switch (id)
            {
                case 0:
                    audioManager.StrawBaleBreak();
                    break;
                case 1:
                    audioManager.MarbleBreak();
                    break;
            }
            if (health <= 0)
            {
                StartCoroutine(board.FadeOut(gameObject));
                taskController.TaskProgress(id);
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

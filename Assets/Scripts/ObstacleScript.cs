using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    public int health;
    public int hierarchy;
    public bool boxObstacle=false;
    public int taskIndex;

    private Board board;
    private TaskController taskController;
    AudioManager audioManager;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        taskController = FindObjectOfType<TaskController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void TakeDamage()
    {     
        health--;
        switch (taskIndex)
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
            taskController.TaskProgress(taskIndex);   
        }
    }
}

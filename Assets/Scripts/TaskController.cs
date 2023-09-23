using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    //StrawBale Index = 0
    public int[] taskNumber = { 0, 0, 0, 0, 0};
    public int moveCount;
    public TextMeshProUGUI moveText;
    bool isLevelCompleted = false;

    public void SetMoveCount(int _moveCount)
    {
        moveCount = _moveCount;
        moveText.text = moveCount.ToString();
    }

    public void MovePlayed()
    {
        moveCount--;
        moveText.text = moveCount.ToString();
        
        if (moveCount <= 0)
        {
            Debug.Log("Out of Moves");
            LevelEndCheck();
        }
    }

    public void SetTask(int taskTypeIndex, int _taskNumber)
    {
        taskNumber[taskTypeIndex] = _taskNumber;
    }

    public void TaskProgress(int taskTypeIndex)
    {
        taskNumber[taskTypeIndex]--;
        LevelEndCheck();
    }

    void LevelEndCheck()
    {
        for (int i = 0; i < taskNumber.Length; i++)
        {
            if (taskNumber[i] != 0)
            {
                isLevelCompleted = false;
                break;
            }
        }

        if (isLevelCompleted && moveCount >= 0)
        {
            Debug.Log("Level Completed");
        }
    }
}

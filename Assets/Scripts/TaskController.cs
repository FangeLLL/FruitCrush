using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    //StrawBale Index = 0
    int[] taskNumber = { 0, 0, 0, 0, 0};

    bool isLevelCompleted = false;

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
        foreach (int element in taskNumber)
        {
            if (element != 0)
            {
                isLevelCompleted = false;
                break;
            }
        }

        if (isLevelCompleted)
        {
            Debug.Log("Level Completed");
        }
    }
}

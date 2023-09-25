using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TaskDisplay
{
    public int taskTypeIndex;
    public Image taskImage;
    public TextMeshProUGUI taskText;

}
public class TaskController : MonoBehaviour
{
    //StrawBale Index = 0
    public TaskDisplay[] taskDisplays;
    public Sprite[] taskSprites;

    int moveCount;
    int currentObjectiveIndex = 0;

    public TextMeshProUGUI moveText;
    bool isLevelCompleted;

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
            //LevelEndCheck();
        }
    }

    public void SetTask(int taskTypeIndex, int _taskNumber)
    {
        if (currentObjectiveIndex < taskDisplays.Length)
        {
            TaskDisplay taskDisplay = taskDisplays[currentObjectiveIndex];

            taskDisplay.taskImage.sprite = taskSprites[taskTypeIndex];
            taskDisplay.taskText.text = _taskNumber.ToString();
            taskDisplay.taskTypeIndex = taskTypeIndex;
            taskDisplay.taskImage.gameObject.SetActive(true);

            currentObjectiveIndex++;
            ObjectiveLocationSetter();
        }
    }

    public void TaskProgress(int taskTypeIndex)
    {
        foreach (TaskDisplay taskDisplay in taskDisplays)
        {
            if (taskDisplay.taskTypeIndex == taskTypeIndex)
            {
                int currentTaskNumber = int.Parse(taskDisplay.taskText.text);
                currentTaskNumber--;

                if (currentTaskNumber <= 0)
                {
                    // Optionally, you can do something when the objective is completed, like hiding the UI element.
                    taskDisplay.taskImage.gameObject.SetActive(false);
                    taskDisplay.taskText.gameObject.SetActive(false);
                }
                else
                {
                    taskDisplay.taskText.text = currentTaskNumber.ToString();
                }

                break; // Exit the loop once we've updated the relevant objective.
            }
        }
    }

    /*void LevelEndCheck()
    {
        isLevelCompleted = true;
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
            Debug.Log("Level Completed!");
        }
    }*/

    void ObjectiveLocationSetter()
    {
        if (currentObjectiveIndex == 1)
        {
            taskDisplays[0].taskImage.transform.localPosition = new Vector2(0, 0);
        }

        if (currentObjectiveIndex == 2)
        {
            taskDisplays[0].taskImage.transform.localPosition = new Vector2(-18, 0);
            taskDisplays[1].taskImage.transform.localPosition = new Vector2(16, 0);
        }

        if (currentObjectiveIndex == 3)
        {
            taskDisplays[0].taskImage.transform.localPosition = new Vector2(-18, 11.4f);
            taskDisplays[1].taskImage.transform.localPosition = new Vector2(16, 11.4f);
            taskDisplays[2].taskImage.transform.localPosition = new Vector2(0, -17.7f);
        }

        if (currentObjectiveIndex == 4)
        {
            taskDisplays[0].taskImage.transform.localPosition = new Vector2(-18, 11.4f);
            taskDisplays[1].taskImage.transform.localPosition = new Vector2(16, 11.4f);
            taskDisplays[2].taskImage.transform.localPosition = new Vector2(-18, -17.7f);
            taskDisplays[3].taskImage.transform.localPosition = new Vector2(16, -17.7f);
        }
    }
}

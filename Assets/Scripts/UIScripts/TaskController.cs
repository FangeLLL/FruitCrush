using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TaskDisplay
{
    public int taskTypeIndex;
    public Image taskImage;
    public GameObject checkMark;
    public TextMeshProUGUI taskText;
    public bool isCompleted;
}
public class TaskController : MonoBehaviour
{
    public UIManager uiManager;
    public LiveRegen liveRegen;
    public Board board;
    public RewardController rewardController;

    //StrawBale Index = 0
    public TaskDisplay[] taskDisplays;
    public Sprite[] taskSprites;

    public int moveCount;
    public int currentObjectiveIndex = 0;
    public int[] taskArray_;
    int[] taskIndexArray = new int[31];

    float timer;
    float timer2;

    public bool onetime = true;
    public bool isBoardActive = true;

    bool isLevelCompleted;
    bool moveCountFlag = true;

    public GameObject moveCountText;
    public TextMeshProUGUI moveText;

    private void Update()
    {
        if (board.hintBool && moveCount <= 0 && onetime)
        {
            timer += Time.deltaTime;

            if (timer >= 1.5f)
            {
                OutofMoves();
                onetime = false;
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }

        if (board.hintBool && isLevelCompleted && onetime)
        {
            timer2 += Time.deltaTime;

            if (timer2 >= 1.5f)
            {
                uiManager.GameFinished(true);
                onetime = false;
                timer2 = 0f;
            }
        }
        else
        {
            timer2 = 0f;
        }
    }

    public void SetMoveCount(int _moveCount)
    {
        moveCount = _moveCount;
        moveText.text = moveCount.ToString();
    }

    public void MovePlayed()
    {
        moveCount--;
        moveText.text = moveCount.ToString();
        
        if (moveCount <= 5 && moveCountFlag)
        {
            moveCountText.GetComponent<Animator>().SetTrigger("MoveWarning");
            moveCountFlag = false;
        }

        if (moveCount <= 0 || AreAllObjectivesComplete())
        {
            isBoardActive = false;
        }

        int madeMove = PlayerPrefs.GetInt("MakeMoveTask", 0);

        madeMove++;

        PlayerPrefs.SetInt("MakeMoveTask", madeMove);
        PlayerPrefs.Save();
    }

    public void SetTask(int[] taskArray)
    {
        if (currentObjectiveIndex < taskDisplays.Length && taskArray != null)
        {
            for (int i = 0; i < taskArray.Length; i++)
            {
                if (taskArray[i] != 0)
                {
                    TaskDisplay taskDisplay = taskDisplays[currentObjectiveIndex];

                    taskDisplay.taskImage.sprite = taskSprites[i];
                    taskDisplay.taskText.text = taskArray[i].ToString();
                    taskDisplay.taskTypeIndex = i;
                    //Debug.Log(currentObjectiveIndex);
                    //taskIndexArray[currentObjectiveIndex] = i;
                    taskDisplay.taskImage.gameObject.SetActive(true);
                    //Debug.Log(currentObjectiveIndex);
                    currentObjectiveIndex++;
                    //Debug.Log(currentObjectiveIndex);
                }
            }

            if (taskArray[4] != 0 && taskArray[1] == 0)
            {
                TaskDisplay taskDisplay = taskDisplays[currentObjectiveIndex];
                
                taskDisplay.taskImage.sprite = taskSprites[1];
                taskDisplay.taskText.text = taskArray[1].ToString();
                taskDisplay.taskTypeIndex = 1;
                taskIndexArray[currentObjectiveIndex] = 1;
                taskDisplay.taskImage.gameObject.SetActive(true);

                currentObjectiveIndex++;
            }

            ObjectiveLocationSetter();
        }
    }


    public void TaskProgress(int taskTypeIndex, int taskDecereaseAmount)
    {
        foreach (TaskDisplay taskDisplay in taskDisplays)
        {
            if (taskDisplay.taskTypeIndex == taskTypeIndex && taskDisplay.taskImage.gameObject.activeSelf)
            {
                int currentTaskNumber = int.Parse(taskDisplay.taskText.text);
                currentTaskNumber -= taskDecereaseAmount;

                if (currentTaskNumber <= 0 && taskTypeIndex != 1)
                {
                    taskDisplay.taskText.gameObject.SetActive(false);
                    taskDisplay.checkMark.GetComponent<Animator>().SetTrigger("TaskCompleteTrigger");
                    taskDisplay.isCompleted = true;
                }
                else if (currentTaskNumber <= 0 && taskTypeIndex == 1 && !taskDisplays[Array.IndexOf(taskIndexArray, 1)].isCompleted)
                {
                    taskDisplay.taskText.gameObject.SetActive(false);
                    taskDisplay.checkMark.GetComponent<Animator>().SetTrigger("TaskCompleteTrigger");
                    taskDisplay.isCompleted = true;
                }

                else
                {
                    taskDisplay.taskText.text = currentTaskNumber.ToString();
                }
            }
        }

        if (AreAllObjectivesComplete() && !isLevelCompleted)
        {
            FinishGame();
        }
    }
    
    public void TaskIncrese(int taskTypeIndex, int taskIncreaseAmount)
    {
        foreach (TaskDisplay taskDisplay in taskDisplays)
        {
            if (taskDisplay.taskTypeIndex == taskTypeIndex && taskDisplay.taskImage.gameObject.activeSelf)
            {
                int currentTaskNumber = int.Parse(taskDisplay.taskText.text);
                currentTaskNumber += taskIncreaseAmount;
                taskDisplay.taskText.text = currentTaskNumber.ToString();
                
            }
        }
    }

    private bool AreAllObjectivesComplete()
    {
        for (int i = 0; i < currentObjectiveIndex; i++)
        {
            if (!taskDisplays[i].isCompleted)
            {
                return false;
            }
        }

        if (taskDisplays[1].isCompleted && !taskDisplays[Array.IndexOf(taskIndexArray, 1)].isCompleted)
        {
            return false;
        }

       else
        {
            // All objectives are complete.
            return true;
        }
    
        
    }


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
    
    void OutofMoves()
    {
        if (!isLevelCompleted)
        {
            uiManager.GameFinished(false);
        }
    }

    public void FinishGame()
    {
        isBoardActive = false;
        isLevelCompleted = true;
        liveRegen.LevelComplete();
        rewardController.GiveStarReward(200);

        int bpProgress = PlayerPrefs.GetInt("BattlePassProgression", 0);
        bpProgress += 8;
        PlayerPrefs.SetInt("BattlePassProgression", bpProgress);

        int levelFinishTask = PlayerPrefs.GetInt("LevelFinishTask", 0);

        levelFinishTask++;

        int userLevel = PlayerPrefs.GetInt("level",0);
        userLevel++;
        PlayerPrefs.SetInt("level", userLevel);

        PlayerPrefs.SetInt("LevelFinishTask", levelFinishTask);
        PlayerPrefs.Save();
    }
}

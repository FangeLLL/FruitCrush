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

        if (moveCount <= 0)
        {
            isBoardActive = false;
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
            if (taskDisplay.taskTypeIndex == taskTypeIndex && taskDisplay.taskImage.gameObject.activeSelf)
            {
                int currentTaskNumber = int.Parse(taskDisplay.taskText.text);
                currentTaskNumber--;

                if (currentTaskNumber <= 0)
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

    private bool AreAllObjectivesComplete()
    {
        for (int i = 0; i < currentObjectiveIndex; i++)
        {
            if (!taskDisplays[i].isCompleted)
            {
                return false;
            }
        }

        // All objectives are complete.
        return true;
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
        int userLevel = PlayerPrefs.GetInt("level");
        userLevel++;
        PlayerPrefs.SetInt("level",userLevel);
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DailyTaskManager : MonoBehaviour
{
    public enum DailyMissionType
    {
        CompleteLevels,
        CrushFruits,
        FirstTry,
        MakeMoves,
        CreateSpecialFruits
    }

    [Serializable]
    public struct DailyMission
    {
        public DailyMissionType type;
        public int targetCount;
    }

    public List<DailyMission> missionPool;
    public int missionsPerDay = 3;

    private List<DailyMission> selectedMissions = new List<DailyMission>();

    public Canvas taskMenuCanvas;
    public List<GameObject> taskBoxes;

    public TextMeshProUGUI countdownText;

    [Serializable]
    public class TaskBox
    {
        public TextMeshProUGUI taskText;
        public TextMeshProUGUI taskProgressText;
        public TextMeshProUGUI rewardText;
    }

    private Dictionary<DailyMissionType, string> missionDescriptions = new Dictionary<DailyMissionType, string>
    {
        { DailyMissionType.CompleteLevels, "Complete levels" },
        { DailyMissionType.CrushFruits, "Crush fruits" },
        { DailyMissionType.FirstTry, "Complete levels in your  first try" },
        { DailyMissionType.MakeMoves, "Make moves" },
        { DailyMissionType.CreateSpecialFruits, "Create special fruits" }
    };

    private DateTime nextResetTime; // Time of the next daily reset

    private void SelectRandomMissions()
    {
        selectedMissions.Clear();

        for (int i = 0; i < missionPool.Count; i++)
        {
            int randomIndex = Random.Range(i, missionPool.Count);
            DailyMission temp = missionPool[i];
            missionPool[i] = missionPool[randomIndex];
            missionPool[randomIndex] = temp;
        }

        for (int i = 0; i < missionsPerDay; i++)
        {
            if (i < missionPool.Count)
            {
                selectedMissions.Add(missionPool[i]);
            }
        }

        // Set the next daily reset time to 3 pm
        nextResetTime = DateTime.Today.Add(new TimeSpan(15, 0, 0));

        // Update the text objects in the task menu canvas
        UpdateTaskMenu();
    }

    private void UpdateTaskMenu()
    {
        for (int i = 0; i < taskBoxes.Count && i < selectedMissions.Count; i++)
        {
            TextMeshProUGUI taskText = taskBoxes[i].transform.Find("TaskText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI taskProgressText = taskBoxes[i].transform.Find("TaskProgressText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rewardText = taskBoxes[i].transform.Find("RewardText").GetComponent<TextMeshProUGUI>();

            DailyMissionType missionType = selectedMissions[i].type;

            taskText.text = $"{missionDescriptions[missionType]}";
            taskProgressText.text = $"{selectedMissions[i].targetCount}";
            rewardText.text = "Reward";
        }
    }

    private void UpdateCountdownText()
    {
        // Calculate the time remaining until the next reset
        TimeSpan timeRemaining = nextResetTime - DateTime.Now;

        countdownText.text = $"{timeRemaining.Hours}h {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
    }

    void Start()
    {
        SelectRandomMissions();
    }

    void Update()
    {
        UpdateCountdownText();
    }
}

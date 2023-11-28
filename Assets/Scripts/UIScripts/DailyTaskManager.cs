using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Import Unity UI namespace
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
            taskProgressText.text = $"0/{selectedMissions[i].targetCount}";
            rewardText.text = "Reward";
        }
    }

    void Start()
    {
        SelectRandomMissions();
    }
}

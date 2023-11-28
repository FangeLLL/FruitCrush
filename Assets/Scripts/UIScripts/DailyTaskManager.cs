using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DailyTaskManager : MonoBehaviour
{
    [Serializable]
    public enum DailyMissionType
    {
        CompleteLevels,
        CrushFruits,
        FirstTry,
        MakeMoves,
        CreateSpecialFruits
    }

    [Serializable]
    public class DailyMission
    {
        public DailyMissionType type;
        public int targetCount;
    }

    [Serializable]
    public class DailyMissionsWrapper
    {
        public List<DailyMission> missions;
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

    private DateTime nextResetTime;

    private const string SaveKey = "SelectedMissions";
    private const string LastOpenedDateKey = "LastOpenedDate";

    public bool newDay;

    private void SelectRandomMissions()
    {
        if (!PlayerPrefs.HasKey(SaveKey) || newDay)
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

            SaveSelectedMissions();
        }
        else
        {
            LoadSelectedMissions();
        }

        nextResetTime = DateTime.Today.AddDays(1).Add(new TimeSpan(24, 0, 0));

        UpdateTaskMenu();
    }

    private void SaveSelectedMissions()
    {
        var missionsWrapper = new DailyMissionsWrapper { missions = selectedMissions };

        string missionsJson = JsonUtility.ToJson(missionsWrapper);
        PlayerPrefs.SetString(SaveKey, missionsJson);
        PlayerPrefs.Save();
    }

    private void LoadSelectedMissions()
    {
        string missionsJson = PlayerPrefs.GetString(SaveKey);

        var missionsWrapper = JsonUtility.FromJson<DailyMissionsWrapper>(missionsJson);

        selectedMissions = missionsWrapper.missions;
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

    private void UpdateCountdownText()
    {
        TimeSpan timeRemaining = nextResetTime - DateTime.Now;

        countdownText.text = $"{timeRemaining.Hours}h {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
    }

    void Start()
    {
        SelectRandomMissions();

        string storedLastOpenedDate = PlayerPrefs.GetString(LastOpenedDateKey, "");

        DateTime currentDate = DateTime.Now;
        string currentDateString = currentDate.ToString("yyyy-MM-dd");

        if (storedLastOpenedDate == currentDateString)
        {
            Debug.Log("The game was opened today.");
            newDay = false;
        }
        else
        {
            Debug.Log("The game was opened on a different day or it's the first time.");
            newDay = true;

            PlayerPrefs.SetString(LastOpenedDateKey, currentDateString);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        UpdateCountdownText();
    }
}

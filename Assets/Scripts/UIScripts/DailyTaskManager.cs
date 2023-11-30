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
        public int currentProgress;
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

            PlayerPrefs.SetInt("MakeMoveTask", 0);
            PlayerPrefs.SetInt("TotalCrushTask", 0);
            PlayerPrefs.SetInt("LevelFinishTask", 0);
            PlayerPrefs.SetInt("LevelFinishFirstTryTask", 0);
            PlayerPrefs.SetInt("SpecialFruitTask", 0);

            PlayerPrefs.Save();

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
        int makeMovesTask = PlayerPrefs.GetInt("MakeMoveTask", 0);
        int totalCrushTask = PlayerPrefs.GetInt("TotalCrushTask", 0);
        int levelFinishTask = PlayerPrefs.GetInt("LevelFinishTask", 0);
        int specialFruitTask = PlayerPrefs.GetInt("SpecialFruitTask", 0);

        for (int i = 0; i < taskBoxes.Count && i < selectedMissions.Count; i++)
        {
            TextMeshProUGUI taskText = taskBoxes[i].transform.Find("TaskText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI taskProgressText = taskBoxes[i].transform.Find("TaskProgressText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rewardText = taskBoxes[i].transform.Find("RewardText").GetComponent<TextMeshProUGUI>();

            DailyMissionType missionType = selectedMissions[i].type;

            taskText.text = $"{missionDescriptions[missionType]}";

            if (taskText.text == "Make moves")
            {
                if (selectedMissions[i].targetCount <= makeMovesTask)
                {
                    selectedMissions[i].currentProgress = selectedMissions[i].targetCount;
                }
                else
                {
                    selectedMissions[i].currentProgress = makeMovesTask;
                }
            }
            else if (taskText.text == "Crush fruits")
            {
                if (selectedMissions[i].targetCount <= totalCrushTask)
                {
                    selectedMissions[i].currentProgress = selectedMissions[i].targetCount;
                }
                else
                {
                    selectedMissions[i].currentProgress = totalCrushTask;
                }
            }
            else if (taskText.text == "Complete levels")
            {
                if (selectedMissions[i].targetCount <= levelFinishTask)
                {
                    selectedMissions[i].currentProgress = selectedMissions[i].targetCount;
                }
                else
                {
                    selectedMissions[i].currentProgress = levelFinishTask;
                }
            }
            else if (taskText.text == "Create special fruits")
            {
                if (selectedMissions[i].targetCount <= specialFruitTask)
                {
                    selectedMissions[i].currentProgress = selectedMissions[i].targetCount;
                }
                else
                {
                    selectedMissions[i].currentProgress = specialFruitTask;
                }
            }


            taskProgressText.text = $"{selectedMissions[i].currentProgress}/{selectedMissions[i].targetCount}";
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

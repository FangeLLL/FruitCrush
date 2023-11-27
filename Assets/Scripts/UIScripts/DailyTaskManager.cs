using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyTaskManager : MonoBehaviour
{
    public enum DailyMissionType
    {
        CompleteLevels,
        CrushFruits,
        FirstTry,
        MakeMoves
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

        foreach (var mission in selectedMissions)
        {
            Debug.Log($"Daily Mission: {mission.type} - Target Count: {mission.targetCount}");
        }
    }

    void Start()
    {
        SelectRandomMissions();
    }
}

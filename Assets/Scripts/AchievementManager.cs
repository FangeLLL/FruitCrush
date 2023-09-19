using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Achievement
{
    public string name;
    public int[] steps;
    public int level = 0;
    public int progress = 0;
}

public class AchievementManager : MonoBehaviour
{
    public Achievement[] achievements;

    public GameObject achivementDisplay;
    public TextMeshProUGUI achievementNameText;
    public TextMeshProUGUI achievementProgressText;

    private Queue<Achievement> achievementQueue = new Queue<Achievement>();
    private bool isDisplayingAchievement = false;

    private void Awake()
    {
        LoadAchievementData();
    }

    private void Update()
    {
        if (achievementQueue.Count > 0 && !isDisplayingAchievement)
        {
            Achievement nextAchievement = achievementQueue.Dequeue();
            DisplayAchievement(nextAchievement);
        }
    }

    public void AchievementProgress(string achievementName, int progress)
    {
        Achievement achievement = FindAchievementByName(achievementName);

        if (achievement != null)
        {
            achievement.progress += progress;

            for (int i = achievement.level; i < achievement.steps.Length; i++)
            {
                if (achievement.progress >= achievement.steps[i])
                {
                    achievement.level = i + 1;

                    // Check if the achievement is not already in the queue.
                    if (!achievementQueue.Contains(achievement))
                    {
                        achievementQueue.Enqueue(achievement); // Add to the queue for display.
                    }

                    SaveAchievementData();
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Achievement not found: " + achievementName);
        }
    }


    private void DisplayAchievement(Achievement achievement)
    {
        isDisplayingAchievement = true;
        Debug.Log("Achievement Unlocked: " + achievement.steps[achievement.level - 1] + " " + achievement.name);
        achievementNameText.text = achievement.name;
        achievementProgressText.text = achievement.steps[achievement.level - 1].ToString();
        StartCoroutine(DisplayTiming());
    }

    IEnumerator DisplayTiming()
    {
        achivementDisplay.SetActive(true);
        yield return null;
        achivementDisplay.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter");
        yield return new WaitForSeconds(3);
        achivementDisplay.GetComponent<Animator>().SetTrigger("AchivementDisplayExit");
        yield return new WaitForSeconds(1);
        achivementDisplay.SetActive(false);
        isDisplayingAchievement = false; // Set to false to allow the next achievement to display.
    }

    public void SaveAchievementData()
    {
        foreach (Achievement achievement in achievements)
        {
            PlayerPrefs.SetInt(achievement.name + "_level", achievement.level);
            PlayerPrefs.SetInt(achievement.name + "_progress", achievement.progress);
        }
    }

    private void LoadAchievementData()
    {
        foreach (Achievement achievement in achievements)
        {
            achievement.level = PlayerPrefs.GetInt(achievement.name + "_level", 0);
            achievement.progress = PlayerPrefs.GetInt(achievement.name + "_progress", 0);
        }
    }

    private Achievement FindAchievementByName(string name)
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement.name == name)
            {
                return achievement;
            }
        }

        return null; // Achievement not found.
    }
}

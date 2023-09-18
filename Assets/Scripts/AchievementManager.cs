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
                    DisplayAchievement(achievement.steps[i], achievement.name);
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

    private void DisplayAchievement(int step, string achievementName)
    {
        Debug.Log("Achievement Unlocked: " + step + " " + achievementName);
        achievementNameText.text = achievementName;
        achievementProgressText.text = step.ToString();
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
    }
}

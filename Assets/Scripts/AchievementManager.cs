using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Achievement
{
    public string name;
    public int index;
    public int[] steps;
    public int level = 1;
    public int progress = 0;
}

public class AchievementManager : MonoBehaviour
{
    public Achievement[] achievements;

    public GameObject achivementDisplay;
    public GameObject star;
    public GameObject star2;
    public GameObject progressBarBackFade;
    public GameObject progressBarFill;
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

    public void AchievementProgress(int[] progressArray)
    {
        Achievement totalAchievement = FindAchievementByIndex(8);

        for (int i = 0; i < progressArray.Length; i++)
        {
            Achievement individualAchievement = FindAchievementByIndex(i);

            if (individualAchievement != null)
            {
                int progress = progressArray[i];
                individualAchievement.progress += progress;
                totalAchievement.progress += progress;

                for (int j = individualAchievement.level; j < individualAchievement.steps.Length; j++)
                {
                    if (individualAchievement.progress >= individualAchievement.steps[j])
                    {
                        individualAchievement.level = j + 1;

                        // Check if the achievement is not already in the queue.
                        if (!achievementQueue.Contains(individualAchievement))
                        {
                            achievementQueue.Enqueue(individualAchievement); // Add to the queue for display.
                        }

                        SaveAchievementData();
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = totalAchievement.level; j < totalAchievement.steps.Length; j++)
                {
                    if (totalAchievement.progress >= totalAchievement.steps[j])
                    {
                        totalAchievement.level = j + 1;

                        // Check if the achievement is not already in the queue.
                        if (!achievementQueue.Contains(totalAchievement))
                        {
                            achievementQueue.Enqueue(totalAchievement); // Add to the queue for display.
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
                Debug.LogWarning("Achievement not found with index: " + i);
            }
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
        yield return null;

        achivementDisplay.SetActive(true);

        yield return null;

        achivementDisplay.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 0.5 sec

        yield return new WaitForSeconds(0.1f);

        star.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); //0.5 sec

        yield return new WaitForSeconds(0.5f);

        star2.SetActive(true);

        yield return null;

        star2.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); //0.25 sec

        yield return new WaitForSeconds(0.25f);
        yield return null;

        star2.SetActive(false);
        progressBarFill.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 1 sec

        yield return new WaitForSeconds(1f);

        progressBarBackFade.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 0.25 sec

        yield return new WaitForSeconds(1f);

        star.GetComponent<Animator>().SetTrigger("AchivementDisplayExit"); //0.5 sec

        yield return new WaitForSeconds(0.25f);

        achivementDisplay.GetComponent<Animator>().SetTrigger("AchivementDisplayExit"); //0.25 sec

        yield return new WaitForSeconds(0.25f);
        yield return null;

        progressBarFill.GetComponent<Image>().fillAmount = 0;
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

    private Achievement FindAchievementByIndex(int index)
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement.index == index)
            {
                return achievement;
            }
        }

        return null; // Achievement not found.
    }
}

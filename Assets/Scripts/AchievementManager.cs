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

    public Sprite[] achievementBanners;

    public GameObject achivementDisplay;
    public GameObject star;
    public GameObject star2;
    public GameObject progressBarBackFade;
    public GameObject progressBarFill;
    public GameObject transitionFade;
    public TextMeshProUGUI achievementNameText;
    public TextMeshProUGUI achievementProgressText1;
    public TextMeshProUGUI achievementProgressText2;

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
        int ach1;
        int ach2;
        int achLevel;
        isDisplayingAchievement = true;
        Debug.Log("Achievement Unlocked: " + achievement.steps[achievement.level - 1] + " " + achievement.name);
        achievementNameText.text = achievement.name;
        if (achievement.level == 1)
        {
            ach1 = 0;
            achLevel = 0;
        }
        else
        {
            ach1 = achievement.steps[achievement.level - 2];
        }
        ach2 = achievement.steps[achievement.level - 1];
        achLevel = achievement.level;
        achievementProgressText1.text = ach1.ToString();
        achievementProgressText2.text = ach2.ToString();
        StartCoroutine(DisplayTiming(ach1, ach2, achLevel));
    }

    IEnumerator DisplayTiming(int ach1, int ach2, int achLevel)
    {
        yield return null;

        achivementDisplay.GetComponent<Image>().sprite = achievementBanners[achLevel - 1];
        achivementDisplay.SetActive(true);

        yield return null;

        achivementDisplay.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 0.5 sec

        yield return new WaitForSeconds(0.6f);

        progressBarFill.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 1 sec
        StartCoroutine(CountToNextMilestone(ach1, ach2));

        yield return new WaitForSeconds(1f);

        progressBarBackFade.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 0.25 sec

        yield return new WaitForSeconds(0.1f);

        transitionFade.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter");

        yield return new WaitForSeconds(0.5f);

        achivementDisplay.GetComponent<Image>().sprite = achievementBanners[achLevel]; ///////////will be changed

        yield return new WaitForSeconds(0.1f);

        star.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 0.5 sec

        yield return new WaitForSeconds(0.5f);

        star2.SetActive(true);

        yield return null;

        star2.GetComponent<Animator>().SetTrigger("AchivementDisplayEnter"); // 0.25 sec

        yield return new WaitForSeconds(0.25f);

        star2.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        star.GetComponent<Animator>().SetTrigger("AchivementDisplayExit"); // 0.5 sec

        yield return new WaitForSeconds(0.25f);

        achivementDisplay.GetComponent<Animator>().SetTrigger("AchivementDisplayExit"); // 0.25 sec

        yield return new WaitForSeconds(0.25f);

        progressBarFill.GetComponent<Image>().fillAmount = 0;
        achivementDisplay.SetActive(false);

        yield return null;

        isDisplayingAchievement = false;
    }

    private IEnumerator CountToNextMilestone(int start, int end)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            float t = elapsedTime / 1;
            int newValue = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            achievementProgressText1.text = newValue.ToString();
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        achievementProgressText1.text = end.ToString(); // Ensure the final value is set.
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

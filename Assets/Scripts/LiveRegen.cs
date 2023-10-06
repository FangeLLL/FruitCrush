using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiveRegen : MonoBehaviour
{
    public int lives;
    int timeToRegen;
    int offlineTime;
    int offlineTimeToRegen;
    int nextLiveRemainingTime;

    public TextMeshProUGUI livesText;
    public TextMeshProUGUI livesStatusText;

    private void Start()
    {
        timeToRegen = 1800;
        lives = PlayerPrefs.GetInt("Lives", 5);
        livesText.text = lives.ToString();
        nextLiveRemainingTime = PlayerPrefs.GetInt("NLRT", timeToRegen);

        if (lives == 5)
        {
            nextLiveRemainingTime = timeToRegen;
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            LevelStarted();
            CheckLiveStatus();
        }
        else
        {
            CalculateOfflineTime();
        }

    }

    void CalculateOfflineTime()
    {
        offlineTime = offlineTimeToRegen;
        CheckLiveStatus();
    }

    void CheckLiveStatus()
    {
        if (lives >= 5)
        {
            lives = 5;
            livesText.text = lives.ToString();
            SaveLives();
            livesStatusText.text = "Full";
            offlineTime = 0;
            nextLiveRemainingTime = 0;
        }
        else
        {
            if (offlineTime >= timeToRegen && offlineTime != 0)
            {
                lives++;
                livesText.text = lives.ToString();
                SaveLives();
                offlineTime -= timeToRegen;
                CheckLiveStatus();
            }
            else
            {
                if (offlineTime >= nextLiveRemainingTime && offlineTime != 0)
                {
                    lives++;
                    livesText.text = lives.ToString();
                    SaveLives();
                    offlineTime -= nextLiveRemainingTime;
                    nextLiveRemainingTime = timeToRegen;
                    CheckLiveStatus();
                }
                else
                {
                    nextLiveRemainingTime -= offlineTime;
                    offlineTime = 0;
                    StopCoroutine(StartCountdown());
                    StartCoroutine(StartCountdown());
                }
            }
        }
    }

    IEnumerator StartCountdown()
    {
        while (nextLiveRemainingTime >= 0)
        {
            nextLiveRemainingTime--;
            PlayerPrefs.SetInt("NLRT", nextLiveRemainingTime);

            if (nextLiveRemainingTime >= 0)
            {
                livesStatusText.gameObject.SetActive(true);
                livesStatusText.text = FormatTime(nextLiveRemainingTime);
            }
            else
            {
                lives++;
                livesText.text = lives.ToString();
                SaveLives();
                nextLiveRemainingTime = timeToRegen;
                CheckLiveStatus();
                break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    void SaveLives()
    {
        PlayerPrefs.SetInt("Lives", lives);
    }

    public void LevelComplete()
    {
        lives++;

        if (lives > 5)
        {
            lives = 5;
            livesText.text = "Full";
            PlayerPrefs.SetInt("NLRT", timeToRegen);
        }
        else
        {
            livesText.text = lives.ToString();
        }

        PlayerPrefs.SetInt("Lives", lives);
    }

    public void LevelStarted()
    {
        lives--;
        livesText.text = lives.ToString();

        PlayerPrefs.SetInt("Lives", lives);
    }

    public void LivesRefilled()
    {
        lives = 5;
        StopCoroutine(StartCountdown());
        CheckLiveStatus();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetString("LastLogoutTime", System.DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        else
        {
            string lastLogoutTimeString = PlayerPrefs.GetString("LastLogoutTime", string.Empty);

            if (!string.IsNullOrEmpty(lastLogoutTimeString))
            {
                System.DateTime lastLogoutTime = System.DateTime.Parse(lastLogoutTimeString);
                System.TimeSpan offlineDuration = System.DateTime.Now - lastLogoutTime;

                double time = offlineDuration.TotalSeconds;
                offlineTimeToRegen = (int)time;
            }
        }
    }
}

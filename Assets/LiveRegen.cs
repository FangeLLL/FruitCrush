using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LiveRegen : MonoBehaviour
{
    public int lives;
    public int timeToRegen = 1200;
    public int offlineTime;
    public int nextLiveRemainingTime;

    public TextMeshProUGUI livesText;
    public TextMeshProUGUI livesStatusText;

    private void Start()
    {
        lives = PlayerPrefs.GetInt("Lives", 4);
        livesText.text = lives.ToString();
        nextLiveRemainingTime = PlayerPrefs.GetInt("NLRT", 1200);
        CalculateOfflineTime();
    }

    void CalculateOfflineTime()
    {
        offlineTime = 0;
        //Calculate the time since last time player opened the game.
        CheckLiveStatus();
    }

    void CheckLiveStatus()
    {
        Debug.Log("1");
        if (lives == 5)
        {
            livesText.text = lives.ToString();
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
                    StartCoroutine(StartCountdown());
                }

            }
        }
    }

    void UpdateUI()
    {
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
            StopCoroutine(StartCountdown());
            nextLiveRemainingTime = timeToRegen;
            CheckLiveStatus();
        }
    }

    IEnumerator StartCountdown()
    {
        while (nextLiveRemainingTime >= 0)
        {
            nextLiveRemainingTime--;
            PlayerPrefs.SetInt("NLRT", nextLiveRemainingTime);
            UpdateUI();
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
}

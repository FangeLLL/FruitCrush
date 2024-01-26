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
    public GameObject infiniteHealth;
    public TextMeshProUGUI infiniteHealthStatusText;

    private bool isInfiniteHealthActive = false;  // Initialize as false
    private float remainingInfiniteHealthTime;
    private float infiniteHealthDuration = 30000f;


    private void Start()
    {
        timeToRegen = 1800;
        lives = PlayerPrefs.GetInt("Lives", 5);
        isInfiniteHealthActive = PlayerPrefs.GetInt("InfiniteHealthActive", 1) == 1;

        if (isInfiniteHealthActive)
        {
            StartInfiniteHealthCountdown();
            lives = 5;
        }
        else
        {
            livesText.gameObject.SetActive(true);
            livesStatusText.gameObject.SetActive(true);
            infiniteHealth.SetActive(false);
            infiniteHealthStatusText.gameObject.SetActive(false);
        }

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

    string FormatTime(float seconds)
    {
        if (seconds >= 3600) // 1 hour or more
        {
            int hours = (int)(seconds / 3600);
            return string.Format("{0}h", hours);
        }
        else if (seconds >= 60) // 1 minute or more
        {
            int minutes = (int)(seconds / 60);
            int remainingSeconds = (int)(seconds % 60);
            return string.Format("{0}m {1}s", minutes, remainingSeconds);
        }
        else // less than 1 minute
        {
            int remainingSeconds = (int)seconds;
            return string.Format("{0}s", remainingSeconds);
        }
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

    private void StartInfiniteHealthCountdown()
    {
        livesText.gameObject.SetActive(false);
        livesStatusText.gameObject.SetActive(false);
        infiniteHealth.SetActive(true);
        infiniteHealthStatusText.gameObject.SetActive(true);

        lives = 5;
        livesText.text = lives.ToString();

        PlayerPrefs.SetInt("Lives", lives);

        remainingInfiniteHealthTime = PlayerPrefs.GetFloat("RemainingInfiniteHealthTime", infiniteHealthDuration);
        remainingInfiniteHealthTime -= offlineTimeToRegen;
        if (remainingInfiniteHealthTime < 0)
        {
            remainingInfiniteHealthTime = 0;
        }
        StartCoroutine(StartInfiniteHealthCountdownCoroutine());
    }

    private IEnumerator StartInfiniteHealthCountdownCoroutine()
    {
        while (remainingInfiniteHealthTime >= -1)
        {
            remainingInfiniteHealthTime -= 1;
            PlayerPrefs.SetFloat("RemainingInfiniteHealthTime", remainingInfiniteHealthTime);

            if (remainingInfiniteHealthTime >= 0)
            {
                infiniteHealthStatusText.text = FormatTime(remainingInfiniteHealthTime);
            }
            else
            {
                infiniteHealthStatusText.gameObject.SetActive(false);
                infiniteHealth.SetActive(false);
                isInfiniteHealthActive = false;
                PlayerPrefs.SetInt("InfiniteHealthActive", 0);
                livesText.gameObject.SetActive(true);
                livesStatusText.gameObject.SetActive(true);
                infiniteHealth.SetActive(false);
                infiniteHealthStatusText.gameObject.SetActive(false);
                yield break;
            }

            yield return new WaitForSeconds(1);
        }
    }
}

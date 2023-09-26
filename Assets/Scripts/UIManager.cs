using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public AchievementManager achivementManager;

    public GameObject congratText;
    public GameObject failedText;
    public GameObject finishBackground;
    public GameObject settingsIcon;
    public GameObject settingsIconShadow;
    public GameObject soundIcon;
    public GameObject soundDisableIcon;
    public GameObject musicIcon;
    public GameObject musicDisableIcon;
    public GameObject settingsGray;

    public bool isSoundOn;
    public bool isMusicOn;

    string gameFinishTrigger = "GameFinishTrigger";
    string gameFinishTriggerReverse = "GameFinishTriggerReverse";

    int[] anan1 = { 3, 0 };
    int[] anan2 = { 0, 3 };
    int[] anan3 = { 10, 10 };

    int settingsButtonCounter = 1;

    public float wait1;
    public float wait2;
    public float wait3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameFinished(true);
        }
        
        else if (Input.GetKeyDown(KeyCode.N))
        {
            GameFinished(false);
        }

        else if (Input.GetKeyDown(KeyCode.T))
        {
            achivementManager.AchievementProgress(anan1);
        }

        else if (Input.GetKeyDown(KeyCode.Y))
        {
            achivementManager.AchievementProgress(anan2);
        }

        else if (Input.GetKeyDown(KeyCode.U))
        {
            achivementManager.AchievementProgress(anan3);
        }
    }

    private void Start()
    {
        int soundSetting = PlayerPrefs.GetInt("SoundSetting", 1);
        isSoundOn = soundSetting == 1;

        int musicSetting = PlayerPrefs.GetInt("MusicSetting", 1);
        isMusicOn = musicSetting == 1;

        if (isSoundOn)
        {
            //ACTIVATE SOUND
            soundDisableIcon.SetActive(false);
        }
        else
        {
            //DEACTIVATE SOUND
            soundDisableIcon.SetActive(true);
        }

        if (isMusicOn)
        {
            //ACTIVATE SOUND
            musicDisableIcon.SetActive(false);
        }
        else
        {
            //DEACTIVATE SOUND
            musicDisableIcon.SetActive(true);
        }
    }

    //FUNCTION TO CALL WHEN PLAYER FAILES OR SUCCESS LEVEL
    //FAILING: PLAYER RUNS OUT OF MOVES BEFORE COMPLETING LEVEL MISSION
    //SUCCESS: PLAYER FINISHED ALL MISSIONS
    public void GameFinished(bool status)
    {
        StartCoroutine(GameFinishUI(status));
    }

    IEnumerator GameFinishUI(bool status)
    {
        achivementManager.SaveAchievementData();

        finishBackground.SetActive(true);
        yield return null;
        finishBackground.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        yield return new WaitForSeconds(wait1);

        if (status)
        {
            congratText.SetActive(true);
            yield return null;
            congratText.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }

        else
        {
            failedText.SetActive(true);
            yield return null;
            failedText.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }

        yield return new WaitForSeconds(wait2);

        if (status)
        {
            congratText.GetComponent<Animator>().SetTrigger(gameFinishTriggerReverse);
            yield return new WaitForSeconds(wait3);
            congratText.SetActive(false);
        }

        else
        {
            failedText.GetComponent<Animator>().SetTrigger(gameFinishTriggerReverse);
            yield return new WaitForSeconds(wait3);
            failedText.SetActive(false);
        }
    }

    public void SettingsButton()
    {
        settingsButtonCounter++;

        if (settingsButtonCounter % 2 == 0)
        {
            StartCoroutine(SettingsOn());
        }
        else
        {
            StartCoroutine(SettingsOff());
        }
    }

    IEnumerator SettingsOn()
    {
        yield return null;

        settingsIcon.GetComponent<Button>().interactable = false;
        settingsGray.GetComponent<Animator>().SetTrigger("SettingsOn");
        settingsIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); 
        settingsIconShadow.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.5 sec

        yield return new WaitForSeconds(0.1f);

        soundIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.33 sec

        yield return new WaitForSeconds(0.1f);

        musicIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.33 sec

        yield return new WaitForSeconds(0.3f);

        settingsIcon.GetComponent<Button>().interactable = true;
    }

    IEnumerator SettingsOff()
    {
        yield return null;

        settingsIcon.GetComponent<Button>().interactable = false;
        settingsGray.GetComponent<Animator>().SetTrigger("SettingsOff");
        settingsIcon.GetComponent<Animator>().SetTrigger("SettingsOff");
        settingsIconShadow.GetComponent<Animator>().SetTrigger("SettingsOff");

        yield return new WaitForSeconds(0.1f);

        musicIcon.GetComponent<Animator>().SetTrigger("SettingsOff"); // 0.33 sec

        yield return new WaitForSeconds(0.1f);

        soundIcon.GetComponent<Animator>().SetTrigger("SettingsOff"); // 0.33 sec

        yield return new WaitForSeconds(0.3f);

        settingsIcon.GetComponent<Button>().interactable = true;
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        soundIcon.GetComponent<Animator>().SetTrigger("SettingsToggle");

        if (isSoundOn)
        {
            //ACTIVATE SOUND
            soundDisableIcon.SetActive(false);
        }
        else
        {
            //DEACTIVATE SOUND
            soundDisableIcon.SetActive(true);
        }

        SaveSoundSetting();
    }

    private void SaveSoundSetting()
    {
        PlayerPrefs.SetInt("SoundSetting", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        musicIcon.GetComponent<Animator>().SetTrigger("SettingsToggle");

        if (isMusicOn)
        {
            //ACTIVATE SOUND
            musicDisableIcon.SetActive(false);

        }
        else
        {
            //DEACTIVATE SOUND
            musicDisableIcon.SetActive(true);
        }

        SaveMusicSetting();
    }

    private void SaveMusicSetting()
    {
        PlayerPrefs.SetInt("MusicSetting", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}

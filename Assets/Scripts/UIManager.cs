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
    public GameObject gameFinishBoxTrue;
    public GameObject gameFinishBoxFalse;
    public GameObject gameFinishBoxLevel;
    public GameObject gameFinishBoxStar;
    public GameObject gameFinishBoxMoveCount1;
    public GameObject gameFinishBoxMoveCount2;
    public GameObject gameFinishBoxTarget1OG;
    public GameObject gameFinishBoxTarget2;
    public GameObject starBox;

    public bool isSoundOn;
    public bool isMusicOn;

    string gameFinishTrigger = "GameFinishTrigger";
    string gameFinishTriggerReverse = "GameFinishTriggerReverse";

    int settingsButtonCounter = 1;

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

        yield return new WaitForSeconds(1.5f);

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

        yield return new WaitForSeconds(1.5f);

        if (status)
        {
            congratText.GetComponent<Animator>().SetTrigger(gameFinishTriggerReverse);
            yield return new WaitForSeconds(1);
            congratText.SetActive(false);
        }

        else
        {
            failedText.GetComponent<Animator>().SetTrigger(gameFinishTriggerReverse);
            yield return new WaitForSeconds(1);
            failedText.SetActive(false);
        }

        if (status)
        {
            gameFinishBoxTrue.SetActive(true);
            gameFinishBoxTrue.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }
        else
        {
            gameFinishBoxFalse.SetActive(true);
            gameFinishBoxFalse.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

            gameFinishBoxMoveCount1.GetComponent<Animator>().SetTrigger("GameFinishTrigger1");
            gameFinishBoxMoveCount2.GetComponent<Animator>().SetTrigger("GameFinishTrigger2");

            gameFinishBoxTarget2 = Instantiate(gameFinishBoxTarget1OG);
            gameFinishBoxTarget2.transform.SetParent(gameFinishBoxFalse.transform);
            gameFinishBoxTarget2.transform.localPosition = new Vector3(-131.6f, 69.8f, 0);
            gameFinishBoxTarget2.transform.localScale = new Vector3(3.98f, 3.98f, 3.98f);
        }

        starBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        yield return new WaitForSeconds(.5f);

        gameFinishBoxLevel.SetActive(true);

        if (status)
        {
            gameFinishBoxStar.SetActive(true);
        }

        yield return null;

        if (status)
        {
            gameFinishBoxStar.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }
        gameFinishBoxLevel.GetComponent<Animator>().SetTrigger(gameFinishTrigger);


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

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

    public TextMeshProUGUI moveCountText;

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
        settingsIcon.GetComponent<Animator>().SetTrigger("SettingsOn");
        settingsIconShadow.GetComponent<Animator>().SetTrigger("SettingsOn");
        yield return new WaitForSeconds(0.5f);
        settingsIcon.GetComponent<Button>().interactable = true;
    }

    IEnumerator SettingsOff()
    {
        yield return null;
        settingsIcon.GetComponent<Button>().interactable = false;
        settingsIcon.GetComponent<Animator>().SetTrigger("SettingsOff");
        settingsIconShadow.GetComponent<Animator>().SetTrigger("SettingsOff");
        yield return new WaitForSeconds(0.5f);
        settingsIcon.GetComponent<Button>().interactable = true;
    }
}

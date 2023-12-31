using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Sounds
{
    public AchievementManager achivementManager;
    public TaskController taskController;
    public ResourceController resourceController;
    public LiveRegen liveRegen;
    public LevelController levelController;
    public ShopController shopController;
    public AudioManager audioManager;

    public GameObject congratText;
    public GameObject failedText;
    public GameObject finishBackground1;
    public GameObject finishBackground2;
    public GameObject settingsIcon;
    public GameObject settingsIconShadow;
    public GameObject soundIcon;
    public GameObject soundDisableIcon;
    public GameObject musicIcon;
    public GameObject musicDisableIcon;
    public GameObject exitGameIcon;
    public GameObject settingsGray;
    public GameObject gameFinishBoxTrue;
    public GameObject gameFinishBoxFalse;
    public GameObject gameFinishBoxLevel;
    public GameObject gameFinishBoxStar;
    public GameObject starGlow;
    public GameObject gameFinishBoxMoveCount1;
    public GameObject gameFinishBoxMoveCount2;
    public GameObject gameFinishBoxTarget1OG;
    public GameObject gameFinishBoxTarget2;
    public GameObject gameFinishBoxMoveBox;
    public GameObject gameFinishBoxPowerUpsBox;
    public GameObject starBox;
    public GameObject livesBox;
    public GameObject quitButton;
    public GameObject quitButton2;
    public GameObject continueButton;
    public GameObject continueWithButton;
    public GameObject retryButton;
    public GameObject shopBackground;
    public GameObject shopTopUI;
    public GameObject shopCloseButton;
    public GameObject outOfLivesBox;
    public GameObject outOfLivesBoxQuitButton;
    public GameObject refillButton;
    public GameObject quitLevelBox;
    public GameObject quitLevelBoxCloseButton;
    public GameObject quitLevelButton;

    public TextMeshProUGUI movePriceText;
    public TextMeshProUGUI refillPriceText;
    public TextMeshProUGUI levelText;

    public bool isSoundOn;
    public bool isMusicOn;
    bool onetimeBool = true;

    string gameFinishTrigger = "GameFinishTrigger";
    string gameFinishTriggerReverse = "GameFinishTriggerReverse";

    int settingsButtonCounter = 1;
    int plusMovePrice = 500;
    int refillPrice = 1000;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            taskController.FinishGame();
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
            UnMuteSoundEffects();
        }
        else
        {
            //DEACTIVATE SOUND
            soundDisableIcon.SetActive(true);
            MuteSoundEffects();
        }

        if (isMusicOn)
        {
            //ACTIVATE SOUND
            musicDisableIcon.SetActive(false);
            UnMuteMusics();
        }
        else
        {
            //DEACTIVATE SOUND
            musicDisableIcon.SetActive(true);
            MuteMusics();
        }
    }

    public void GameFinished(bool status)
    {
        int levelCount = PlayerPrefs.GetInt("level") + 1;
        levelText.text = "Level " + levelCount.ToString();
        StartCoroutine(GameFinishUI(status));
    }

    IEnumerator GameFinishUI(bool status)
    {
        achivementManager.SaveAchievementData();

        if (status)
        {
            finishBackground1.SetActive(true);
            yield return null;
            finishBackground1.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }
        else
        {
            finishBackground2.SetActive(true);
            finishBackground2.GetComponent<Button>().interactable = true;
            yield return null;
            finishBackground2.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }
        
            onetimeBool = true;

        yield return new WaitForSeconds(1.5f);

        if (status)
        {
            levelController.LevelPassed();
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

        finishBackground1.GetComponent<Button>().interactable = false;
        finishBackground2.GetComponent<Button>().interactable = false;

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

            if (gameFinishBoxTarget2 != null)
            {
                Destroy(gameFinishBoxTarget2);
            }
            gameFinishBoxTarget2 = Instantiate(gameFinishBoxTarget1OG);
            gameFinishBoxTarget2.transform.SetParent(gameFinishBoxFalse.transform);
            gameFinishBoxTarget2.transform.localPosition = new Vector3(-131.6f, 69.8f, 0);
            gameFinishBoxTarget2.transform.localScale = new Vector3(3.98f, 3.98f, 3.98f);

            quitButton.GetComponent<Button>().interactable = true;

            starBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }


        yield return new WaitForSeconds(.5f);

        gameFinishBoxLevel.SetActive(true);

        if (status)
        {
            gameFinishBoxStar.SetActive(true);
        }

        yield return null;

        gameFinishBoxLevel.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        if (status)
        {
            gameFinishBoxStar.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

            yield return new WaitForSeconds(1);

            starGlow.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        }
    }

    public void SettingsButton()
    {
        audioManager.MenuClick();
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

        taskController.isBoardActive = false;
        settingsIcon.GetComponent<Button>().interactable = false;
        settingsGray.SetActive(true);
        settingsGray.GetComponent<Animator>().SetTrigger("SettingsOn");
        settingsIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); 
        settingsIconShadow.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.5 sec

        yield return new WaitForSeconds(0.1f);

        soundIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.33 sec

        yield return new WaitForSeconds(0.1f);

        musicIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.33 sec

        yield return new WaitForSeconds(0.1f);

        exitGameIcon.GetComponent<Animator>().SetTrigger("SettingsOn"); // 0.33 sec

        yield return new WaitForSeconds(0.3f);

        settingsIcon.GetComponent<Button>().interactable = true;
    }

    IEnumerator SettingsOff()
    {
        yield return null;

        taskController.isBoardActive = true;
        settingsIcon.GetComponent<Button>().interactable = false;
        if (!quitLevelBox.activeSelf) settingsGray.GetComponent<Animator>().SetTrigger("SettingsOff");
        settingsIcon.GetComponent<Animator>().SetTrigger("SettingsOff");
        settingsIconShadow.GetComponent<Animator>().SetTrigger("SettingsOff");

        yield return new WaitForSeconds(0.1f);

        exitGameIcon.GetComponent<Animator>().SetTrigger("SettingsOff"); // 0.33 sec

        yield return new WaitForSeconds(0.1f);

        musicIcon.GetComponent<Animator>().SetTrigger("SettingsOff"); // 0.33 sec

        yield return new WaitForSeconds(0.1f);

        soundIcon.GetComponent<Animator>().SetTrigger("SettingsOff"); // 0.33 sec

        yield return new WaitForSeconds(0.14f);

        if (!quitLevelBox.activeSelf) settingsGray.SetActive(false);

        yield return new WaitForSeconds(0.16f);

        settingsIcon.GetComponent<Button>().interactable = true;
    }

    public void ToggleSound()
    {
        audioManager.MenuClick();
        isSoundOn = !isSoundOn;
        soundIcon.GetComponent<Animator>().SetTrigger("SettingsToggle");

        if (isSoundOn)
        {
            //ACTIVATE SOUND
            soundDisableIcon.SetActive(false);
            UnMuteSoundEffects();

        }
        else
        {
            //DEACTIVATE SOUND
            soundDisableIcon.SetActive(true);
            MuteSoundEffects();
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
        audioManager.MenuClick();
        isMusicOn = !isMusicOn;
        musicIcon.GetComponent<Animator>().SetTrigger("SettingsToggle");

        if (isMusicOn)
        {
            //ACTIVATE SOUND
            musicDisableIcon.SetActive(false);
            UnMuteMusics();

        }
        else
        {
            //DEACTIVATE SOUND
            musicDisableIcon.SetActive(true);
            MuteMusics();
        }

        SaveMusicSetting();
    }

    private void SaveMusicSetting()
    {
        PlayerPrefs.SetInt("MusicSetting", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlusMovesBought()
    {
        audioManager.MenuClick();
        if (ResourceController.star > plusMovePrice)
        {
            taskController.onetime = true;
            resourceController.StarSpent(plusMovePrice);
            plusMovePrice += 1000;
            StartCoroutine(PlusMoveBoughtEnum());
        }
        
        else
        {
            BuyStarsButtonTapped();
        }
    }

    IEnumerator PlusMoveBoughtEnum()
    {
        continueWithButton.GetComponent<Animator>().SetTrigger("Tapped");

        yield return new WaitForSeconds(0.1f);

        taskController.moveCount = 5;
        taskController.moveText.text = taskController.moveCount.ToString();
        taskController.isBoardActive = true;

        yield return null;

        gameFinishBoxFalse.GetComponent<Animator>().SetTrigger("GameRestartTrigger");
        finishBackground2.GetComponent<Animator>().SetTrigger("GameRestartTrigger");
        starBox.GetComponent<Animator>().SetTrigger("GameRestartTrigger");
        gameFinishBoxLevel.SetActive(false);
        gameFinishBoxLevel.transform.localPosition = new Vector3(0, 200, 0);

        yield return new WaitForSeconds(0.35f);

        finishBackground2.SetActive(false);
        movePriceText.text = plusMovePrice.ToString();
        gameFinishBoxFalse.SetActive(false);
    }

    public void QuitButton1Tapped()
    {
        audioManager.MenuClickReturn();
        StartCoroutine(QuitButton1TappedEnum());

        for (int i = 1; i <= taskController.currentObjectiveIndex; i++)
        {
            string objectiveName = "Objective" + i.ToString();
            Transform objec = GameObject.Find("TargetsBox(Clone)").transform.Find("TargetsDisplay");
            Transform objective = objec.Find(objectiveName);

            if (objective != null)
            {
                GameObject checkMark = objective.Find("CheckMark").gameObject;
                GameObject text = objective.Find("ObjectiveTEXT").gameObject;

                if (checkMark.transform.localScale == new Vector3(0,0,0))
                {
                    text.SetActive(false);
                    GameObject crossMark = objective.Find("CrossMark").gameObject;
                    crossMark.GetComponent<Animator>().SetTrigger("TaskCompleteTrigger");
                }
            }
        }
    }

    IEnumerator QuitButton1TappedEnum()
    {
        quitButton.GetComponent<Animator>().SetTrigger("Tapped");
        quitButton.GetComponent<Button>().interactable = false;

        yield return null;

        livesBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        gameFinishBoxMoveBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        retryButton.SetActive(true);
        yield return null;
        continueWithButton.SetActive(false);

        yield return new WaitForSeconds(0.17f);

        quitButton.SetActive(false);
        quitButton2.SetActive(true);

        yield return new WaitForSeconds(0.13f);

        gameFinishBoxPowerUpsBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
    }

    public void RetryButtonTapped()
    {
        audioManager.MenuClick();
        StartCoroutine(RetryButtonTappedEnum());
    }

    IEnumerator RetryButtonTappedEnum()
    {
        if (liveRegen.lives <= 0)
        {
            outOfLivesBox.SetActive(true);
            outOfLivesBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        }
        else
        {
            yield return new WaitForSeconds(.1f);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    public void QuitButton2Tapped()
    {
        audioManager.MenuClickReturn();
        StartCoroutine(QuitButton2TappedEnum());
    }

    IEnumerator QuitButton2TappedEnum()
    {
        quitButton2.GetComponent<Animator>().SetTrigger("Tapped");
        quitButton2.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(0.15f);

        SceneManager.LoadScene("MainMenu");
    }

    public void ContinueButton()
    {
        audioManager.MenuClick();
        StartCoroutine(ContinueButtonEnum());
    }

    IEnumerator ContinueButtonEnum()
    {
        continueButton.GetComponent<Animator>().SetTrigger("Tapped");
        continueButton.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(0.15f);

        SceneManager.LoadScene("MainMenu");
    }

    public void ShopCloseButtonTapped()
    {
        audioManager.MenuClickReturn();
        shopBackground.SetActive(false);
        shopTopUI.GetComponent<Animator>().SetTrigger("ShopClose");
        shopCloseButton.GetComponent<Animator>().SetTrigger("Tapped");

        for (int i = 0; i < shopController.shopItems.Length; i++)
        {
            shopController.shopItems[i].item.transform.localPosition += new Vector3(500, 0, 0);
        }
    }

    public void BuyStarsButtonTapped()
    {
        audioManager.MenuClick();
        if (!shopBackground.activeSelf)
        {
            StartCoroutine(BuyStarsButtonTappedEnum());
        }
    }

    IEnumerator BuyStarsButtonTappedEnum()
    {
        yield return null;

        resourceController.starShopText.text = resourceController.starText.text;
        shopBackground.SetActive(true);
        shopTopUI.GetComponent<Animator>().SetTrigger("ShopOpen");

        for (int i = 0; i < shopController.shopItems.Length; i++)
        {
            shopController.shopItems[i].item.GetComponent<Animator>().SetTrigger("ShopOpen");
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void OutOfLivesBoxQuitButtonTapped()
    {
        audioManager.MenuClickReturn();
        StartCoroutine(OutOfLivesBoxQuitButtonTappedEnum());
    }

    IEnumerator OutOfLivesBoxQuitButtonTappedEnum()
    {
        outOfLivesBoxQuitButton.GetComponent<Animator>().SetTrigger("Tapped");

        yield return null;

        outOfLivesBox.GetComponent<Animator>().SetTrigger("GameRestartTrigger");

        yield return new WaitForSeconds(0.5f);

        outOfLivesBox.SetActive(false);
    }

    public void RefillButtonTapped()
    {
        audioManager.MenuClick();
        if (ResourceController.star >= refillPrice)
        {
            liveRegen.LivesRefilled();
            resourceController.StarSpent(refillPrice);
            StartCoroutine(RefillButtonTappedEnum());
        }
        else
        {
            BuyStarsButtonTapped();
        }
    }

    IEnumerator RefillButtonTappedEnum()
    {
        refillButton.GetComponent<Animator>().SetTrigger("Tapped");

        yield return null;

        outOfLivesBox.GetComponent<Animator>().SetTrigger("GameRestartTrigger");

        yield return new WaitForSeconds(0.5f);

        outOfLivesBox.SetActive(false);
    }

    public void GameFinishSkipped1()
    {
        if (onetimeBool)
        {
            StopAllCoroutines();
            StartCoroutine(GameFinish1Skipped());
            onetimeBool = false;
            congratText.SetActive(false);
        }
    }

    IEnumerator GameFinish1Skipped()
    {
        gameFinishBoxTrue.SetActive(true);
        gameFinishBoxTrue.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        starBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        yield return new WaitForSeconds(.5f);

        gameFinishBoxLevel.SetActive(true);
        gameFinishBoxStar.SetActive(true);
        
        yield return null;

        gameFinishBoxLevel.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
        gameFinishBoxStar.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        yield return new WaitForSeconds(1);

        starGlow.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
    }

    public void GameFinishSkipped2()
    {
        if (onetimeBool)
        {
            StopAllCoroutines();
            StartCoroutine(GameFinish2Skipped());
            onetimeBool = false;
            failedText.SetActive(false);
            failedText.GetComponent<TextMeshProUGUI>().fontSize = 0;
        }
    }

    IEnumerator GameFinish2Skipped()
    {
        gameFinishBoxFalse.SetActive(true);
        gameFinishBoxFalse.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        gameFinishBoxMoveCount1.GetComponent<Animator>().SetTrigger("GameFinishTrigger1");
        gameFinishBoxMoveCount2.GetComponent<Animator>().SetTrigger("GameFinishTrigger2");

        if (gameFinishBoxTarget2 != null)
        {
            Destroy(gameFinishBoxTarget2);
        }
        gameFinishBoxTarget2 = Instantiate(gameFinishBoxTarget1OG);
        gameFinishBoxTarget2.transform.SetParent(gameFinishBoxFalse.transform);
        gameFinishBoxTarget2.transform.localPosition = new Vector3(-131.6f, 69.8f, 0);
        gameFinishBoxTarget2.transform.localScale = new Vector3(3.98f, 3.98f, 3.98f);

        quitButton.GetComponent<Button>().interactable = true;
        starBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);

        yield return new WaitForSeconds(.5f);

        gameFinishBoxLevel.SetActive(true);

        yield return null;

        gameFinishBoxLevel.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
    }

    public void ExitGameButtonTapped()
    {
        audioManager.MenuClick();
        SettingsButton();
        settingsGray.GetComponent<Button>().interactable = false;
        StartCoroutine(ExitGameButtonTappedEnum());
    }

    IEnumerator ExitGameButtonTappedEnum()
    {
        quitLevelBox.SetActive(true);
        exitGameIcon.GetComponent<Animator>().SetTrigger("SettingsToggle");

        yield return null;

        quitLevelBox.GetComponent<Animator>().SetTrigger(gameFinishTrigger);
    }

    public void ExitGameBoxCloseButtonTapped()
    {
        audioManager.MenuClickReturn();
        StartCoroutine(ExitGameBoxCloseButtonTappedEnum());
    }

    IEnumerator ExitGameBoxCloseButtonTappedEnum()
    {
        quitLevelBoxCloseButton.GetComponent<Animator>().SetTrigger("Tapped");
        settingsGray.GetComponent<Animator>().SetTrigger("SettingsOff");
        quitLevelBox.GetComponent<Animator>().SetTrigger("GameRestartTrigger");

        yield return new WaitForSeconds(0.5f);

        quitLevelBox.SetActive(false);
        settingsGray.SetActive(false);
        settingsGray.GetComponent<Button>().interactable = true;
    }

    public void ExitGameBoxQuitButtonTapped()
    {
        audioManager.MenuClick();
        StartCoroutine(ExitGameBoxQuitButtonTappedEnum());
    }

    IEnumerator ExitGameBoxQuitButtonTappedEnum()
    {
        quitLevelButton.GetComponent<Animator>().SetTrigger("Tapped");
        quitLevelButton.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(0.15f);

        SceneManager.LoadScene("MainMenu");
    }
}

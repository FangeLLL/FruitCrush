using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : Sounds
{
    public LiveRegen liveRegen;
    public LevelController levelController;
    public ResourceController resourceController;
    public ShopController shopController;
    public DailyTaskManager dailyTaskManager;

    public TextMeshProUGUI starText;
    public TextMeshProUGUI starShopText;
    public TextMeshProUGUI levelBoxText;

    public GameObject playButton;
    public GameObject starBox;
    public GameObject livesBox;
    public GameObject settingsIcon;
    public GameObject tasksIcon;
    public GameObject grayBack;
    public GameObject shopBackground;
    public GameObject shopTopUI;
    public GameObject shopCloseButton;
    public GameObject outOfLivesBox;
    public GameObject outOfLivesBoxQuitButton;
    public GameObject refillButton;
    public GameObject playBox;
    public GameObject playBoxQuitButton;
    public GameObject playBoxPlayButton;
    public GameObject levelBox;
    public GameObject MenuBackground;
    public GameObject settingsMenu;
    public GameObject settingsCloseButton;
    public GameObject tasksMenu;
    public GameObject tasksCloseButton;
    public GameObject musicToggleBlock;
    public GameObject soundToggleBlock;
    public GameObject hintToggleBlock;

    int refillPrice = 1000;

    public bool isSoundOn;
    public bool isMusicOn;
    public bool isHintOn;

    private AudioSource sound;
    private AudioSource music;

    private void Start()
    {
        StartCoroutine(ActivateUICoroutine());

        int soundSetting = PlayerPrefs.GetInt("SoundSetting", 1);
        isSoundOn = soundSetting == 1;

        int musicSetting = PlayerPrefs.GetInt("MusicSetting", 1);
        isMusicOn = musicSetting == 1;

        int hintSetting = PlayerPrefs.GetInt("HintSetting", 1);
        isHintOn = hintSetting == 1;

        sound = GameObject.Find("AudioManager").GetComponent<AudioSource>();

        if (isSoundOn)
        {
            //ACTIVATE SOUND
            soundToggleBlock.transform.localPosition = new Vector3(116, 0, 0);
            UnMuteSoundEffects();
        }
        else
        {
            //DEACTIVATE SOUND
            soundToggleBlock.transform.localPosition = new Vector3(-116, 0, 0);
            MuteSoundEffects();
        }

        if (isMusicOn)
        {
            //ACTIVATE MUSIC
            musicToggleBlock.transform.localPosition = new Vector3(116, 0, 0);
            UnMuteMusics();
        }
        else
        {
            //DEACTIVATE MUSIC
            musicToggleBlock.transform.localPosition = new Vector3(-116, 0, 0);
            MuteMusics();
        }

        if (isHintOn)
        {
            //ACTIVATE HINT
            hintToggleBlock.GetComponent<Animator>().SetTrigger("SettingOn");
        }
        else
        {
            //DEACTIVATE HINT
            hintToggleBlock.GetComponent<Animator>().SetTrigger("SettingOff");
        }
    }

    IEnumerator ActivateUICoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        starBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
        livesBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
        settingsIcon.GetComponent<Animator>().SetTrigger("GameFinishTrigger");

        yield return new WaitForSeconds(0.25f);

        tasksIcon.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            //ACTIVATE MUSIC
            musicToggleBlock.GetComponent<Animator>().SetTrigger("SettingOn");
            UnMuteMusics();
        }
        else
        {
            //DEACTIVATE MUSIC
            musicToggleBlock.GetComponent<Animator>().SetTrigger("SettingOff");
            MuteMusics();
        }

        SaveMusicSetting();
    }

    private void SaveMusicSetting()
    {
        PlayerPrefs.SetInt("MusicSetting", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        if (isSoundOn)
        {
            //ACTIVATE SOUND
            soundToggleBlock.GetComponent<Animator>().SetTrigger("SettingOn");
            UnMuteSoundEffects();
        }
        else
        {
            //DEACTIVATE SOUND
            soundToggleBlock.GetComponent<Animator>().SetTrigger("SettingOff");
            MuteSoundEffects();
        }

        SaveSoundSetting();
    }

    private void SaveSoundSetting()
    {
        PlayerPrefs.SetInt("SoundSetting", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleHint()
    {
        isHintOn = !isHintOn;

        if (isHintOn)
        {
            //ACTIVATE HINT
            hintToggleBlock.GetComponent<Animator>().SetTrigger("SettingOn");
        }
        else
        {
            //DEACTIVATE HINT
            hintToggleBlock.GetComponent<Animator>().SetTrigger("SettingOff");
        }

        SaveHintSetting();
    }

    private void SaveHintSetting()
    {
        PlayerPrefs.SetInt("HintSetting", isHintOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayButtonTapped()
    {
        int levelcount = PlayerPrefs.GetInt("level", 0) + 1;
        grayBack.SetActive(true);
        levelBoxText.text = "Level " + levelcount.ToString();
        StartCoroutine(PlayButtonTappedEnum());
    }

    IEnumerator PlayButtonTappedEnum()
    {
        playButton.GetComponent<Animator>().SetTrigger("Tapped");
        playBox.SetActive(true);
        playBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");

        yield return new WaitForSeconds(.5f);

        levelBox.SetActive(true);
        levelBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
    }

    public void PlayBoxQuitButtonTapped()
    {
        StartCoroutine(PlayBoxQuitButtonTappedEnum());
    }

    IEnumerator PlayBoxQuitButtonTappedEnum()
    {
        grayBack.SetActive(false);
        playBoxQuitButton.GetComponent<Animator>().SetTrigger("Tapped");
        playBox.GetComponent<Animator>().SetTrigger("GameRestartTrigger");
        levelBox.SetActive(false);
        levelBox.transform.localPosition = new Vector3(0, 200, 0);

        yield return new WaitForSeconds(0.35f);

        playBox.SetActive(false);
    }

    public void PlayBoxPlayButtonTapped()
    {
        StartCoroutine(PlayBoxPlayButtonTappedEnum());
    }

    IEnumerator PlayBoxPlayButtonTappedEnum()
    {
        playButton.GetComponent<Animator>().SetTrigger("Tapped");

        if (liveRegen.lives <= 0)
        {
            outOfLivesBox.SetActive(true);
            outOfLivesBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");

        }
        else
        {
            yield return new WaitForSeconds(0.1f);

            SceneManager.LoadScene("Level1");
        }
    }

    public void ShopCloseButtonTapped()
    {
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
        if (!shopBackground.activeSelf)
        {
            StartCoroutine(BuyStarsButtonTappedEnum());
        }
    }

    IEnumerator BuyStarsButtonTappedEnum()
    {
        yield return null;

        starShopText.text = starText.text;
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

    public void SettingsButtonTapped()
    {
        MenuBackground.SetActive(true);
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<Animator>().SetTrigger("MenuOpen");
    }

    public void SettingsCloseButtonTapped()
    {
        MenuBackground.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void TasksButtonTapped()
    {
        MenuBackground.SetActive(true);
        tasksMenu.SetActive(true);
        tasksMenu.GetComponent<Animator>().SetTrigger("MenuOpen");
        dailyTaskManager.TaskIconNotification();
        dailyTaskManager.UpdateTaskMenu();
    }

    public void TasksCloseButtonTapped()
    {
        MenuBackground.SetActive(false);
        tasksMenu.SetActive(false);
        dailyTaskManager.TaskIconNotification();
    }
}

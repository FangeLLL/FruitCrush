using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : Sounds
{
    [SerializeField] private LiveRegen liveRegen;
    public LevelController levelController;
    public ResourceController resourceController;
    public ShopController shopController;
    public DailyTaskManager dailyTaskManager;
    public AudioManager audioManager;

    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI starShopText;
    [SerializeField] private TextMeshProUGUI levelBoxText;

    [SerializeField] private GameObject playButton;

    [SerializeField] private GameObject starBox;
    [SerializeField] private GameObject livesBox;

    [SerializeField] private GameObject settingsIcon;
    [SerializeField] private GameObject tasksIcon;
    [SerializeField] private GameObject battlePassIcon;
    [SerializeField] private GameObject farmIcon;

    [SerializeField] private GameObject grayBack;
    [SerializeField] private GameObject shopBackground;
    [SerializeField] private GameObject shopTopUI;
    [SerializeField] private GameObject shopCloseButton;

    [SerializeField] private GameObject outOfLivesBox;
    [SerializeField] private GameObject outOfLivesBoxQuitButton;
    [SerializeField] private GameObject refillButton;

    [SerializeField] private GameObject playBox;
    [SerializeField] private GameObject playBoxQuitButton;
    [SerializeField] private GameObject playBoxPlayButton;
    [SerializeField] private GameObject levelBox;

    [SerializeField] private GameObject MenuBackground;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsCloseButton;
    [SerializeField] private GameObject tasksMenu;
    [SerializeField] private GameObject tasksCloseButton;
    [SerializeField] private GameObject battlePassMenu;
    [SerializeField] private GameObject battlePassCloseButton;
    [SerializeField] private GameObject farmMenu;
    [SerializeField] private GameObject farmCloseButton;

    [SerializeField] private GameObject musicToggleBlock;
    [SerializeField] private GameObject soundToggleBlock;
    [SerializeField] private GameObject hintToggleBlock;

    int refillPrice = 1000;

    public bool isSoundOn;
    public bool isMusicOn;
    public bool isHintOn;
    [SerializeField] private bool isHapticSupported;

    private void Start()
    {
        StartCoroutine(ActivateUICoroutine());

        isHapticSupported = SystemInfo.supportsVibration;

        int soundSetting = PlayerPrefs.GetInt("SoundSetting", 1);
        isSoundOn = soundSetting == 1;

        int musicSetting = PlayerPrefs.GetInt("MusicSetting", 1);
        isMusicOn = musicSetting == 1;

        int hintSetting = PlayerPrefs.GetInt("HintSetting", 1);
        isHintOn = hintSetting == 1;

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
        farmIcon.GetComponent<Animator>().SetTrigger("GameFinishTriggerReverse");

        yield return new WaitForSeconds(0.25f);

        tasksIcon.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
        battlePassIcon.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
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
        audioManager.MenuClick();
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
        audioManager.MenuClick();
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
        audioManager.MenuClick();
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

        audioManager.MenuClick();
    }

    IEnumerator PlayButtonTappedEnum()
    {
        //playButton.GetComponent<Animator>().SetTrigger("Tapped");
        playBox.SetActive(true);
        playBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");

        yield return new WaitForSeconds(.5f);

        levelBox.SetActive(true);
        levelBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
    }

    public void PlayBoxQuitButtonTapped()
    {
        StartCoroutine(PlayBoxQuitButtonTappedEnum());
        audioManager.MenuClickReturn();
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
        audioManager.MenuClick();
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
        audioManager.MenuClickReturn();
    }

    public void BuyStarsButtonTapped()
    {
        if (!shopBackground.activeSelf)
        {
            StartCoroutine(BuyStarsButtonTappedEnum());
        }
        audioManager.MenuClick();
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
        audioManager.MenuClickReturn();
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
        audioManager.MenuClick();
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
        audioManager.MenuClick();
    }

    public void SettingsCloseButtonTapped()
    {
        MenuBackground.SetActive(false);
        settingsMenu.SetActive(false);
        audioManager.MenuClickReturn();
    }

    public void TasksButtonTapped()
    {
        MenuBackground.SetActive(true);
        tasksMenu.SetActive(true);
        tasksMenu.GetComponent<Animator>().SetTrigger("MenuOpen");
        dailyTaskManager.TaskIconNotification();
        dailyTaskManager.UpdateTaskMenu();
        audioManager.MenuClick();
    }

    public void TasksCloseButtonTapped()
    {
        MenuBackground.SetActive(false);
        tasksMenu.SetActive(false);
        dailyTaskManager.TaskIconNotification();
        audioManager.MenuClickReturn();
    }

    public void BattlePassButtonTapped()
    {
        MenuBackground.SetActive(true);
        battlePassMenu.SetActive(true);
        battlePassMenu.GetComponent<Animator>().SetTrigger("MenuOpen");
        audioManager.MenuClick();
    }

    public void BattlePassCloseButtonTapped()
    {
        MenuBackground.SetActive(false);
        battlePassMenu.SetActive(false);
        //notification ayarlamaca
        audioManager.MenuClickReturn();
    }

    public void FarmButtonTapped()
    {
        Debug.Log("Farm");
    }
}

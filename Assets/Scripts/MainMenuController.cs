using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public LiveRegen liveRegen;
    public LevelController levelController;

    public TextMeshProUGUI starText;
    public TextMeshProUGUI starShopText;
    public TextMeshProUGUI levelBoxText;

    public GameObject playButton;
    public GameObject starBox;
    public GameObject livesBox;
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

    public int star;
    int refillPrice = 1000;

    private void Start()
    {
        GetSetValues();
        Invoke("ActivateUI", 0.25f);
    }

    private void GetSetValues()
    {
        star = PlayerPrefs.GetInt("Star", 99999);
        starText.text = star.ToString();
    }
    public void StarSpent(int spentValue)
    {
        star -= spentValue;
        starText.text = star.ToString();

        PlayerPrefs.SetInt("Star", star);
    }

    private void ActivateUI()
    {
        starBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
        livesBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
    }

    public void PlayButtonTapped()
    {
        levelBoxText.text = "Level " + levelController.currentLevel.ToString();
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
    }

    public void BuyStarsButtonTapped()
    {
        if (!shopBackground.activeSelf)
        {
            starShopText.text = starText.text;
            shopBackground.SetActive(true);
            shopTopUI.GetComponent<Animator>().SetTrigger("ShopOpen");
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
        if (star >= refillPrice)
        {
            liveRegen.LivesRefilled();
            StarSpent(refillPrice);
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
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject playButton;

    public GameObject starBox;
    public GameObject livesBox;
    public TextMeshProUGUI starText;

    public int star;

    private void Start()
    {
        GetSetValues();
        //ActivateUI();
        Invoke("ActivateUI", 0.25f);
    }

    private void GetSetValues()
    {
        star = PlayerPrefs.GetInt("Star", 1000);
        starText.text = star.ToString();
    }

    private void ActivateUI()
    {
        starBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
        livesBox.GetComponent<Animator>().SetTrigger("GameFinishTrigger");
    }

    public void PlayButtonTapped()
    {
        StartCoroutine(PlayButtonTappedEnum());
    }

    IEnumerator PlayButtonTappedEnum()
    {
        playButton.GetComponent<Animator>().SetTrigger("Tapped");
        playButton.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("Level1");
    }
}

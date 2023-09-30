using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public TextMeshProUGUI starText;
    public TextMeshProUGUI livesText;

    public static int star;
    public static int lives;

    private void Awake()
    {
        star = PlayerPrefs.GetInt("Star", 1000);
        lives = PlayerPrefs.GetInt("Lives", 5);
    }

    private void Start()
    {
        lives--;
        livesText.text = lives.ToString();
        starText.text = star.ToString();
    }

    public void StarSpent(int spentValue)
    {
        star -= spentValue;
        starText.text = star.ToString();

        PlayerPrefs.SetInt("Star", star);
    }

    public void GetHealtBack(int value)
    {
        lives += value;
        livesText.text = lives.ToString();

        PlayerPrefs.SetInt("Lives", lives);
    }

    public void RetryOptionTrigger()
    {
        lives--;
        livesText.text = lives.ToString();

        PlayerPrefs.SetInt("Lives", lives);
    }
}

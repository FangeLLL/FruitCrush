using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public TextMeshProUGUI starText;
    public TextMeshProUGUI livesText;

    public static int star = 3000;
    public static int lives = 5;

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
    }

    public void GetHealtBack(int value)
    {
        lives += value;
        livesText.text = lives.ToString();
    }

    public void RetryOptionTrigger()
    {
        lives--;
        livesText.text = lives.ToString();
    }
}

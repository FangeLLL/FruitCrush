using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public PowerUpController powerUpController;

    public TextMeshProUGUI starText;
    public TextMeshProUGUI starShopText;

    public static int star;

    private void Awake()
    {
        star = PlayerPrefs.GetInt("Star", 50000);
    }

    private void Start()
    {
        starText.text = star.ToString();
        starShopText.text = star.ToString();
    }

    public void StarSpent(int spentValue)
    {
        star -= spentValue;
        starText.text = star.ToString();
        starShopText.text = star.ToString();

        PlayerPrefs.SetInt("Star", star);
        PlayerPrefs.Save();
    }

    public void StarReward(int rewardValue)
    {
        star += rewardValue;
        starText.text = star.ToString();
        starShopText.text = star.ToString();

        PlayerPrefs.SetInt("Star", star);
        PlayerPrefs.Save();
    }

    public void PowerUp1Rewards(int rewardValue)
    {
        powerUpController.powerUps[0].amount += rewardValue;
        powerUpController.PowerUpUIUpdate();
    }

    public void PowerUp2Rewards(int rewardValue)
    {
        powerUpController.powerUps[1].amount += rewardValue;
        powerUpController.PowerUpUIUpdate();
    }
}

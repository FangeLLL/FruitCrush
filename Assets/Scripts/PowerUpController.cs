using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PowerUps
{
    public int amount;
    public GameObject powerUp;
    public GameObject countBoxDeactive;
    public GameObject countBoxActivated;
    public TextMeshProUGUI count;
    public bool isActivated;
}

public class PowerUpController : MonoBehaviour
{
    public PowerUps[] powerUps;

    public Sprite activeSprite;
    public Sprite deactiveSprite;

    private string amountSaveKeyPrefix = "PowerUpAmount_";
    private string statusSaveKeyPrefix = "PowerUpStatus_";

    void Start()
    {
        for (int i = 0; i < powerUps.Length; i++)
        {
            string amountSaveKey = amountSaveKeyPrefix + i.ToString();
            string statusSaveKey = statusSaveKeyPrefix + i.ToString();

            int savedAmount = PlayerPrefs.GetInt(amountSaveKey, powerUps[i].amount);
            bool savedStatus = PlayerPrefs.GetInt(statusSaveKey, powerUps[i].isActivated ? 1 : 0) == 1;

            powerUps[i].amount = savedAmount;
            powerUps[i].isActivated = savedStatus;
            powerUps[i].count.text = savedAmount.ToString();

            if (savedStatus)
            {
                powerUps[i].powerUp.GetComponent<Image>().sprite = activeSprite;
                powerUps[i].countBoxDeactive.SetActive(false);
                powerUps[i].count.gameObject.SetActive(false);
                powerUps[i].countBoxActivated.SetActive(true);
                powerUps[i].amount--;
                powerUps[i].count.text = powerUps[i].amount.ToString();
            }
            else
            {
                powerUps[i].powerUp.GetComponent<Image>().sprite = deactiveSprite;
                powerUps[i].countBoxDeactive.SetActive(true);
                powerUps[i].count.gameObject.SetActive(true);
                powerUps[i].countBoxActivated.SetActive(false);
            }
        }
    }

    public void PowerUpToggle(int index)
    {
        if (index >= 0 && index < powerUps.Length)
        {
            PowerUps selectedPowerUp = powerUps[index];

            if (!selectedPowerUp.isActivated)
            {
                selectedPowerUp.powerUp.GetComponent<Animator>().SetTrigger("Tapped");
                selectedPowerUp.powerUp.GetComponent<Image>().sprite = activeSprite;
                selectedPowerUp.isActivated = true;
                selectedPowerUp.countBoxDeactive.SetActive(false);
                selectedPowerUp.count.gameObject.SetActive(false);
                selectedPowerUp.countBoxActivated.SetActive(true);
                selectedPowerUp.amount--;

                selectedPowerUp.count.text = selectedPowerUp.amount.ToString();
            }
            else
            {
                selectedPowerUp.powerUp.GetComponent<Animator>().SetTrigger("Tapped");
                selectedPowerUp.powerUp.GetComponent<Image>().sprite = deactiveSprite;
                selectedPowerUp.isActivated = false;
                selectedPowerUp.countBoxDeactive.SetActive(true);
                selectedPowerUp.count.gameObject.SetActive(true);
                selectedPowerUp.countBoxActivated.SetActive(false);
                selectedPowerUp.amount++;

                selectedPowerUp.count.text = selectedPowerUp.amount.ToString();
            }

            string amountSaveKey = amountSaveKeyPrefix + index.ToString();
            string statusSaveKey = statusSaveKeyPrefix + index.ToString();

            PlayerPrefs.SetInt(amountSaveKey, selectedPowerUp.amount);
            PlayerPrefs.SetInt(statusSaveKey, selectedPowerUp.isActivated ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool GetPowerUpStatus(int index)
    {
        if (index >= 0 && index < powerUps.Length)
        {
            return powerUps[index].isActivated;
        }
        else
        {
            return false;
        }
    }
}

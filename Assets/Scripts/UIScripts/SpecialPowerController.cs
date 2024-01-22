using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpecialPower
{
    public int amount;
    public GameObject powerUp;
    public GameObject buySpecialPowerUps;
    public GameObject countSpecialPowerUpBoxDeactive;
    public TextMeshProUGUI count;
    public bool isActivated;
}
public class SpecialPowerController : PowerUpController
{
    public Board board;

    public SpecialPower[] specialPowerUps;

    private string amountSSaveKeyPrefix = "SpecialPowerUpAmount_";

    void Start()
    {
        SpecialPowerUpUIUpdate();            
    }

    public void SpecialPowerUpUIUpdate()
    {
        for (int i = 0; i < specialPowerUps.Length; i++)
        {
            string amountSaveKey = amountSSaveKeyPrefix + i.ToString();

            int savedAmount = PlayerPrefs.GetInt(amountSaveKey, specialPowerUps[i].amount);

            specialPowerUps[i].amount = savedAmount;
            specialPowerUps[i].count.text = savedAmount.ToString();

            if (specialPowerUps[i].amount > 0)
            {
                specialPowerUps[i].buySpecialPowerUps.SetActive(false);
            }
            else
            {
                specialPowerUps[i].countSpecialPowerUpBoxDeactive.SetActive(false);
                specialPowerUps[i].count.gameObject.SetActive(false);
                specialPowerUps[i].isActivated = false;
                specialPowerUps[i].buySpecialPowerUps.SetActive(true);
            }
        }
    }

    public void SpecialPowerUpSelected(int index)
    {
        if (index >= 0 && index < specialPowerUps.Length)
        {
            SpecialPower selectedSpecialPowerUp = specialPowerUps[index];

            //Animation???

            if (selectedSpecialPowerUp.amount > 0 || selectedSpecialPowerUp.isActivated)
            {
                if (!selectedSpecialPowerUp.isActivated)
                {
                    board.SelectedSpecialPower(index + 1);
                    selectedSpecialPowerUp.isActivated = true;
                    selectedSpecialPowerUp.countSpecialPowerUpBoxDeactive.SetActive(false);
                    selectedSpecialPowerUp.count.gameObject.SetActive(false);

                    for (int i = 0; i < specialPowerUps.Length; i++)
                    {
                        if (i != index)
                        {
                            specialPowerUps[i].powerUp.GetComponent<Button>().interactable = false;
                        }
                    }
                }
                else
                {
                    board.DisableSpecialPowers();
                    selectedSpecialPowerUp.isActivated = false;
                    selectedSpecialPowerUp.countSpecialPowerUpBoxDeactive.SetActive(true);
                    selectedSpecialPowerUp.count.gameObject.SetActive(true);

                    for (int i = 0; i < specialPowerUps.Length; i++)
                    {
                        if (i != index)
                        {
                            specialPowerUps[i].powerUp.GetComponent<Button>().interactable = true;
                        }
                    }
                }
            }

            else
            {
                BuyPowerUpButtonTapped();
            }

            string amountSSaveKey = amountSSaveKeyPrefix + index.ToString();
            PlayerPrefs.SetInt(amountSSaveKey, selectedSpecialPowerUp.amount);
            PlayerPrefs.Save();
        }
    }

    public void SpecialPowerUpUsed(int index)
    {
        SpecialPower selectedSpecialPowerUp = specialPowerUps[index];
        selectedSpecialPowerUp.amount--;
        selectedSpecialPowerUp.count.text = selectedSpecialPowerUp.amount.ToString();
        selectedSpecialPowerUp.isActivated = false;

        if (selectedSpecialPowerUp.amount > 0)
        {
            selectedSpecialPowerUp.countSpecialPowerUpBoxDeactive.SetActive(true);
            selectedSpecialPowerUp.count.gameObject.SetActive(true);
            selectedSpecialPowerUp.buySpecialPowerUps.SetActive(false);
        }
        else
        {
            selectedSpecialPowerUp.countSpecialPowerUpBoxDeactive.SetActive(false);
            selectedSpecialPowerUp.count.gameObject.SetActive(false);
            selectedSpecialPowerUp.buySpecialPowerUps.SetActive(true);
        }

        for (int i = 0; i < specialPowerUps.Length; i++)
        {
            if (i != index)
            {
                specialPowerUps[i].powerUp.GetComponent<Button>().interactable = true;
            }
        }

        string amountSSaveKey = amountSSaveKeyPrefix + index.ToString();
        PlayerPrefs.SetInt(amountSSaveKey, selectedSpecialPowerUp.amount);
        PlayerPrefs.Save();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class FreeStages
{
    public int[] freeReward;

    public bool freeRewardUnlocked;
    public bool freeRewardTaken;

    public GameObject freeRewardTick;

    public GameObject freeRewardButton;

    public GameObject stage;

    public Slider progressSlider;
}

[System.Serializable]
public class PremiumStages
{
    public int[] premiumReward;

    public bool premiumRewardUnlocked;
    public bool premiumRewardAvailable;
    public bool premiumRewardTaken;

    public GameObject premiumRewardTick;
    public GameObject premiumRewardLock;

    public GameObject premiumRewardButton;

    public GameObject stage;
}

public class BattlePassManager : MonoBehaviour
{
    [SerializeField] private RewardController rewardController;

    public bool isPremiumActive;
    private bool onetime = true;

    public int totalProgression;

    public FreeStages[] freeStages;
    public PremiumStages[] premiumStages;

    public int[] stageRequirements = { 1, 3, 3, 3, 5, 5, 5, 7, 7, 7, 70};

    public ScrollRect scrollRect;
    public GameObject notif;
    public GameObject infoText;
    public GameObject info;

    private void Start()
    {
        for (int i = 0; i < freeStages.Length; i++)
        {
            freeStages[i].freeRewardButton = freeStages[i].stage.transform.GetChild(0).gameObject;
            freeStages[i].freeRewardTick = freeStages[i].stage.transform.GetChild(2).gameObject;
            freeStages[i].progressSlider = freeStages[i].stage.transform.GetChild(5).GetChild(0).gameObject.GetComponent<Slider>();
            freeStages[i].progressSlider.maxValue = stageRequirements[i];
            premiumStages[i].premiumRewardButton = premiumStages[i].stage.transform.GetChild(1).gameObject;
            premiumStages[i].premiumRewardTick = premiumStages[i].stage.transform.GetChild(3).gameObject;
            premiumStages[i].premiumRewardLock = premiumStages[i].stage.transform.GetChild(4).gameObject;

            freeStages[i].stage.transform.GetChild(5).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
        }
            
        totalProgression = PlayerPrefs.GetInt("BattlePassProgression", 0);
        int bpProgress = totalProgression;

        for (int i = 0; i < freeStages.Length; i++)
        {
            bpProgress -= stageRequirements[i];
            if (bpProgress >= 0)
            {
                freeStages[i].freeRewardUnlocked = true;
                premiumStages[i].premiumRewardUnlocked = true;

                freeStages[i].progressSlider.value = freeStages[i].progressSlider.maxValue;
            }
            else
            {
                freeStages[i].freeRewardUnlocked = false;
                premiumStages[i].premiumRewardUnlocked = false;

                if (onetime)
                {
                    scrollRect.verticalNormalizedPosition = 1 - (1 / freeStages.Length) * i;
                    freeStages[i].progressSlider.value = bpProgress + stageRequirements[i];
                    onetime = false;
                }
            }
        }

        if (isPremiumActive)
        {
            for (int i = 0; i < premiumStages.Length; i++)
            {
                premiumStages[i].premiumRewardAvailable = true;
                premiumStages[i].premiumRewardLock.SetActive(false);
            }
        }

        for (int i = 0; i < freeStages.Length; i++)
        {
            if (PlayerPrefs.GetInt("FreeRewardTaken_" + i, 0) == 1)
            {
                freeStages[i].freeRewardTaken = true;
                freeStages[i].freeRewardTick.SetActive(true);
                freeStages[i].freeRewardTick.transform.localScale = new Vector3(0.112f, 0.112f, 0.112f);
            }

            if (PlayerPrefs.GetInt("PremiumRewardTaken_" + i, 0) == 1)
            {
                premiumStages[i].premiumRewardTaken = true;
                premiumStages[i].premiumRewardTick.SetActive(true);
                premiumStages[i].premiumRewardTick.transform.localScale = new Vector3(0.112f, 0.112f, 0.112f);
            }
        }

        NotificationNumber();
    }

    public void FreeRewardTapped(int stage)
    {
        FreeStages currentStage = freeStages[stage];

        if (!currentStage.freeRewardUnlocked)
        {
            info.transform.SetParent(currentStage.stage.transform, false);
            info.transform.position = currentStage.freeRewardButton.transform.position;
            infoText.SetActive(true);
            infoText.GetComponent<Animator>().SetTrigger("InfoDisplay");
            infoText.GetComponentInChildren<TextMeshProUGUI>().text = "Reward  is not unlocked  yet.";
        }
        else if (currentStage.freeRewardUnlocked && !currentStage.freeRewardTaken)
        {
            rewardController.DisplayRewards(currentStage.freeReward);
            currentStage.freeRewardTick.SetActive(true);
            currentStage.freeRewardTaken = true;
            currentStage.freeRewardTick.GetComponent<Animator>().SetTrigger("Tapped");
            PlayerPrefs.SetInt("FreeRewardTaken_" + stage, 1);
            NotificationNumber();
        }
        else
        {
            info.transform.SetParent(currentStage.stage.transform, false);
            info.transform.position = currentStage.freeRewardButton.transform.position;
            info.transform.SetSiblingIndex(6);
            infoText.SetActive(true);
            infoText.GetComponent<Animator>().SetTrigger("InfoDisplay");
            infoText.GetComponentInChildren<TextMeshProUGUI>().text = "Reward  is taken";
        }
    }
    
    public void PremiumRewardTapped(int stage)
    {
        PremiumStages currentStage = premiumStages[stage];

        if (!currentStage.premiumRewardUnlocked)
        {
            info.transform.SetParent(currentStage.stage.transform, false);
            info.transform.position = currentStage.premiumRewardButton.transform.position;
            info.transform.SetSiblingIndex(6);
            infoText.SetActive(true);
            infoText.GetComponent<Animator>().SetTrigger("InfoDisplay");
            infoText.GetComponentInChildren<TextMeshProUGUI>().text = "Reward  is not unlocked  yet.";
        }
        else if (!currentStage.premiumRewardAvailable)
        {
            info.transform.SetParent(currentStage.stage.transform, false);
            info.transform.position = currentStage.premiumRewardButton.transform.position;
            info.transform.SetSiblingIndex(6);
            infoText.SetActive(true);
            infoText.GetComponent<Animator>().SetTrigger("InfoDisplay");
            infoText.GetComponentInChildren<TextMeshProUGUI>().text = "Buy Fruit Pass for  this reward !";
        }
        else if (currentStage.premiumRewardUnlocked && !currentStage.premiumRewardTaken && currentStage.premiumRewardAvailable)
        {
            rewardController.DisplayRewards(currentStage.premiumReward);
            currentStage.premiumRewardTick.SetActive(true);
            currentStage.premiumRewardTaken = true;
            currentStage.premiumRewardTick.GetComponent<Animator>().SetTrigger("Tapped");
            PlayerPrefs.SetInt("PremiumRewardTaken_" + stage, 1);
            NotificationNumber();
        }
        else
        {
            info.transform.SetParent(currentStage.stage.transform, false);
            info.transform.position = currentStage.premiumRewardButton.transform.position;
            info.transform.SetSiblingIndex(6);
            infoText.SetActive(true);
            infoText.GetComponent<Animator>().SetTrigger("InfoDisplay");
            infoText.GetComponentInChildren<TextMeshProUGUI>().text = "Reward  is taken";
        }
    }

    public void PremiumBought()
    {
        for (int i = 0; i < premiumStages.Length; i++)
        {
            premiumStages[i].premiumRewardAvailable = true;
            premiumStages[i].premiumRewardLock.SetActive(false);
        }
    }

    public void NotificationNumber()
    {
        int count= 0;

        for (int i = 0; i < freeStages.Length; i++)
        {
            if (!freeStages[i].freeRewardTaken && freeStages[i].freeRewardUnlocked)
            {
                count++;
            }
            if (!premiumStages[i].premiumRewardTaken && premiumStages[i].premiumRewardUnlocked && premiumStages[i].premiumRewardAvailable)
            {
                count++;
            }
        }

        if (count < 1)
        {
            notif.SetActive(false);
        }
        else
        {
            notif.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FreeStages
{
    public int stageNumber;
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
    public int stageNumber;
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

    private void Start()
    {
        for (int i = 0; i < freeStages.Length; i++)
        {
            freeStages[i].freeRewardButton = freeStages[i].stage.transform.GetChild(0).gameObject;
            freeStages[i].freeRewardTick = freeStages[i].stage.transform.GetChild(2).gameObject;
            freeStages[i].progressSlider = freeStages[i].stage.transform.GetChild(4).GetChild(0).gameObject.GetComponent<Slider>();
            premiumStages[i].premiumRewardButton = premiumStages[i].stage.transform.GetChild(1).gameObject;
            premiumStages[i].premiumRewardTick = premiumStages[i].stage.transform.GetChild(3).gameObject;
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
                    freeStages[i].progressSlider.value = bpProgress + stageRequirements[i];
                    onetime = false;
                }
            }
        }

        if (!isPremiumActive)
        {
            for (int i = 0; i < premiumStages.Length; i++)
            {
                premiumStages[i].premiumRewardAvailable = false;
                //Stages[i].premiumRewardLock.SetActive(true);
            }
        }

        for (int i = 0; i < freeStages.Length; i++)
        {
            if (PlayerPrefs.GetInt("FreeRewardTaken_" + i, 0) == 1)
            {
                freeStages[i].freeRewardTaken = true;
                freeStages[i].freeRewardTick.SetActive(true);
            }

            if (PlayerPrefs.GetInt("PremiumRewardTaken_" + i, 0) == 1)
            {
                premiumStages[i].premiumRewardTaken = true;
                freeStages[i].freeRewardTick.SetActive(true);
            }
        }
    }

    public void FreeRewardTapped(int stage)
    {
        FreeStages currentStage = freeStages[stage];

        if (!currentStage.freeRewardUnlocked)
        {
            Debug.Log("Reward(s) are not unlocked yet.");
        }
        else if (currentStage.freeRewardUnlocked && !currentStage.freeRewardTaken)
        {
            rewardController.DisplayRewards(currentStage.freeReward);
            currentStage.freeRewardTick.SetActive(true);
            currentStage.freeRewardTaken = true;
            PlayerPrefs.SetInt("FreeRewardTaken_" + stage, 1);
        }
        else
        {
            Debug.Log("Reward(s) are taken.");
        }
    }
    
    public void PremiumRewardTapped(int stage)
    {
        PremiumStages currentStage = premiumStages[stage];

        if (!currentStage.premiumRewardUnlocked)
        {
            Debug.Log("Reward(s) are not unlocked yet.");
        }
        else if (!currentStage.premiumRewardAvailable)
        {
            Debug.Log("Buy Fruit Pass for this reward!");
        }
        else if (currentStage.premiumRewardUnlocked && !currentStage.premiumRewardTaken && currentStage.premiumRewardAvailable)
        {
            rewardController.DisplayRewards(currentStage.premiumReward);
            currentStage.premiumRewardTick.SetActive(true);
            currentStage.premiumRewardTaken = true;
            PlayerPrefs.SetInt("PremiumRewardTaken_" + stage, 1);
        }
        else
        {
            Debug.Log("Reward(s) are taken.");
        }
    }

    public void PremiumBought()
    {
        for (int i = 0; i < premiumStages.Length; i++)
        {
            premiumStages[i].premiumRewardAvailable = true;
            //premiumStages[i].premiumRewardLock.SetActive(false);
        }
    }

    public int NotificationNumber()
    {
        int count= 0;

        for (int i = 0; i < freeStages.Length; i++)
        {
            if (!freeStages[i].freeRewardTaken && freeStages[i].freeRewardUnlocked)
            {
                count++;
            }
            if (!premiumStages[i].premiumRewardTaken && premiumStages[i].premiumRewardUnlocked)
            {
                count++;
            }
        }
        return count;
    }
}

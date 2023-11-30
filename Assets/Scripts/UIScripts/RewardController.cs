using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Rewards
{
    public GameObject rewardObject;
    public GameObject rewardText;
    public GameObject rewardAmount;
}

public class RewardController : MonoBehaviour
{
    public ResourceController resourceController;
    public DailyTaskManager dailyTaskManager;

    public Rewards[] rewards;

    public GameObject rewardGray;
    public GameObject rewardText;
    public GameObject tapToClaimText;

    bool rewardTrigger;

    private void Start()
    {
        if (rewardTrigger)
        {
            StartCoroutine(RewardTriggerEnum());
        }
    }
    public void GiveDailyReward(int index)
    {
        StartCoroutine(RewardTriggerEnum());
        dailyTaskManager.TaskRewardTaken(index);
    }

    IEnumerator RewardTriggerEnum()
    {
        yield return null;

        rewardGray.SetActive(true);
    }

    public void RewardGrayTap()
    {
        rewardGray.SetActive(false);
    }
    public void GiveStarReward(int starReward)
    {
        resourceController.StarReward(starReward);
        rewardTrigger = true;
    }
}

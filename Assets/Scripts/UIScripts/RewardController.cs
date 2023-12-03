using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class RewardUI
{
    public GameObject rewardSprite;
    public TextMeshProUGUI rewardText;
}

public class RewardController : MonoBehaviour
{
    public ResourceController resourceController;
    public DailyTaskManager dailyTaskManager;

    public RewardUI[] rewardUI;

    public Sprite[] rewardSprites;

    public GameObject rewardGray;
    public GameObject rewardText;
    public GameObject tapToClaimText;

    bool rewardTrigger;

    private void Start()
    {
        if (rewardTrigger)
        {
            int[] a = { 150, 0, 0, 0, 0 };
            DisplayRewards(a);
        }
    }

    public void GiveStarReward(int starReward)
    {
        resourceController.StarReward(starReward);
        rewardTrigger = true;
    }

    public void DisplayRewards(int[] rewards)
    {
        ClearRewards();

        int givenReward = 0;

        for (int i = 0; i < rewards.Length; i++)
        {
            int rewardAmount = rewards[i];

            if (rewardAmount > 0)
            {
                rewardUI[givenReward].rewardSprite.SetActive(true);
                rewardUI[givenReward].rewardText.gameObject.SetActive(true);
                rewardUI[givenReward].rewardSprite.GetComponent<Image>().sprite = rewardSprites[i];
                rewardUI[givenReward].rewardText.text = "x " + rewardAmount.ToString();

                StartCoroutine(DisplayAnimation());

                givenReward++;
            }
        }

        GiveRewards(rewards);
    }

    private void ClearRewards()
    {
        for (int i = 0; i < rewardUI.Length; i++)
        {
            rewardUI[i].rewardSprite.SetActive(false);
            rewardUI[i].rewardText.gameObject.SetActive(false);
        }
    }

    public void CreateRandomReward(int i)
    {
        if (i == 0)
        {
            int[] a = { 150, 0, 1, 0, 0 };
            DisplayRewards(a);
        }
        else if (i == 1)
        {
            int[] a = { 100, 1, 0, 0, 0 };
            DisplayRewards(a);
        }
        else
        {
            int[] a = { 50, 1, 1, 0, 0 };
            DisplayRewards(a);
        }

        dailyTaskManager.TaskRewardTaken(i);
    }

    public void GiveRewards(int[] rewards)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            if (i == 0)
            {
                resourceController.StarReward(rewards[i]);
            }
            if (i == 1)
            {
                resourceController.PowerUp1Rewards(rewards[i]);
            }
            if (i == 2)
            {
                resourceController.PowerUp2Rewards(rewards[i]);
            }
        }
    }
    
    IEnumerator DisplayAnimation()
    {
        yield return null;

        rewardGray.SetActive(true);
        rewardGray.GetComponent<Animator>().SetTrigger("RewardDisplayTrigger");

        rewardText.SetActive(true);
        tapToClaimText.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        rewardText.GetComponent<Animator>().SetTrigger("RewardDisplayTrigger");

        for (int i = 0; i < rewardUI.Length; i++)
        {
            yield return new WaitForSeconds(0.2f);
            rewardUI[i].rewardSprite.GetComponent<Animator>().SetTrigger("RewardDisplayTrigger");
            yield return new WaitForSeconds(0.1f);
        }

        tapToClaimText.GetComponent<Animator>().SetTrigger("RewardDisplayTrigger");
    }

    public void RewardGrayTap()
    {
        StartCoroutine(DisplayAnimationReverse());

        for (int i = 0; i < rewardUI.Length; i++)
        {
            rewardUI[i].rewardSprite.SetActive(false);
            rewardUI[i].rewardText.gameObject.SetActive(false);
            rewardUI[i].rewardSprite.transform.localPosition += new Vector3(340, 0, 0);
        }
    }

    IEnumerator DisplayAnimationReverse()
    {
        yield return null;

        rewardGray.GetComponent<Animator>().SetTrigger("RewardDisplayTriggerReverse");
        rewardText.GetComponent<Animator>().SetTrigger("RewardDisplayTriggerReverse");
        tapToClaimText.GetComponent<Animator>().SetTrigger("RewardDisplayTriggerReverse");

        yield return new WaitForSeconds(0.5f);

        rewardGray.SetActive(false);
        rewardText.SetActive(false);
        tapToClaimText.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardController : MonoBehaviour
{
    public ResourceController resourceController;

    public void GiveStarReward(int starReward)
    {
        resourceController.StarReward(starReward);
    }
}

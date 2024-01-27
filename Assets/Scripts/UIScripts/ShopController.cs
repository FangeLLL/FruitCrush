using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItems
{
    public GameObject item;
    public int rewardedLife;
    public int rewardedStar;
    public int rewardedPowerUp1;
    public int rewardedPowerUp2;
    public int rewardedPowerUp3;
}

public class ShopController : MonoBehaviour
{
    public ShopItems[] shopItems;

    public ResourceController resourceController;
    public LiveRegen liveRegen;

    public void ShopItemBought(int shopItemIndex)
    {
        resourceController.StarReward(shopItems[shopItemIndex].rewardedStar);
        resourceController.PowerUp1Rewards(shopItems[shopItemIndex].rewardedPowerUp1);
        resourceController.PowerUp2Rewards(shopItems[shopItemIndex].rewardedPowerUp2);
        if (shopItems[shopItemIndex].rewardedLife > 0)
        {
            liveRegen.rewardTime = shopItems[shopItemIndex].rewardedLife;
            liveRegen.InfiniteHealthObtained();
        }
        //shopItems[shopItemIndex].item.transform.GetChild(1).GetComponent<Animator>().SetTrigger("Tapped");
    }
}

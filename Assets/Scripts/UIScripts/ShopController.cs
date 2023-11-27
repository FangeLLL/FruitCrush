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

    public void ShopItemBought(int shopItemIndex)
    {
        resourceController.StarReward(shopItems[shopItemIndex].rewardedStar);
        shopItems[shopItemIndex].item.transform.GetChild(1).GetComponent<Animator>().SetTrigger("Tapped");
    }
}

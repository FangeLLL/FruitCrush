using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class SpecialPowerUps
{
    public int amount;
    public GameObject powerUp;
    public GameObject buySpecialPowerUps;
    public TextMeshProUGUI count;
    public bool isActivated;
}
public class SpecialPowerController : MonoBehaviour
{
    public List<SpecialPowerUps> specialPowerUpsList;

    public void SpecialPowerUpSelected(int index)
    {
        if (index >= 0 && index < specialPowerUpsList.Count)
        {
            specialPowerUpsList[index].isActivated = !specialPowerUpsList[index].isActivated;
        }

        if (specialPowerUpsList[index].isActivated)
        {
            //BertuFunction(index + 1);
            Debug.Log("Burito " + index + 1);
        }

        else
        {
            DeselectUI(index);
            Debug.Log("Closed");
        }
    }

    public void SpecialPowerUpUsed(int index)
    {
        specialPowerUpsList[index].amount--;
        DeselectUI(index);
    }

    private void DeselectUI(int index)
    {

    }
}

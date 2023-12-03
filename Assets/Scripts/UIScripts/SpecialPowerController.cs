using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class SpecialPower
{
    public int amount;
    public GameObject powerUp;
    public GameObject buySpecialPowerUps;
    public TextMeshProUGUI count;
    public bool isActivated;
}
public class SpecialPowerController : MonoBehaviour
{
    public Board board;

    public List<SpecialPower> specialPowerUpsList;

    public void SpecialPowerUpSelected(int index)
    {
        if (index >= 0 && index < specialPowerUpsList.Count)
        {
            specialPowerUpsList[index].isActivated = !specialPowerUpsList[index].isActivated;
            board.SelectedSpecialPower(index);
        }

        if (specialPowerUpsList[index].isActivated)
        {
            Debug.Log("Activated");
        }

        else
        {
            DeselectUI(index);
            Debug.Log("Deactivated");
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

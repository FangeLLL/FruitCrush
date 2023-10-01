using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public TextMeshProUGUI starText;

    public static int star;

    private void Awake()
    {
        star = PlayerPrefs.GetInt("Star", 1000);
    }

    private void Start()
    {
        starText.text = star.ToString();
    }

    public void StarSpent(int spentValue)
    {
        star -= spentValue;
        starText.text = star.ToString();

        PlayerPrefs.SetInt("Star", star);
    }
}

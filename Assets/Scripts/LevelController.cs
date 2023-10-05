using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public int currentLevel = 1;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("Level", 1);
    }

    public void LevelPassed()
    {
        currentLevel++;
        PlayerPrefs.SetInt("Level ", currentLevel);
        PlayerPrefs.Save();
    }
}

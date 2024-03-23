using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FarmArea
{
    public GameObject farmObject;
    public Transform farmPlot;
    public float growthTime;
    public bool isEmpty = true;
    public int seedIndex;
}

public class FarmManager : MonoBehaviour
{
    public FarmArea[] farmAreas;
    public Sprite[] farmSprites;

    public MainMenuController controller;

    public int selectedSeedIndex;

    private const string FarmAreaKeyPrefix = "FarmArea_";

    private void Start()
    {
        LoadFarmAreaStates();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && selectedSeedIndex != 0 && controller.isFarmAreaOpen)
        {
            Vector2 raycastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.zero);

            if (hit.collider != null)
            {
                for (int i = 0; i < farmAreas.Length; i++)
                {
                    if (farmAreas[i].farmPlot == hit.collider.transform)
                    {
                        if (farmAreas[i].isEmpty)
                        {
                            PlantSeed(selectedSeedIndex, i);
                        }
                        break;
                    }
                }
            }
        }
    }

    public void PlantSeed(int seedIndex, int farmIndex)
    {
        FarmArea farmArea = farmAreas[farmIndex];

        farmArea.isEmpty = false;
        farmArea.seedIndex = seedIndex;
        farmArea.farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[1];

        PlayerPrefs.SetInt("FarmArea_" + farmIndex + "_IsEmpty", farmArea.isEmpty ? 1 : 0);

        PlayerPrefs.SetInt("FarmArea_" + farmIndex + "_SeedIndex", farmArea.seedIndex);

        int spriteIndex = GetSpriteIndex(farmArea.farmObject.GetComponent<SpriteRenderer>().sprite);
        PlayerPrefs.SetInt("FarmArea_" + farmIndex + "_SpriteIndex", spriteIndex);

        PlayerPrefs.Save();
    }

    public void SelectSeed(int seedIndex)
    {
        selectedSeedIndex = seedIndex;
    }

    private void LoadFarmAreaStates()
    {
        foreach (var farmArea in farmAreas)
        {
            int farmIndex = System.Array.IndexOf(farmAreas, farmArea);

            farmArea.isEmpty = PlayerPrefs.GetInt("FarmArea_" + farmIndex + "_IsEmpty", 1) == 1;

            farmArea.seedIndex = PlayerPrefs.GetInt("FarmArea_" + farmIndex + "_SeedIndex", 0);

            int spriteIndex = PlayerPrefs.GetInt("FarmArea_" + farmIndex + "_SpriteIndex", 0);
            farmArea.farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[spriteIndex];
        }
    }
    private int GetSpriteIndex(Sprite sprite)
    {
        for (int i = 0; i < farmSprites.Length; i++)
        {
            if (farmSprites[i] == sprite)
            {
                return i;
            }
        }
        return -1;
    }
}

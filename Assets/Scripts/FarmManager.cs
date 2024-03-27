using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class FarmArea
{
    public GameObject farmObject;
    public Image farmNotif;
    public float growthTime;
    public bool isEmpty = true;
    public int seedIndex;
    public float countdownTimer = 0f;
}

public class FarmManager : MonoBehaviour
{
    public FarmArea[] farmAreas;
    public Sprite[] farmSprites;

    public MainMenuController controller;

    public int selectedSeedIndex;

    private const string FarmAreaKeyPrefix = "FarmArea_";

    private int offlineSec;

    private void Start()
    {
        string lastLogoutTimeString = PlayerPrefs.GetString("LastLogoutTime", string.Empty);

        if (!string.IsNullOrEmpty(lastLogoutTimeString))
        {
            System.DateTime lastLogoutTime = System.DateTime.Parse(lastLogoutTimeString);
            System.TimeSpan offlineDuration = System.DateTime.Now - lastLogoutTime;

            double time = offlineDuration.TotalSeconds;
            offlineSec = ((int)time);
        }

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
                    if (farmAreas[i].farmObject.transform == hit.collider.transform)
                    {
                        if (farmAreas[i].isEmpty)
                        {
                            PlantSeed(selectedSeedIndex, i);
                            StartGrowthCountdown(i);
                        }
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < farmAreas.Length; i++)
        {
            if (!farmAreas[i].isEmpty)
            {
                farmAreas[i].countdownTimer -= Time.deltaTime;

                if (farmAreas[i].countdownTimer <= 2 * (farmAreas[i].growthTime / 3) && farmAreas[i].countdownTimer > 1 * (farmAreas[i].growthTime / 3))
                {
                    farmAreas[i].farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[2];
                }

                else if (farmAreas[i].countdownTimer <= 1 * (farmAreas[i].growthTime / 3) && farmAreas[i].countdownTimer > 0)
                {
                    farmAreas[i].farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[3];
                }

                else if (farmAreas[i].countdownTimer <= 0)
                {
                    farmAreas[i].farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[4];
                    PlantReady(farmAreas[i]);
                }

                PlayerPrefs.SetFloat(FarmAreaKeyPrefix + i + "_CountdownTimer", farmAreas[i].countdownTimer);
            }
        }
    }

    private void PlantSeed(int seedIndex, int farmIndex)
    {
        FarmArea farmArea = farmAreas[farmIndex];

        farmArea.isEmpty = false;
        farmArea.seedIndex = seedIndex;
        farmArea.farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[1];

        farmArea.growthTime = seedIndex == 1 ? 120f : 15f;

        StartGrowthCountdown(farmIndex);

        SaveFarmAreaState(farmIndex);
    }

    private void StartGrowthCountdown(int farmIndex)
    {
        farmAreas[farmIndex].countdownTimer = farmAreas[farmIndex].growthTime;
    }

    private void PlantReady(FarmArea farmArea)
    {
        Vector3 azyukari = new Vector3(0, 150, 0); 
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(farmArea.farmObject.transform.position);
        farmArea.farmNotif.gameObject.SetActive(true);
        farmArea.farmNotif.rectTransform.position = screenPosition + azyukari;
    }

    public void HarvestPlant(int farmIndex)
    {
        farmAreas[farmIndex].isEmpty = true;
        farmAreas[farmIndex].seedIndex = 0;
        farmAreas[farmIndex].farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[0];
        farmAreas[farmIndex].farmNotif.gameObject.SetActive(false);

        SaveFarmAreaState(System.Array.IndexOf(farmAreas, farmAreas[farmIndex]));
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

            farmArea.isEmpty = PlayerPrefs.GetInt(FarmAreaKeyPrefix + farmIndex + "_IsEmpty", 1) == 1;
            farmArea.seedIndex = PlayerPrefs.GetInt(FarmAreaKeyPrefix + farmIndex + "_SeedIndex", 0);

            if (farmArea.seedIndex == 1)
            {
                farmArea.growthTime = 120;
            }
            else if(farmArea.seedIndex == 2)
            {
                farmArea.growthTime = 15;
            }

            int spriteIndex = PlayerPrefs.GetInt(FarmAreaKeyPrefix + farmIndex + "_SpriteIndex", 0);
            farmArea.farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[spriteIndex];

            farmArea.countdownTimer = PlayerPrefs.GetFloat(FarmAreaKeyPrefix + farmIndex + "_CountdownTimer", 0f) - offlineSec;

            if (farmArea.countdownTimer <= 0f && !farmArea.isEmpty)
            {
                farmArea.farmObject.GetComponent<SpriteRenderer>().sprite = farmSprites[4];
                PlantReady(farmArea);
            }
        }
    }

    private void SaveFarmAreaState(int farmIndex)
    {
        FarmArea farmArea = farmAreas[farmIndex];

        PlayerPrefs.SetInt(FarmAreaKeyPrefix + farmIndex + "_IsEmpty", farmArea.isEmpty ? 1 : 0);

        PlayerPrefs.SetInt(FarmAreaKeyPrefix + farmIndex + "_SeedIndex", farmArea.seedIndex);

        int spriteIndex = GetSpriteIndex(farmArea.farmObject.GetComponent<SpriteRenderer>().sprite);
        PlayerPrefs.SetInt(FarmAreaKeyPrefix + farmIndex + "_SpriteIndex", spriteIndex);

        PlayerPrefs.Save();
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
        return -1; // If sprite is not found in the sprite array
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastLogoutTime", System.DateTime.Now.ToString());
        PlayerPrefs.Save();
    }
    
}

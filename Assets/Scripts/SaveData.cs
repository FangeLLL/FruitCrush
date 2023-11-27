using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

public class SaveData : MonoBehaviour
{
    //public static SaveData Instance { get; private set; }
    [HideInInspector]
    public Grid[] gridData;
    public string saveFilePath;

    private void Start()
    {
        gridData = new Grid[1000];
        LoadFromJson();
        saveFilePath = Application.persistentDataPath + "/GridData.json";
    }

    public void SaveToJson()
    {
        string saveGridData = JsonHelper.ToJson(gridData,true);
        File.WriteAllText(saveFilePath, saveGridData);
        Debug.Log("Save file created at: " + saveFilePath);

        //Debug.Log(filePath);
        //System.IO.File.WriteAllText(filePath, gridData);
        //Debug.Log("Saved");
    }

    public void LoadFromJson()
    {

        if (File.Exists(saveFilePath = Application.persistentDataPath + "/GridData.json"))
        {
            string loadGridData = File.ReadAllText(saveFilePath);

            gridData = JsonHelper.FromJson<Grid>(loadGridData);

            //string filePath = Application.persistentDataPath + "/GridData.json";
            //string inventoryData = System.IO.File.ReadAllText(filePath);

            //grid = JsonUtility.FromJson<Grid>(inventoryData);
            Debug.Log("Loaded");
        }
        else
            Debug.Log("There is no save files to load!");

        
    }
}

[System.Serializable]
public class Grid
{
    public int level;
    public int width;
    public int height;
    public GameObject[] fruits;
    // Json files does not support 2d arrays so these values store all fruits by starting down left corner and finishing top right corner. So, total lenght
    // for these arrays is (width x height) .
    public int[] allFruitsTotal;
    public int[] allTilesTotal;
    public int moveCount;
    public int[] taskElements;
}



using UnityEngine;
using System.Collections.Generic;

public class SaveData : MonoBehaviour
{ 
    public Grid grid = new Grid();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveToJson();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFromJson();
        }
    }

    public void SaveToJson()
    {
        string gridData = JsonUtility.ToJson(grid);
        string filePath = Application.persistentDataPath + "/GridData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, gridData);
        Debug.Log("Saved");
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/GridData.json";
        string inventoryData = System.IO.File.ReadAllText(filePath);

        grid = JsonUtility.FromJson<Grid>(inventoryData);
        Debug.Log("Loaded");
    }
}

    [System.Serializable]
    public class Grid
    {
        public int width;
        public int height;
        public GameObject[] fruits;
        public GameObject strawBalePrefab;
        public GameObject tilePrefab;
}


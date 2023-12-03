using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    static public int width = 5;
    static public int height = 5;
    public int chosenId;

    public int widthChanger = width;
    public int heightChanger = height;

    public float scaleNumber;
    public float scaleFactorFruit;

    IndexLibrary indexLibrary = new IndexLibrary();

    [SerializeField]
    public int[] existFruits;
    public GameObject[] fruitCheckboxes;
    public GameObject[] fruits;
    public GameObject[] obstacles;
    public GameObject tilePrefab;

    public GameObject[,] allFruits;
    public GameObject[,] allTiles;

    public int moveCount;
    public int[] taskElements;

    private SaveData saveData;

    public int level=1;

    private void Start()
    {
        widthChanger = width;
        heightChanger = height;
        RearrangeScaleNumber();
        taskElements = new int[2];
        saveData= GetComponent<SaveData>();
        existFruits = new int[5];

        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
  
        SetUp();
    }

    private void Update()
    {
        // Changing static variables from inspector
        width = widthChanger;
        height = heightChanger;
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveTheBoard(level);
        }
     
    }

    private void SaveTheBoard(int level)
    {
        level--;
        saveData.gridData[level] = new Grid();

        saveData.gridData[level].width = width;
        saveData.gridData[level].height = height;
        saveData.gridData[level].level= level+1;
        saveData.gridData[level].moveCount= moveCount;
        saveData.gridData[level].taskElements= taskElements;

        int[] arrangeFruits = new int[width * height];
        int[] arrangeTiles = new int[width * height];
        saveData.gridData[level].fruits= existFruits;

        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j< width; j++)
            {
                // If fruit does not exist then type will be -1 .
                if (allFruits[i, j])
                {
                    arrangeFruits[(width * i) + j] = allFruits[i, j].GetComponent<Fruit>().fruitType;
                }
                else
                {
                    arrangeFruits[(width * i) + j] = -1;
                }
                arrangeTiles[(width * i) + j] = allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().tileType;
            }
        }

        saveData.gridData[level].allFruitsTotal= arrangeFruits;
        saveData.gridData[level].allTilesTotal= arrangeTiles;
        saveData.SaveToJson();
    }

    private void SetUpWithArray(int[,] arrangedFruits, int[,] arrangedTiles)
    {
        // width = arrangedTiles.GetLength(0);
        // height = arrangedTiles.GetLength(1);

        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        tilePrefab.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);

        foreach (GameObject fruitScale in fruits)
        {
            fruitScale.transform.localScale = new Vector3(scaleFactorFruit, scaleFactorFruit, scaleFactorFruit);
        }

        foreach (GameObject obstacleScale in obstacles)
        {
            obstacleScale.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, j * scaleNumber - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                allTiles[i, j] = backgroundTile;
                if (arrangedTiles[i, j] != 0)
                {
                    bool[] obstacleBools = indexLibrary.GetBoolObstacles(arrangedTiles[i, j]);
                    if (obstacleBools[0])
                    {
                        backgroundTile.GetComponent<BackgroundTile>().strawBale = true;
                        backgroundTile.GetComponent<BackgroundTile>().strawBaleObj = Instantiate(obstacles[0], tempPosition, Quaternion.identity);
                    }

                    if (obstacleBools[1])
                    {
                        backgroundTile.GetComponent<BackgroundTile>().wheatFarm = true;
                        backgroundTile.GetComponent<BackgroundTile>().wheatFarmObj = Instantiate(obstacles[1], tempPosition, Quaternion.identity);
                    }
                }
                // If type of fruit -1 then it means fruit does not exist.
                if (arrangedFruits[i, j] > 0)
                {
                    int fruitToUse = arrangedFruits[i, j];
                    GameObject fruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
                    fruit.transform.parent = this.transform;
                    fruit.name = "( " + i + ", " + j + " )";
                    fruit.GetComponent<Fruit>().column = i;
                    fruit.GetComponent<Fruit>().row = j;
                    fruit.GetComponent<Fruit>().fruitType = fruitToUse;
                    allFruits[i, j] = fruit;
                }


            }
        }
    }

    private void SetUp()
    {
        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        tilePrefab.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);

        foreach (GameObject fruitScale in fruits)
        {
            fruitScale.transform.localScale = new Vector3(scaleFactorFruit, scaleFactorFruit, scaleFactorFruit);
        }

        foreach (GameObject obstacleScale in obstacles)
        {
            obstacleScale.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, j * scaleNumber - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.GetComponent<LevelEditorBackgroundTile>().row = j;
                backgroundTile.GetComponent<LevelEditorBackgroundTile>().column = i;
                backgroundTile.name = "( " + i + ", " + j + " )";
                allTiles[i, j] = backgroundTile;

                int fruitToUse = Random.Range(0, fruits.Length);
                GameObject fruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
                fruit.transform.parent = this.transform;
                fruit.name = "( " + i + ", " + j + " )";
                fruit.GetComponent<Fruit>().column = i;
                fruit.GetComponent<Fruit>().row = j;
                fruit.GetComponent<Fruit>().fruitType = fruitToUse;
                allFruits[i, j] = fruit;
            }
        }
    }

    public void ReplaceObject(int column, int row)
    {
        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        Vector2 tempPosition = new Vector2(column * scaleNumber - xOffset, row * scaleNumber - yOffset);

        // INSTANTIATE A NEW FRUIT AT THE POSITION OF THE DESTROYED FRUIT
        if (chosenId >= 0)
        {
            if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().tileType!=0)
            {
                // Destroy strawbale
                Destroy(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[0]);
                StartCoroutine(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().RearrangeTileType());
            }
            GameObject fruit = Instantiate(fruits[chosenId], tempPosition, Quaternion.identity);
            fruit.transform.parent = this.transform;
            fruit.name = "( " + column + ", " + row + " )";

            // SET THE COLUMN AND ROW OF THE NEW FRUIT
            fruit.GetComponent<Fruit>().column = column;
            fruit.GetComponent<Fruit>().row = row;
            fruit.GetComponent<Fruit>().fruitType = chosenId;
            // ADD THE NEW FRUIT TO THE ALLFRUITS ARRAY
            Destroy(allFruits[column, row]);
            allFruits[column, row] = fruit;
        }
        else
        {
            // OBSTACLE IDS START FROM -1 SO SYSTEM GET NEGATIVE OF IT AND MINUS 1
            int obstacleID = (-chosenId) - 1;

            // IF OBSTACLE ALREADY EXÝST THEN DESTROY THE OBSTACLE BUT IF DOES NOT EXÝST THEN CREATE THE OBSTACLE
            if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[obstacleID])
            {
                Destroy(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[obstacleID]);
            }
            else
            {
               
                GameObject obstacle = Instantiate(obstacles[obstacleID], tempPosition, Quaternion.identity);
                obstacle.transform.parent = this.transform;
                obstacle.name = "( " + column + ", " + row + " )";
                allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[obstacleID] = obstacle;
            }

            // IF OBSTACLE IS STRAWBALE THEN DESTROY FRUIT

            if (chosenId == -1)
            {
                Destroy(allFruits[column, row]);
            }
           StartCoroutine(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().RearrangeTileType());
        }       

    
    }

    public void CheckBox(int id)
    {
        if (existFruits[id]==0)
        {
            fruitCheckboxes[id].GetComponent<Image>().color = new UnityEngine.Color(0, 255, 0, 255);
            existFruits[id] = 1;
        }
        else
        {
            fruitCheckboxes[id].GetComponent<Image>().color = new UnityEngine.Color(255, 0, 0, 255);
            existFruits[id] = 0;
        }
    }

    public void GetObject(int id)
    {
        /*
         Id:
        0 - Banana
        1 - Cherry
        2 - Grape
        3 - Orange
        4 - Avocado

        -1 - Strawbale
        -2 - Wheat Farm
          
          */

        chosenId= id;

    }

    public void RestartScene()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RearrangeScaleNumber()
    {
        int max;

        if (width >= height)
        {
            max = width;
        }
        else
        {
            max = height;
        }

        switch (max)
        {
            case 4:
            case 5:
                scaleNumber = 1.5f;
                break;
            case 6:
                scaleNumber = 1.3f;
                break;
            case 7:
            case 8:
                scaleNumber = 1.2f;
                break;
            case 9:
            case 10:
                scaleNumber = 1f;
                break;
            case 11:
                scaleNumber = 0.9f;
                break;
        }

        scaleFactorFruit = scaleNumber / 1.2f;
    }

}

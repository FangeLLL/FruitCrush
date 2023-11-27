using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public int width;
    public int height;

    // INITIALIZE TO 0
    private int currentFruitIndex = 0;
    //private int currentObstacleIndex = 0;
    public int obstacleType; 

    [SerializeField]
    public GameObject[] fruits;
    public GameObject[] obstacles;
    public GameObject tilePrefab;
    public GameObject strawBalePrefab;

    public GameObject[,] allFruits;
    public GameObject[,] allTiles;
    public GameObject[,] allObstacles;

    public int moveCount;
    public int[] taskElements;

    private SaveData saveData;

    public int level=1;

    private void Start()
    {        
        taskElements= new int[2];
        saveData= GetComponent<SaveData>();

        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
        allObstacles = new GameObject[width, height];
  
        SetUp();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            saveTheBoard(level);
        }
     
    }

    private void saveTheBoard(int level)
    {
        level--;
        saveData.gridData[level] = new Grid();

        saveData.gridData[level].width = width;
        saveData.gridData[level].height = height;
        saveData.gridData[level].fruits= fruits;
        saveData.gridData[level].level= level+1;
        saveData.gridData[level].moveCount= moveCount;
        saveData.gridData[level].taskElements= taskElements;

        int[] arrangeFruits = new int[width * height];
        int[] arrangeTiles = new int[width * height];

        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j< width; j++)
            {
                arrangeFruits[(width * i) + j] = allFruits[i,j].GetComponent<Fruit>().fruitType;
                arrangeTiles[(width * i) + j] = allTiles[i, j].GetComponent<BackgroundTile>().tileType;
            }
        }

        saveData.gridData[level].allFruitsTotal= arrangeFruits;
        saveData.gridData[level].allTilesTotal= arrangeTiles;
        saveData.SaveToJson();
    }

    private void SetUp()
    {
        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i - xOffset, j - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
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

    public void ReplaceDestroyedFruit(int column, int row)
    {
        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;
        Vector2 tempPosition = new Vector2(column - xOffset, row - yOffset);

        // INSTANTIATE A NEW FRUIT AT THE POSITION OF THE DESTROYED FRUIT
        GameObject fruit = Instantiate(fruits[currentFruitIndex], tempPosition, Quaternion.identity);

        // SET THE PARENT AND NAME OF THE NEW FRUIT
        fruit.transform.parent = this.transform;
        fruit.name = "( " + column + ", " + row + " )";

        // SET THE COLUMN AND ROW OF THE NEW FRUIT
        fruit.GetComponent<Fruit>().column = column;
        fruit.GetComponent<Fruit>().row = row;
        fruit.GetComponent<Fruit>().fruitType = currentFruitIndex;

        // ADD THE NEW FRUIT TO THE ALLFRUITS ARRAY
        Destroy(allFruits[column, row]);
        allFruits[column, row] = fruit;

        // INCREMENT THE currentFruitIndex AND WRAP IT AROUND IF IT GOES BEYOND THE ARRAY LENGTH
        currentFruitIndex = (currentFruitIndex + 1) % fruits.Length;
    }

    public void ReplaceObstacle(int column, int row)
    {
        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;
        Vector2 tempPosition = new Vector2(column - xOffset, row - yOffset);

        if (allFruits[column, row])
        {
            Destroy(allFruits[column, row]);

            // Spawn straw bales at the specified position
            GameObject strawBale = Instantiate(strawBalePrefab, tempPosition, Quaternion.identity);

            // Update the allFruits array if necessary
            allFruits[column, row] = null; // You can set it to null since the obstacle is destroyed
        }
        else
        {
            // Handle the case where there was no fruit or obstacle at the specified position
            // You can add code for this scenario if needed
        }
    }

}

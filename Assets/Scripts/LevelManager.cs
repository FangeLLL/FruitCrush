using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
        allObstacles = new GameObject[width, height];
        int[,] arrangeFruits = new int[width, height];
        int[,] arrangeTiles = new int[width, height];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < height; j++)
            {
                arrangeTiles[i, j] = 1;
            }
        }

        SetUpWithArray(arrangeFruits, arrangeTiles);
        //SetUp();
    }

    /*private void SetUp()
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
    }*/

    private void SetUpWithArray(int[,] arrangedFruits, int[,] arrangedTiles)
    {
        width = arrangedTiles.GetLength(0);
        height = arrangedTiles.GetLength(1);

        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i - xOffset, j - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                allTiles[i, j] = backgroundTile;
                if (arrangedTiles[i, j] == 3)
                {
                    backgroundTile.GetComponent<BackgroundTile>().strawBale = true;
                    backgroundTile.GetComponent<BackgroundTile>().strawBaleObj = Instantiate(strawBalePrefab, tempPosition, Quaternion.identity);
                }
                else
                {
                    // int fruitToUse = arrangedFruits[i,j];
                    int fruitToUse = UnityEngine.Random.Range(0, fruits.Length);
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

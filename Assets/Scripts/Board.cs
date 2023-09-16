using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;

    private BackgroundTile[,] allTiles;

    public float xOffset;
    public float yOffset;

    [SerializeField]
    public GameObject[] fruits;

    public GameObject[,] allFruits;

    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allFruits = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
         xOffset = width * 0.5f - 0.5f;
         yOffset = height * 0.5f - 0.5f;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i - xOffset, j - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                int fruitToUse = Random.Range(0, fruits.Length);
                GameObject fruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
                fruit.transform.parent = this.transform;
                fruit.name = "( " + i + ", " + j + " )";
                allFruits[i, j] = fruit;
            }
        }
    }
}

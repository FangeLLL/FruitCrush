using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;

    public GameObject backgroundPrefab;

    public enum fruitType
    {
        normal,
        count,
    };

    private Dictionary<fruitType, GameObject> fruitPrefabDictionary;

    [System.Serializable]
    public struct FruitPrefab
    {
        public fruitType type;
        public GameObject prefab;
    };

    public FruitPrefab[] fruitPrefabs;
    private GameFruit[,] fruits;

    void Start()
    {
        fruitPrefabDictionary = new Dictionary<fruitType, GameObject>();

        for (int i = 0; i < fruitPrefabs.Length; i++)
        {
            if (!fruitPrefabDictionary.ContainsKey(fruitPrefabs[i].type))
            {
                fruitPrefabDictionary.Add(fruitPrefabs[i].type, fruitPrefabs[i].prefab);
            }
        }

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(i, j), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        fruits = new GameFruit[gridX, gridY];
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                GameObject newFruit = Instantiate(fruitPrefabDictionary[fruitType.normal], Vector3.zero, Quaternion.identity);
                newFruit.name = "piece(" + i + "," + j + ")";
                newFruit.transform.parent = transform;
                fruits[i,j] = newFruit.GetComponent<GameFruit>();
                fruits[i, j].Init(i, j, this, fruitType.normal);
                if (fruits[i, j].isMoveable())
                {
                    fruits[i,j].MoveableFruit.Move(i, j);
                } 
            }
        }
    }

    void Update()
    {

    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        // Calculate the new position with the offsets
        float newX = transform.position.x - (gridX - 1) / 2.0f + x;
        float newY = transform.position.y + (gridY - 1) / 2.0f - y;

        return new Vector2(newX, newY);
    }
}

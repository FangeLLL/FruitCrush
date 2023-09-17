using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;

    public float xOffset;
    public float yOffset;


    [SerializeField]
    public GameObject[] fruits;

    public GameObject[,] allFruits;

    void Start()
    {
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
                fruit.GetComponent<Fruit>().column = i;
                fruit.GetComponent<Fruit>().row = j;
                fruit.GetComponent<Fruit>().fruitType = fruitToUse;
                allFruits[i, j] = fruit;
            }
        }
    }
   
    

    public void MoveFruits(float swipeAngle,int column,int row)
    {
        GameObject otherFruit;
        GameObject fruit = allFruits[column, row];
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Vector2 currentFruitLoc= fruit.transform.position;
        Vector2 otherFruitLoc;
        if (swipeAngle > -45 && swipeAngle <= 45 && column < width)
        {
            // RIGHT SWIPE
            otherFruit = allFruits[column + 1, row];
            otherFruitLoc= otherFruit.transform.position;
            otherFruit.GetComponent<Fruit>().column -= 1;
            otherFruit.GetComponent<Fruit>().targetV = currentFruitLoc;
            fruitScript.column++;
            fruitScript.targetV = otherFruitLoc;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < height)
        {
            // UP SWIPE
            otherFruit = allFruits[column, row + 1];
            otherFruitLoc = otherFruit.transform.position;
            otherFruit.GetComponent<Fruit>().row -= 1;
            otherFruit.GetComponent<Fruit>().targetV = currentFruitLoc;
            fruitScript.row++;
            fruitScript.targetV = otherFruitLoc;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // LEFT SWIPE
            otherFruit = allFruits[column - 1, row];
            otherFruitLoc = otherFruit.transform.position;
            otherFruit.GetComponent<Fruit>().column += 1;
            otherFruit.GetComponent<Fruit>().targetV = currentFruitLoc;
            fruitScript.column--;
            fruitScript.targetV = otherFruitLoc;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // DOWN SWIPE
            otherFruit = allFruits[column, row - 1];
            otherFruitLoc = otherFruit.transform.position;
            otherFruit.GetComponent<Fruit>().row += 1;
            otherFruit.GetComponent<Fruit>().targetV = currentFruitLoc;
            fruitScript.row--;
            fruitScript.targetV = otherFruitLoc;
        }
    }

    public void CheckAndDestroyMatches()
    {
        // Check for matches in columns
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 2; j++)
            {
                GameObject fruit1 = allFruits[i, j];
                GameObject fruit2 = allFruits[i, j + 1];
                GameObject fruit3 = allFruits[i, j + 2];

                // Check if the fruits have the same type (you can implement this check in your Fruit script)
                if (fruit1.GetComponent<Fruit>().fruitType == fruit2.GetComponent<Fruit>().fruitType &&
                    fruit1.GetComponent<Fruit>().fruitType == fruit3.GetComponent<Fruit>().fruitType)
                {
                    // Destroy the matching fruits
                    Destroy(fruit1);
                    Destroy(fruit2);
                    Destroy(fruit3);

                    // Optionally, you can spawn new fruits to replace the destroyed ones
                    // Implement spawning logic here

                    // Update the allFruits array to reflect the destroyed fruits
                    allFruits[i, j] = null;
                    allFruits[i, j + 1] = null;
                    allFruits[i, j + 2] = null;

                    return;
                }
            }
        }

        // Check for matches in rows (similar logic as columns)
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width - 2; i++)
            {
                GameObject fruit1 = allFruits[i, j];
                GameObject fruit2 = allFruits[i + 1, j];
                GameObject fruit3 = allFruits[i + 2, j];

                // Check if the fruits have the same type
                if (fruit1.GetComponent<Fruit>().fruitType == fruit2.GetComponent<Fruit>().fruitType &&
                    fruit1.GetComponent<Fruit>().fruitType == fruit3.GetComponent<Fruit>().fruitType)
                {
                    Destroy(fruit1);
                    Destroy(fruit2);
                    Destroy(fruit3);

                    // Optionally, spawn new fruits to replace the destroyed ones

                    allFruits[i, j] = null;
                    allFruits[i + 1, j] = null;
                    allFruits[i + 2, j] = null;

                    return;
                }
            }
        }

    }
}

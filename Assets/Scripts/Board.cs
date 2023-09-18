using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
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
    public GameObject[,] allTiles;

    void Start()
    {
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
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



    public void MoveFruits(float swipeAngle, int column, int row)
    {
        GameObject otherFruit;
        GameObject fruit = allFruits[column, row];
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Vector2 currentFruitLoc = fruit.transform.position;
        Vector2 otherFruitLoc;
        if (swipeAngle > -45 && swipeAngle <= 45 && column < width)
        {
            // RIGHT SWIPE
            otherFruit = allFruits[column + 1, row];
            otherFruitLoc = otherFruit.transform.position;
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
        StartCoroutine(CheckAndDestroyMatches());
    }
    public IEnumerator CheckAndDestroyMatches()
    {
        yield return null;

        // Check for matches in columns
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 2; j++)
            {
                GameObject[] fruitsCheck;
                fruitsCheck = new GameObject[height];
                bool same = true;
                int k = 0;
                while (same)
                {
                    if (j + k + 1 < height && allFruits[i, j + k].GetComponent<Fruit>().fruitType == allFruits[i, j + k + 1].GetComponent<Fruit>().fruitType)
                    {
                        fruitsCheck[k] = allFruits[i, j + k];
                        k++;
                    }
                    else
                    {
                        fruitsCheck[k] = allFruits[i, j + k];
                        k++;
                        same = false;
                    }
                }

                if (k >= 3)
                {
                    for (int e = 0; e < k; e++)
                    {
                        StartCoroutine(FadeOut(fruitsCheck[e]));
                    }

                    j += k;
                }


            }
        }


        // Check for matches in rows (similar logic as columns)
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width - 2; i++)
            {
                GameObject[] fruitsCheck;
                fruitsCheck = new GameObject[width];
                bool same = true;
                int k = 0;
                while (same)
                {
                    if (i + k + 1 < width && allFruits[i + k, j].GetComponent<Fruit>().fruitType == allFruits[i + k + 1, j].GetComponent<Fruit>().fruitType)
                    {
                        fruitsCheck[k] = allFruits[i + k, j];
                        k++;
                    }
                    else
                    {
                        fruitsCheck[k] = allFruits[i + k, j];
                        k++;
                        same = false;
                    }
                }

                if (k >= 3)
                {
                    for (int e = 0; e < k; e++)
                    {
                        StartCoroutine(FadeOut(fruitsCheck[e]));
                    }

                    i += k;
                }
            }
        }

    }

    private IEnumerator FadeOut(GameObject fruit)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;
        Color color = fruit.GetComponent<SpriteRenderer>().color;

        while (elapsedTime < fadeDuration)
        {
            // Calculate the new alpha value
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Update the object's color with the new alpha
            fruit.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the object is completely transparent

        fruit.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0f);
        ReplaceDestroyedFruit(fruit.GetComponent<Fruit>().column, fruit.GetComponent<Fruit>().row);

    }

    public void ReplaceDestroyedFruit(int column, int row)
    {

        Vector2 tempPosition = new Vector2(column - xOffset, row - yOffset);

        // Instantiate a new fruit at the position of the destroyed fruit
        int fruitToUse = Random.Range(0, fruits.Length);
        GameObject fruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);

        // Set the parent and name of the new fruit
        fruit.transform.parent = this.transform;
        fruit.name = "( " + column + ", " + row + " )";

        // Set the column and row of the new fruit
        fruit.GetComponent<Fruit>().column = column;
        fruit.GetComponent<Fruit>().row = row;
        fruit.GetComponent<Fruit>().fruitType = fruitToUse;


        // Add the new fruit to the allFruits array
        Destroy(allFruits[column, row]);
        allFruits[column, row] = fruit;
    }
}

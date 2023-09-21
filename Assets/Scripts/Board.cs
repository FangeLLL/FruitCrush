using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;

    public float xOffset;
    public float yOffset;

    [SerializeField]
    public AchievementManager achievementManager;

    bool checkingMatch = false;

    AudioManager audioManager;

    [SerializeField]
    public GameObject[] fruits;

    public GameObject[,] allFruits;
    public GameObject[,] allTiles;

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
        SetUp();
        StartCoroutine(CheckAndDestroyMatches());
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

    public void SwipeFruits(float swipeAngle, int column, int row)
    {

        GameObject otherFruit;
        GameObject fruit = allFruits[column, row];
        if (fruit.GetComponent<Fruit>().isSwiped)
        {
            return;
        }
        if (swipeAngle > -45 && swipeAngle <= 45 && column + 1 < width)
        {
            // RIGHT SWIPE
            if ((otherFruit) = allFruits[column + 1, row])
            {
                if (Vector2.Distance(otherFruit.GetComponent<Fruit>().targetV, otherFruit.transform.position) > .1f)
                {
                    Debug.Log("You cant swipe right!!!");
                    audioManager.SwipeResistBorder();
                    return;
                }
            }
            else
            {
                Debug.Log("You cant swipe right!!!");
                audioManager.SwipeResistBorder();
                return;
            }

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row + 1 < height)
        {
            // UP SWIPE
            if((otherFruit) = allFruits[column, row + 1])
            {
                if (Vector2.Distance(otherFruit.GetComponent<Fruit>().targetV, otherFruit.transform.position) > .1f)
                {
                    Debug.Log("You cant swipe up!!!");
                    audioManager.SwipeResistBorder();
                    return;
                }
            }
            else
            {
                Debug.Log("You cant swipe up!!!");
                audioManager.SwipeResistBorder();
                return;
            } 
           
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // LEFT SWIPE
            if ((otherFruit) = allFruits[column - 1, row])
            {
                if (Vector2.Distance(otherFruit.GetComponent<Fruit>().targetV, otherFruit.transform.position) > .1f)
                {
                    Debug.Log("You cant swipe left!!!");
                    audioManager.SwipeResistBorder();
                    return;
                }
            }
            else
            {
                Debug.Log("You cant swipe left!!!");
                audioManager.SwipeResistBorder();
                return;
            }
                          
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // DOWN SWIPE
            if((otherFruit) = allFruits[column, row - 1])
            {
                if (Vector2.Distance(otherFruit.GetComponent<Fruit>().targetV, otherFruit.transform.position) > .1f)
                {
                    Debug.Log("You cant swipe down!!!");
                    audioManager.SwipeResistBorder();
                    return;
                }
            }
            else
            {
                Debug.Log("You cant swipe down!!!");
                audioManager.SwipeResistBorder();
                return;
            }          
        }
        else
        {
            audioManager.SwipeResistBorder();
            return;
        }

        if (otherFruit.GetComponent<Fruit>().isSwiped)
        {
            return;
        }
        fruit.GetComponent<Fruit>().isSwiped = true;
        otherFruit.GetComponent<Fruit>().isSwiped = true;
        audioManager.Swipe();
        StartCoroutine(CheckMove(fruit, otherFruit));
        if (!checkingMatch)
        {
            StartCoroutine(CheckAndDestroyMatches());
        }

    }

    private IEnumerator CheckAndDestroyMatches()
    {
        List<GameObject> willPop = new List<GameObject>();
        checkingMatch = true;
        yield return null;
        List<int> fillColumns = new List<int>();
   
        int[] typeFruits = new int[fruits.Length];

        // Check for matches in columns
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 2; j++)
            {
                GameObject[] fruitsCheck;
                fruitsCheck = new GameObject[height];
                bool same = true;
                int k = 0;
                // it checking if bottom piece and one above it same. In every cycle it adds the one is currently below checking and if two of them not same
                // then its out of the  cycle. Checking if the space is null or exist.
                if (allFruits[i, j])
                {
                    while (same)
                    {
                        if (j + k + 1 >= height || !allFruits[i, j + k + 1] || allFruits[i, j + k].GetComponent<Fruit>().fruitType != allFruits[i, j + k + 1].GetComponent<Fruit>().fruitType)
                        {
                            same = false;
                        }
                        fruitsCheck[k] = allFruits[i, j + k];
                        k++;
                    }

                    // If in this cycle there is more than 3 match then its pops up them. 

                    if (k >= 3)
                    {
                        for (int e = 0; e < k; e++)
                        {
                            if (!willPop.Contains(fruitsCheck[e]))
                            {
                                willPop.Add(fruitsCheck[e]);
                                // Increasing the number that represent how many fruits popped of that type.
                                typeFruits[fruitsCheck[e].GetComponent<Fruit>().fruitType]++;
                            }
                        }
                        // Putting the column index to fillColumn list because in FillGaps function system just going to fill these columns
                        if (!fillColumns.Contains(i))
                        {
                            fillColumns.Add(i);
                        }
                        audioManager.FruitCrush();
                        // It checked the popped ones so it can check after that index.
                        j += k;
                        
                    }

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
                if (allFruits[i, j])
                {
                    while (same)
                    {
                        if (i + k + 1 >= width || !allFruits[i + k + 1, j] || allFruits[i + k, j].GetComponent<Fruit>().fruitType != allFruits[i + k + 1, j].GetComponent<Fruit>().fruitType)
                        {
                            same = false;
                        }
                        fruitsCheck[k] = allFruits[i + k, j];
                        k++;
                    }

                    if (k >= 3)
                    {
                        for (int e = 0; e < k; e++)
                        {
                            if (!willPop.Contains(fruitsCheck[e]))
                            {
                                willPop.Add(fruitsCheck[e]);
                                typeFruits[fruitsCheck[e].GetComponent<Fruit>().fruitType]++;
                                if (!fillColumns.Contains(i+e))
                                {
                                    fillColumns.Add(i+e);
                                }
                            }
                        }
                        audioManager.FruitCrush();
                        i += k;
                    }

                }                
             
            }
        }

        // Check for matches in quaternary. (!!! It can be more optimized !!!)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height - 1; j++)
            {
                List<GameObject> fruitsCheck = new List<GameObject>();
                int type;

                // it checking if bottom piece and one above it same. In every cycle it adds the one is currently below checking and if two of them not same
                // then its out of the  cycle.
               
                    if (j + 1 < height && allFruits[i, j] && allFruits[i, j + 1] && (type=allFruits[i, j].GetComponent<Fruit>().fruitType) == allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                    {
                        if (i + 1 < width && allFruits[i + 1, j] && allFruits[i + 1, j + 1] && allFruits[i + 1, j].GetComponent<Fruit>().fruitType 
                        == allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && type == allFruits[i + 1, j].GetComponent<Fruit>().fruitType)
                        {
                            fruitsCheck.Add(allFruits[i, j]);
                            fruitsCheck.Add(allFruits[i, j + 1]);
                            fruitsCheck.Add(allFruits[i + 1, j]);
                            fruitsCheck.Add(allFruits[i + 1, j + 1]);

                        if (!fillColumns.Contains(i))
                        {
                            fillColumns.Add(i);
                        }

                        if (!fillColumns.Contains(i+1))
                        {
                            fillColumns.Add(i+1);
                        }

                        j += 2;

                            for (int e = 0; e < 4; e++)
                            {
                                if (!willPop.Contains(fruitsCheck[e]))
                                {
                                    willPop.Add(fruitsCheck[e]);
                                    typeFruits[fruitsCheck[e].GetComponent<Fruit>().fruitType]++;

                                }
                            }
                        audioManager.FruitCrush();


                    }
                }                       
            }
        }
    
        if (willPop.Count>1)
        {
            for(int i = 0; i < willPop.Count; i++)
            {
                StartCoroutine(FadeOut(willPop[i]));
            }
            //achievementManager.AchievementProgress(typeFruits);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FillTheGaps(fillColumns));
        }
        else
        {
            checkingMatch = false;
        }


    }

    private IEnumerator CheckMove(GameObject fruit, GameObject otherFruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();

        ChangeTwoFruit(fruit, otherFruit);
        yield return new WaitForSeconds(0.3f);


        // Checking if this is right move

        if (CheckMatchSides(fruitScript.row, fruitScript.column) || CheckMatchSides(otherFruitScript.row, otherFruitScript.column))
        {
            // If any of object has a match then function can just finish.
            if (fruit)
            {
                fruitScript.isSwiped = false;

            }
            if (otherFruit)
            {
                otherFruitScript.isSwiped = false;

            }
        }
        else
        {
            if (fruit && otherFruit)
            {
                Debug.Log("Revert");
                audioManager.SwipeResist();
                ChangeTwoFruit(fruit, otherFruit);
                yield return new WaitForSeconds(0.3f);
                fruit.GetComponent<Fruit>().isSwiped = false;
                otherFruit.GetComponent<Fruit>().isSwiped = false;
            }
        }
    }

    private void ChangeTwoFruit(GameObject fruit,GameObject otherFruit)
    {

        // Changing two fruit loc and info.

        Fruit fruitScript = fruit.GetComponent<Fruit>();
        int tempRow=fruitScript.row, tempCol=fruitScript.column;
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();

        fruitScript.row = otherFruitScript.row;
        fruitScript.column = otherFruitScript.column;
      //  fruitScript.speed = 0.08f;

        otherFruitScript.row = tempRow;
        otherFruitScript.column=tempCol;
     //   otherFruitScript.speed = 0.08f;

        Vector2 fruitLoc = fruit.transform.position;
        fruitScript.targetV=otherFruit.transform.position;
        otherFruitScript.targetV = fruitLoc;
        /*
        yield return new WaitForSeconds(0.2f);

        if (fruit)
        {
            fruitScript.speed = 0.04f;

        }
        if (otherFruit)
        {
            otherFruitScript.speed = 0.04f;

        }
        */
    }

    private bool CheckMatchSides(int row,int column)
    {
        /* Checking row of the object by starting 2 left from object. Function works like this;
         1. First if index of check exist
         2. The objects that going to work with exist
         3. If two object type same then it incrase the "match" number else "match" number will be zero.
         4.  If match number 2 then it means there is a match function return true.
        */


        int match =0;

        for (int k=-2; k<2;k++)
        {
            if (column+k>0 && column+k+1<width && allFruits[column + k, row] && allFruits[column + k + 1, row])
            {
                if (allFruits[column + k, row].GetComponent<Fruit>().fruitType == allFruits[column + k + 1, row].GetComponent<Fruit>().fruitType)
                {
                    match++;
                    if (match == 2)
                    {
                        return true;
                    }
                }
                else
                {
                    match = 0;
                }
            }
           
        }

        // There is no match so match should be zero and the function works same as row function.

        match = 0;

        for (int k = -2; k < 2; k++)
        {
            if (row + k > 0 && row + k + 1 < height && allFruits[column, row + k] && allFruits[column, row + k + 1])
            {
                if (allFruits[column, row + k].GetComponent<Fruit>().fruitType == allFruits[column, row + k + 1].GetComponent<Fruit>().fruitType)
                {
                    match++;
                    if (match == 2)
                    {
                        return true;
                    }
                }
                else
                {
                    match = 0;
                }
            }  
        }
        return false;

    }

    private IEnumerator FadeOut(GameObject fruit)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.3f;
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
        Destroy(allFruits[fruit.GetComponent<Fruit>().column, fruit.GetComponent<Fruit>().row]);
        
    }

    private IEnumerator FillTheGaps(List<int> columns)
    {

        for (int i = 0; i < columns.Count; i++)
        {
            // Starting from (0,0) location and its checks every column. It starts from botton to up and takes every empty place index and put it to queue
            // variable (emptyPlaces). 
            StartCoroutine(FillTheColumn(columns[i]));

        }
        yield return new WaitForSeconds(0.7f);
        StartCoroutine(CheckAndDestroyMatches());
    }

    private IEnumerator FillTheColumn(int i)
    {
        Queue<int> emptyPlaces = new Queue<int>();


        for (int j = 0; j < height; j++)
        {
            if (!allFruits[i, j])
            {
                // Putting empty place index to variable
                emptyPlaces.Enqueue(j);
            }
            else if (emptyPlaces.Count > 0)
            {
                // if there is a piece then piece new location will be first empty place in queue.

                int emptyRowIndex = emptyPlaces.Dequeue();
                GameObject fruit = allFruits[i, j];
                allFruits[i, emptyRowIndex] = fruit;
                allFruits[i, j] = null;
                Fruit fruitScript = fruit.GetComponent<Fruit>();

                fruitScript.row = emptyRowIndex;
                fruitScript.column = i;
                fruitScript.targetV.y = allTiles[i, emptyRowIndex].transform.position.y;
                emptyPlaces.Enqueue(j);
            }
        }

        while (emptyPlaces.Count > 0)
        {

            int emptyRowIndex = emptyPlaces.Dequeue();
            Vector2 tempPosition = new Vector2(i - xOffset, height - yOffset);

            // Instantiate a new fruit at the position of the destroyed fruit
            int fruitToUse = UnityEngine.Random.Range(0, fruits.Length);
            GameObject newFruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
            Fruit newFruitScript = newFruit.GetComponent<Fruit>();

            // Set the parent and name of the new fruit
            newFruit.transform.parent = this.transform;
            newFruit.name = "( " + i + ", " + emptyRowIndex + " )";

            // Set the column and row of the new fruit
            newFruitScript.column = i;
            newFruitScript.row = emptyRowIndex;
            newFruitScript.fruitType = fruitToUse;
            newFruitScript.targetV.y = allTiles[i, emptyRowIndex].transform.position.y;


            // Add the new fruit to the allFruits array
            allFruits[i, emptyRowIndex] = newFruit;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ReplaceDestroyedFruit(int column, int row)
    {

        Vector2 tempPosition = new Vector2(column - xOffset, row - yOffset);

        // Instantiate a new fruit at the position of the destroyed fruit
        int fruitToUse = UnityEngine.Random.Range(0, fruits.Length);
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

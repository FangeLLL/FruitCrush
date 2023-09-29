using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;
using static UnityEngine.GraphicsBuffer;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    private float timer = 0f;
    private float waitTime = 2f;

    [SerializeField]
    public AchievementManager achievementManager;
    public TaskController taskController;
    public SwipeHint swipeHint;
    public SaveData saveData;


    public bool checkingMatch = false;
    public bool isRunning = false;
    public bool exitUpdate = false;

    public bool[] columnsFilling;

    AudioManager audioManager;
    Animator animator;

    [SerializeField]
    private GameObject[] fruits;
    public GameObject strawBalePrefab;
    public GameObject tilePrefab;


    public GameObject[,] allFruits;
    public GameObject[,] allTiles;

    private void Awake()
    {
        saveData = FindFirstObjectByType<SaveData>();
        saveData.LoadFromJson();
    }

    void Start()
    {      
        width = saveData.grid.width;
        height = saveData.grid.height;
        fruits = saveData.grid.fruits;

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];

        columnsFilling = new bool[width]; 
        // SetUp();
        int[,] arrangeFruits = new int[width, height];
        int[,] arrangeTiles = new int[width, height];

        taskController.SetTask(0, height * 1);
        taskController.SetMoveCount(10);

        for(int i = 0;i < 3; i++)
        {
            for (int j = 0; j < height; j++)
            {
                arrangeTiles[i, j] = 1;
            }
        }

        SetUpWithArray(arrangeFruits,arrangeTiles);
        StartCoroutine(CheckAndDestroyMatches());
    }

    private void Update()
    {
        // Check if the conditions are met
        if (!checkingMatch && !exitUpdate)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Check if two seconds have passed
            if (timer >= waitTime)
            {                
                swipeHint.isHintSearching = true;
                swipeHint.continueIteration = true;
                timer = 0f;
                exitUpdate = true;
            }
        }
        else
        {
            // Reset the timer if conditions are not met
            timer = 0f;
        }
    }

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
                if (arrangedTiles[i, j] == 1)
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

    private void SetUp()
    {
      float  xOffset = width * 0.5f - 0.5f;
      float  yOffset = height * 0.5f - 0.5f;

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
        //StopCoroutine(GiveHint());
        swipeHint.isHintSearching = false;

        if (swipeHint.fruit != null)
        {
            Animator animator = swipeHint.fruit.GetComponentInChildren<Animator>();
            if (animator != null && animator.gameObject.activeSelf && animator.runtimeAnimatorController != null && animator.isActiveAndEnabled)
            {
                animator.SetBool(swipeHint.fruit.swipeUp, false);
                animator.SetBool(swipeHint.fruit.swipeDown, false);
                animator.SetBool(swipeHint.fruit.swipeLeft, false);
                animator.SetBool(swipeHint.fruit.swipeRight, false);
            }
        }

        GameObject otherFruit;
        GameObject fruit = allFruits[column, row];
        if (fruit.GetComponent<Fruit>().isSwiped)
        {
            audioManager.SwipeResistBorder();
            return;
        }
        if (swipeAngle > -45 && swipeAngle <= 45 && column + 1 < width)
        {
            // RIGHT SWIPE
            if ((otherFruit) = allFruits[column + 1, row])
            {
                if (Mathf.Abs(otherFruit.GetComponent<Fruit>().targetV.y - otherFruit.transform.position.y) > .1f)
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
                if (Mathf.Abs(otherFruit.GetComponent<Fruit>().targetV.y - otherFruit.transform.position.y) > .1f)
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
                if (Mathf.Abs(otherFruit.GetComponent<Fruit>().targetV.y - otherFruit.transform.position.y) > .1f)
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
                if (Mathf.Abs(otherFruit.GetComponent<Fruit>().targetV.y - otherFruit.transform.position.y) > .1f)
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

        if (swipeHint.fruit != null)
        {
            Animator animator = swipeHint.fruit.GetComponentInChildren<Animator>();
            if (animator != null && animator.gameObject.activeSelf && animator.runtimeAnimatorController != null && animator.isActiveAndEnabled)
            {
                animator.SetBool(swipeHint.fruit.swipeUp, false);
                animator.SetBool(swipeHint.fruit.swipeDown, false);
                animator.SetBool(swipeHint.fruit.swipeLeft, false);
                animator.SetBool(swipeHint.fruit.swipeRight, false);
            }
        }

        audioManager.Swipe();
        StartCoroutine(CheckMove(fruit, otherFruit));
        if (!checkingMatch)
        {
            StartCoroutine(CheckAndDestroyMatches());
        }
    }

    private IEnumerator CheckAndDestroyMatches()
    {
        List<GameObject> fruitsCheck = new List<GameObject>();
        checkingMatch = true;
        yield return null;
        bool popped = false;

        int[] typeFruits = new int[fruits.Length];

        // Check for matches in columns
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                List<GameObject> fruitsCheckTemp = new List<GameObject>();
                bool same = true;
                int k = 0;
                bool row = false, column = false;
                // it checking if bottom piece and one above it same. In every cycle it adds the one is currently below checking and if two of them not same
                // then its out of the  cycle. Checking if the space is null or exist.
                if (allFruits[i, j])
                {
                    // Column
                    while (same)
                    {
                        if (j + k + 1 >= height || !allFruits[i, j + k + 1] || allFruits[i, j + k].GetComponent<Fruit>().fruitType != allFruits[i, j + k + 1].GetComponent<Fruit>().fruitType)
                        {
                            same = false;
                        }
                        fruitsCheckTemp.Add(allFruits[i, j + k]);
                        k++;
                    }
                    same = true;
                    if (k < 3)
                    {
                        fruitsCheckTemp.Clear();
                    }
                    else
                    {
                        column = true;
                        fruitsCheck.AddRange(fruitsCheckTemp);
                    }
                    k = 0;

                    //Row
                    while (same)
                    {
                        if (i + k + 1 >= width || !allFruits[i + k + 1, j ] || allFruits[i + k, j ].GetComponent<Fruit>().fruitType != allFruits[i + k + 1, j ].GetComponent<Fruit>().fruitType)
                        {
                            same = false;
                        }
                        fruitsCheckTemp.Add(allFruits[i + k, j]);
                        k++;
                    }

                    if (k < 3)
                    {
                        fruitsCheckTemp.Clear();
                    }
                    else
                    {
                        row= true;
                        fruitsCheck.AddRange(fruitsCheckTemp.Except(fruitsCheck).ToList());
                    }



                    int type;

                    if (j + 1 < height && allFruits[i, j] && allFruits[i, j + 1] && (type = allFruits[i, j].GetComponent<Fruit>().fruitType) == allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                    {
                        if (i + 1 < width && allFruits[i + 1, j] && allFruits[i + 1, j + 1] && allFruits[i + 1, j].GetComponent<Fruit>().fruitType
                        == allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && type == allFruits[i + 1, j].GetComponent<Fruit>().fruitType)
                        {
                            fruitsCheckTemp.Add(allFruits[i, j]);
                            fruitsCheckTemp.Add(allFruits[i, j + 1]);
                            fruitsCheckTemp.Add(allFruits[i + 1, j]);
                            fruitsCheckTemp.Add(allFruits[i + 1, j + 1]);

                            audioManager.FruitCrush();
                        }
                    }

                    if (fruitsCheckTemp.Count < 3)
                    {
                        fruitsCheckTemp.Clear();
                    }
                    else
                    {
                        fruitsCheck.AddRange(fruitsCheckTemp.Except(fruitsCheck).ToList());
                    }

                    if (fruitsCheck.Count > 0)
                    {
                        //Debug.Log(fruitsCheck.Count+" popped same time");
                        if (row && column)
                        {
                            Debug.Log("L or + shape happend");
                        }
                        audioManager.FruitCrush();
                        type = allFruits[i, j].GetComponent<Fruit>().fruitType;
                        typeFruits[type] += fruitsCheck.Count;
                      //   fruitsCheck[UnityEngine.Random.Range(0, fruitsCheck.Count)];
                        for (int e = 0; e < fruitsCheck.Count; e++)
                        {
                            StartCoroutine(FadeOut(fruitsCheck[e], true));
                        }
                        fruitsCheck.Clear();

                        popped= true;

                        // SWIPE HINT ANIMATION STOP

                        if (swipeHint.fruit != null)
                        {
                            Animator animator = swipeHint.fruit.GetComponentInChildren<Animator>();
                            if (animator != null && animator.gameObject.activeSelf && animator.runtimeAnimatorController != null && animator.isActiveAndEnabled)
                            {
                                animator.SetBool(swipeHint.fruit.swipeUp, false);
                                animator.SetBool(swipeHint.fruit.swipeDown, false);
                                animator.SetBool(swipeHint.fruit.swipeLeft, false);
                                animator.SetBool(swipeHint.fruit.swipeRight, false);
                            }
                        }
                        achievementManager.AchievementProgress(typeFruits);                     
                    }

                }
            }
        }

        if (popped)
        {        
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(FillTheGaps());
        }
        else
        {
            checkingMatch = false;
            exitUpdate = false;
            
        }

        /*
        if (fruitsCheck.Count > 0)
        {
            for (int i = 0; i < fruitsCheck.Count; i++)
            {
                StartCoroutine(FadeOut(fruitsCheck[i], true));
            }
            achievementManager.AchievementProgress(typeFruits);
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(FillTheGaps());
        }
        else
        {
            checkingMatch = false;
        }
        */

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
            taskController.MovePlayed();
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
                audioManager.SwipeResist();
                ChangeTwoFruit(fruit, otherFruit);
                yield return new WaitForSeconds(0.3f);
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
                taskController.MovePlayed();
                if (fruit)
                {
                    fruitScript.isSwiped = false;

                }
                if (otherFruit)
                {
                    otherFruitScript.isSwiped = false;

                }
            }
        }
    }

    private void ChangeTwoFruit(GameObject fruit,GameObject otherFruit)
    {

        // Changing two fruit loc and info.

        Fruit fruitScript = fruit.GetComponent<Fruit>();
        int tempRow=fruitScript.row, tempCol=fruitScript.column;
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();

        fruitScript.targetV = allTiles[otherFruitScript.column, otherFruitScript.row].transform.position;
        otherFruitScript.targetV = allTiles[tempCol,tempRow].transform.position;

        fruitScript.row = otherFruitScript.row;
        fruitScript.column = otherFruitScript.column;
      //  fruitScript.speed = 0.08f;

        otherFruitScript.row = tempRow;
        otherFruitScript.column=tempCol;
     //   otherFruitScript.speed = 0.08f;

       
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
            if (column+k>=0 && column+k+1<width && allFruits[column + k, row] && allFruits[column + k + 1, row])
            {
                if (allFruits[column + k, row].GetComponent<Fruit>().fruitType == allFruits[column + k + 1, row].GetComponent<Fruit>().fruitType)
                {
                    match++;
                    if (match > 1)
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
            if (row + k >= 0 && row + k + 1 < height && allFruits[column, row + k] && allFruits[column, row + k + 1])
            {
                if (allFruits[column, row + k].GetComponent<Fruit>().fruitType == allFruits[column, row + k + 1].GetComponent<Fruit>().fruitType)
                {
                    match++;
                    if (match > 1)
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

    public IEnumerator FadeOut(GameObject obj,bool explosion)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.3f;
        Color color = obj.GetComponentInChildren<SpriteRenderer>().color;
        if (explosion)
        {
            int row = obj.GetComponent<Fruit>().row;
            int column = obj.GetComponent<Fruit>().column;

            allTiles[column, row].GetComponent<BackgroundTile>().Explosion(column, row);
        }
       
        while (elapsedTime < fadeDuration)
        {
            // Calculate the new alpha value
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Update the object's color with the new alpha
            obj.GetComponentInChildren<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the object is completely transparent

        obj.GetComponentInChildren<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0f);
        Destroy(obj);
        
    }

    private IEnumerator FillTheGaps()
    {
        yield return null;
        for (int i = 0; i < width; i++)
        {
            // Starting from (0,0) location and its checks every column. It starts from botton to up and takes every empty place index and put it to queue
            // variable (emptyPlaces). 
            if (!columnsFilling[i])
            {
                StartCoroutine(FillTheColumn(i));
            }

        }
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(CheckAndDestroyMatches());
    }

    public IEnumerator FillTheColumn(int i)
    {
        columnsFilling[i] = true;

        Queue<int> emptyPlaces = new Queue<int>();

        for (int j = 0; j < height; j++)
        {
            if (!allTiles[i, j].GetComponent<BackgroundTile>().strawBale)
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
            else
            {
                if (j - 1 >= 0 && !allFruits[i, j - 1] && !allTiles[i, j - 1].GetComponent<BackgroundTile>().strawBale)
                {
                    StartCoroutine(CrossFall(i, j));
                }
                emptyPlaces.Clear();

            }
        }

        while (emptyPlaces.Count > 0)
        {
            float xOffset = width * 0.5f - 0.5f;
            float yOffset = height * 0.5f - 0.5f;
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

        columnsFilling[i] = false;

    }

    private IEnumerator CrossFall(int column,int row)
    {
        GameObject fruit=null;
        Fruit fruitScript;
        int previousColumn=-1;
        yield return new WaitForSeconds(0.3f);
        if (column - 1 >= 0 && allFruits[column - 1, row] && !allFruits[column, row-1])
        {
            fruit = allFruits[column - 1, row];
            allFruits[column-1, row] = null;
            previousColumn=column - 1;
        }
        else if(column + 1 < width && allFruits[column + 1, row] && !allFruits[column, row - 1])
        {
            fruit = allFruits[column + 1, row];
            allFruits[column + 1, row] = null;
            previousColumn = column + 1;
        }

        if (fruit)
        {
            fruitScript = fruit.GetComponent<Fruit>();
            fruitScript.row = row - 1;
            fruitScript.column = column;
            fruitScript.targetV= allTiles[column,row-1].transform.position;
            yield return new WaitForSeconds(0.5f);
            if (previousColumn != -1 && !columnsFilling[previousColumn])
            {
                StartCoroutine(FillTheColumn(previousColumn));
            }
            yield return new WaitForSeconds(0.3f);
            if (!columnsFilling[column])
            {
                StartCoroutine(FillTheColumn(column));
            }  
            if(!checkingMatch)
            {
                StartCoroutine(CheckAndDestroyMatches());
            }
        }
        
    }

    public void ReplaceDestroyedFruit(int column, int row)
    {
        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;
        Vector2 tempPosition = new Vector2(column - xOffset, row - yOffset);

        // INSTANTIATE A NEW FRUIT AT THE POSITION OF THE DESTROYED FRUIT
        int fruitToUse = UnityEngine.Random.Range(0, fruits.Length);
        GameObject fruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);

        // SET THE PARENT AND NAME OF THE NEW FRUIT
        fruit.transform.parent = this.transform;
        fruit.name = "( " + column + ", " + row + " )";

        // SET THE COLUMN AND ROW OF THE NEW FRUIT
        fruit.GetComponent<Fruit>().column = column;
        fruit.GetComponent<Fruit>().row = row;
        fruit.GetComponent<Fruit>().fruitType = fruitToUse;

        // ADD THE NEW FRUIT TO THE allFruits ARRAY
        Destroy(allFruits[column, row]);
        allFruits[column, row] = fruit;
    }
}

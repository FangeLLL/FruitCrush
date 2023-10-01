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
  //  public SaveData saveData;


    public bool checkingMatch = false;
    public bool isRunning = false;
    public bool exitUpdate = false;

    AudioManager audioManager;
    Animator animator;

    [SerializeField]
    private GameObject[] fruits;
    [SerializeField]
    private GameObject[] powerUps;
    public GameObject strawBalePrefab;
    public GameObject tilePrefab;

    private bool[] fillingColumn;

    public bool hintBool = false;
    bool popped = false;


    public GameObject[,] allFruits;
    public GameObject[,] allTiles;

    private void Awake()
    {
      //  saveData = FindFirstObjectByType<SaveData>();
      //  saveData.LoadFromJson();
    }

    void Start()
    {
        //   width = saveData.grid.width;
        //  height = saveData.grid.height;
        //  fruits = saveData.grid.fruits;

        fillingColumn = new bool[width];

        StartCoroutine(FillTheGaps());

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];

        // SetUp();
        int[,] arrangeFruits = new int[width, height];
        int[,] arrangeTiles = new int[width, height];

        taskController.SetTask(0, height * 3);
        taskController.SetMoveCount(10);

        for(int i = 0;i < 3; i++)
        {
            for (int j = 0; j < height; j++)
            {
                arrangeTiles[i, j] = 1;
            }
        }

        SetUpWithArray(arrangeFruits,arrangeTiles);
    }

    private void Update()
    {
        // Check if the conditions are met
        if (hintBool && !exitUpdate)
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
        popped = false;
        hintBool = false;

        int[] typeFruits = new int[fruits.Length];

        // Check for matches in columns
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                List<GameObject> fruitsCheckTemp = new List<GameObject>();
                bool same = true;
                int k = 0;
                bool rowPopped = false, columnPopped = false;
                // it checking if bottom piece and one above it same. In every cycle it adds the one is currently below checking and if two of them not same
                // then its out of the  cycle. Checking if the space is null or exist.
                if (allFruits[i, j] && allFruits[i, j].GetComponent<Fruit>().fruitType>=0)
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
                        columnPopped = true;
                        fruitsCheck.AddRange(fruitsCheckTemp);
                    }
                    k = 0;

                    //Row
                    while (same)
                    {
                        if (i + k + 1 >= width || !allFruits[i + k + 1, j] || allFruits[i + k, j].GetComponent<Fruit>().fruitType != allFruits[i + k + 1, j].GetComponent<Fruit>().fruitType)
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
                        rowPopped = true;
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
                        audioManager.FruitCrush();
                        type = allFruits[i, j].GetComponent<Fruit>().fruitType;
                        typeFruits[type] += fruitsCheck.Count;
                      //   fruitsCheck[UnityEngine.Random.Range(0, fruitsCheck.Count)];
                        for (int e = 0; e < fruitsCheck.Count; e++)
                        {
                            StartCoroutine(FadeOut(fruitsCheck[e], true));
                        }
                        if (fruitsCheck.Count > 3)
                        {
                            GameObject fruitToChange = fruitsCheck[UnityEngine.Random.Range(0, fruitsCheck.Count)];
                            int row = fruitToChange.GetComponent<Fruit>().row;
                            int column = fruitToChange.GetComponent<Fruit>().column;
                            if (rowPopped && columnPopped)
                            {
                                CreatePowerUp(column, row, -3);
                            }
                            else if (rowPopped)
                            {
                                CreatePowerUp(column, row, -1);

                            }
                            else if (columnPopped)
                            {
                                CreatePowerUp(column, row, -2);

                            }

                        }
                        fruitsCheck.Clear();                     
                       
                        popped = true;

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

        if (!popped)
        {
            exitUpdate = false;
            hintBool= true;
        }
        checkingMatch = false;

    }

    private IEnumerator CheckMove(GameObject fruit, GameObject otherFruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();
        bool succesfulMove = false;

        if (fruitScript.fruitType < 0 && otherFruitScript.fruitType < 0)
        {
            fruitScript.row = otherFruitScript.row;
            fruitScript.column = otherFruitScript.column;
            fruitScript.targetV = otherFruitScript.targetV;

            ActivateMergePowerUp(fruit, otherFruit);
            succesfulMove = true;

        }
        else
        {
            ChangeTwoFruit(fruit, otherFruit);
            yield return new WaitForSeconds(0.2f);

            if (fruitScript.fruitType < 0 || otherFruitScript.fruitType < 0)
            {
                // If one of the fruits is power up 
                if (fruitScript.fruitType < 0)
                {
                    ActivatePowerUp(fruit);

                }
                else
                {
                    ActivatePowerUp(otherFruit);

                }
                succesfulMove = true;
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                if (CheckMatchSides(fruitScript.row, fruitScript.column) || CheckMatchSides(otherFruitScript.row, otherFruitScript.column))
                {
                    // Checked if this is right move
                    succesfulMove = true;
                }
                else
                {
                    if (!fruit || !otherFruit)
                    {

                        succesfulMove = true;

                    }

                }
            }        
        }
           

        if (succesfulMove)
        {
            taskController.MovePlayed();

        }
        else
        {
            audioManager.SwipeResist();
            ChangeTwoFruit(fruit, otherFruit);
            yield return new WaitForSeconds(0.3f);
        }

        if (fruit)
        {
            fruitScript.isSwiped = false;

        }
        if (otherFruit)
        {
            otherFruitScript.isSwiped = false;

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
            if (row + k >= 0 && row + k + 1 < height && allFruits[column, row + k] && allFruits[column, row + k + 1])
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

    public IEnumerator FadeOut(GameObject obj,bool explosion)
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.25f;
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
            if (obj)
            {
                obj.GetComponentInChildren<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);
            }
            else
            {
                elapsedTime = fadeDuration;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the object is completely transparent
        if (obj)
        {
            obj.GetComponentInChildren<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0f);
            Destroy(obj);
        }
        
        
    }

    private IEnumerator FillTheGaps()
    {
        yield return null;
        for (int i = 0; i < width; i++)
        {
            // Starting from (0,0) location and its checks every column. It starts from botton to up and takes every empty place index and put it to queue
            // variable (emptyPlaces). 
            if (!fillingColumn[i])
            {
                StartCoroutine(FillTheColumn(i));
            }

        }
        yield return new WaitForSeconds(0.3f);
        if (!checkingMatch)
        {
            StartCoroutine(CheckAndDestroyMatches());
        }
        StartCoroutine(FillTheGaps());
    }

    public IEnumerator FillTheColumn(int i)
    {
        fillingColumn[i] = true;

        Queue<int> emptyPlaces = new Queue<int>();

        for (int j = 0; j < height; j++)
        {
            if (!allTiles[i, j].GetComponent<BackgroundTile>().strawBale)
            {
                if (!allFruits[i, j])
                {
                    // Putting empty place index to variable
                    emptyPlaces.Enqueue(j);
                    if ((j + 1 < height && allTiles[i, j + 1].GetComponent<BackgroundTile>().strawBale)|| (j + 2 < height && allTiles[i, j + 2].GetComponent<BackgroundTile>().strawBale))
                    {
                        StartCoroutine(CrossFall(i, j+1));
                    }
                }
                else if (emptyPlaces.Count > 0)
                {
                    // if there is a piece then piece new location will be first empty place in queue.

                    int emptyRowIndex = emptyPlaces.Dequeue();
                    GameObject fruit = allFruits[i, j];
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
            yield return new WaitForSeconds(0.1f);
        }
        fillingColumn[i] = false;

    }

    private void CreatePowerUp(int column,int row,int type)
    {
        /*
        Power Up Type Number;
        
        -1 : Horizantal Harvester
        -2 : Vertical Harvester
        -3 : TNT
         */


        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;
        Vector2 tempPosition = new Vector2(column - xOffset, height - yOffset);

        GameObject newPowerUp = Instantiate(powerUps[(type*-1)-1], tempPosition, powerUps[(type * -1) - 1].transform.rotation);
        Fruit newPowerUpScript = newPowerUp.GetComponent<Fruit>();

        // Set the parent and name of the new fruit
        newPowerUp.transform.parent = this.transform;
        newPowerUp.name = "( " + column + ", " + row + " )";

        // Set the column and row of the new fruit
        newPowerUpScript.column = column;
        newPowerUpScript.row = row;
        newPowerUpScript.fruitType = type;
        newPowerUp.gameObject.transform.position = allTiles[column, row].transform.position;
        newPowerUpScript.targetV = allTiles[column, row].transform.position;


        // Add the new fruit to the allFruits array
        allFruits[column, row] = newPowerUp;
    }

    private IEnumerator CrossFall(int column,int row)
    {
        GameObject fruit=null;
        Fruit fruitScript;
        yield return new WaitForSeconds(0.5f);
        if (column - 1 >= 0 && allFruits[column - 1, row] && !allFruits[column, row-1])
        {
            fruit = allFruits[column - 1, row];
            allFruits[column-1, row] = null;
        }
        else if(column + 1 < width && allFruits[column + 1, row] && !allFruits[column, row - 1])
        {
            fruit = allFruits[column + 1, row];
            allFruits[column + 1, row] = null;
        }

        if (fruit)
        {
            fruitScript = fruit.GetComponent<Fruit>();
            fruitScript.row = row - 1;
            fruitScript.column = column;
            fruitScript.targetV= allTiles[column,row-1].transform.position; 
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

    public void ActivateMergePowerUp(GameObject fruit, GameObject otherFruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>(), otherFruitScript=otherFruit.GetComponent<Fruit>();
        if (otherFruitScript.fruitType==fruitScript.fruitType)
        {
            // If two power up same it goes here

            switch (fruitScript.fruitType)
            {
                case -1:
                    fruitScript.fruitType = -2;
                    fruitScript.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    ActivatePowerUp(fruit);
                    ActivatePowerUp(otherFruit);
                    break;

                case -2:
                    fruitScript.fruitType = -1;
                    fruitScript.transform.rotation = Quaternion.Euler(0f, 0f, 0);
                    ActivatePowerUp(fruit);
                    ActivatePowerUp(otherFruit);
                    break;
            }


        }
        else
        {

            // If two power up diffirent it goes here

            switch (fruitScript.fruitType)
            {
                case -1:
                    switch (otherFruitScript.fruitType)
                    {
                        case -2:
                            ActivatePowerUp(fruit);
                            ActivatePowerUp(otherFruit);
                            break;
                        case -3:
                            break;
                    }
                    break;
                case -2:
                    switch (otherFruitScript.fruitType)
                    {
                        case -1:
                            ActivatePowerUp(fruit);
                            ActivatePowerUp(otherFruit);
                            break;
                        case -3:
                            break;
                    }
                    break;
            }


        }
    
    }

    private void ActivatePowerUp(GameObject fruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        int column = fruitScript.column, row = fruitScript.row, type = fruitScript.fruitType;
        switch (type)
        {
            // Horizontal Harvester power up
            case -1:

                GameObject cloneHorizontal = Instantiate(powerUps[0], allTiles[column, row].transform.position, powerUps[0].transform.rotation);
                Fruit cloneHorizontalScript = cloneHorizontal.GetComponent<Fruit>();
                cloneHorizontal.GetComponent<SpriteRenderer>().flipX = false;

                cloneHorizontalScript.row = row;
                cloneHorizontalScript.column = column;
                cloneHorizontal.gameObject.transform.position = allTiles[column, row].transform.position;

                HorizontalHarvesterMove(cloneHorizontal, true);
                HorizontalHarvesterMove(fruit, false);

                break;
            // Vertical Harvester power up
            case -2:

                GameObject cloneVertical = Instantiate(powerUps[1], allTiles[column, row].transform.position, powerUps[1].transform.rotation);
                Fruit cloneVerticalScript = cloneVertical.GetComponent<Fruit>();
                cloneVertical.GetComponent<SpriteRenderer>().flipY = true;

                cloneVerticalScript.row = row;
                cloneVerticalScript.column = column;
                cloneVertical.gameObject.transform.position = allTiles[column, row].transform.position;

                VerticalHarvesterMove(cloneVertical, true);
                VerticalHarvesterMove(fruit, false);
                break;
            // TNT power up
            case -3:

                for (int i = column - 1; i <= column + 1; i++)
                {
                    for (int j = row - 1; j <= row + 1; j++)
                    {
                        if (row - 1 >= 0 && row + 1 < height && column - 1 >= 0 && column + 1 < width)
                        {
                            if (allFruits[i, j])
                            {
                                if (allFruits[i, j].GetComponent<Fruit>().fruitType < 0 && i != column && j != row && allFruits[i, j].GetComponent<Fruit>().fruitType != type)
                                {
                                    ActivatePowerUp(allFruits[i, j]);
                                }
                                else
                                {
                                    StartCoroutine(FadeOut(allFruits[i, j], false));
                                }
                            }
                            else
                            {
                                allTiles[i, j].GetComponent<BackgroundTile>().Boom(i, j);
                            }
                        }
                    }
                }

                break;

        }
    }

    private void HorizontalHarvesterMove(GameObject harvester,bool clone)
    {
        harvester.GetComponent<Collider2D>().enabled = false;
        Fruit harvesterScript=harvester.GetComponent<Fruit>();
        //harvesterScript.speed = 0.04f;
        int row = harvesterScript.row,column=harvesterScript.column;
        if (clone)
        {
            harvesterScript.targetV.x = allTiles[width-1, row].transform.position.x + 1;

            for (int i = column+1; i < width; i++)
            {
                if (allFruits[i, row])
                {
                    if (allFruits[i, row].GetComponent<Fruit>().fruitType < 0 && allFruits[i, row].GetComponent<Fruit>().fruitType != -1)
                    {
                        ActivatePowerUp(allFruits[i, row]);
                    }
                    else
                    {
                        StartCoroutine(FadeOut(allFruits[i, row], false));
                    }
                }
                else
                {
                    allTiles[i, row].GetComponent<BackgroundTile>().Boom(i, row);

                }

            }
        }
        else
        {
            harvester.GetComponent<Fruit>().targetV.x = allTiles[0, row].transform.position.x - 1;

            for (int i = column-1; i >= 0; i--)
            {
                if (allFruits[i, row])
                {
                    if (allFruits[i, row].GetComponent<Fruit>().fruitType < 0 && allFruits[i, row].GetComponent<Fruit>().fruitType != -1)
                    {
                        ActivatePowerUp(allFruits[i, row]);
                    }
                    else
                    {
                        StartCoroutine(FadeOut(allFruits[i, row], false));
                    }
                }
                else
                {
                    allTiles[i, row].GetComponent<BackgroundTile>().Boom(i, row);

                }

            }
        }
        StartCoroutine(FadeOut(harvester,false));

    }

    public void VerticalHarvesterMove(GameObject harvester, bool clone)
    {
        harvester.GetComponent<Collider2D>().enabled = false;
        Fruit harvesterScript = harvester.GetComponent<Fruit>();
        int row = harvesterScript.row, column = harvesterScript.column;
        if (!clone)
        {
            harvesterScript.targetV.y = allTiles[column, height - 1].transform.position.y + 1;

            for (int i = row + 1; i < height; i++)
            {
                if (allFruits[column,i])
                {
                    if (allFruits[column, i].GetComponent<Fruit>().fruitType < 0 && allFruits[column, i].GetComponent<Fruit>().fruitType != -2)
                    {
                        ActivatePowerUp(allFruits[column, i]);
                    }
                    else
                    {
                        StartCoroutine(FadeOut(allFruits[column, i], false));
                    }
                }
                else
                {
                    allTiles[column, i].GetComponent<BackgroundTile>().Boom(column, i);

                }

            }
        }
        else
        {
            harvesterScript.targetV.y = allTiles[column, 0].transform.position.y - 1;

            for (int i = row - 1; i >= 0; i--)
            {
                if (allFruits[column, i])
                {
                    if (allFruits[column, i].GetComponent<Fruit>().fruitType < 0 && allFruits[column, i].GetComponent<Fruit>().fruitType != -2)
                    {
                        ActivatePowerUp(allFruits[column, i]);
                    }
                    else
                    {
                        StartCoroutine(FadeOut(allFruits[column, i], false));
                    }
                }
                else
                {
                    allTiles[column, i].GetComponent<BackgroundTile>().Boom(column, i);

                }

            }
        }
        StartCoroutine(FadeOut(harvester, false));

    }
}

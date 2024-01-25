using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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
    public SpecialPowerController specialPowerController;

    public bool checkingMatch = false;
    public bool isRunning = false;
    public bool exitUpdate = false;

    AudioManager audioManager;
    Animator animator;

    [SerializeField]
    private GameObject[] fruits;
    [SerializeField]
    private GameObject[] powerUps;
    public GameObject[] obstaclePrefabs;
    public GameObject tilePrefab;

    private bool[] fillingColumn;

    public bool hintBool = false;
    bool popped = false;

    IndexLibrary indexLibrary = new IndexLibrary();

    List<int> existFruits = new List<int>();

    public int userLevel;

    public GameObject[,] allFruits;
    public GameObject[,] allTiles;
    public float scaleNumber;
    private float scaleFactorFruit;

    private int[] columnsFallIndexY;

    // Test variable
    public int specialPowerID = 0;
    public bool specialSwipe = false;

    private void Awake()
    {
        saveData.LoadFromJson();

    }

    void Start()
    {
        userLevel = PlayerPrefs.GetInt("level", 0);

        Grid gridData = saveData.gridData[userLevel];

        width = gridData.width;
        height = gridData.height;

        // Some of columns falling point can be diffirent because of missing tiles in that column. 
        columnsFallIndexY = new int[width];

        RearrangeScaleNumber();

        // Getting avaliable fruits indexes and adding them to a list. existFruits list has indexes of avaliable fruits and this list going to be used when
        // creating new fruits.

        for (int i = 0; i < gridData.fruits.Length; i++)
        {
            if (gridData.fruits[i] == 1)
            {
                existFruits.Add(i);
            }
        }

        fillingColumn = new bool[width];

        StartCoroutine(FillTheGaps());

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];

        int[] savedTiles = gridData.allTilesTotal;
        int[] savedFruits = gridData.allFruitsTotal;
        int[] savedTilesZero = gridData.tilesIndexZero;
        int[] savedTilesOne = gridData.tilesIndexOne;
        int[] savedTilesTwo = gridData.tilesIndexTwo;

        taskController.SetTask(gridData.taskElements);
        taskController.SetMoveCount(gridData.moveCount);

        if(gridData.level == 0)
        {
            Debug.Log("This level did not made yet. Please, don't assume that this is a some sort of bug and tell Bertuð to fix it, just make this level in Level Editor.");
        }
        else
        {
            SetUpWithArray(indexLibrary.Convert2DTo3D(width, height, savedFruits), indexLibrary.Convert2DTo3D(width, height, savedTiles), indexLibrary.Convert2DTo3D(width, height, savedTilesZero), indexLibrary.Convert2DTo3D(width, height, savedTilesOne), indexLibrary.Convert2DTo3D(width, height, savedTilesTwo));
        }

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
                if (!swipeHint.oneHintActive)
                {
                    swipeHint.isHintSearching = true;
                    swipeHint.continueIteration = true;
                    timer = 0f;
                    exitUpdate = true;
                }
                
            }
        }
        else
        {
            // Reset the timer if conditions are not met
            timer = 0f;
        }

    }

    /// <summary>
    /// Arranging the general scale variable of prefabs according to size of board.
    /// </summary>
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

    /// <summary>
    /// According to level save data it creates the board and change the scale of prefabs (fruits, powerups and tiles).
    /// </summary>
    /// <param name="arrangedFruits"></param>
    /// <param name="arrangedTiles"></param>
    /// <param name="arrangedTilesZero"></param>
    /// <param name="arrangedTilesOne"></param>
    /// <param name="arrangedTilesTwo"></param>
    private void SetUpWithArray(int[,] arrangedFruits, int[,] arrangedTiles, int[,] arrangedTilesZero, int[,] arrangedTilesOne, int[,] arrangedTilesTwo)
    {

        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        tilePrefab.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);

        // Changing scales of prefabs.

        foreach (GameObject fruitScale in fruits)
        {
            fruitScale.transform.localScale = new Vector3(scaleFactorFruit, scaleFactorFruit, scaleFactorFruit);
        }

        foreach (GameObject powerUpScale in powerUps)
        {
            powerUpScale.transform.localScale = new Vector3(scaleFactorFruit, scaleFactorFruit, scaleFactorFruit);
        }

        foreach (GameObject obstaclePrefab in obstaclePrefabs)
        {
            obstaclePrefab.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
        }

        // Putting and creating fruits and tiles in cells.

        for (int i = 0; i < width; i++)
        {
            // Start with -1 because it is possible that one of the columns does not exist because of shape of board.
            int fallPoint = -1;
            for (int j = 0; j < height; j++)
            {
                // If tiles value 0 then it means it is missing (not existing) tile.
                if (arrangedTiles[i, j] == 1)
                {
                    // If tile exist than updating fallpoint.
                    fallPoint = j;

                    Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, j * scaleNumber - yOffset);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";
                    backgroundTile.GetComponent<BackgroundTile>().column = i;
                    backgroundTile.GetComponent<BackgroundTile>().row = j;
                    allTiles[i, j] = backgroundTile;

                    // arrangedTilesZero, one and two represents obstacles that inside of each other. For example, if we put wheatfarm inside of
                    // strawbale then we must put strawbale to arrangedTilesZero and wheatfarm to arrangedTilesOne so basically arrangedTilesZero will 
                    // be front and arrangedTilesOne will be in back and they will break according to this order.

                    // For, trying to make more standart mostly of transparent and filliable obstacles will be put arrangedTilesOne and obstacles that
                    // blocking fruits and other stuff (Block obstacles) will be put arrangedTilesZero.

                    if (arrangedTilesZero[i, j] != -1)
                    {
                        backgroundTile.GetComponent<BackgroundTile>().obstacles[0] = Instantiate(obstaclePrefabs[arrangedTilesZero[i, j]], tempPosition, Quaternion.identity);
                    }

                    if (arrangedTilesOne[i, j] != -1)
                    {
                        backgroundTile.GetComponent<BackgroundTile>().obstacles[1] = Instantiate(obstaclePrefabs[arrangedTilesOne[i, j]], tempPosition, Quaternion.identity);
                    }

                    if (arrangedTilesTwo[i, j] != -1)
                    {
                        backgroundTile.GetComponent<BackgroundTile>().obstacles[2] = Instantiate(obstaclePrefabs[arrangedTilesTwo[i, j]], tempPosition, Quaternion.identity);
                    }

                    // After creating obstacles tile needs to check which obstacle will be break first.

                    backgroundTile.GetComponent<BackgroundTile>().DetectVisibleOne();

                    // If type of fruit -1 then it means fruit does not exist in this cell.
                    if (arrangedFruits[i, j] >= 0)
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
            // Inserting fallPoint to array
            columnsFallIndexY[i] = fallPoint;
        }
    }

    /// <summary>
    /// In swipe action this function will call from the swiped object script. This functions checks if swipe is possible. If it is possible then calls
    /// checkMove function for if the move is valid. Which is not then it revert it with inside of that function. 
    /// </summary>
    /// <param name="swipeAngle"></param>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public void SwipeFruits(float swipeAngle, int column, int row)
    {
        if (taskController.moveCount < 1 || !taskController.isBoardActive)
        {
            return;
        }

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
            if (FruitAvailable(allFruits[column + 1, row]))
            {
                otherFruit = allFruits[column + 1, row];
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
            if (FruitAvailable(allFruits[column, row + 1]))
            {
                otherFruit = allFruits[column, row + 1];
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
            if (FruitAvailable(allFruits[column - 1, row]))
            {
                otherFruit = allFruits[column - 1, row];
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
            if (FruitAvailable(allFruits[column, row - 1]))
            {
                otherFruit = allFruits[column, row - 1];
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

    /// <summary>
    /// After move played or some match happend then this function called by other function or itself depends on situation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckAndDestroyMatches()
    {
        // List of fruits going to be popped.
        List<GameObject> fruitsCheck = new List<GameObject>();
        checkingMatch = true;
        yield return null;
        popped = false;
        hintBool = false;

        // For, achivement progress it contains type of fruits popped count.  
        int[] typeFruits = new int[fruits.Length];

        // Check for matches in columns
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                List<GameObject> fruitsCheckTemp = new List<GameObject>();
                bool same = true;
                int k = 0;
                bool rowPopped = false, columnPopped = false, squarePopped = false;
                // it checking if bottom piece and one above it same. In every cycle it adds the one is currently below checking and if two of them not same
                // then its out of the  cycle. Checking if the space is null or exist.
                if (FruitAvailable(allFruits[i, j]) && allFruits[i, j].GetComponent<Fruit>().fruitType >= 0)
                {
                    // Column
                    while (same)
                    {
                        if (j + k + 1 >= height || !FruitAvailable(allFruits[i, j + k + 1]) || allFruits[i, j + k].GetComponent<Fruit>().fruitType != allFruits[i, j + k + 1].GetComponent<Fruit>().fruitType)
                        {
                            same = false;
                        }
                        fruitsCheckTemp.Add(allFruits[i, j + k]);
                        k++;
                    }
                    same = true;
                    // If there is a more than 2 dual matches then it adds the fruits to main pop list. There is no need to check if there is a same
                    // fruits inside in main list because this is first filling of main list.
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

                    // Same goes for ROW
                    while (same)
                    {
                        if (i + k + 1 >= width || !FruitAvailable(allFruits[i + k + 1, j]) || allFruits[i + k, j].GetComponent<Fruit>().fruitType != allFruits[i + k + 1, j].GetComponent<Fruit>().fruitType)
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
                        // It adds the fruits to main pop list (adding the just diffirent ones). 
                        fruitsCheck.AddRange(fruitsCheckTemp.Except(fruitsCheck).ToList());
                    }

                    int type;

                    // Checking Square match

                    if (j + 1 < height && FruitAvailable(allFruits[i, j]) && FruitAvailable(allFruits[i, j + 1]) && (type = allFruits[i, j].GetComponent<Fruit>().fruitType) == allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                    {
                        if (i + 1 < width && FruitAvailable(allFruits[i + 1, j]) && FruitAvailable(allFruits[i + 1, j + 1]) && allFruits[i + 1, j].GetComponent<Fruit>().fruitType
                        == allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && type == allFruits[i + 1, j].GetComponent<Fruit>().fruitType)
                        {
                            fruitsCheckTemp.Add(allFruits[i, j]);
                            fruitsCheckTemp.Add(allFruits[i, j + 1]);
                            fruitsCheckTemp.Add(allFruits[i + 1, j]);
                            fruitsCheckTemp.Add(allFruits[i + 1, j + 1]);
                            squarePopped = true;
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
                        for (int e = 0; e < fruitsCheck.Count; e++)
                        {
                            DestroyController(fruitsCheck[e], true);
                        }
                        if (fruitsCheck.Count > 3)
                        {
                            // Creating Power Up according to shape of match.

                            GameObject fruitToChange = fruitsCheck[UnityEngine.Random.Range(0, fruitsCheck.Count)];
                            int row = fruitToChange.GetComponent<Fruit>().row;
                            int column = fruitToChange.GetComponent<Fruit>().column;
                            if (rowPopped && columnPopped)
                            {
                                //  CreatePowerUp(column, row, -3);
                            }
                            else if (rowPopped)
                            {
                                CreatePowerUp(column, row, -1);
                                audioManager.PowerUpGain();

                            }
                            else if (columnPopped)
                            {
                                CreatePowerUp(column, row, -2);
                                audioManager.PowerUpGain();

                            }
                            else if (squarePopped)
                            {
                                CreatePowerUp(column, row, -3);
                                audioManager.PowerUpGain2();
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
            hintBool = true;
        }
        checkingMatch = false;

    }

    /// <summary>
    /// Basically checks if fruits exist and not in a move to somewhere.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool FruitAvailable(GameObject obj)
    {
        if (obj && Vector2.Distance(obj.GetComponent<Fruit>().targetV, obj.transform.position) < 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the played move possible if not it reverts move.
    /// </summary>
    /// <param name="fruit"></param>
    /// <param name="otherFruit"></param>
    /// <returns></returns>
    private IEnumerator CheckMove(GameObject fruit, GameObject otherFruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();
        bool succesfulMove = false;

        // If two object are power ups then merge happens.

        if (fruitScript.fruitType < 0 && otherFruitScript.fruitType < 0)
        {
            // Selected power up moves towards to other power up
            fruitScript.row = otherFruitScript.row;
            fruitScript.column = otherFruitScript.column;
            fruitScript.targetV = otherFruitScript.targetV;

            ActivateMergePowerUp(fruit, otherFruit);
            succesfulMove = true;

        }
        else
        {
            // If one of them is power up then they switch and power up activate.
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
                yield return new WaitForSeconds(0.1f);

                succesfulMove = IsSuccesfulMove(fruit, otherFruit);
            }
        }


        if (succesfulMove)
        {
            // If special swipe happen then its not count as move.
            if (!specialSwipe)
            {
                taskController.MovePlayed();
            }
            else
            {
                specialSwipe = false;
            }

        }
        else
        {
            swipeHint.oneHintActive = false;
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

    /// <summary>
    /// It returns moves succesfullnes.
    /// </summary>
    /// <param name="fruit"></param>
    /// <param name="otherFruit"></param>
    /// <returns></returns>
    private bool IsSuccesfulMove(GameObject fruit, GameObject otherFruit)
    {
        if (!fruit || !otherFruit)
        {
            return true;
        }

        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();

        if (specialSwipe)
        {
            return true;
        }

        if (CheckMatchSides(fruitScript.row, fruitScript.column) || CheckMatchSides(otherFruitScript.row, otherFruitScript.column))
        {
            // Checked if this is right move
            return true;
        }
        else
        {
            if (!fruit || !otherFruit || fruitScript.fadeout || otherFruitScript.fadeout)
            {

                return true;

            }

        }
        return false;
    }

    /// <summary>
    /// Changing two fruit loc and info. 
    /// </summary>
    /// <param name="fruit"></param>
    /// <param name="otherFruit"></param>
    private void ChangeTwoFruit(GameObject fruit, GameObject otherFruit)
    {

        Fruit fruitScript = fruit.GetComponent<Fruit>();
        int tempRow = fruitScript.row, tempCol = fruitScript.column;
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();

        fruitScript.targetV = allTiles[otherFruitScript.column, otherFruitScript.row].transform.position;
        otherFruitScript.targetV = allTiles[tempCol, tempRow].transform.position;

        fruitScript.row = otherFruitScript.row;
        fruitScript.column = otherFruitScript.column;

        otherFruitScript.row = tempRow;
        otherFruitScript.column = tempCol;

    }

    /// <summary>
    /// After move played if fruits didnt popped then it means either this was not possible move or CheckAndDestroyMatches function didnt catch the 
    /// match yet. So, this function checks if the move is valid move. Simply does the CheckAndDestroyMatches job but much faster and just for that 
    /// location
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private bool CheckMatchSides(int row, int column)
    {
        /* Checking row of the object by starting 2 left from object. Function works like this;
         1. First if index of check exist
         2. The objects that going to work with exist
         3. If two object type same then it incrase the "match" number else "match" number will be zero.
         4.  If match number 2 then it means there is a match function return true.
        */


        int match = 0;

        for (int k = -2; k < 2; k++)
        {
            if (column + k >= 0 && column + k + 1 < width && allFruits[column + k, row] && allFruits[column + k + 1, row])
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

    /// <summary>
    /// Object will be fade away slowly before destroye.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public IEnumerator FadeOut(GameObject obj)
    {

        float elapsedTime = 0f;
        float fadeDuration = 0.25f;
        Color color = obj.GetComponentInChildren<SpriteRenderer>().color;

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

    /// <summary>
    /// General filling function. Every column has their FillTheColumn function and this function calls them in a for loop.
    /// </summary>
    /// <returns></returns>
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
        yield return new WaitForSeconds(0.2f);
        if (!checkingMatch)
        {
            StartCoroutine(CheckAndDestroyMatches());
        }
        StartCoroutine(FillTheGaps());
    }

    /// <summary>
    /// Filling the specifing column. Adding empty places to queue and making fall fruits that floating. After all empty places on top then creating
    /// fruits and making fall to empty places. Also, if there is a missing tile or block obstacle top of empty place then empty place queue resets and 
    /// CrossFall function will be call to fill that empty place.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public IEnumerator FillTheColumn(int i)
    {
        fillingColumn[i] = true;

        Queue<int> emptyPlaces = new Queue<int>();

        for (int j = 0; j <= columnsFallIndexY[i]; j++)
        {
            if (!allTiles[i, j] || allTiles[i, j].GetComponent<BackgroundTile>().isCurrentObstacleBox)
            {
                emptyPlaces.Clear();
            }
            else
            {

                if (!allFruits[i, j])
                {
                    // Putting empty place index to variable
                    emptyPlaces.Enqueue(j);

                    // THESE CODES CAN BE IMPROVE PLEASE CHECK THE ALGO
                    if((j + 1 < height && !allTiles[i, j + 1]) || (j + 2 < height && !allTiles[i, j + 2]))
                    {
                        StartCoroutine(CrossFall(i, j + 1));

                    }
                    else
                    {
                        if ((j + 1 < height && allTiles[i, j + 1] && allTiles[i, j + 1].GetComponent<BackgroundTile>().isCurrentObstacleBox) || (j + 2 < height && allTiles[i, j + 2] && allTiles[i, j + 2].GetComponent<BackgroundTile>().isCurrentObstacleBox))
                        {
                            StartCoroutine(CrossFall(i, j + 1));

                        }
                    }
                   
                }
                else if (emptyPlaces.Count > 0)
                {
                    // if there is a piece then piece new location will be first empty place in queue.
                    audioManager.FruitFall();
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
        }

        while (emptyPlaces.Count > 0)
        {
            float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
            float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
            int emptyRowIndex = emptyPlaces.Dequeue();
            Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, columnsFallIndexY[i]+1 * scaleNumber - yOffset);

            // Instantiate a new fruit at the position of the destroyed fruit. Fruit that going to be created must be from existFruits variable. existFruits
            // list contains indexes of avaliable fruits.
            int fruitToUse = existFruits[UnityEngine.Random.Range(0, existFruits.Count)];
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

            audioManager.FruitFall();
            // Add the new fruit to the allFruits array
            yield return new WaitForSeconds(0.1f);
        }
        fillingColumn[i] = false;

    }

    /// <summary>
    /// Powerups same as fruits but just their type is negative numbers so that when they popped they will act like powerup according to their type.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="type"></param>
    private void CreatePowerUp(int column, int row, int type)
    {
        /*
        Power Up Type Number;
        
        -1 : Horizantal Harvester
        -2 : Vertical Harvester
        -3 : TNT
         */

        int powerUpCreate = PlayerPrefs.GetInt("SpecialFruitTask", 0);

        powerUpCreate++;

        PlayerPrefs.SetInt("SpecialFruitTask", powerUpCreate);
        PlayerPrefs.Save();

        float xOffset = width * 0.5f - 0.5f;
        float yOffset = height * 0.5f - 0.5f;
        Vector2 tempPosition = new Vector2(column - xOffset, height - yOffset);

        GameObject newPowerUp = Instantiate(powerUps[(type * -1) - 1], tempPosition, powerUps[(type * -1) - 1].transform.rotation);
        Fruit newPowerUpScript = newPowerUp.GetComponent<Fruit>();

        // Set the parent and name of the new powerup
        newPowerUp.transform.parent = this.transform;
        newPowerUp.name = "( " + column + ", " + row + " )";

        // Set the column and row of the new powerup
        newPowerUpScript.column = column;
        newPowerUpScript.row = row;
        newPowerUpScript.fruitType = type;
        newPowerUp.gameObject.transform.position = allTiles[column, row].transform.position;
        newPowerUpScript.targetV = allTiles[column, row].transform.position;


        // Add the new powerup to the allFruits array
        allFruits[column, row] = newPowerUp;
    }

    /// <summary>
    /// If the empty place that has an obstacle or missing tile on top of itself then this fuction steals a fruit from other column. First checks left
    /// column if not possible than checks right column for stealing fruit.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private IEnumerator CrossFall(int column, int row)
    {
        GameObject fruit = null;
        Fruit fruitScript;
        yield return new WaitForSeconds(0.5f);
        if (column - 1 >= 0 && FruitAvailable(allFruits[column - 1, row]) && !allFruits[column, row - 1])
        {
            fruit = allFruits[column - 1, row];
            allFruits[column - 1, row] = null;
        }
        else if (column + 1 < width && FruitAvailable(allFruits[column + 1, row]) && !allFruits[column, row - 1])
        {
            fruit = allFruits[column + 1, row];
            allFruits[column + 1, row] = null;
        }

        if (fruit)
        {
            audioManager.FruitFall();
            fruitScript = fruit.GetComponent<Fruit>();
            fruitScript.row = row - 1;
            fruitScript.column = column;
            fruitScript.targetV = allTiles[column, row - 1].transform.position;
        }

    }

    /// <summary>
    /// If one powerup swiped with other powerup than there will be a merge and this function will be called. It will call necessary functions 
    /// according to types of this two powerups.
    /// </summary>
    /// <param name="fruit"></param>
    /// <param name="otherFruit"></param>
    public void ActivateMergePowerUp(GameObject fruit, GameObject otherFruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>(), otherFruitScript = otherFruit.GetComponent<Fruit>();
        if (otherFruitScript.fruitType == fruitScript.fruitType)
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
                case -3:
                    fruitScript.fadeout = true;
                    StartCoroutine(FadeOut(fruit));
                    TNTExplosion(otherFruitScript.column, otherFruitScript.row, 2);
                    break;
            }

        }
        else
        {

            // If two power up diffirent it goes here

            switch (fruitScript.fruitType)
            {
                // Horizontal Harvester
                case -1:
                    switch (otherFruitScript.fruitType)
                    {
                        case -2:
                            ActivatePowerUp(fruit);
                            ActivatePowerUp(otherFruit);
                            break;
                        case -3:
                            Destroy(otherFruit);
                            if (fruitScript.row + 1 < height)
                            {
                                fruitScript.row++;
                                ActivatePowerUp(fruit);
                                fruitScript.row--;
                            }
                            ActivatePowerUp(fruit);
                            if (fruitScript.row - 1 >= 0)
                            {
                                fruitScript.row--;
                                ActivatePowerUp(fruit);
                            }
                            break;
                    }
                    break;
                // Vertical Harvester
                case -2:
                    switch (otherFruitScript.fruitType)
                    {
                        case -1:
                            ActivatePowerUp(fruit);
                            ActivatePowerUp(otherFruit);
                            break;
                        case -3:
                            Destroy(otherFruit);
                            if (fruitScript.column - 1 >= 0)
                            {
                                fruitScript.column--;
                                ActivatePowerUp(fruit);
                                fruitScript.column++;
                            }
                            ActivatePowerUp(fruit);
                            if (fruitScript.column + 1 < width)
                            {
                                fruitScript.column++;
                                ActivatePowerUp(fruit);
                            }
                            break;
                    }
                    break;
                // TNT 
                case -3:
                    switch (otherFruitScript.fruitType)
                    {
                        case -1:
                            Destroy(fruit);
                            if (otherFruitScript.row + 1 < height)
                            {
                                otherFruitScript.row++;
                                ActivatePowerUp(otherFruit);
                                otherFruitScript.row--;
                            }
                            ActivatePowerUp(otherFruit);
                            if (otherFruitScript.row - 1 >= 0)
                            {
                                otherFruitScript.row--;
                                ActivatePowerUp(otherFruit);
                            }
                            break;
                        case -2:
                            Destroy(fruit);
                            if (otherFruitScript.column - 1 >= 0)
                            {
                                otherFruitScript.column--;
                                ActivatePowerUp(otherFruit);
                                otherFruitScript.column++;
                            }
                            ActivatePowerUp(otherFruit);
                            if (otherFruitScript.column + 1 < width)
                            {
                                otherFruitScript.column++;
                                ActivatePowerUp(otherFruit);
                            }
                            break;
                    }
                    break;
            }
        }

    }

    /// <summary>
    /// Activating powerup according to type.
    /// </summary>
    /// <param name="fruit"></param>
    public void ActivatePowerUp(GameObject fruit)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        fruitScript.fadeout = true;
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
                audioManager.Harvester();

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
                audioManager.Harvester();
                break;
            // TNT power up
            case -3:
                TNTExplosion(column, row, 1);
                audioManager.Pickaxe();
                break;

        }
    }

    /// <summary>
    /// Destroying objects in a control. If object already fading out there is no need to destroy it. If explosion is true then it will destroy nearby
    /// block obstacles.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="explosion"></param>
    private void DestroyController(GameObject obj, bool explosion)
    {

        if (obj.GetComponent<Fruit>().fadeout)
        {
            return;
        }
        else
        {
            obj.GetComponent<Fruit>().fadeout = true;
            // If object is not a powerup than it will be fadeout. Power ups will fadeout by their cycle. If obj do not have script than it means its
            // straw bale or something like that
            if (obj.GetComponent<Fruit>().fruitType >= 0)
            {
                if (explosion)
                {
                    int row = obj.GetComponent<Fruit>().row;
                    int column = obj.GetComponent<Fruit>().column;

                    allTiles[column, row].GetComponent<BackgroundTile>().Explosion(column, row);
                }
                StartCoroutine(FadeOut(obj));
            }
            else
            {
                ActivatePowerUp(obj);
            }
        }
    }

    /// <summary>
    /// Explosing depends on tnt power. It used from normal tnt and merged tnt. 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="range"></param>
    private void TNTExplosion(int column, int row, int range)
    {
        allFruits[column, row].GetComponent<Fruit>().fadeout = true;
        StartCoroutine(FadeOut(allFruits[column, row]));
        for (int i = column - range; i <= column + range; i++)
        {
            for (int j = row - range; j <= row + range; j++)
            {
                if (j >= 0 && j < height && i >= 0 && i < width)
                {
                    if (allFruits[i, j])
                    {
                        DestroyController(allFruits[i, j], false);
                        audioManager.FruitCrush();
                    }
                    if (allTiles[i, j])
                    {
                        allTiles[i, j].GetComponent<BackgroundTile>().Boom();
                    }

                }
            }
        }
    }

    /// <summary>
    /// Horizontal Harvester move left and right so clone goes right and real one goes left. 
    /// </summary>
    /// <param name="harvester"></param>
    /// <param name="clone"></param>
    private void HorizontalHarvesterMove(GameObject harvester, bool clone)
    {
        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
      //  float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;

       // Vector2 tempPosition = new Vector2((width-1) * scaleNumber - xOffset, (height-1) * scaleNumber - yOffset);

        harvester.GetComponent<Collider2D>().enabled = false;
        Fruit harvesterScript = harvester.GetComponent<Fruit>();
        //harvesterScript.speed = 0.04f;
        int row = harvesterScript.row, column = harvesterScript.column;
        if (clone)
        {
            harvesterScript.targetV.x = ((width - 1) * scaleNumber - xOffset)+1;

            HorizontalDestroy(column, row, true);
        }
        else
        {
            harvesterScript.targetV.x = -xOffset - 1;

            HorizontalDestroy(column, row, false);

        }
        if (allTiles[column, row])
        {
            allTiles[column, row].GetComponent<BackgroundTile>().Boom();
        }
        StartCoroutine(FadeOut(harvester));

    }

    /// <summary>
    /// Vertical Harvester move up and down so clone goes down and real one goes up. 
    /// </summary>
    /// <param name="harvester"></param>
    /// <param name="clone"></param>
    public void VerticalHarvesterMove(GameObject harvester, bool clone)
    {
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        harvester.GetComponent<Collider2D>().enabled = false;
        Fruit harvesterScript = harvester.GetComponent<Fruit>();
        int row = harvesterScript.row, column = harvesterScript.column;
        if (clone)
        {
            //  harvesterScript.targetV.y = allTiles[column, 0].transform.position.y - 1;
            harvesterScript.targetV.y = (height - 1) * scaleNumber - yOffset - 1;

            VerticalDestroy(column, row, false);
        }
        else
        {
        //    harvesterScript.targetV.y = allTiles[column, height - 1].transform.position.y + 1;
            harvesterScript.targetV.y =  - yOffset - 1;

            VerticalDestroy(column, row, true);        

        }
        if (allTiles[column, row])
        {
            allTiles[column, row].GetComponent<BackgroundTile>().Boom();
        }
        StartCoroutine(FadeOut(harvester));

    }

    /// <summary>
    /// Destroy one side of vertical from the point. If up boolean true then it destroy up of the point and if not down. This function
    /// currently used by vertical special power and vertical harvester.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="up"></param>
    private void VerticalDestroy(int column, int row, bool up)
    {
        if (up)
        {
            for (int i = row + 1; i < height; i++)
            {
                if (allFruits[column, i])
                {
                    DestroyController(allFruits[column, i], false);
                    audioManager.FruitCrush();
                }
                if (allTiles[column, i])
                {
                    allTiles[column, i].GetComponent<BackgroundTile>().Boom();
                }
            }
        }
        else
        {
            for (int i = row - 1; i >= 0; i--)
            {
                if (allFruits[column, i])
                {
                    DestroyController(allFruits[column, i], false);
                    audioManager.FruitCrush();
                }
                if (allTiles[column, i])
                {
                    allTiles[column, i].GetComponent<BackgroundTile>().Boom();
                }
            }
        }
    }

    /// <summary>
    /// Destroy one side of horizontal from the point. If right boolean true then it destroy right side of the point and if not left side. This function
    /// currently used by horizontal special power and horizontal harvester.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="right"></param>
    private void HorizontalDestroy(int column, int row, bool right)
    {
        if (right)
        {
            for (int i = column + 1; i < width; i++)
            {

                if (allFruits[i, row])
                {
                    DestroyController(allFruits[i, row], false);
                    audioManager.FruitCrush();

                }
                if (allTiles[i, row])
                {
                    allTiles[i, row].GetComponent<BackgroundTile>().Boom();
                }

            }
        }
        else
        {
            for (int i = column - 1; i >= 0; i--)
            {

                if (allFruits[i, row])
                {
                    DestroyController(allFruits[i, row], false);
                    audioManager.FruitCrush();
                }
                if (allTiles[i, row])
                {
                    allTiles[i, row].GetComponent<BackgroundTile>().Boom();
                }
            }
        }
    }

    /// <summary>
    /// Destroy just one tile.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    private void DestroyOneTile(int column, int row)
    {
        if (allFruits[column, row])
        {
            DestroyController(allFruits[column, row], false);
        }
        audioManager.FruitCrush();
        if (allTiles[column, row])
        {
            allTiles[column, row].GetComponent<BackgroundTile>().Boom();
        }

    }

    /// <summary>
    /// This function activate selected special power. 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public void ActivateSpecialPower(int column, int row)
    {
        switch (specialPowerID)
        {
            case 1:
                // Special Power: Vertical Destroyer
                specialPowerController.SpecialPowerUpUsed(0);
                DestroyOneTile(column, 0);
                VerticalDestroy(column, 0, true);
                break;
            case 2:
                // Special Power: Horizontal Destroyer
                specialPowerController.SpecialPowerUpUsed(1);
                DestroyOneTile(0, row);
                HorizontalDestroy(0, row, true);
                break;
            case 3:
                // Special Power: One Tile Destroyer
                specialPowerController.SpecialPowerUpUsed(2);
                DestroyOneTile(column, row);
                break;
            case 4:
                // Special Power: Super Swipe
                specialPowerController.SpecialPowerUpUsed(3);
                break;
        }
        specialPowerID = 0;
    }

    /// <summary>
    /// This function will call from Special Power Controller script and related to Special Power buttons. specialPowerID represent selected special power.
    /// According to ability some of the arrangment wll be made.
    /// </summary>
    /// <param name="id"></param>
    public void SelectedSpecialPower(int id)
    {
        specialPowerID = id;


        switch (specialPowerID)
        {
            // Special Power: Super Swipe
            case 4:
                specialSwipe = true;
                break;
        }
    }

    public void DisableSpecialPowers()
    {
        specialPowerID = 0;
        specialSwipe = false;
    }

}

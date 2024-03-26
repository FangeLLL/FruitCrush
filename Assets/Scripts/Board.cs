using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEditor.SearchService;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    private float timer = 0f;
    private float waitTime = 2f;

    [SerializeField]
    public AchievementManager achievementManager;
    public TaskController taskController;
    public PowerUpController powerUpController;
    public SwipeHint swipeHint;
    public SaveData saveData;
    public SpecialPowerController specialPowerController;

    public bool isRunning = false;
    public bool exitUpdate = false;

    AudioManager audioManager;
    Animator fruitAnimator;
    Animator fruitAnimator2;
    Animator fruitAnimator3;
    Animator fruitAnimator4;
    Animator fruitAnimator5;

    public GameObject selectedFruit;

    [SerializeField]
    private Sprite[] tileSprites;

    [SerializeField]
    private GameObject[] specialPowerUps;

    public Sprite harvesterUpSprite, harvesterDownSprite;

    [SerializeField]
    private GameObject[] fruits;
    [SerializeField]
    private GameObject[] powerUps;
    public GameObject[] obstaclePrefabs;
    public GameObject tilePrefab;

    public bool hintBool = false;
    bool popped = false;

    IndexLibrary indexLibrary = new IndexLibrary();

    List<int> existFruits = new List<int>();

    public int userLevel;

    public bool blockUserMove = false;

    public GameObject[,] allFruits;
    public GameObject[,] allTiles;
    public float scaleNumber;
    private float scaleFactorFruit;

    private int[] columnsFallIndexY;
    private string[] columnStopperId; 

    private int indexOfCreatableObstacle=-1;

    private Vector3[,] tilePositions;

    // When fruits swiped these gameobjects fills and used for creation powerup positions 
    private GameObject currentFruit, currentOtherFruit;

    // Each index represents related id of an fruit. 
    public int[] totalNumberOfFruits;

    public int specialPowerID = 0;
    public bool specialSwipe = false;
    public bool specialPowerUsing = false;

    private bool shuffling = false;

    [SerializeField]
    private GameObject lighteningPrefab;

    private int isBoomerangCreated = Animator.StringToHash("isBoomerangCreated");
    private int is2BoomerangMerged = Animator.StringToHash("is2BoomerangMerged");
    private int boomerangRotating = Animator.StringToHash("isRotating");

    private int isHarvesterCreated = Animator.StringToHash("isHarvesterCreated");
    private int isHarvesterRunning = Animator.StringToHash("isHarvesterRunning");

    private int isTNTCreated = Animator.StringToHash("isTNTCreated");
    private int isTNTMerged = Animator.StringToHash("isTNTMerged");

    private int isDiscoBallCreated = Animator.StringToHash("isDiscoBallCreated");
    private int isDiscoBallUsed = Animator.StringToHash("isDiscoBallUsed");

    private int isFruitDestroyed = Animator.StringToHash("isFruitDestroyed");

    private int isHarvesterMergedBoomerang = Animator.StringToHash("isHarvesterMergedBoomerang");
    private int isTNTMergedBoomerang = Animator.StringToHash("isTNTMergedBoomerang");

    private int TNTHarvesterVerticalMerge = Animator.StringToHash("TNTHarvesterVerticalMerge");
    private int TNTHarvesterHorizontalMerge = Animator.StringToHash("TNTHarvesterHorizontalMerge");

    ObjectSpeedAndTimeWaitingLibrary speedAndTimeLibrary = new ObjectSpeedAndTimeWaitingLibrary();

    private void Awake()
    {
        saveData.LoadFromJson();

    }

    void Start()
    {
        Application.targetFrameRate = 60;
        //Time.timeScale = 0.5f;

        userLevel = PlayerPrefs.GetInt("level", 0);

        Grid gridData = saveData.gridData[userLevel];

        width = gridData.width;
        height = gridData.height;

        totalNumberOfFruits = new int[fruits.Length];

        // Some of columns falling point can be diffirent because of missing tiles in that column. 
        columnsFallIndexY = new int[width];
        columnStopperId = new string[width];

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


        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
        tilePositions = new Vector3[width, height];

        int[] savedTiles = gridData.allTilesTotal;
        int[] savedFruits = gridData.allFruitsTotal;
        int[] savedTilesZero = gridData.tilesIndexZero;
        int[] savedTilesOne = gridData.tilesIndexOne;
        int[] savedTilesTwo = gridData.tilesIndexTwo;

        taskController.SetTask(gridData.taskElements);
        taskController.SetMoveCount(gridData.moveCount);

        if (gridData.level == 0)
        {
            Debug.Log("This level did not made yet. Please, don't assume that this is a some sort of bug and tell Bertu� to fix it, just make this level in Level Editor.");
        }
        else
        {
            SetUpWithArray(indexLibrary.Convert2DTo3D(width, height, savedFruits), indexLibrary.Convert2DTo3D(width, height, savedTiles), indexLibrary.Convert2DTo3D(width, height, savedTilesZero), indexLibrary.Convert2DTo3D(width, height, savedTilesOne), indexLibrary.Convert2DTo3D(width, height, savedTilesTwo));
        }
        if(swipeHint.fruit)
            fruitAnimator = swipeHint.fruit.GetComponentInChildren<Animator>();
        if (swipeHint.fruit2)
            fruitAnimator2 = swipeHint.fruit2.GetComponentInChildren<Animator>();
        if (swipeHint.fruit3)
            fruitAnimator3 = swipeHint.fruit2.GetComponentInChildren<Animator>();
        if (swipeHint.fruit4)
            fruitAnimator4 = swipeHint.fruit3.GetComponentInChildren<Animator>();
        if (swipeHint.fruit5)
            fruitAnimator5 = swipeHint.fruit4.GetComponentInChildren<Animator>();


    }

    /// <summary>
    /// Arranging the general scale variable of prefabs according to size of board.
    /// </summary>
    private void RearrangeScaleNumber()
    {
        scaleNumber = 1.19f;
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
                    tilePositions[i, j] = backgroundTile.transform.position;
                }
            }
            // Inserting fallPoint to array
            columnsFallIndexY[i] = fallPoint;
        }


        // Creating obstacles and fruits.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j])
                {
                    allTiles[i, j].GetComponent<SpriteRenderer>().sprite = TileWithBorder(i, j);
                 //   Debug.Log(Array.IndexOf(tileSprites, TileWithBorder(i, j)));
                }

                if (arrangedTiles[i, j] == 1)
                {
                    GameObject tempObstacle = null;
                    Vector2 tempPosition = allTiles[i, j].transform.position;

                    int repeat = 1;

                    int arrangedTilesIndex = 0;

                    for (int c = 0; c < 3; c++)
                    {
                        // Checking every layer because system will put obstacles according to layer ids.
                        switch (c)
                        {
                            case 0:
                                arrangedTilesIndex = arrangedTilesZero[i, j];
                                break;
                            case 1:
                                arrangedTilesIndex = arrangedTilesOne[i, j];
                                break;
                            case 2:
                                arrangedTilesIndex = arrangedTilesTwo[i, j];
                                break;
                        }

                        // arrangedTilesZero, one and two represents obstacles that inside of each other. For example, if we put wheatfarm inside of
                        // strawbale then we must put strawbale to arrangedTilesZero and wheatfarm to arrangedTilesOne so basically arrangedTilesZero will 
                        // be front and arrangedTilesOne will be in back and they will break according to this order.

                        // For, trying to make more standart mostly of transparent and filliable obstacles will be put arrangedTilesOne and obstacles that
                        // blocking fruits and other stuff (Block obstacles) will be put arrangedTilesZero.

                        // Checking if layer has obstacle and that tile has a obstacle in its that layer. 
                        if (arrangedTilesIndex != -1 && !allTiles[i, j].GetComponent<BackgroundTile>().obstacles[c])
                        {
                            // If obstacle is big obstacle then it will arrange position and assign this object 4 tile. 
                            if (obstaclePrefabs[arrangedTilesIndex].GetComponent<ObstacleScript>().obstacleSpecs.is4TimesBigger)
                            {
                                tempPosition = new Vector2((i + 0.5f) * scaleNumber - xOffset, (j + 0.5f) * scaleNumber - yOffset);
                                repeat = 2;
                            }
                            else
                            {
                                tempPosition = allTiles[i, j].transform.position;
                                repeat = 1;
                            }

                            tempObstacle = Instantiate(obstaclePrefabs[arrangedTilesIndex], tempPosition, Quaternion.identity);

                            tempObstacle.GetComponent<ObstacleScript>().row = j;
                            tempObstacle.GetComponent<ObstacleScript>().column = i;

                            if (tempObstacle.GetComponent<ObstacleScript>().obstacleSpecs.creatableOnPlay)
                            {
                                indexOfCreatableObstacle = arrangedTilesIndex;
                            }

                            if (tempObstacle.GetComponent<ObstacleScript>().obstacleSpecs.isMovable)
                            {
                                tempObstacle.GetComponent<Fruit>().column = i;
                                tempObstacle.GetComponent<Fruit>().row = j;
                                allFruits[i, j] = tempObstacle;
                            }
                            else
                            {
                                for (int a = 0; a < repeat; a++)
                                {
                                    for (int b = 0; b < repeat; b++)
                                    {
                                        // Assigning object to relevant tile/tiles.
                                        allTiles[i + b, j + a].GetComponent<BackgroundTile>().obstacles[c] = tempObstacle;
                                        // After creating obstacles tile needs to check which obstacle will be break first.
                                        allTiles[i + b, j + a].GetComponent<BackgroundTile>().DetectVisibleOne();
                                    }
                                }
                            }
                           
                        }
                    }

                    // If type of fruit -1 then it means fruit does not exist.
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
                        totalNumberOfFruits[fruitToUse]++;
                    }

                }

            }
        }
        CheckForStarterPowerUps();
        for (int i = 0; i < width; i++)
        {
            StartCoroutine(FillTheColumn(i));
        }
        StartCoroutine(CheckAndDestroyMatches());
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

        if (blockUserMove || !allFruits[column, row])
        {
            return;
        }

        //StopCoroutine(GiveHint());
        StopHint();

        StopHintAnimations();

        currentFruit = null;
        currentOtherFruit = null;

        bool horizontalSwipe=true;

        GameObject otherFruit=null;
        GameObject fruit = allFruits[column, row];
        GameObject targetTile=null;
        if (fruit.GetComponent<Fruit>().isSwiped)
        {
            Debug.Log("This fruit already swiping");
            return;
        }
        if (swipeAngle > -45 && swipeAngle <= 45 && column + 1 < width)
        {

            // RIGHT SWIPE
            if (FruitAvailableWithoutTypeCheck(allFruits[column + 1, row]))
            {
                otherFruit = allFruits[column + 1, row];
            }
            else
            {
                if(allTiles[column + 1, row] && !allFruits[column + 1, row])
                {
                    targetTile = allTiles[column + 1, row];
                }
                else
                {
                    Debug.Log("You cant swipe right!!!");
                    return;
                }        
            }

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row + 1 < height)
        {
            // UP SWIPE
            if (FruitAvailableWithoutTypeCheck(allFruits[column, row + 1]))
            {
                horizontalSwipe = false;
                otherFruit = allFruits[column, row + 1];
            }
            else
            {
                if (allTiles[column, row + 1] && !allFruits[column, row + 1])
                {
                    targetTile = allTiles[column, row + 1];
                }
                else
                {
                    Debug.Log("You cant swipe up!!!");
                    return;
                }
            }

        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // LEFT SWIPE
            if (FruitAvailableWithoutTypeCheck(allFruits[column - 1, row]))
            {
                otherFruit = allFruits[column - 1, row];
            }
            else
            {
                if (allTiles[column - 1, row] && !allFruits[column - 1, row])
                {
                    targetTile = allTiles[column - 1, row];
                }
                else
                {
                    Debug.Log("You cant swipe left!!!");
                    return;
                }
            }

        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // DOWN SWIPE
            if (FruitAvailableWithoutTypeCheck(allFruits[column, row - 1]))
            {
                horizontalSwipe = false;
                otherFruit = allFruits[column, row - 1];
            }
            else
            {
                if (allTiles[column, row - 1] && !allFruits[column, row - 1])
                {
                    targetTile = allTiles[column, row - 1];
                }
                else
                {
                    Debug.Log("You cant swipe down!!!");
                    return;
                }
            }
        }
        else
        {
            audioManager.SwipeResistBorder();
            return;
        }

        if (otherFruit)
        {
            if (otherFruit.GetComponent<Fruit>().isSwiped)
            {
                return;
            }

            if (otherFruit.GetComponent<Fruit>().fruitType == fruit.GetComponent<Fruit>().fruitType && specialSwipe)
            {
                Debug.Log("You cant swipe same type of fruits with specialSwipe");
                return;
            }
            otherFruit.GetComponent<Fruit>().isSwiped = true;
            currentOtherFruit = otherFruit;
        }
        else if (targetTile.GetComponent<BackgroundTile>().obstacles[0])
        {
            Debug.Log("There is a obstacle on the way");
            return;
        }

        fruit.GetComponent<Fruit>().isSwiped = true;

        StopHintAnimations();

        currentFruit = fruit;

        audioManager.Swipe();
        if (otherFruit)
        {
            StartCoroutine(CheckMove(fruit, otherFruit,horizontalSwipe));

        }
        else
        {
            StartCoroutine(CheckMoveForOneFruit(fruit, targetTile));

        }
    }

    /// <summary>
    /// After move played or some match happend then this function called by other function or itself depends on situation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckAndDestroyMatches()
    {
        // List of fruits going to be popped.
        List<GameObject> fruitsCheckTotal = new List<GameObject>();
        yield return null;
        //yield return new WaitForSeconds(0.2f);
        popped = false;
        hintBool = false;

        // Check for matches in columns
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // it checking if bottom piece and one above it same. In every cycle it adds the one is currently below checking and if two of them not same
                // then its out of the  cycle. Checking if the space is null or exist.
                if (FruitAvailable(allFruits[i, j])&& allFruits[i, j].GetComponent<Fruit>().fruitType >= 0)
                {

                    bool rowPopped = false, columnPopped = false, squarePopped = false;

                    List<GameObject> fruitsCheckRow = new List<GameObject>();
                    List<GameObject> fruitsCheckColumn = new List<GameObject>();
                    List<GameObject> fruitsCheckSquare = new List<GameObject>();

                    fruitsCheckRow = RowCheck(i, j);

                    if (fruitsCheckRow.Count >= 3)
                    {
                        List<GameObject> tempFruitsCheckColumn = new List<GameObject>();
                        rowPopped = true;
                        for(int e=0; e < fruitsCheckRow.Count; e++)
                        {
                            tempFruitsCheckColumn = ColumnCheck(fruitsCheckRow[e].GetComponent<Fruit>().column,j);
                            if(tempFruitsCheckColumn.Count >= 3)
                            {
                                fruitsCheckColumn = tempFruitsCheckColumn;
                                columnPopped = true;
                                break;
                            }
                            tempFruitsCheckColumn.Clear();
                        }

                        // If both column and row happend than no need to check square
                        if (!columnPopped && fruitsCheckRow.Count==3)
                        {

                            List<GameObject> tempFruitsCheckSquare = new List<GameObject>();
                            for (int e = 0; e < fruitsCheckRow.Count; e++)
                            {
                                tempFruitsCheckSquare = SquareCheck(fruitsCheckRow[e].GetComponent<Fruit>().column, j);
                                if (tempFruitsCheckSquare.Count == 4)
                                {
                                    fruitsCheckSquare = tempFruitsCheckSquare;
                                    squarePopped = true;
                                    break;
                                }
                                tempFruitsCheckSquare.Clear();
                            }
                        }

                    }
                    else
                    {
                        fruitsCheckRow.Clear();
                        fruitsCheckColumn = ColumnCheck(i,j);
                        if(fruitsCheckColumn.Count >= 3)
                        {
                            List<GameObject> tempFruitsCheckRow = new List<GameObject>();
                            columnPopped = true;
                            for (int e = 0; e < fruitsCheckColumn.Count; e++)
                            {
                                tempFruitsCheckRow = RowCheck(i, fruitsCheckColumn[e].GetComponent<Fruit>().row);
                                if (tempFruitsCheckRow.Count >= 3)
                                {
                                    fruitsCheckRow = tempFruitsCheckRow;
                                    rowPopped = true;
                                    break;
                                }
                                tempFruitsCheckRow.Clear();
                            }

                            // If both column and row happend than no need to check square
                            if (!rowPopped && fruitsCheckColumn.Count == 3)
                            {
                                List<GameObject> tempFruitsCheckSquare = new List<GameObject>();
                                for (int e = 0; e < fruitsCheckColumn.Count; e++)
                                {
                                    tempFruitsCheckSquare = SquareCheck(i, fruitsCheckColumn[e].GetComponent<Fruit>().row);
                                    if (tempFruitsCheckSquare.Count == 4)
                                    {
                                        fruitsCheckSquare = tempFruitsCheckSquare;
                                        squarePopped = true;
                                        break;
                                    }
                                    tempFruitsCheckSquare.Clear();
                                }
                            }                        
                        }
                        else
                        {
                            fruitsCheckSquare = SquareCheck(i, j);
                            if (fruitsCheckSquare.Count == 4)
                            {
                                List<GameObject> tempFruitsCheckRow = new List<GameObject>();
                                for (int e = 0; e < fruitsCheckSquare.Count; e++)
                                {
                                    tempFruitsCheckRow = RowCheck(i, fruitsCheckSquare[e].GetComponent<Fruit>().row);
                                    if (tempFruitsCheckRow.Count >= 3)
                                    {
                                        fruitsCheckRow = tempFruitsCheckRow;
                                        rowPopped = true;
                                        break;
                                    }
                                    tempFruitsCheckRow.Clear();
                                }

                                List<GameObject> tempFruitsCheckColumn = new List<GameObject>();

                                for (int e = 0; e < fruitsCheckSquare.Count; e++)
                                {
                                    tempFruitsCheckColumn = ColumnCheck(fruitsCheckSquare[e].GetComponent<Fruit>().column, j);
                                    if (tempFruitsCheckColumn.Count >= 3)
                                    {
                                        fruitsCheckColumn = tempFruitsCheckColumn;
                                        columnPopped = true;
                                        break;
                                    }
                                    tempFruitsCheckColumn.Clear();
                                }

                                if (fruitsCheckRow.Count < 4 && fruitsCheckColumn.Count < 4)
                                {
                                    squarePopped = true;
                                }
                                else
                                {
                                    fruitsCheckSquare.Clear();
                                }
                            }
                            else
                            {
                                fruitsCheckColumn.Clear();
                            }
                        }
                    }

                    int powerUpID=0;

                    fruitsCheckTotal.AddRange(fruitsCheckRow);
                    fruitsCheckTotal.AddRange(fruitsCheckColumn.Except(fruitsCheckTotal).ToList());
                    fruitsCheckTotal.AddRange(fruitsCheckSquare.Except(fruitsCheckTotal).ToList());

                    if (squarePopped)
                    {
                        // TNT just square
                       
                        powerUpID = -3;
                    }
                    else if (columnPopped || rowPopped)
                    {
                        if (columnPopped && rowPopped)
                        {
                            if ((fruitsCheckColumn.Count == 5 && fruitsCheckRow.Count == 3) || (fruitsCheckRow.Count == 5 && fruitsCheckColumn.Count == 3))
                            {
                                // Big T
                                /*

                                * * * * *
                                    *
                                    *
                                    *

                                  */

                                powerUpID = -5;
                            }
                            else
                            {
                                powerUpID = -4;
                            }                          
                        }
                        else
                        {
                            // 5 line light ball
                            if(fruitsCheckColumn.Count>4 || fruitsCheckRow.Count > 4)
                            {
                             
                                powerUpID = -5;
                            }
                            else
                            {
                                if (rowPopped)
                                {
                                    // 4 match Row popped vertical harvester
                                    if(fruitsCheckRow.Count > 3)
                                    {
                                        powerUpID = -2;
                                    }
                                }
                                else
                                {
                                    // 4 match Column popped horizontal harvester
                                    if (fruitsCheckColumn.Count > 3)
                                    {
                                        powerUpID = -1;
                                    }
                                }
                            }
                           
                        }
                    }

                    if (fruitsCheckTotal.Count >= 3)
                    {
                       
                        string damageID = Guid.NewGuid().ToString();

                        for (int e = 0; e < fruitsCheckTotal.Count; e++)
                        {
                            fruitsCheckTotal[e].GetComponent<Fruit>().damageID = damageID;
                            
                            if (powerUpID != 0)
                            {
                                fruitsCheckTotal[e].GetComponent<Fruit>().outsideOfBoard = true;
                            }

                            DestroyController(fruitsCheckTotal[e], true,true);
                        }

                        if (powerUpID!=0)
                        {
                            // Creating Power Up according to shape of match.

                            GameObject fruitToChange;

                            if (currentFruit && fruitsCheckTotal.Contains(currentFruit))
                            {
                                fruitToChange = currentFruit;
                            }else  if (currentOtherFruit && fruitsCheckTotal.Contains(currentOtherFruit))
                            {
                                fruitToChange = currentOtherFruit;
                            }
                            else
                            {
                                fruitToChange = fruitsCheckTotal[UnityEngine.Random.Range(0, fruitsCheckTotal.Count)];
                            }

                            StartCoroutine(FruitsGatheringAnim(fruitsCheckTotal, fruitToChange.GetComponent<Fruit>().column, fruitToChange.GetComponent<Fruit>().row,powerUpID));
                        }
                        else
                        {
                            audioManager.FruitCrush();
                        }
                        fruitsCheckTotal.Clear();

                        popped = true;
                        swipeHint.oneHintActive = false;
                        //swipeHint.StopCoroutines();

                        // SWIPE HINT ANIMATION STOP

                      //  StopHintAnimations();
                      //  achievementManager.AchievementProgress(typeFruits);
                    }
                }
            }
        }

        if (!popped)
        {
            bool allFruitsStopped = true;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allFruits[i, j] && allFruits[i, j].GetComponent<Fruit>().isMoving)
                    {
                        allFruitsStopped = false;

                        j = height;
                        i = width;
                    }
                }
            }
            if (allFruitsStopped)
            {
                hintBool = true;
            }
            else
            {
                StopHint();
            }
        }
        else
        {
            StopHint();
        }

        StartCoroutine(CheckAndDestroyMatches());

    }

    private List<GameObject> SquareCheck(int column, int row)
    {
        List<GameObject> fruitsCheckTemp = new List<GameObject>();
        int type;

        // Checking Square match


        if (row + 1 < height && FruitAvailable(allFruits[column, row]) && FruitAvailable(allFruits[column, row + 1]) && (type = allFruits[column, row].GetComponent<Fruit>().fruitType) == allFruits[column, row + 1].GetComponent<Fruit>().fruitType)
        {
            if (column + 1 < width && FruitAvailable(allFruits[column + 1, row]) && FruitAvailable(allFruits[column + 1, row + 1]) && allFruits[column + 1, row].GetComponent<Fruit>().fruitType
            == allFruits[column + 1, row + 1].GetComponent<Fruit>().fruitType && type == allFruits[column + 1, row].GetComponent<Fruit>().fruitType)
            {
                fruitsCheckTemp.Add(allFruits[column, row]);
                fruitsCheckTemp.Add(allFruits[column, row + 1]);
                fruitsCheckTemp.Add(allFruits[column + 1, row]);
                fruitsCheckTemp.Add(allFruits[column + 1, row + 1]);
                return fruitsCheckTemp;
            }

            if (column - 1 >= 0 && FruitAvailable(allFruits[column - 1, row]) && FruitAvailable(allFruits[column - 1, row + 1]) && allFruits[column - 1, row].GetComponent<Fruit>().fruitType
           == allFruits[column - 1, row + 1].GetComponent<Fruit>().fruitType && type == allFruits[column - 1, row].GetComponent<Fruit>().fruitType)
            {
                fruitsCheckTemp.Add(allFruits[column, row]);
                fruitsCheckTemp.Add(allFruits[column, row + 1]);
                fruitsCheckTemp.Add(allFruits[column - 1, row]);
                fruitsCheckTemp.Add(allFruits[column - 1, row + 1]);
                return fruitsCheckTemp;
            }
        }

        return fruitsCheckTemp;
    }

    /// <summary>
    /// First try to start position by checking if is same type of fruit to downward direction. After finding out lowest positioned fruit then putting same type of fruits to
    /// list.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private List<GameObject> ColumnCheck(int column, int row)
    {
        bool same = true;
        List<GameObject> fruitsCheckTemp = new List<GameObject>();
        int k = 0;
        int startRow = row;

        // Finding lowest positioned fruits.
        while (same)
        {
            if (row + k - 1 < 0 || !FruitAvailable(allFruits[column, row + k - 1]) || (allFruits[column, row + k] && allFruits[column, row + k - 1] && allFruits[column, row + k].GetComponent<Fruit>().fruitType != allFruits[column, row + k - 1].GetComponent<Fruit>().fruitType))
            {
                same = false;
                startRow = row + k;
            }
            k--;
        }

        k = 0;
        same = true;

        while (same)
        {
            if (startRow + k + 1 >= height || !FruitAvailable(allFruits[column, startRow + k + 1]) || (allFruits[column, startRow + k] && allFruits[column, startRow + k + 1] && allFruits[column, startRow + k].GetComponent<Fruit>().fruitType != allFruits[column, startRow + k + 1].GetComponent<Fruit>().fruitType))
            {
                same = false;
            }
            fruitsCheckTemp.Add(allFruits[column, startRow + k]);
            k++;
        }

        return fruitsCheckTemp;
    }

    /// <summary>
    /// First try to start position by checking if is same type of fruit to left direction. After finding out leftmost positioned fruit then putting same type of fruits to 
    /// list.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private List<GameObject> RowCheck(int column, int row)
    {
        bool same = true;
        List<GameObject> fruitsCheckTemp = new List<GameObject>();
        int k = 0;
        int startColumn=column;


        // Finding leftmost positinoed fruit.
        while (same)
        {
            if (column + k - 1 < 0 || !FruitAvailable(allFruits[column + k - 1, row]) || (allFruits[column + k, row] && allFruits[column + k - 1, row] && allFruits[column + k, row].GetComponent<Fruit>().fruitType != allFruits[column + k - 1, row].GetComponent<Fruit>().fruitType))
            {
                same = false;
                startColumn = column + k;
            }
            k--;
        }

      
        k = 0;
        same = true;

        while (same)
        {
            if (startColumn + k + 1 >= width || !FruitAvailable(allFruits[startColumn + k + 1, row]) || (allFruits[startColumn + k, row] && allFruits[startColumn + k + 1, row] && allFruits[startColumn + k, row].GetComponent<Fruit>().fruitType != allFruits[startColumn + k + 1, row].GetComponent<Fruit>().fruitType))
            {
                same = false;
            }
            fruitsCheckTemp.Add(allFruits[startColumn + k, row]);
            k++;
        }

        return fruitsCheckTemp;
    }

    public IEnumerator FruitsGatheringAnim(List<GameObject> changingFruits,int column,int row,int powerUpID)
    {
        List<GameObject> DestroyFruits = new List<GameObject>();
        DestroyFruits.AddRange( changingFruits);
        Vector2 gatherPosition = allTiles[column, row].transform.position;
        foreach(GameObject fruit in DestroyFruits)
        {
            fruit.GetComponent<Fruit>().targetV = gatherPosition;
            fruit.GetComponent<Fruit>().speedMultiplier = speedAndTimeLibrary.powerUpCreationFruitSpeed;
        }
        allTiles[column, row].GetComponent<BackgroundTile>().isTempEmptyTile = true;


        yield return new WaitForSeconds(speedAndTimeLibrary.powerUpCreationFruitGatheringDuration);


        CreatePowerUp(column, row, powerUpID, true);

        foreach (GameObject obj in DestroyFruits)
        {
            Destroy(obj);
        }

        allTiles[column, row].GetComponent<BackgroundTile>().isTempEmptyTile = false;


     //   yield return new WaitForSeconds(0.036f);


        audioManager.PowerUpGain();
    }

    /// <summary>
    /// Basically checks if fruits exist and not in a move to somewhere.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool FruitAvailable(GameObject obj)
    {
        if (obj && Vector2.Distance(obj.GetComponent<Fruit>().targetV, obj.transform.position) < 0.01f && !obj.GetComponent<Fruit>().fadeout && obj.GetComponent<Fruit>().fruitType >= -100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// This function used by only powerups because for this version distance check removed.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool FruitAvailableWithoutDistanceCheck(GameObject obj)
    {
        if (obj && !obj.GetComponent<Fruit>().fadeout && obj.GetComponent<Fruit>().fruitType >= -100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// This function used by only powerups because for this version type check removed.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool FruitAvailableWithoutTypeCheck(GameObject obj)
    {
        if (obj && Vector2.Distance(obj.GetComponent<Fruit>().targetV, obj.transform.position) < 0.2f && !obj.GetComponent<Fruit>().fadeout)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator CheckMoveForOneFruit(GameObject fruit, GameObject targetTile)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        BackgroundTile backgroundTileScript = targetTile.GetComponent<BackgroundTile>();
        bool succesfulMove = false;

        int tempRow = fruitScript.row, tempCol = fruitScript.column;

        fruitScript.row = backgroundTileScript.row;
        fruitScript.column = backgroundTileScript.column;
        allTiles[tempCol, tempRow].GetComponent<BackgroundTile>().isTempEmptyTile = true;
        allFruits[tempCol, tempRow] = null;
        fruitScript.targetV = allTiles[backgroundTileScript.column, backgroundTileScript.row].transform.position;


        yield return new WaitForSeconds(speedAndTimeLibrary.fruitSwipeDuration);
        if (!specialSwipe)
        {
            if ((fruitScript.fruitType > -100 && fruitScript.fruitType < 0))
            {
                // If one of the fruits is power up 
                if (fruitScript.fruitType < 0)
                {
                    ActivatePowerUp(fruit);

                }
               
                succesfulMove = true;
            }
            else
            {
                //yield return new WaitForSeconds(0.1f);

                if (!fruit)
                {
                    succesfulMove = true;
                }
                else
                {
                    if (CheckMatchSides(fruitScript.row, fruitScript.column))
                    {
                        // Checked if this is right move
                        succesfulMove = true;
                    }
                    else
                    {
                        if (!fruit || fruitScript.fadeout)
                        {
                            succesfulMove = true;

                        }

                    }
                }
               
            }
        }
        else
        {
            succesfulMove = true;
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
                ActivateSpecialPower(fruitScript.column, fruitScript.row);
                specialSwipe = false;
            }

        }
        else
        {
            allFruits[backgroundTileScript.column, backgroundTileScript.row] = null;
            //swipeHint.oneHintActive = false;
            audioManager.SwipeResist();

            fruitScript.row =tempRow;
            fruitScript.column = tempCol;

            fruitScript.targetV = allTiles[tempCol, tempRow].transform.position;

            yield return new WaitForSeconds(speedAndTimeLibrary.afterReverseWait);
        }

        if (fruit)
        {
            fruitScript.isSwiped = false;

        }

        allTiles[tempCol, tempRow].GetComponent<BackgroundTile>().isTempEmptyTile = false;

    }

    /// <summary>
    /// Checks if the played move possible if not it reverts move.
    /// </summary>
    /// <param name="fruit"></param>
    /// <param name="otherFruit"></param>
    /// <returns></returns>
    private IEnumerator CheckMove(GameObject fruit, GameObject otherFruit,bool horizontalSwipe)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>();
        Fruit otherFruitScript = otherFruit.GetComponent<Fruit>();
        bool succesfulMove = false;

        // If two object are power ups then merge happens.

        if (fruitScript.fruitType > -100 && otherFruitScript.fruitType > -100 && fruitScript.fruitType < 0 && otherFruitScript.fruitType < 0 && !specialSwipe)
        {
            // Selected power up moves towards to other power up
            fruitScript.row = otherFruitScript.row;
            fruitScript.column = otherFruitScript.column;
            fruitScript.targetV = otherFruitScript.targetV;
            fruitScript.fadeout = true;
            otherFruitScript.fadeout = true;
            yield return new WaitForSeconds(speedAndTimeLibrary.beforeTwoPowerUpMergeWait);
            StartCoroutine(ActivateMergePowerUp(fruit, otherFruit,horizontalSwipe));
            succesfulMove = true;
        }
        else
        {
            // If one of them is power up then they switch and power up activate.
            ChangeTwoFruit(fruit, otherFruit);
            yield return new WaitForSeconds(speedAndTimeLibrary.fruitSwipeDuration);
            if(!specialSwipe)
            {
                if ((fruitScript.fruitType > -100 && fruitScript.fruitType < 0) || (otherFruitScript.fruitType > -100 && otherFruitScript.fruitType < 0))
                {
                    // If one of the fruits is power up 
                    if (fruitScript.fruitType < 0)
                    {
                        ActivatePowerUp(fruit,otherFruitScript.fruitType);

                    }
                    else
                    {
                        ActivatePowerUp(otherFruit,fruitScript.fruitType);

                    }
                    succesfulMove = true;
                }
                else
                {
                    //yield return new WaitForSeconds(0.1f);

                    succesfulMove = IsSuccesfulMove(fruit, otherFruit);
                }
            }
            else
            {
                succesfulMove = true;
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
                ActivateSpecialPower(fruitScript.column, fruitScript.row);
                specialSwipe = false;
            }

        }
        else
        {
            //swipeHint.oneHintActive = false;
            audioManager.SwipeResist();
            ChangeTwoFruit(fruit, otherFruit);
            yield return new WaitForSeconds(speedAndTimeLibrary.afterReverseWait);
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

        List<GameObject> avaliableFruits = new List<GameObject>();

         avaliableFruits = RowCheck(column,row);

        if(avaliableFruits.Count >= 3)
        {
            return true;
        }
        avaliableFruits.Clear();

        avaliableFruits = ColumnCheck(column, row);

        if (avaliableFruits.Count >= 3)
        {
            return true;
        }
        avaliableFruits.Clear();

        avaliableFruits = SquareCheck(column, row);

        if (avaliableFruits.Count >= 3)
        {
            return true;
        }
        avaliableFruits.Clear();

        if (row-1>=0 && allFruits[column, row - 1] && allFruits[column, row] && allFruits[column,row].GetComponent<Fruit>().fruitType == allFruits[column, row - 1].GetComponent<Fruit>().fruitType)
        {
            avaliableFruits = SquareCheck(column, row-1);
            if (avaliableFruits.Count >= 3)
            {
                return true;
            }
        }
        avaliableFruits.Clear();

        return false;

    }

   
    /// <summary>
    /// Object will be fade away slowly before destroye.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public IEnumerator FruitPop(GameObject obj,bool playFruitDestroyAnim)
    {
        if (playFruitDestroyAnim)
        {
            //  obj.transform.position = obj.GetComponent<Fruit>().targetV;
            obj.transform.GetChild(0).GetComponent<Animator>().SetBool(isFruitDestroyed, true);
            yield return new WaitForSeconds(speedAndTimeLibrary.fruitDestroyAnimDuration);
        }
      
        if (obj)
        {
            if (!playFruitDestroyAnim)
            {
                obj.GetComponentInChildren<SpriteRenderer>().enabled = false;
                obj.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                obj.GetComponent<ParticleSystem>().Play();
                yield return new WaitForSeconds(speedAndTimeLibrary.waitBeforeClosingFruitSprite);
                obj.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }       
            yield return new WaitForSeconds(speedAndTimeLibrary.waitBeforeRemovingFruitFromBoard);
            obj.GetComponent<Fruit>().outsideOfBoard = true;
            allFruits[obj.GetComponent<Fruit>().column, obj.GetComponent<Fruit>().row] = null;
            
        }
        yield return new WaitForSeconds(speedAndTimeLibrary.maxFruitParticleDuration);
        if (obj)
        {
            Destroy(obj);
        }

    }

    private IEnumerator StopAndStartAllFillings(float waitTime)
    {
        if (!blockUserMove)
        {
            string stopperId = Guid.NewGuid().ToString();
            for (int i = 0; i < width; i++)
            {
                StopCoroutine(FillTheColumn(i));
                columnStopperId[i] = stopperId;
            }
            yield return new WaitForSeconds(waitTime);

            if (!blockUserMove)
            {
                
                for (int i = 0; i < width; i++)
                {
                   if(columnStopperId[i] == stopperId)
                    {
                        columnStopperId[i] = null;
                    }
                }
                
               // Array.Clear(fillingColumn, 0, fillingColumn.Length);
            }
        }
    }

    private IEnumerator StopAndStartSingleColumn(float waitTime, int column)
    {
        if (!blockUserMove)
        {
            string stopperId = Guid.NewGuid().ToString();
            columnStopperId[column] = stopperId;
            StopCoroutine(FillTheColumn(column));

            yield return new WaitForSeconds(waitTime);

            if (!blockUserMove)
            {
                  StopCoroutine(FillTheColumn(column));
                if (columnStopperId[column] == stopperId)
                {
                    columnStopperId[column] = null;
                }
            }
        }

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
        if (!shuffling || blockUserMove)
        {

            Queue<int> emptyPlaces = new Queue<int>();

            if (columnStopperId[i] == null)
            {
                for (int j = 0; j <= columnsFallIndexY[i]; j++)
                {
                    if (columnStopperId[i] == null)
                    {
                        if (!allTiles[i, j] || allTiles[i, j].GetComponent<BackgroundTile>().isTempEmptyTile || allTiles[i, j].GetComponent<BackgroundTile>().isCurrentObstacleBox)
                        {
                            if (allTiles[i, j])
                            {
                                  emptyPlaces.Clear();
                            }

                        }
                        else
                        {

                            if (!allFruits[i, j])
                            {
                                // Putting empty place index to variable
                                emptyPlaces.Enqueue(j);

                            }
                            else if (allFruits[i, j].GetComponent<Fruit>().isSwiped || allFruits[i, j].GetComponent<Fruit>().fadeout)
                            {
                                 emptyPlaces.Clear();
                            }
                            else if (emptyPlaces.Count > 0)
                            {
                                // if there is a piece then piece new location will be first empty place in queue.
                                audioManager.FruitFall();
                                int emptyRowIndex = emptyPlaces.Dequeue();
                                GameObject fruit = allFruits[i, j];
                                Fruit fruitScript = fruit.GetComponent<Fruit>();
                                fruitScript.row = emptyRowIndex;
                                fruitScript.column = i;
                                allFruits[i, j] = null;

                                fruitScript.targetV.y = allTiles[i, emptyRowIndex].transform.position.y;
                                emptyPlaces.Enqueue(j);
                                yield return new WaitForSeconds(speedAndTimeLibrary.waitBeforeFallingOtherFruit);
                            }
                        }
                    }
                }
            }

            //yield return new WaitForSeconds(0.05f);

            // Checking for crossfalls  
            if (columnStopperId[i] == null)
            {
                for (int j = columnsFallIndexY[i]; j >= 0; j--)
                {
                    if (j - 1 >= 0 && !allFruits[i, j - 1] && allTiles[i, j] && allTiles[i, j].GetComponent<BackgroundTile>().isCurrentObstacleBox && columnStopperId[i] == null)
                    {

                        int k = 0;

                        while (j + k - 1 >= 0 && !allFruits[i, j + k] && !CrossFall(i, j + k))
                        {
                            k--;
                        }
                    }
                }
            }

            if (columnStopperId[i] == null)
            {
                while (emptyPlaces.Count > 0)
                {
                    float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
                    float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
                    int emptyRowIndex = emptyPlaces.Dequeue();
                    if (columnStopperId[i] == null)
                    {

                        // If this place did not filled then create object for it.
                        if (!allFruits[i, emptyRowIndex])
                        {

                            Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, (columnsFallIndexY[i] + 1) * scaleNumber - yOffset);

                            // Instantiate a new fruit at the position of the destroyed fruit. Fruit that going to be created must be from existFruits variable. existFruits
                            // list contains indexes of avaliable fruits.
                            int fruitToUse = existFruits[UnityEngine.Random.Range(0, existFruits.Count)];
                            GameObject newFruit;
                            int random = 1;
                            if (indexOfCreatableObstacle != -1)
                            {
                                random = UnityEngine.Random.Range(0, 4);
                            }

                            if (random == 0)
                            {
                                // generatable type of obstacle creation
                                newFruit = Instantiate(obstaclePrefabs[indexOfCreatableObstacle], tempPosition, Quaternion.identity);
                                newFruit.GetComponent<ObstacleScript>().row = emptyRowIndex;
                                newFruit.GetComponent<ObstacleScript>().column = i;

                            }
                            else
                            {
                                // normal fruit creation
                                newFruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
                            }
                            Fruit newFruitScript = newFruit.GetComponent<Fruit>();

                            // Set the parent and name of the new fruit
                            newFruit.transform.parent = this.transform;
                            newFruit.name = "( " + i + ", " + emptyRowIndex + " )";

                            // Set the column and row of the new fruit
                            newFruitScript.column = i;
                            newFruitScript.row = emptyRowIndex;
                            newFruitScript.targetV.y = allTiles[i, emptyRowIndex].transform.position.y;
                            if (random != 0)
                            {
                                totalNumberOfFruits[fruitToUse]++;
                                newFruitScript.fruitType = fruitToUse;
                            }

                            allFruits[i, emptyRowIndex] = newFruit;

                            audioManager.FruitFall();
                            // Add the new fruit to the allFruits array
                            yield return new WaitForSeconds(speedAndTimeLibrary.waitBeforeCreatingFruitsTopOfBoard);
                        }
                    }
                }
            }
        }
        yield return null;
        StartCoroutine(FillTheColumn(i));
    }

    /// <summary>
    /// Powerups same as fruits but just their type is negative numbers so that when they popped they will act like powerup according to their type.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="type"></param>
    public GameObject CreatePowerUp(int column, int row, int type, bool isItInteractable, bool playCreationAnim = true)
    {
        /*
        Power Up Type Number;
        
        -1 : Horizontal Harvester
        -2 : Vertical Harvester
        -3 : TNT
        -4 : Boomerang 
        -5: Disco Ball
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
        newPowerUpScript.damageID = Guid.NewGuid().ToString();
        if (playCreationAnim)
        {
            switch (type)
            {
                case -1:
                    newPowerUp.GetComponentInChildren<Animator>().SetTrigger(isHarvesterCreated);
                    break;
                case -2:
                    newPowerUp.GetComponentInChildren<Animator>().SetTrigger(isHarvesterCreated);
                    break;
                case -3:
                    newPowerUp.GetComponentInChildren<Animator>().SetTrigger(isTNTCreated);
                    break;
                case -4:
                    newPowerUp.transform.GetChild(0).GetComponent<Animator>().SetTrigger(isBoomerangCreated);
                    break;
                case -5:
                    newPowerUp.GetComponentInChildren<Animator>().SetTrigger(isDiscoBallCreated);
                    break;
            }
        }


        // Add the new powerup to the allFruits array
        if (allFruits[column, row])
        {
            allFruits[column, row].GetComponent<Fruit>().outsideOfBoard = true;
        }
        allFruits[column, row] = newPowerUp;

        // it means this is not for immidate use and normally created not for merge or something for.
        if (isItInteractable)
        {
            StartCoroutine(WaitAndActivatePowerUp(speedAndTimeLibrary.createdPowerupUninteractableDuration, newPowerUpScript));
        }
        return newPowerUp;
    }

    /// <summary>
    /// Wait and after make powerUp can interact other things. This wait important because if powerUp created after match but same time powerup activated system must not activate new created powerUp.  
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="powerUpScript"></param>
    /// <returns></returns>
    private IEnumerator WaitAndActivatePowerUp(float waitTime,Fruit powerUpScript)
    {
        yield return new WaitForSeconds(waitTime);
        powerUpScript.fadeout = false;
    }

    /// <summary>
    /// If the empty place that has an obstacle or missing tile on top of itself then this fuction steals a fruit from other column. First checks left
    /// column if not possible than checks right column for stealing fruit.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private bool CrossFall(int column, int row)
    {
        GameObject fruit = null;
        Fruit fruitScript;

        if(row-1 < 0)
        {
            return false;
        }

        if (!allTiles[column, row - 1])
        {
            return false;
        }
        else if(allTiles[column, row - 1].GetComponent<BackgroundTile>().isTempEmptyTile)
        {
            return false;
        }

        if (allFruits[column, row - 1])
        {
            return false;
        }

        if(allTiles[column, row - 1].GetComponent<BackgroundTile>().isCurrentObstacleBox)
        {
            return false;
        }

        //  yield return new WaitForSeconds(0.1f);
        if (column - 1 >= 0 && FruitAvailableWithoutTypeCheck(allFruits[column - 1, row]))
        {
            fruit = allFruits[column - 1, row];
            allFruits[column - 1, row] = null;
        }
        else if (column + 1 < width && FruitAvailableWithoutTypeCheck(allFruits[column + 1, row]))
        {
            fruit = allFruits[column + 1, row];
            allFruits[column + 1, row] = null;
        }
        else
        {
            if (((column - 1 >= 0 && allFruits[column - 1, row]) || (column + 1 < width && allFruits[column + 1, row])))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (fruit)
        {
            audioManager.FruitFall();
            fruitScript = fruit.GetComponent<Fruit>();
            fruitScript.row = row - 1;
            fruitScript.column = column;
            fruitScript.targetV = allTiles[column, row - 1].transform.position;
            return true;
        }
        return false;
    }

    /// <summary>
    /// If one powerup swiped with other powerup than there will be a merge and this function will be called. It will call necessary functions 
    /// according to types of this two powerups.
    /// </summary>
    /// <param name="fruit"></param>
    /// <param name="otherFruit"></param>
    public IEnumerator ActivateMergePowerUp(GameObject fruit, GameObject otherFruit,bool horizontalSwipe)
    {
        Fruit fruitScript = fruit.GetComponent<Fruit>(), otherFruitScript = otherFruit.GetComponent<Fruit>();
        fruitScript.damageID = otherFruitScript.damageID;
        fruitScript.isPowerUpSoundPlayed = true;
        otherFruitScript.isPowerUpSoundPlayed = true;
        StopHint();
        if (otherFruitScript.fruitType == fruitScript.fruitType)
        {
            // If two power up same it goes here

            switch (fruitScript.fruitType)
            {
                case -1:
                    fruitScript.fruitType = -2;
                    fruitScript.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    audioManager.Harvester();
                    ActivatePowerUp(fruit);
                    ActivatePowerUp(otherFruit);
                    break;

                case -2:
                    fruitScript.fruitType = -1;
                    fruitScript.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    audioManager.Harvester();
                    ActivatePowerUp(fruit);
                    ActivatePowerUp(otherFruit);
                    break;
                case -3:
                //    yield return new WaitForSeconds(0.1f);
                    Destroy(fruit);
                    otherFruit.GetComponentInChildren<SpriteRenderer>().sortingOrder = 10;
                    otherFruit.GetComponentInChildren<Animator>().SetTrigger(isTNTMerged);
                    if (otherFruitScript.column - 2 >= 0)
                    {
                        StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.twoTNTMergeColumnStopDuration, otherFruitScript.column - 2));
                    }
                    if (otherFruitScript.column - 1 >= 0)
                    {
                        StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.twoTNTMergeColumnStopDuration, otherFruitScript.column - 1));
                    }
                    StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.twoTNTMergeColumnStopDuration, otherFruitScript.column));
                    if (otherFruitScript.column + 1 < width)
                    {
                        StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.twoTNTMergeColumnStopDuration, otherFruitScript.column + 1));
                    }
                    if (otherFruitScript.column + 2 < width)
                    {
                        StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.twoTNTMergeColumnStopDuration, otherFruitScript.column + 2));
                    }
                    yield return new WaitForSeconds(speedAndTimeLibrary.twoTNTMergeAnimDuration);
                    
                    otherFruitScript.activePowerUp = true;
                    otherFruit.GetComponent<BoxCollider2D>().size = new Vector2(5, 5);

                    audioManager.Pickaxe();

                    otherFruit.GetComponentInChildren<SpriteRenderer>().enabled = false;

                    StartCoroutine(WaitAndDestroyObj(otherFruit, speedAndTimeLibrary.waitBeforeDestroyingTNT));

                    break;
                case -4:

                    fruit.GetComponentInChildren<SpriteRenderer>().enabled = false;
                    otherFruit.GetComponentInChildren<Animator>().SetTrigger(is2BoomerangMerged);

                    StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.twoBoomerangMergeColumnStopDuration,otherFruitScript.column));

                    yield return new WaitForSeconds(speedAndTimeLibrary.twoBoomerangMergeAnimDuration);                 

                    GameObject cloneBoomerang = CreatePowerUp(otherFruitScript.column, otherFruitScript.row,-4,false,false);
                //    yield return new WaitForSeconds(0.1f);
                    fruit.GetComponentInChildren<SpriteRenderer>().enabled = true;

                    audioManager.Pickaxe();
                    ActivatePowerUp(fruit);
                    ActivatePowerUp(otherFruit);
                    ActivatePowerUp(cloneBoomerang);
                    break;
                case -5:
                    audioManager.Pickaxe();
                    StartCoroutine(StopAndStartAllFillings(speedAndTimeLibrary.twoDiscoBallMergeColumnStopDuration));
                    Destroy(fruit);
                    blockUserMove = true;
                    otherFruitScript.activePowerUp = true;
                    otherFruit.transform.GetChild(0).gameObject.SetActive(false);
                    otherFruit.transform.GetChild(1).gameObject.SetActive(true);
                    yield return new WaitForSeconds(speedAndTimeLibrary.twoDiscoballMergeAnimDuration);
                    allFruits[otherFruitScript.column, otherFruitScript.row] = null;
                    otherFruit.transform.GetChild(2).gameObject.SetActive(true);
                    otherFruit.GetComponent<ExplosionAreaScript>().enabled = true;
                    yield return new WaitForSeconds(speedAndTimeLibrary.twoDiscoBallMergeBoardFillAfterExplosionStart);
                    StopHint();
                    otherFruitScript.outsideOfBoard = true;
                    blockUserMove = false;
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
                            audioManager.Harvester();
                            ActivatePowerUp(fruit);
                            ActivatePowerUp(otherFruit);
                            break;
                        case -3:

                            StartCoroutine(HarvesterTNTMerge(fruit,otherFruit,fruitScript.column,fruitScript.row,false,horizontalSwipe));

                            break;
                        case -4:
                            audioManager.Pickaxe();
                            fruitScript.outsideOfBoard = true;
                            fruit.transform.GetChild(0).gameObject.SetActive(false);
                            allFruits[fruitScript.column, fruitScript.row] = null;
                            otherFruitScript.attachedPowerUp = fruit;
                            ActivatePowerUp(otherFruit,-1);
                            break;
                        case -5:
                            audioManager.Pickaxe();
                            Destroy(fruit);
                            ActivatePowerUp(otherFruit, -1);
                            break;
                    }
                    break;
                // Vertical Harvester
                case -2:
                    switch (otherFruitScript.fruitType)
                    {
                        case -1:
                            audioManager.Harvester();
                            ActivatePowerUp(fruit);
                            ActivatePowerUp(otherFruit);
                            break;
                        case -3:

                            StartCoroutine(HarvesterTNTMerge(fruit,otherFruit, fruitScript.column, fruitScript.row, true, horizontalSwipe));

                            break;
                        case -4:
                            audioManager.Pickaxe();
                            fruitScript.outsideOfBoard = true;
                            fruit.transform.GetChild(0).gameObject.SetActive(false);
                            allFruits[fruitScript.column, fruitScript.row] = null;
                            otherFruitScript.attachedPowerUp = fruit;
                            ActivatePowerUp(otherFruit,-2);
                            break;
                        case -5:
                            audioManager.Pickaxe();
                            Destroy(fruit);
                            ActivatePowerUp(otherFruit, -2);
                            break;
                    }
                    break;
                // TNT 
                case -3:
                    switch (otherFruitScript.fruitType)
                    {
                        case -1:

                            StartCoroutine(HarvesterTNTMerge(otherFruit,fruit, otherFruitScript.column, otherFruitScript.row, false, horizontalSwipe));

                            break;
                        case -2:

                            StartCoroutine(HarvesterTNTMerge(otherFruit,fruit, otherFruitScript.column, otherFruitScript.row, true, horizontalSwipe));

                            break;
                        case -4:
                            audioManager.Pickaxe();
                            fruitScript.outsideOfBoard = true;
                            fruit.transform.GetChild(0).gameObject.SetActive(false);
                            allFruits[fruitScript.column, fruitScript.row] = null;
                            otherFruitScript.attachedPowerUp = fruit;
                            ActivatePowerUp(otherFruit,-3);
                            break;
                        case -5:
                            audioManager.Pickaxe();
                            Destroy(fruit);
                            ActivatePowerUp(otherFruit, -3);
                            break;
                    }
                    break;
                // Boomerang
                case -4:
                    audioManager.Pickaxe();
                    if (otherFruitScript.fruitType == -5)
                    {
                        ActivatePowerUp(otherFruit, -4);
                        Destroy(fruit);
                    }
                    else
                    {
                        otherFruitScript.outsideOfBoard = true;
                        allFruits[otherFruitScript.column, otherFruitScript.row] = null;
                        fruitScript.attachedPowerUp = otherFruit;
                        otherFruit.transform.GetChild(0).gameObject.SetActive(false);
                        ActivatePowerUp(fruit,otherFruitScript.fruitType);
                    }              

                    break;
                // Discoball
                case -5:

                    ActivatePowerUp(fruit, otherFruitScript.fruitType);
                    Destroy(otherFruit);

                    break;
            }
        }
        StopHint();
    }

    /// <summary>
    /// Activating powerup according to type.
    /// </summary>
    /// <param name="fruit"></param>
    public void ActivatePowerUp(GameObject fruit, int swipedFruitType=0,bool isSwiped=true)
    {

        Fruit fruitScript = fruit.GetComponent<Fruit>();
        fruitScript.fadeout = true;
        int column = fruitScript.column, row = fruitScript.row, type = fruitScript.fruitType;
        StopHint();
        switch (type)
        {
            // Horizontal Harvester power up
            case -1:
                int waitTileAmountHorizontal;
                if (column > width-column)
                {
                    waitTileAmountHorizontal = column;
                }
                else
                {
                    waitTileAmountHorizontal = width - column;
                }
                StartCoroutine(StopAndStartAllFillings(speedAndTimeLibrary.horizontalHarvesterColumnStopDurationMultiplier * waitTileAmountHorizontal));

                GameObject cloneHorizontal = Instantiate(powerUps[0], allTiles[column, row].transform.position, powerUps[0].transform.rotation);
                Fruit cloneHorizontalScript = cloneHorizontal.GetComponent<Fruit>();

                cloneHorizontalScript.GetComponentInChildren<SpriteRenderer>().sprite = harvesterUpSprite;
                fruitScript.GetComponentInChildren<SpriteRenderer>().sprite = harvesterUpSprite;
                Vector3 newRotationH = new Vector3(0f, 0f, -180);
                cloneHorizontal.transform.rotation = Quaternion.Euler(newRotationH);

                fruit.transform.position = allTiles[column, row].transform.position;

                cloneHorizontal.gameObject.transform.position = allTiles[column, row].transform.position;
                cloneHorizontalScript.damageID = fruitScript.damageID;
                cloneHorizontalScript.fruitType = type;

                fruitScript.moveToward = true;
                cloneHorizontalScript.moveToward = true;

                fruitScript.harvester = true;
                cloneHorizontalScript.harvester = true;

                fruitScript.outsideOfBoard = true;
                cloneHorizontalScript.outsideOfBoard = true;

                fruitScript.targetV.x = -11;
                cloneHorizontalScript.targetV.x = 11;             

                fruitScript.destroyOnReach = true;
                cloneHorizontalScript.destroyOnReach = true;

                allFruits[column, row] = null;
                fruitScript.activePowerUp = true;
                cloneHorizontalScript.activePowerUp = true;

                fruit.GetComponentInChildren<Animator>().SetBool(isHarvesterRunning, true);
                cloneHorizontal.GetComponentInChildren<Animator>().SetBool(isHarvesterRunning, true);

                if (!fruit.GetComponent<Fruit>().isPowerUpSoundPlayed)
                {
                    audioManager.Harvester();
                }

                break;
            // Vertical Harvester power up
            case -2:
                int waitTileAmountVertical;
                if (row > height - row)
                {
                    waitTileAmountVertical = row;
                }
                else
                {
                    waitTileAmountVertical = height - row;
                }
                StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.verticalHarvesterColumnStopDurationMultiplier * waitTileAmountVertical, column));

                GameObject cloneVertical = Instantiate(powerUps[1], allTiles[column, row].transform.position, powerUps[1].transform.rotation);
                Fruit cloneVerticalScript = cloneVertical.GetComponent<Fruit>();

                cloneVertical.GetComponentInChildren<SpriteRenderer>().sprite = harvesterUpSprite;
              
                fruitScript.GetComponentInChildren<SpriteRenderer>().sprite = harvesterUpSprite;
                Vector3 newRotationV = new Vector3(0f, 0f, -180);
                fruit.transform.rotation = Quaternion.Euler(newRotationV);

                fruit.transform.position = allTiles[column, row].transform.position;

                cloneVertical.gameObject.transform.position = allTiles[column, row].transform.position;
                cloneVerticalScript.damageID=fruitScript.damageID;
                cloneVerticalScript.fruitType = type;

                fruitScript.moveToward = true;
                cloneVerticalScript.moveToward = true;

                fruitScript.harvester = true;
                cloneVerticalScript.harvester = true;

                fruitScript.outsideOfBoard = true;
                cloneVerticalScript.outsideOfBoard = true;

                fruitScript.targetV.y = -14;
                cloneVerticalScript.targetV.y = 14;            

                fruitScript.destroyOnReach = true;
                cloneVerticalScript.destroyOnReach = true;

                allFruits[column, row] = null;
                fruitScript.activePowerUp = true;
                cloneVerticalScript.activePowerUp = true;

                fruit.GetComponentInChildren<Animator>().SetBool(isHarvesterRunning, true);
                cloneVertical.GetComponentInChildren<Animator>().SetBool(isHarvesterRunning, true);

                if (!fruit.GetComponent<Fruit>().isPowerUpSoundPlayed)
                {
                    audioManager.Harvester();
                }
                break;
            // TNT power up
            case -3:
                fruitScript.activePowerUp = true;
                fruit.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                fruit.GetComponentInChildren<SpriteRenderer>().enabled = false;

                if (column - 1 >= 0)
                {
                    StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.singleTNTColumnStopDuration, column - 1));
                }
                StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.singleTNTColumnStopDuration, column));
                if (column + 1 < width)
                {
                    StartCoroutine(StopAndStartSingleColumn(speedAndTimeLibrary.singleTNTColumnStopDuration, column + 1));
                }              

                if (!fruit.GetComponent<Fruit>().isPowerUpSoundPlayed)
                {
                    audioManager.Pickaxe();
                }
                StartCoroutine(WaitAndDestroyObj(fruit,speedAndTimeLibrary.waitBeforeDestroyingTNT));

                break;

            // Boomerang power up
            case -4:
                fruitScript.moveToward = true;
                fruitScript.outsideOfBoard = true;
                allFruits[fruitScript.column, fruitScript.row] = null;
                fruitScript.targetV = GetBoomerangTargetLoc(fruitScript.column, fruitScript.row);
                fruitScript.activePowerUp = true;


                if(swipedFruitType == 0)
                {
                    fruit.transform.GetChild(0).GetComponent<Animator>().SetBool(boomerangRotating, true);
                }
                else
                {
                    fruit.transform.GetChild(0).gameObject.SetActive(false);
                    fruit.transform.GetChild(2).gameObject.SetActive(true);
                    switch(swipedFruitType)
                    {
                        case -1:
                            // Horizontal
                         //   fruit.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.SetActive(true);
                            Vector3 newRotation = new Vector3(0f, 0f, 90f);
                            fruit.transform.GetChild(2).gameObject.transform.GetChild(2).rotation = Quaternion.Euler(newRotation);
                            fruit.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger(isHarvesterMergedBoomerang);
                            break;
                        case -2:
                            // Vertical
                       //     fruit.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.SetActive(true);
                            fruit.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger(isHarvesterMergedBoomerang);
                            break;
                        case -3:
                            // TNT
                         //   fruit.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                            fruit.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<Animator>().SetTrigger(isTNTMergedBoomerang);
                            break;
                    }
                }

                if (!fruit.GetComponent<Fruit>().isPowerUpSoundPlayed)
                {
                    audioManager.Pickaxe();
                }
                break;

            // Disco Ball power up
            case -5:
              
                int targetFruitType = swipedFruitType;
                // If discoball just clicked then returns most avaliable fruit type.
                if (!isSwiped || swipedFruitType < 0)
                {
                    int totalNumber = totalNumberOfFruits[0];
                    targetFruitType = 0;
                    for (int i = 1; i < fruits.Length; i++)
                    {
                        if (totalNumber < totalNumberOfFruits[i])
                        {
                            targetFruitType = i;
                            totalNumber = totalNumberOfFruits[i];
                        }
                    }
                }         

                StartCoroutine(DiscoBallSelectAndDestroy(fruit,targetFruitType, swipedFruitType));


                if (!fruit.GetComponent<Fruit>().isPowerUpSoundPlayed)
                {
                    audioManager.Pickaxe();
                }
                break;

        }
        StopHint();
    }

    /// <summary>
    /// Destroying objects in a control. If object already fading out there is no need to destroy it. If explosion is true then it will destroy nearby
    /// block obstacles.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="explosion"></param>
    public void DestroyController(GameObject obj, bool explosion,bool playFruitDestroyAnim=false)
    {
        if (!obj)
        {
            return;
        }

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
                int row = obj.GetComponent<Fruit>().row;
                int column = obj.GetComponent<Fruit>().column;
                totalNumberOfFruits[obj.GetComponent<Fruit>().fruitType]--;
                achievementManager.AchievementProgress(obj.GetComponent<Fruit>().fruitType);
                if (explosion)
                {
                    allTiles[column, row].GetComponent<BackgroundTile>().Explosion(column, row, obj.GetComponent<Fruit>().damageID, obj.GetComponent<Fruit>().colorType);
                }
                if (!obj.GetComponent<Fruit>().outsideOfBoard)
                {
                    StartCoroutine(FruitPop(obj, playFruitDestroyAnim));
                }
            }
            else
            {
                ActivatePowerUp(obj);
            }
        }
    }

    /// <summary>
    /// Destroy one side of vertical from the point. If up boolean true then it destroy up of the point and if not down. This function
    /// currently used by vertical special power and vertical harvester.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="up"></param>
    private void VerticalDestroy(int column, int row, bool up,string damageID)
    {
        if (up)
        {
            for (int i = row + 1; i < height; i++)
            {
                if (allTiles[column, i])
                {
                    if (FruitAvailable(allFruits[column, i]) && allFruits[column, i].GetComponent<Fruit>().fruitType > -100)
                    {
                        DestroyController(allFruits[column, i], false);
                        audioManager.FruitCrush();
                    }

                    allTiles[column, i].GetComponent<BackgroundTile>().PowerUpBoom(damageID);

                }
            }
        }
        else
        {
            for (int i = row - 1; i >= 0; i--)
            {
                if (allTiles[column, i])
                {
                    if (FruitAvailable(allFruits[column, i]) && allFruits[column, i].GetComponent<Fruit>().fruitType > -100)
                    {
                        DestroyController(allFruits[column, i], false);
                        audioManager.FruitCrush();
                    }
                    allTiles[column, i].GetComponent<BackgroundTile>().PowerUpBoom(damageID);

                }
            }
        }
    }

    /// <summary>
    /// This function activate selected special power. 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public void ActivateSpecialPower(int column, int row)
    {
        string damageID = Guid.NewGuid().ToString();

        bool failed = false;
        if (shuffling || blockUserMove)
        {
            return;
        }

        switch (specialPowerID)
        {
            case 1:
                // Special Power: Vertical Destroyer

                specialPowerController.SpecialPowerUpUsed(0);
                VerticalDestroy(column, -1, true, damageID);


                break;
            case 2:
                // Special Power: Horizontal Destroyer

                specialPowerController.SpecialPowerUpUsed(1);

                StartCoroutine(SunflowerCreationAndMovement(row));

                // HorizontalDestroy(-1, row, true, damageID);
                break;
            case 3:
                // Special Power: One Tile Destroyer

                // If tile has no obstacle it destroy fruit but if tile has obstacle then it checks if it is just break with powerup if it is then
                // it gives log. Also, if tile does exist but there is no obstacle or fruit then gives log.

                failed = true;


                if (FruitAvailable(allFruits[column, row]) && allFruits[column, row].GetComponent<Fruit>().fruitType > -100)
                {
                    failed = false;
                    specialPowerController.SpecialPowerUpUsed(2);
                    DestroyController(allFruits[column, row], false);
                    audioManager.FruitCrush();
                }

                if(allTiles[column, row].GetComponent<BackgroundTile>().indexOfVisibleOne >= 0)
                {
                    failed = false;
                    specialPowerController.SpecialPowerUpUsed(2);
                    allTiles[column, row].GetComponent<BackgroundTile>().PowerUpBoom(damageID);
                }
                

                break;
            case 4:
                // Special Power: Super Swipe
                specialPowerController.SpecialPowerUpUsed(3);
                break;
        }
        if (!failed)
        {
            StopHint();
            specialPowerID = 0;
            specialPowerUsing = false;
        }

    }

    /// <summary>
    /// This function will call from Special Power Controller script and related to Special Power buttons. specialPowerID represent selected special power.
    /// According to ability some of the arrangment wll be made.
    /// </summary>
    /// <param name="id"></param>
    public void SelectedSpecialPower(int id)
    {
        specialPowerID = id;
        specialPowerUsing = true;


        switch (specialPowerID)
        {
            // Special Power: Super Swipe
            case 4:
                specialSwipe = true;
                break;
        }
    }
    /// <summary>
    /// Disabling special power.
    /// </summary>
    public void DisableSpecialPowers()
    {
        specialPowerID = 0;
        specialSwipe = false;
    }
    /// <summary>
    /// Stopping hint system.
    /// </summary>
    public void StopHint()
    {

        if (swipeHint.oneHintActive)
        {
            swipeHint.StopHintCoroutines();

        }

        hintBool = false;
    }

    /// <summary>
    /// Checking if user selected starter powerup and if selected then put powerups randomly selected fruits instead. 
    /// </summary>
    private void CheckForStarterPowerUps()
    {
        /* 
         powerUpController powerUps Indexes
        0 - Harvester
        1 - TNT

         */


        for (int i = 0; i < 2; i++)
        {

            if (powerUpController.powerUps[i].isActivated)
            {
                int column, row;
                column =UnityEngine.Random.Range(0, width);
                row = UnityEngine.Random.Range(0, height);
                while (!allFruits[column, row])
                {
                    column = UnityEngine.Random.Range(0, width);
                    row = UnityEngine.Random.Range(0, height);
                }

                Destroy(allFruits[column, row]);

                switch (i)
                {
                    case 0:
                        // Harvester creation randomly horizontal or vertical
                        CreatePowerUp(column, row, -UnityEngine.Random.Range(1, 3), true);
                        break;
                    case 1:
                        // Tnt or other powerUps. Tnt = -3 that why -i-2.
                        CreatePowerUp(column, row, -i - 2, true);
                        break;
                    case 2:
                        // Discoball or other powerUps. Discoball = -5 that why -i-3.
                        CreatePowerUp(column, row, -i - 3, true);
                        break;
                }

            }
        }
    }

    /// <summary>
    /// Shuffle Function
    /// </summary>
    /// <returns></returns>
    public IEnumerator Shuffle()
    {
        shuffling = true;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allFruits[i, j] && allFruits[i, j].GetComponent<Fruit>().fruitType >= 0)
                {
                    DestroyController(allFruits[i, j], false);
                }
            }
        }

        yield return new WaitForSeconds(speedAndTimeLibrary.shuffleAfterDestroyBeforeCreationWait);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] && !allFruits[i, j] && !allTiles[i, j].GetComponent<BackgroundTile>().isCurrentObstacleBox)
                {
                    int fruitToUse = existFruits[UnityEngine.Random.Range(0, existFruits.Count)];
                    GameObject newFruit = Instantiate(fruits[fruitToUse], allTiles[i, j].transform.position, Quaternion.identity);
                    Fruit newFruitScript = newFruit.GetComponent<Fruit>();

                    // Set the parent and name of the new fruit
                    //   newFruit.transform.parent = this.transform;
                    newFruit.name = "( " + i + ", " + j + " )";

                    newFruitScript.targetV = allTiles[i, j].transform.position;

                    // Set the column and row of the new fruit
                    newFruitScript.column = i;
                    newFruitScript.row = j;
                    newFruitScript.fruitType = fruitToUse;

                }
            }
        }
        shuffling = false;
        StopHint();
    }

    /// <summary>
    /// This function checks if there is a fruit to flash, if there is then it stops the fruit's animation.
    /// </summary>
    private void StopHintAnimations()
    {
        if (swipeHint.fruit != null)
        {
            if (fruitAnimator != null && fruitAnimator.gameObject.activeSelf && fruitAnimator.runtimeAnimatorController != null && fruitAnimator.isActiveAndEnabled)
            {
                fruitAnimator.SetBool(swipeHint.fruit.swipeUp, false);
                fruitAnimator.SetBool(swipeHint.fruit.swipeDown, false);
                fruitAnimator.SetBool(swipeHint.fruit.swipeLeft, false);
                fruitAnimator.SetBool(swipeHint.fruit.swipeRight, false);
                fruitAnimator.SetBool(swipeHint.fruit.swipeFlash, false);
            }

            if (swipeHint.fruit2 != null)
            {
                if (fruitAnimator2 != null && fruitAnimator2.gameObject.activeSelf && fruitAnimator2.runtimeAnimatorController != null && fruitAnimator2.isActiveAndEnabled)
                {
                    fruitAnimator2.SetBool(swipeHint.fruit2.swipeUp, false);
                    fruitAnimator2.SetBool(swipeHint.fruit2.swipeDown, false);
                    fruitAnimator2.SetBool(swipeHint.fruit2.swipeLeft, false);
                    fruitAnimator2.SetBool(swipeHint.fruit2.swipeRight, false);
                    fruitAnimator2.SetBool(swipeHint.fruit2.swipeFlash, false);
                }
            }

            if (swipeHint.fruit3 != null)
            {
                if (fruitAnimator3 != null && fruitAnimator3.gameObject.activeSelf && fruitAnimator3.runtimeAnimatorController != null && fruitAnimator3.isActiveAndEnabled)
                {
                    fruitAnimator3.SetBool(swipeHint.fruit2.swipeUp, false);
                    fruitAnimator3.SetBool(swipeHint.fruit2.swipeDown, false);
                    fruitAnimator3.SetBool(swipeHint.fruit2.swipeLeft, false);
                    fruitAnimator3.SetBool(swipeHint.fruit2.swipeRight, false);
                    fruitAnimator3.SetBool(swipeHint.fruit2.swipeFlash, false);
                }
            }

            if (swipeHint.fruit4 != null)
            {
                if (fruitAnimator4 != null && fruitAnimator4.gameObject.activeSelf && fruitAnimator4.runtimeAnimatorController != null && fruitAnimator4.isActiveAndEnabled)
                {
                    fruitAnimator4.SetBool(swipeHint.fruit2.swipeUp, false);
                    fruitAnimator4.SetBool(swipeHint.fruit2.swipeDown, false);
                    fruitAnimator4.SetBool(swipeHint.fruit2.swipeLeft, false);
                    fruitAnimator4.SetBool(swipeHint.fruit2.swipeRight, false);
                    fruitAnimator4.SetBool(swipeHint.fruit2.swipeFlash, false);
                }
            }

            if (swipeHint.fruit5 != null)
            {
                if (fruitAnimator5 != null && fruitAnimator5.gameObject.activeSelf && fruitAnimator5.runtimeAnimatorController != null && fruitAnimator5.isActiveAndEnabled)
                {
                    fruitAnimator5.SetBool(swipeHint.fruit2.swipeUp, false);
                    fruitAnimator5.SetBool(swipeHint.fruit2.swipeDown, false);
                    fruitAnimator5.SetBool(swipeHint.fruit2.swipeLeft, false);
                    fruitAnimator5.SetBool(swipeHint.fruit2.swipeRight, false);
                    fruitAnimator5.SetBool(swipeHint.fruit2.swipeFlash, false);
                }
            }

        }
    }

    /// <summary>
    /// Calls DetectVisibleOne() function on every tile.
    /// </summary>
    public void AllTilesDetectVisibleOne()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                allTiles[i, j].GetComponent<BackgroundTile>().DetectVisibleOne();
            }
        }
    }

    private IEnumerator DiscoBallSelectAndDestroy(GameObject discoBall,int targetFruitType,int powerUpCreateType)
    {

        blockUserMove = true;
    
        discoBall.GetComponentInChildren<Animator>().SetTrigger(isDiscoBallUsed);
        
        string stopID = Guid.NewGuid().ToString();

        for (int i = 0; i < width; i++)
        {
            columnStopperId[i] = stopID;
            StopCoroutine(FillTheColumn(i));
        }

        discoBall.GetComponent<Fruit>().outsideOfBoard = true;
        allFruits[discoBall.GetComponent<Fruit>().column, discoBall.GetComponent<Fruit>().row] = null;

        List<GameObject> fruitsToDisappear = new List<GameObject>();
        for(int i = 0;i < height; i++)
        {
            for( int j = 0; j < width;j++)
            {
                if (allFruits[j, i] && allFruits[j,i].GetComponent<Fruit>().fruitType == targetFruitType)
                {
                    fruitsToDisappear.Add(allFruits[j,i]);
                }
            }
        }

        List<GameObject> fruitsLeft = new List<GameObject>();
        fruitsLeft.AddRange(fruitsToDisappear);
        int random;
        GameObject fruit;
        Fruit fruitScript;
        List<String> powerUpleft = new List<String>();
        string powerUpLocation;
        if (powerUpCreateType < 0)
        {
            fruitsToDisappear.Clear();
        }

        while (fruitsLeft.Count > 0)
        {
           
            random = UnityEngine.Random.Range(0, fruitsLeft.Count);

            fruit = fruitsLeft[random];
            fruitsLeft.Remove(fruit);

            if (fruit)
            {
                GameObject lightening = Instantiate(lighteningPrefab, discoBall.transform.position, lighteningPrefab.transform.rotation);
                LighteningScript lighteningScript = lightening.GetComponent<LighteningScript>();
                String id = Guid.NewGuid().ToString();
                lighteningScript.id = id;
                fruitScript = fruit.GetComponent<Fruit>();
                fruitScript.selectedID = id;
                lighteningScript.targetV = fruit.transform.position;
                if (powerUpCreateType < 0)
                {
                   
                    powerUpLocation = fruitScript.column.ToString() + "|" + fruitScript.row.ToString();

                    powerUpleft.Add(powerUpLocation);
                    lighteningScript.attachedPowerUpType = powerUpCreateType;
                   
                }              
            }
          
            yield return new WaitForSeconds(speedAndTimeLibrary.discoballWaitBetweenMarkingFruits);
        }

        yield return new WaitForSeconds(speedAndTimeLibrary.afterLastLighteningWaitTimeBeforeDestroyingFruits);

        if (powerUpCreateType < 0)
        {
            int tempColumn, tempRow;
            string[] splitStrings;
            while (powerUpleft.Count > 0)
            {
                random = UnityEngine.Random.Range(0, powerUpleft.Count);

                splitStrings = powerUpleft[random].Split('|');

                // Convert them back to integers
                tempColumn = int.Parse(splitStrings[0]);
                tempRow = int.Parse(splitStrings[1]);
                ActivatePowerUp(allFruits[tempColumn,tempRow]);
                powerUpleft.Remove(powerUpleft[random]);
                yield return new WaitForSeconds(speedAndTimeLibrary.discoballWaitBetweenActivatingMarkedPowerUps);

            }
        }
        else
        {
            while (fruitsToDisappear.Count > 0)
            {
                random = UnityEngine.Random.Range(0, fruitsToDisappear.Count);
                DestroyController(fruitsToDisappear[random], false);
                fruitsToDisappear.Remove(fruitsToDisappear[random]);
                yield return new WaitForSeconds(speedAndTimeLibrary.discoballWaitBetweenDestroyingMarkedFruits);

            }
        }
       
       // yield return new WaitForSeconds(0.05f);
        blockUserMove = false;
        StopHint();

        Array.Clear(columnStopperId, 0, columnStopperId.Length);

        discoBall.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(speedAndTimeLibrary.waitForEffectOfSingleUsedDiscoball);
        Destroy(discoBall);

    }

    public Vector2 GetBoomerangTargetLoc(int column,int row)
    {

        int targetRow=0,targetCol=0;

        int tryNumber = 0;

        do
        {
            int k = 0;

            // Randomly select to stick either row or column. 
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                // Stick to last row

                if (column >= (width - 1) / 2)
                {
                    targetCol = UnityEngine.Random.Range(0, Mathf.RoundToInt(width / 2));
                }
                else
                {
                    targetCol = UnityEngine.Random.Range(Mathf.RoundToInt(width / 2), width);
                }

            
                do{
                  
                    if (row >= (height - 1) / 2)
                    {
                        targetRow = 0+k;
                    }
                    else
                    {
                        targetRow = height - 1 - k;
                    }
                    k++;
                } while (k < ((height - 1) / 2)+1 && !allTiles[targetCol, targetRow]) ;

            }
            else
            {
                // Stick to last column
               

                if (row >= (height - 1) / 2)
                {
                    targetRow = UnityEngine.Random.Range(0, Mathf.RoundToInt(height / 2));
                }
                else
                {
                    targetRow = UnityEngine.Random.Range(Mathf.RoundToInt(height / 2), height);
                }

                do
                {

                    if (column >= (width - 1) / 2)
                    {
                        targetCol = 0 + k;
                    }
                    else
                    {
                        targetCol = width - 1 - k;
                    }
                    k++;
                } while (k < ((width - 1) / 2) + 1 && !allTiles[targetCol, targetRow]);

            }
            tryNumber++;
        } while (!allTiles[targetCol, targetRow] || tryNumber<50);
        
        return allTiles[targetCol, targetRow].transform.position;

    }

    private IEnumerator SunflowerCreationAndMovement(int row)
    {

        Vector2 tempPosition = new Vector2(allTiles[0, row].transform.position.x - 1.5f, allTiles[0, row].transform.position.y);


        GameObject newSpecialPowerUp = Instantiate(specialPowerUps[0], tempPosition, specialPowerUps[0].transform.rotation);
        Fruit newSpecialPowerUpScript = newSpecialPowerUp.GetComponentInChildren<Fruit>();

        newSpecialPowerUpScript.outsideOfBoard = true;
        newSpecialPowerUp.transform.position = tempPosition;
        newSpecialPowerUpScript.damageID = Guid.NewGuid().ToString();

        yield return new WaitForSeconds(speedAndTimeLibrary.sunflowerCreationAnimDuration);

        newSpecialPowerUpScript.targetV.x = allTiles[width-1, row].transform.position.x+5;

        StartCoroutine(StopAndStartAllFillings(speedAndTimeLibrary.sunflowerColumnStopDurationMultiplier * width));

        yield return new WaitForSeconds(speedAndTimeLibrary.waitBeforeDestroyingSunflowerGrassAfterActivated);

        Destroy(newSpecialPowerUp.transform.GetChild(1).gameObject);

        yield return new WaitForSeconds(speedAndTimeLibrary.waitBeforeDestroyingSunflower);

        Destroy(newSpecialPowerUp);

    }

    private Sprite TileWithBorder(int column,int row)
    {
        int nearbyTileAmount = 0;

        bool[] tileNearby = new bool[9];
        int k = 0;
        for (int i = row - 1; i < row + 2; i++)
        {
            for (int j = column - 1; j < column + 2; j++)
            {
                tileNearby[k] = true;
                if (j >= 0 && j < width && i >= 0 && i < height && allTiles[j, i])
                {
                    tileNearby[k] = false;
                }
                k++;
            }
        }


        if (tileNearby[3])
        {
            nearbyTileAmount++;
        }

        if (tileNearby[5])
        {
            nearbyTileAmount++;
        }

        if (tileNearby[7])
        {
            nearbyTileAmount++;
        }

        if (tileNearby[1])
        {
            nearbyTileAmount++;
        }

        // full border tile

        if (nearbyTileAmount == 0)
        {
            return tileSprites[0];
        }

        // default tile non-border

        if (nearbyTileAmount == 4)
        {
            return tileSprites[1];
        }

        /*

          6 7 8
          3 4 5
          0 1 2

       */

        if (nearbyTileAmount == 1)
        {
            if (tileNearby[3])
            {

                if (tileNearby[6])
                {
                    if (!tileNearby[0])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[3];
                    }
                    else
                    {

                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[2];
                    }
                }
                else
                {
                    if (!tileNearby[0])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[4];
                    }
                    else
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        return tileSprites[3];
                     
                    }
                }
            

            }
            else if (tileNearby[7])
            {

                if (tileNearby[6])
                {
                    if (!tileNearby[8])
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[3];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[2];
                    }
                }
                else
                {
                    if (!tileNearby[8])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[4];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[3];

                    }
                }

            }
            else if (tileNearby[5])
            {
                if (tileNearby[2])
                {
                    if (!tileNearby[8])
                    {
                        return tileSprites[3];
                    }
                    else
                    {
                        return tileSprites[2];
                    }
                }
                else
                {
                    if (!tileNearby[8])
                    {
                        return tileSprites[4];
                    }
                    else
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[3];
                    }
                }

            }
            else
            {
                if (tileNearby[2])
                {
                    if (!tileNearby[0])
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[3];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[2];
                    }
                }
                else
                {
                    if (!tileNearby[0])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[4];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[3];
                    }
                }

            }
        }


        if (nearbyTileAmount == 3)
        {
            if (!tileNearby[3])
            {

                if (tileNearby[6])
                {
                    if (!tileNearby[0])
                    {
                        return tileSprites[14];
                    }
                    else
                    {
                        return tileSprites[13];
                    }
                }
                else
                {
                    if (!tileNearby[0])
                    {
                        return tileSprites[15];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        return tileSprites[14];
                    }
                }


            }
            else if (!tileNearby[7])
            {

                if (!tileNearby[6])
                {
                    if (!tileNearby[8])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[15];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[14];
                    }
                }
                else
                {
                    if (!tileNearby[8])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        return tileSprites[14];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[13];
                    }
                }

            }
            else if (!tileNearby[5])
            {
                if (!tileNearby[2])
                {
                    if (!tileNearby[8])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[15];
                    }
                    else
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        return tileSprites[14];
                    }
                }
                else
                {
                    if (!tileNearby[8])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[14];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[13];
                    }
                }

            }
            else
            {
                if (!tileNearby[2])
                {
                    if (!tileNearby[0])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[15];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[14];
                    }
                }
                else
                {
                    if (!tileNearby[0])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        return tileSprites[14];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[13];
                    }
                }

            }
        }


        if (nearbyTileAmount == 2)
        {
            // Crossing tiles

            if (tileNearby[3] && tileNearby[7])
            {
                if (tileNearby[8])
                {
                    if (!tileNearby[0])
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        return tileSprites[11];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[10];
                    }
                }
                else
                {
                    if (!tileNearby[0])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[12];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[11];

                    }
                }
            }

            if (tileNearby[3] && tileNearby[1])
            {
                if (tileNearby[6])
                {
                    if (!tileNearby[2])
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        Vector3 newRotation = new Vector3(0f, 0f, -270f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[11];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[10];
                    }
                }
                else
                {
                    if (!tileNearby[2])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[12];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[11];

                    }
                }
            }

            if (tileNearby[7] && tileNearby[5])
            {
                if (tileNearby[6])
                {
                    if (!tileNearby[2])
                    {
                        return tileSprites[11];
                    }
                    else
                    {
                        return tileSprites[10];
                    }
                }
                else
                {
                    if (!tileNearby[2])
                    {
                        return tileSprites[12];
                    }
                    else
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[11];
                    }
                }
            }

            if (tileNearby[1] && tileNearby[5])
            {
                if (tileNearby[0])
                {
                    if (!tileNearby[8])
                    {
                        allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                        Vector3 newRotation = new Vector3(0f, 0f, -180f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[11];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[10];
                    }
                }
                else
                {
                    if (!tileNearby[8])
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[12];
                    }
                    else
                    {
                        Vector3 newRotation = new Vector3(0f, 0f, -90f);
                        allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                        return tileSprites[11];

                    }
                }
            }

            // Face to face direction

            if (tileNearby[3] && tileNearby[5])
            {
                if (tileNearby[0] && tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    return tileSprites[5];
                }

                // one missing tile

                if (!tileNearby[0] && tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -180f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[6];
                }

                if (tileNearby[0] && !tileNearby[6] && tileNearby[2] && tileNearby[8])
                {                  
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[6];
                }

                if (tileNearby[0] && tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -180f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[6];
                }

                if (tileNearby[0] && tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    return tileSprites[6];
                }

                // two missing tile

                if (!tileNearby[0] && !tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -180f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[7];
                }

                if (tileNearby[0] && !tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    return tileSprites[17];
                }

                if (tileNearby[0] && tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    return tileSprites[7];
                }

                if (!tileNearby[0] && tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[17];
                }

                if (!tileNearby[0] && tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -180f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[16];
                }

                if (tileNearby[0] && !tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    return tileSprites[16];
                }

                // three missing tile

                if (!tileNearby[0] && !tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -180f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[8];
                }

                if (tileNearby[0] && !tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    return tileSprites[8];
                }

                if (!tileNearby[0] && tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -180f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[8];
                }

                if (!tileNearby[0] && !tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {                 
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[8];
                }

                // four missing tile

                if (!tileNearby[0] && !tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    return tileSprites[9];
                }

            }

            if (tileNearby[1] && tileNearby[7])
            {
                if (tileNearby[0] && tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[5];
                }

                // one missing tile

                if (!tileNearby[0] && tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -270f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[6];
                }

                if (tileNearby[0] && !tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -270f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[6];
                }

                if (tileNearby[0] && tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[6];
                }

                if (tileNearby[0] && tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[6];
                }

                // two missing tile

                if (!tileNearby[0] && !tileNearby[6] && tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -270f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[16];
                }

                if (tileNearby[0] && !tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[17];
                }

                if (tileNearby[0] && tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[16];
                }

                if (!tileNearby[0] && tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[17];
                }

                if (!tileNearby[0] && tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[7];
                }

                if (tileNearby[0] && !tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -270f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[7];
                }

                // three missing tile

                if (!tileNearby[0] && !tileNearby[6] && !tileNearby[2] && tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -270f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[8];
                }

                if (tileNearby[0] && !tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    allTiles[column, row].GetComponent<SpriteRenderer>().flipX = true;
                    return tileSprites[8];
                }

                if (!tileNearby[0] && tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[8];
                }

                if (!tileNearby[0] && !tileNearby[6] && tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -270f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[8];
                }

                // four missing tile

                if (!tileNearby[0] && !tileNearby[6] && !tileNearby[2] && !tileNearby[8])
                {
                    Vector3 newRotation = new Vector3(0f, 0f, -90f);
                    allTiles[column, row].transform.rotation = Quaternion.Euler(newRotation);
                    return tileSprites[9];
                }

            }

        }

        return tileSprites[0];
    }

    private IEnumerator HarvesterTNTMerge(GameObject harvester,GameObject TNT,int column,int row, bool verticalHarvester,bool horizontalSwipe)
    {
        harvester.GetComponentInChildren<SpriteRenderer>().enabled = false;
        TNT.transform.GetChild(0).gameObject.SetActive(false);
        TNT.transform.GetChild(1).gameObject.SetActive(true);
        if(!verticalHarvester)
        {
            Vector3 newRotation = new Vector3(0f, 0f, 90f);
            TNT.transform.GetChild(1).GetChild(1).transform.rotation = Quaternion.Euler(newRotation);
        }

        if (horizontalSwipe)
        {
            TNT.transform.GetChild(1).GetComponent<Animator>().SetTrigger(TNTHarvesterHorizontalMerge);
        }
        else
        {
            TNT.transform.GetChild(1).GetComponent<Animator>().SetTrigger(TNTHarvesterVerticalMerge);
        }
        yield return new WaitForSeconds(speedAndTimeLibrary.harvesterTNTMergeAnimDuration);
        harvester.GetComponentInChildren<SpriteRenderer>().enabled = true;
        Destroy(TNT);

     //   StartCoroutine(StopAndStartSingleColumn(0.3f, column));

        GameObject PowerUp1 = null, PowerUp2 = null;       

        if (verticalHarvester)
        {
            if (column + 1 < width && allTiles[column + 1, row])
            {
             //   StartCoroutine(StopAndStartSingleColumn(0.3f, column + 1));
                if (allFruits[column + 1, row])
                {
                    DestroyController(allFruits[column + 1, row], false);
                }
                PowerUp1 = CreatePowerUp(column + 1, row, -2,false);
            }

            if (column - 1 >= 0 && allTiles[column - 1, row])
            {
             //   StartCoroutine(StopAndStartSingleColumn(0.3f, column - 1));
                if (allFruits[column - 1, row])
                {
                    DestroyController(allFruits[column - 1, row], false);
                }
                PowerUp2 = CreatePowerUp(column - 1, row, -2, false);
            }
        }
        else
        {
            if (row + 1 < height && allTiles[column, row + 1])
            {
                if (allFruits[column, row + 1])
                {
                    DestroyController(allFruits[column, row + 1], false);
                }
                PowerUp1 = CreatePowerUp(column, row + 1, -1, false);
            }

            if (row - 1 >= 0 && allTiles[column, row - 1])
            {
                if (allFruits[column, row - 1])
                {
                    DestroyController(allFruits[column, row - 1], false);
                }
                PowerUp2 = CreatePowerUp(column, row - 1, -1, false);
            }
        }

      

    //    yield return new WaitForSeconds(0.1f);

        if (PowerUp1)
        {
            ActivatePowerUp(PowerUp1);
        }
        ActivatePowerUp(harvester);
        if (PowerUp2)
        {
            ActivatePowerUp(PowerUp2);
        }
        audioManager.Harvester();

    }

    private IEnumerator WaitAndDestroyObj(GameObject obj,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(obj);
    }

}



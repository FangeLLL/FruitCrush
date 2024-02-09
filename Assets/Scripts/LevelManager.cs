using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    static public int width = 5;
    static public int height = 5;
    public int chosenId;

    public int widthChanger = width;
    public int heightChanger = height;

    public float scaleNumber;
    public float scaleFactorFruit;

    IndexLibrary indexLibrary = new IndexLibrary();

    [SerializeField]
    public int[] existFruits;
    public GameObject[] fruitCheckboxes;
    public GameObject[] fruits;
    public GameObject[] obstacles;
    public GameObject tilePrefab;

    public GameObject[,] allFruits;
    public GameObject[,] allTiles;

    public int moveCount = 20;
    public int[] taskElements;

    private SaveData saveData;
    private SaveData loadData = new SaveData();

    static public int level = 1;
    public int levelChanger = level;


    static bool uploadLevel = false;


    private void Start()
    {
        taskElements = new int[obstacles.Length];
        saveData = GetComponent<SaveData>();
        existFruits = new int[fruits.Length];

        levelChanger = level;

        moveCount = 20;

        if (uploadLevel)
        {
            uploadLevel = false;
            loadData.LoadFromJson();
            int indexLevel = level;
            indexLevel--;
            Grid gridData = loadData.gridData[indexLevel];

            if (level == gridData.level)
            {
                width = gridData.width;
                height = gridData.height;

                ArrangeValuesRelatedToSizes();

                // Getting avaliable fruits indexes and adding them to a list. existFruits list has indexes of avaliable fruits and this list going to be used when
                // creating new fruits.

                existFruits = gridData.fruits;

                for (int i = 0; i < existFruits.Length; i++)
                {
                    if (existFruits[i] == 1)
                    {
                        fruitCheckboxes[i].GetComponent<Image>().color = new UnityEngine.Color(0, 255, 0, 255);
                    }
                }

                int[] savedTiles = gridData.allTilesTotal;
                int[] savedFruits = gridData.allFruitsTotal;
                int[] savedTilesZero = gridData.tilesIndexZero;
                int[] savedTilesOne = gridData.tilesIndexOne;
                int[] savedTilesTwo = gridData.tilesIndexTwo;

                taskElements = gridData.taskElements;
                moveCount = gridData.moveCount;

                SetUpWithArray(indexLibrary.Convert2DTo3D(width, height, savedFruits), indexLibrary.Convert2DTo3D(width, height, savedTiles), indexLibrary.Convert2DTo3D(width, height, savedTilesZero), indexLibrary.Convert2DTo3D(width, height, savedTilesOne), indexLibrary.Convert2DTo3D(width, height, savedTilesTwo));

            }
            else
            {
                ArrangeValuesRelatedToSizes();
                Debug.Log("This level never saved so it will setup level randomly");
                SetUp();
            }

        }
        else
        {
            ArrangeValuesRelatedToSizes();
            SetUp();
        }


    }

    private void Update()
    {
        // Changing static variables from inspector
        level = levelChanger;
        width = widthChanger;
        height = heightChanger;
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveTheBoard(level);
        }

    }

    private void ArrangeValuesRelatedToSizes()
    {
        allFruits = new GameObject[width, height];
        allTiles = new GameObject[width, height];
        RearrangeScaleNumber();
        widthChanger = width;
        heightChanger = height;
    }

    private void SaveTheBoard(int saveLevel)
    {
        saveLevel--;
        saveData.gridData[saveLevel] = new Grid();

        saveData.gridData[saveLevel].width = width;
        saveData.gridData[saveLevel].height = height;
        saveData.gridData[saveLevel].level = saveLevel + 1;
        saveData.gridData[saveLevel].moveCount = moveCount;
        saveData.gridData[saveLevel].taskElements = taskElements;

        int[] arrangeFruits = new int[width * height];
        int[] arrangeTiles = new int[width * height];

        int[] arrangeTilesIndexZero = new int[width * height];
        int[] arrangeTilesIndexOne = new int[width * height];
        int[] arrangeTilesIndexTwo = new int[width * height];

        saveData.gridData[saveLevel].fruits = existFruits;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // If fruit does not exist then type will be -1 .
                if (allFruits[i, j])
                {
                    arrangeFruits[(width * i) + j] = allFruits[i, j].GetComponent<Fruit>().fruitType;
                }
                else
                {
                    arrangeFruits[(width * i) + j] = -1;
                }

                // If tile does not exist then type will be 0.

                if (allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().active)
                {
                    arrangeTiles[(width * i) + j] = 1;


                    // Checking if the tile has obstacle box. And if it has it then placing obstacle id to proper data place.
                    if (allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[0])
                    {
                        arrangeTilesIndexZero[(width * i) + j] = allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[0].
                            GetComponent<ObstacleScript>().obstacleSpecs.id;
                    }
                    else
                    {
                        arrangeTilesIndexZero[(width * i) + j] = -1;
                    }
                    // Checking if the tile has obstacle wheatfarm (or something like that).
                    if (allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[1])
                    {
                        arrangeTilesIndexOne[(width * i) + j] = allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[1].
                            GetComponent<ObstacleScript>().obstacleSpecs.id;
                    }
                    else
                    {
                        arrangeTilesIndexOne[(width * i) + j] = -1;
                    }
                    // Spare place.
                    if (allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[2])
                    {
                        arrangeTilesIndexTwo[(width * i) + j] = allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[2].
                            GetComponent<ObstacleScript>().obstacleSpecs.id;
                    }
                    else
                    {
                        arrangeTilesIndexTwo[(width * i) + j] = -1;
                    }

                }
                else
                {
                    arrangeTiles[(width * i) + j] = 0;
                }

            }
        }

        saveData.gridData[saveLevel].allFruitsTotal = arrangeFruits;
        saveData.gridData[saveLevel].allTilesTotal = arrangeTiles;
        saveData.gridData[saveLevel].tilesIndexZero = arrangeTilesIndexZero;
        saveData.gridData[saveLevel].tilesIndexOne = arrangeTilesIndexOne;
        saveData.gridData[saveLevel].tilesIndexTwo = arrangeTilesIndexTwo;
        saveData.SaveToJson();
    }

    private void SetUpWithArray(int[,] arrangedFruits, int[,] arrangedTiles, int[,] arrangedTilesZero, int[,] arrangedTilesOne, int[,] arrangedTilesTwo)
    {

        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        tilePrefab.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);

        foreach (GameObject fruitScale in fruits)
        {
            fruitScale.transform.localScale = new Vector3(scaleFactorFruit, scaleFactorFruit, scaleFactorFruit);
        }

        foreach (GameObject obstacleScale in obstacles)
        {
            obstacleScale.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
        }
        
        // Creating tiles.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, j * scaleNumber - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                backgroundTile.GetComponent<LevelEditorBackgroundTile>().column = i;
                backgroundTile.GetComponent<LevelEditorBackgroundTile>().row = j;
                allTiles[i, j] = backgroundTile;

               
            }
        }

        // Creating obstacles and fruits.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
                if (arrangedTiles[i, j] == 1)
                {
                    GameObject tempObstacle = null;
                    Vector2 tempPosition = allTiles[i, j].transform.position;

                    int repeat = 1;

                    int arrangedTilesIndex = 0;

                    for(int c = 0; c < 3; c++)
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

                        // Checking if layer has obstacle and that tile has a obstacle in its that layer. 
                        if (arrangedTilesIndex != -1 && !allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().obstacles[c])
                        {
                            // If obstacle is big obstacle then it will arrange position and assign this object 4 tile. 
                            if (obstacles[arrangedTilesIndex].GetComponent<ObstacleScript>().obstacleSpecs.is4TimesBigger)
                            {
                                tempPosition = new Vector2((i + 0.5f) * scaleNumber - xOffset, (j + 0.5f) * scaleNumber - yOffset);
                                repeat = 2;
                            }
                            else
                            {
                                tempPosition = allTiles[i, j].transform.position;
                                repeat = 1;
                            }
                            tempObstacle = Instantiate(obstacles[arrangedTilesIndex], tempPosition, Quaternion.identity);

                            for (int a = 0; a < repeat; a++)
                            {
                                for (int b = 0; b < repeat; b++)
                                {
                                    // Assigning object to relevant tile/tiles.
                                    allTiles[i + b, j + a].GetComponent<LevelEditorBackgroundTile>().obstacles[c] = tempObstacle;

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
                    }

                }
                else
                {
                    allTiles[i, j].GetComponent<LevelEditorBackgroundTile>().active = false;
                    Color color = allTiles[i, j].GetComponent<SpriteRenderer>().color;
                    color.a = 0.5f;
                    allTiles[i, j].GetComponent<SpriteRenderer>().color = color;
                }

            }
        }


       
    }

    private void SetUp()
    {
        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        tilePrefab.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);

        foreach (GameObject fruitScale in fruits)
        {
            fruitScale.transform.localScale = new Vector3(scaleFactorFruit, scaleFactorFruit, scaleFactorFruit);
        }

        foreach (GameObject obstacleScale in obstacles)
        {
            obstacleScale.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i * scaleNumber - xOffset, j * scaleNumber - yOffset);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.GetComponent<LevelEditorBackgroundTile>().row = j;
                backgroundTile.GetComponent<LevelEditorBackgroundTile>().column = i;
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

    public void ReplaceObject(int column, int row)
    {
        if (!allTiles[column,row].GetComponent<LevelEditorBackgroundTile>().active) {
            Debug.Log("This tile deactivated please activate this tile before putting something in.");
            return;
        }

        float xOffset = width * scaleNumber * 0.5f - scaleNumber * 0.5f;
        float yOffset = height * scaleNumber * 0.5f - 0.5f + 1.1f;
        Vector2 tempPosition;
        GameObject obstacle = null;
        bool isObstacle4TimesBigger = false;

        // If the chosen asset is obstacle and obstacle is 4 times bigger obstacle then placed it center of four tile. 
        if (chosenId < 0 && obstacles[(-chosenId) - 1].GetComponent<ObstacleScript>().obstacleSpecs.is4TimesBigger)
        {
            isObstacle4TimesBigger = true;
            tempPosition = new Vector2((column + 0.5f) * scaleNumber - xOffset, (row + 0.5f) * scaleNumber - yOffset);
        }
        else
        {
            tempPosition = new Vector2(column * scaleNumber - xOffset, row * scaleNumber - yOffset);
        }

        if(isObstacle4TimesBigger && (column+1>=width || row + 1 >= height))
        {
            Debug.Log("You can't put big obstacle in that tile. It exceeds the limit of board");
            return;
        }

        // INSTANTIATE A NEW FRUIT AT THE POSITION OF THE DESTROYED FRUIT
        if (chosenId >= 0)
        {
            if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[0])
            {
                // Destroy box
                Destroy(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[0]);
            }
            GameObject fruit = Instantiate(fruits[chosenId], tempPosition, Quaternion.identity);
            fruit.transform.parent = this.transform;
            fruit.name = "( " + column + ", " + row + " )";

            // SET THE COLUMN AND ROW OF THE NEW FRUIT
            fruit.GetComponent<Fruit>().column = column;
            fruit.GetComponent<Fruit>().row = row;
            fruit.GetComponent<Fruit>().fruitType = chosenId;
            // ADD THE NEW FRUIT TO THE ALLFRUITS ARRAY
            Destroy(allFruits[column, row]);
            allFruits[column, row] = fruit;
        }
        else
        {
            // OBSTACLE IDS START FROM -1 SO SYSTEM GET NEGATIVE OF IT AND MINUS 1
            int obstacleIndex = (-chosenId) - 1;
            //  GETTING CHOSEN OBSTACLEN PREFAB INDEX LOCATION ON THE TILE OBSTACLE VARIABLE.
            int placeOfObstacle = obstacles[obstacleIndex].GetComponent<ObstacleScript>().obstacleSpecs.indexOfLayer;
            int currentObstacleId = -1;
            if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[placeOfObstacle])
            {
                currentObstacleId = allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[placeOfObstacle].GetComponent<ObstacleScript>().obstacleSpecs.id;

            }

            int currentObstacleTaskID;

            // If obstacle is4TimesBigger then repeat cehck and destroy method 4 times.
            int repeat = isObstacle4TimesBigger ? 2 : 1;

            // IF CHOSEN OBSTACLE ALREADY CRATED IN THAT TILE THEN IT MEANS USER WANTED TO DESTROY IT.
            if (obstacleIndex != currentObstacleId)
            {
                obstacle = Instantiate(obstacles[obstacleIndex], tempPosition, Quaternion.identity);
                obstacle.transform.parent = this.transform;
                obstacle.name = "( " + column + ", " + row + " )";

                if (obstacle.GetComponent<ObstacleScript>().obstacleSpecs.isCollectible)
                {
                    // If obstacle collectiable type of obstacle then system must add amount of collectiable things to task. 
                    int[] collectArray = obstacle.GetComponent<ObstacleScript>().obstacleSpecs.amountOfCollect;
                    int totalCollect = 0;
                    for (int i = 0; i < collectArray.Length; i++)
                    {
                        totalCollect += collectArray[i];
                    }
                    taskElements[obstacle.GetComponent<ObstacleScript>().obstacleSpecs.taskID] += totalCollect;
                }
                else
                {
                    taskElements[obstacle.GetComponent<ObstacleScript>().obstacleSpecs.taskID]++;
                }

                if (obstacle.GetComponent<ObstacleScript>().obstacleSpecs.spreadWheatfarm)
                {
                    taskElements[1]+=16;
                }

            }
            else
            {
                // If system will just destroy the obstacle then there is no need to repeat it.
                repeat = 1;
            }

            // IF THERE IS A OBSTACLE ALREADY EXÝST IN THE CURRENT PLACE THEN DESTROY THE OBSTACLE 

            if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[placeOfObstacle])
            {
                currentObstacleId = allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[placeOfObstacle].GetComponent<ObstacleScript>().obstacleSpecs.id;
                RemoveTasks(column,row,placeOfObstacle);
            }

            for (int i = 0; i < repeat; i++)
            {
                for (int j = 0; j < repeat; j++)
                {     

                    // MAKE THE ASSIGMENTS
                    if (obstacleIndex != currentObstacleId)
                    {

                        allTiles[column + j, row + i].GetComponent<LevelEditorBackgroundTile>().obstacles[placeOfObstacle] = obstacle;

                    }

                    // IF OBSTACLE INDEX OF PLACE IS 0 THEN IT MEANS IT IS A BOX TYPE OBSTACLE SO SYSTEM MUST DESTROY FRUIT IF FRUIT EXIST.

                    if (placeOfObstacle == 0)
                    {
                        if (allFruits[column + j, row + i])
                        {
                            Destroy(allFruits[column + j, row + i]);
                        }
                    }
                }
            }

        }
    }

    public void DeactivateTile(int column,int row)
    {
        // If tile is active then system must deactivate and if its deactive then it must activate it.
        if(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().active)
        {
            // Destroy if there is a fruit.
            if (allFruits[column, row])
            {
                Destroy(allFruits[column, row]);
            }

            // Destroying tile's obstacles.
            for (int i = 0; i < 3; i++)
            {
                if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[i])
                {
                    RemoveTasks(column, row,i);
                }
            }

            allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().active = false;
            Color color = allTiles[column, row].GetComponent<SpriteRenderer>().color;
            color.a = 0.5f;
            allTiles[column, row].GetComponent<SpriteRenderer>().color = color;
        }
        else
        {


            allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().active = true;
            Color color = allTiles[column, row].GetComponent<SpriteRenderer>().color;
            color.a = 1;
            allTiles[column, row].GetComponent<SpriteRenderer>().color = color;

        }

    }

    public void CheckBox(int id)
    {
        if (existFruits[id] == 0)
        {
            fruitCheckboxes[id].GetComponent<Image>().color = new UnityEngine.Color(0, 255, 0, 255);
            existFruits[id] = 1;
        }
        else
        {
            fruitCheckboxes[id].GetComponent<Image>().color = new UnityEngine.Color(255, 0, 0, 255);
            existFruits[id] = 0;
        }
    }

    public void GetObject(int id)
    {
        /*
         Id:
        0 - Banana
        1 - Cherry
        2 - Grape
        3 - Orange
        4 - Avocado

        -1 - Strawbale
        -2 - Wheat Farm
          
          */

        chosenId = id;

    }

    public void RestartScene()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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

    public void UploadLevel()
    {
        uploadLevel = true;
        RestartScene();
    }

    /// <summary>
    /// Removing tasks realted to that obstacle and destroying obstacle
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="layer"></param>
    private void RemoveTasks(int column,int row,int layer)
    {
        int currentObstacleTaskID = allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[layer].GetComponent<ObstacleScript>().obstacleSpecs.taskID;
        if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[layer].GetComponent<ObstacleScript>().obstacleSpecs.isCollectible)
        {
            // If obstacle collectiable type of obstacle then system must extract amount of collectiable things from task. 
            int[] collectArray = allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[layer].GetComponent<ObstacleScript>().obstacleSpecs.amountOfCollect;
            int totalCollect = 0;
            for (int j = 0; j < collectArray.Length; j++)
            {
                totalCollect += collectArray[j];
            }
            taskElements[currentObstacleTaskID] -= totalCollect;
        }
        else
        {
            taskElements[currentObstacleTaskID]--;
        }

        if (allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[layer].GetComponent<ObstacleScript>().obstacleSpecs.spreadWheatfarm)
        {
            taskElements[1] -= 16;
        }
        Destroy(allTiles[column, row].GetComponent<LevelEditorBackgroundTile>().obstacles[layer]);
    }

}

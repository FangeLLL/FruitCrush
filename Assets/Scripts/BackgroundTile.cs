using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    private Board board;
 
    public int tileType=0;
    public int row, column;
    public GameObject[] obstacles = new GameObject[3];
    public int indexOfVisibleOne=-1;
    public bool isCurrentObstacleBox=false;

    // it used for detected if this on borders of table.
    public bool border =false;
    private Vector2 firstTouchPosition;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        board = FindObjectOfType<Board>();
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0) && board.specialPowerID != 0)
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos) && !board.specialSwipe)
            {
                board.ActivateSpecialPower(column, row);
            }
        }
        */

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Fruit fruitScript = other.GetComponent<Fruit>();

        if (fruitScript.fruitType == -4)
        {
            if (board.FruitAvailable(board.allFruits[column, row]))
            {
                audioManager.FruitCrush();
                board.DestroyController(board.allFruits[column, row], false);
            }
        }
        else
        {
            if (board.FruitAvailableWithoutDistanceCheck(board.allFruits[column, row]))
            {
                audioManager.FruitCrush();
                board.DestroyController(board.allFruits[column, row], false);
            }
        }
            

        if (indexOfVisibleOne>=0)
        {
            PowerUpBoom(fruitScript.damageID);
            if (fruitScript.fruitType==-4 && fruitScript.attachedPowerUp)
            {
                Fruit powerUpScript = fruitScript.attachedPowerUp.GetComponent<Fruit>();
                powerUpScript.targetV = transform.position;
                powerUpScript.row = row;
                powerUpScript.column = column;
                powerUpScript.moveToward = false;
                board.ActivatePowerUp(fruitScript.attachedPowerUp);

                Destroy(other.gameObject);

            }
        }


        if (other && border)
        {
            //  Vector2 vector2 = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

            if (fruitScript.fruitType == -4)
            {
                fruitScript.hitBorder++;
                if (fruitScript.hitBorder == 2)
                {
                    if (fruitScript.attachedPowerUp)
                    {
                        Fruit powerUpScript = fruitScript.attachedPowerUp.GetComponent<Fruit>();
                        powerUpScript.targetV = transform.position;
                        powerUpScript.row = row;
                        powerUpScript.column = column;
                        powerUpScript.moveToward = false;
                        board.ActivatePowerUp(fruitScript.attachedPowerUp);
                    }
                    StartCoroutine(board.FadeOut(other.gameObject));
                }
                else
                {
                    fruitScript.targetV = board.GetBoomerangTargetLoc(column, row);

                }
            }
            else
            {
                StartCoroutine(WaitAndReleaseColumnForFilling(fruitScript.fruitType, fruitScript.column, other.gameObject));
            }
        }
       
        // PowerUpBoom(other.GetComponent<Fruit>().damageID);
    }

    private IEnumerator WaitAndReleaseColumnForFilling(int type,int column,GameObject obj)
    {
        yield return new WaitForSeconds(0.3f);

        Destroy(obj);

        if (type == -2)
        {

            board.fillingColumn[column] = false;
        }

        if (type == -1)
        {

            Array.Clear(board.fillingColumn, 0, board.fillingColumn.Length);
        }
    }

    private void OnMouseDown()
    {
        if (board.specialPowerID == 0 || board.specialSwipe)
        {
            if (board.allFruits[column, row])
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                board.allFruits[column, row].GetComponent<Fruit>().firstTouchPosition = firstTouchPosition;
                board.allFruits[column, row].GetComponent<Fruit>().isClicked = true;
            }
        }
    }

    private void OnMouseUp()
    {

        if (board.specialPowerID == 0 || board.specialSwipe)
        {
            if (board.allFruits[column, row])
            {
                if (!board.blockUserMove && board.allFruits[column, row].GetComponent<Fruit>().isClicked&&!board.specialSwipe && board.allFruits[column, row].GetComponent<Fruit>().fruitType < 0 && board.taskController.moveCount > 0 && !board.allFruits[column, row].GetComponent<Fruit>().isSwiped && board.taskController.isBoardActive)
                {
                    if (Vector2.Distance(transform.position, firstTouchPosition) < 0.3f)
                    {
                        board.taskController.MovePlayed();
                        board.StopHint();
                        board.ActivatePowerUp(board.allFruits[column, row],0,false);

                    }              
                }
                if (board.allFruits[column, row])
                {
                    board.allFruits[column, row].GetComponent<Fruit>().isClicked = false;

                }

            }
        }
        else
        {
            board.ActivateSpecialPower(column, row);

        }

    }

    public void Explosion(int column,int row, string damageID,string colorType)
    {

        for(int i=-1; i<2; i+=2)
        {
            if (column + i<board.width && column + i>=0 && board.allTiles[column + i, row])
            {
                if (board.allTiles[column + i, row].GetComponent<BackgroundTile>().indexOfVisibleOne==0)
                {
                    board.allTiles[column + i, row].GetComponent<BackgroundTile>().Boom(damageID,colorType);
                }
            }
            
        }

        for (int i = -1; i < 2; i += 2)
        {
            if (row + i < board.height && row + i >= 0 && board.allTiles[column, row + i])
            {
                if (board.allTiles[column, row + i].GetComponent<BackgroundTile>().indexOfVisibleOne==0)
                {
                    board.allTiles[column, row + i].GetComponent<BackgroundTile>().Boom(damageID, colorType);
                }
            }      
        }

        Boom(damageID, colorType);

    }
 
    public void Boom(string damageID,string colorType)
    {
        if (indexOfVisibleOne>=0 && !obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.powerUpNeed)
        {
            string obstacleColorType = obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.colorType;
            if (string.IsNullOrEmpty(obstacleColorType) || colorType==obstacleColorType)
            {
                obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().TakeDamage(damageID);
                DetectVisibleOne();
            }       
        }
    }

    public void PowerUpBoom(string damageID)
    {
        if (indexOfVisibleOne >= 0)
        {
            obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().TakeDamage(damageID);
            DetectVisibleOne();
        }
    }

    /// <summary>
    /// If one of the obstacle of tile has been destroyed then detect the current visible one.   
    /// </summary>
    public void DetectVisibleOne()
    {
        int tempIndex=-1;

        for(int i=0;i<obstacles.Length;i++)
        {
            // Checking if obstacle exist and after if its health bigger then zero because when obstacles destroy they will destroy by fadeout function and it
            // takes a little time to disappear so system must check its health.
            if (obstacles[i] && obstacles[i].GetComponent<ObstacleScript>().health>0)
            {
                tempIndex = i;
                i = obstacles.Length;
            }
        }
       
        indexOfVisibleOne = tempIndex;
        // If there is no obstacle left then isCurrentObstacleBox variable needs to be false.
        if (indexOfVisibleOne >= 0)
        {
            isCurrentObstacleBox = obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.boxObstacle;

        }
        else
        {
            isCurrentObstacleBox = false;

        }
    }

   

}

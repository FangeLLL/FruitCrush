using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    private Board board;

    public int tileType = 0;
    public int row, column;
    public GameObject[] obstacles = new GameObject[3];
    public int indexOfVisibleOne = -1;
    public bool isCurrentObstacleBox = false;

    public bool isTempEmptyTile = false;

    private Vector2 firstTouchPosition;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        board = FindObjectOfType<Board>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Fruit fruitScript;

        if (!(fruitScript = other.GetComponent<Fruit>()))
        {
            return;
        }

        if (!fruitScript.activePowerUp)
        {
            return;
        }

        if (indexOfVisibleOne >= 0)
        {
            PowerUpBoom(fruitScript.damageID);
            if (fruitScript.fruitType == -4 && fruitScript.attachedPowerUp)
            {
                GameObject powerUp = fruitScript.attachedPowerUp;
                Fruit powerUpFruitScript = powerUp.GetComponent<Fruit>();
                powerUpFruitScript.targetV = transform.position;
                powerUp.transform.position = transform.position;
                powerUp.transform.GetChild(0).gameObject.SetActive(true);
                powerUpFruitScript.row = row;
                powerUpFruitScript.column = column;
                board.ActivatePowerUp(fruitScript.attachedPowerUp);

                Destroy(other.gameObject);

            }
        }


        if (other)
        {

            if (fruitScript.fruitType == -4)
            {
                Vector2 vector2 = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
                if (fruitScript.targetV == vector2)
                {
                    fruitScript.hitBorder++;
                    if (fruitScript.hitBorder == 2)
                    {
                        if (fruitScript.attachedPowerUp)
                        {
                            GameObject powerUp = fruitScript.attachedPowerUp;
                            Fruit powerUpFruitScript = powerUp.GetComponent<Fruit>();
                            powerUpFruitScript.targetV = transform.position;
                            powerUp.transform.position = transform.position;
                            powerUp.transform.GetChild(0).gameObject.SetActive(true);
                            powerUpFruitScript.row = row;
                            powerUpFruitScript.column = column;
                            board.ActivatePowerUp(fruitScript.attachedPowerUp);
                        }
                        StartCoroutine(DestroyBoomerangSlowly(other.gameObject));
                    }
                    else
                    {
                        StartCoroutine(ChangeDirectionOfBoomerangWithDelay(fruitScript));
                    }
                }
            }
        }
    }

    private IEnumerator DestroyBoomerangSlowly(GameObject boomerang)
    {
        yield return new WaitForSeconds(0.1f);
        boomerang.GetComponent<BoxCollider2D>().enabled = false;
        boomerang.transform.GetChild(0).gameObject.SetActive(false);
        boomerang.transform.GetChild(2).gameObject.SetActive(false);
        boomerang.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
        boomerang.transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>().Play();
        boomerang.transform.GetChild(1).GetChild(2).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2.9f);
        Destroy(boomerang.gameObject);
    }

    private IEnumerator ChangeDirectionOfBoomerangWithDelay(Fruit boomerangScript)
    {

        yield return new WaitForSeconds(0.15f);
        boomerangScript.targetV = board.GetBoomerangTargetLoc(column, row);

    }

    private void OnMouseDown()
    {
        if (board.specialPowerID == 0 || board.specialSwipe)
        {
            if (board.allFruits[column, row])
            {
                board.selectedFruit = board.allFruits[column, row];
                ChangeFruitMask(board.selectedFruit,true);
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                board.allFruits[column, row].GetComponent<Fruit>().firstTouchPosition = firstTouchPosition;
                board.allFruits[column, row].GetComponent<Fruit>().isClicked = true;
            }
        }
    }

    private void OnMouseUp()
    {

        if (board.selectedFruit)
        {
            ChangeFruitMask(board.selectedFruit, false);
        }

        if (board.specialPowerID == 0 || board.specialSwipe)
        {
            if (!board.blockUserMove && board.FruitAvailableWithoutTypeCheck(board.allFruits[column, row]) && board.allFruits[column, row].GetComponent<Fruit>().isClicked && !board.specialSwipe && board.allFruits[column, row].GetComponent<Fruit>().fruitType < 0 && board.taskController.moveCount > 0 && !board.allFruits[column, row].GetComponent<Fruit>().isSwiped && board.taskController.isBoardActive)
            {
                if (Vector2.Distance(transform.position, firstTouchPosition) < 0.5f && board.selectedFruit == board.allFruits[column, row])
                {
                    board.taskController.MovePlayed();
                    board.StopHint();
                    board.ActivatePowerUp(board.allFruits[column, row], 0, false);

                }
            }
            if (board.allFruits[column, row])
            {
                board.allFruits[column, row].GetComponent<Fruit>().isClicked = false;

            }
        }
        else
        {
            board.ActivateSpecialPower(column, row);

        }

    }

    public void Explosion(int column, int row, string damageID, string colorType)
    {

        for (int i = -1; i < 2; i += 2)
        {
            if (column + i < board.width && column + i >= 0 && board.allTiles[column + i, row])
            {
                if (board.allTiles[column + i, row].GetComponent<BackgroundTile>().indexOfVisibleOne == 0)
                {
                    board.allTiles[column + i, row].GetComponent<BackgroundTile>().Boom(damageID, colorType);
                }
                if (board.FruitAvailableWithoutTypeCheck(board.allFruits[column + i, row]) && board.allFruits[column + i, row].GetComponent<Fruit>().fruitType < -100)
                {
                    board.allFruits[column + i, row].GetComponent<ObstacleScript>().TakeDamage(damageID);
                }
            }

        }

        for (int i = -1; i < 2; i += 2)
        {
            if (row + i < board.height && row + i >= 0 && board.allTiles[column, row + i])
            {
                if (board.allTiles[column, row + i].GetComponent<BackgroundTile>().indexOfVisibleOne == 0)
                {
                    board.allTiles[column, row + i].GetComponent<BackgroundTile>().Boom(damageID, colorType);
                }
                if (board.FruitAvailableWithoutTypeCheck(board.allFruits[column, row + i]) && board.allFruits[column, row + i].GetComponent<Fruit>().fruitType < -100)
                {
                    board.allFruits[column, row + i].GetComponent<ObstacleScript>().TakeDamage(damageID);
                }
            }
        }

        Boom(damageID, colorType);

    }

    public void Boom(string damageID, string colorType)
    {
        if (indexOfVisibleOne >= 0 && !obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.powerUpNeed)
        {
            string obstacleColorType = obstacles[indexOfVisibleOne].GetComponent<ObstacleScript>().obstacleSpecs.colorType;
            if (string.IsNullOrEmpty(obstacleColorType) || colorType == obstacleColorType)
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
        int tempIndex = -1;

        for (int i = 0; i < obstacles.Length; i++)
        {
            // Checking if obstacle exist and after if its health bigger then zero because when obstacles destroy they will destroy by fadeout function and it
            // takes a little time to disappear so system must check its health.
            if (obstacles[i] && obstacles[i].GetComponent<ObstacleScript>().health > 0)
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

    private void ChangeFruitMask(GameObject fruit,bool showMask)
    {
        GameObject fruitImage = fruit.transform.GetChild(0).gameObject;
        for (int i = 0; i < fruitImage.transform.childCount; i++)
        {
            if (fruitImage.transform.GetChild(i).CompareTag("Fruit Mask"))
            {
                if (showMask)
                {
                    fruitImage.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255,255);
                }
                else
                {
                    fruitImage.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
                }
                return;
            }
        }
    }
}

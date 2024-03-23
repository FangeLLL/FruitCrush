using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fruit : MonoBehaviour
{

    public int fruitType;
    private Vector2 tempPosition;

    public int column;
    public int row;

    private float swipeAngle = 0;

    private Vector2 lastTouchPosition;
    public Vector2 firstTouchPosition;

    public string colorType;

    public bool fadeout = false;

    public Vector2 targetV;

    private Board board;

    public bool isClicked,isSwiped=false,isMoving=false;

    public Animator animator;

    public string damageID;

    public float speedMultiplier=20f;

    public bool falling = false;

    private AudioManager audioManager;

    public bool isPowerUpSoundPlayed = false;

    public bool outsideOfBoard = false;

    public bool moveToward = false;

    public int hitBorder=0;

    public GameObject attachedPowerUp=null;

    public bool activePowerUp = false;

    public bool destroyOnReach = false;

    public bool discoballDestroyer = false;

    float timer = 0;

    private Vector2 tempStartPos;

    [HideInInspector]
    public int swipeRight = Animator.StringToHash("isSwipeRight");
    [HideInInspector]
    public int swipeLeft = Animator.StringToHash("isSwipeLeft");
    [HideInInspector]
    public int swipeUp = Animator.StringToHash("isSwipeUp");
    [HideInInspector]
    public int swipeDown = Animator.StringToHash("isSwipeDown");
    [HideInInspector]
    public int swipeFlash = Animator.StringToHash("isSwipeHintIdle");
    /*
    [HideInInspector]
    private int isFruitLanded = Animator.StringToHash("isFruitLanded");
    */
    [HideInInspector]
    private int isFruitFalling = Animator.StringToHash("isFruitFalling");


    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        board = FindObjectOfType<Board>();
        targetV.x = transform.position.x;
        targetV.y = transform.position.y;

    }

    void Update()
    {


        if(SceneManager.GetActiveScene().name != "LevelEditor")
        {
            

            if (!outsideOfBoard)
            {
                board.allFruits[column, row] = this.gameObject;
            }

            if(destroyOnReach && Vector2.Distance(targetV, transform.position) < 0.5f)
            {

                Destroy(gameObject);

            }

         
            if (isSwiped)
            {
                // Swipe movement
                transform.position = Vector2.Lerp(transform.position, targetV, 30f * Time.deltaTime);

            }
            else
            {
                if (!falling && transform.position.y - targetV.y > 0.2)
                {
                    falling = true;
                    transform.GetChild(0).GetComponent<Animator>().SetBool(isFruitFalling, true);
                }

                if (falling && transform.position.y - targetV.y < 0.2f)
                {
                    falling = false;
                    transform.GetChild(0).GetComponent<Animator>().SetBool(isFruitFalling, false);
                }



                if (moveToward)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetV, speedMultiplier * Time.deltaTime);
                }
                else
                {
                    /*Vector2 currentPosition = transform.position;
                    float distanceMoved = Vector2.Distance(currentPosition, previousPosition);

                    // Calculate speed based on distance moved and time elapsed since last frame
                    speed = distanceMoved / Time.deltaTime;
                    // Update previousPosition with the current position for the next frame
                    previousPosition = currentPosition;*/

                    //timer += Time.deltaTime;

                    if(speedMultiplier <= 25)
                    {
                        speedMultiplier += Time.deltaTime * 15 * Vector2.Distance(targetV, transform.position);
                    }
                    /*
                    else if(speedMultiplier <= 25 && Vector2.Distance(targetV, tempStartPos) <= 1.5f)
                    {
                        speedMultiplier = 7f;
                    }
                    */
                    transform.position = Vector2.MoveTowards(transform.position, targetV, Time.deltaTime * speedMultiplier);

                    /*if (timer>0.5f)
                    {
                        Debug.Log("I AM CONSTANT");
                        transform.position = Vector2.MoveTowards(transform.position, targetV, Time.deltaTime * 10);
                    }
                    else
                    {
                        Debug.Log("I AM SPEED");
                        transform.position = Vector2.Lerp(transform.position, targetV, Time.deltaTime * speedMultiplier);
                    }*/
                    //transform.position = Vector2.MoveTowards(transform.position, targetV, Time.deltaTime * speedMultiplier);
                }            


            }        

            if (Vector2.Distance(targetV, transform.position) > 0.1f)
            {
                isMoving = true;           
            }
            else
            {
                tempStartPos = transform.position;
                isMoving = false;
                speedMultiplier = 10f;
             
            }

            if (isClicked)
            {
                lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(transform.position, lastTouchPosition) >= 0.6f)
                {
                    CalculateAngle();
                    isClicked = false;

                }
            }            
        }      
    }

    private void CalculateAngle()
    {
        float angleInRadians = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x);
        swipeAngle = angleInRadians * Mathf.Rad2Deg;

        board.SwipeFruits(swipeAngle, column, row);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Fruit fruitScript;
        if(fruitScript = other.GetComponent<Fruit>())
        {
            if (fruitScript.fruitType < 0 && fruitScript.activePowerUp && !activePowerUp && !fadeout)
            {

                if (fruitType < -100){
                    GetComponent<ObstacleScript>().TakeDamage(other.GetComponent<Fruit>().damageID);
                }
                else
                {
                    if(fruitType < 0)
                    {

                        if (fruitScript.discoballDestroyer)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            board.DestroyController(board.allFruits[column, row], false);
                        }
                    }
                    else
                    {
                        audioManager.FruitCrush();
                        board.DestroyController(board.allFruits[column, row], false);
                    }               
                }
               
            }
        }        
  
    }

}

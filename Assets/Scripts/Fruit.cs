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

    private bool newlyCreated = true;

    public Vector2 targetV;

    private Board board;

    public bool isClicked,isSwiped=false,isMoving=false;

    public Animator animator;

    public string damageID;

    ObjectSpeedAndTimeWaitingLibrary speedLibrary = new ObjectSpeedAndTimeWaitingLibrary();

    public float speedMultiplier;

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

    public bool harvester = false;

    public string selectedID = null;

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
    [HideInInspector]
    private int isFruitLanded = Animator.StringToHash("isFruitLanded");


    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        board = FindObjectOfType<Board>();
        targetV.x = transform.position.x;
        targetV.y = transform.position.y;
        speedMultiplier = speedLibrary.fruitStartSpeed;
        StartCoroutine(WaitAndActivateFruit());
    }

    void Update()
    {


        if(SceneManager.GetActiveScene().name != "LevelEditor")
        {
            

            if (!outsideOfBoard)
            {
                board.allFruits[column, row] = this.gameObject;
            }

            if(destroyOnReach && Vector2.Distance(transform.position, targetV) < 0.5f)
            {

                Destroy(gameObject);

            }

         
            if (isSwiped)
            {
                // Swipe movement
                transform.position = Vector2.Lerp(transform.position, targetV, speedLibrary.fruitSwipeSpeed * Time.deltaTime);

            }
            else
            {
                if (!falling && transform.position.y - targetV.y > 0.01f)
                {
                    falling = true;
                }

                if (falling && transform.position.y - targetV.y < 0.01f && !fadeout)
                {
                    falling = false;
                    transform.GetChild(0).GetComponent<Animator>().SetTrigger(isFruitLanded);
                }



                if (moveToward)
                {
                    if (harvester)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, targetV, Time.deltaTime * speedLibrary.harvesterSpeed);
                    }
                    else
                    {
                        transform.position = Vector2.MoveTowards(transform.position, targetV, Time.deltaTime * speedLibrary.boomerangSpeed);
                    }
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

                    if(speedMultiplier <= speedLibrary.fruitMaxSpeed)
                    {
                        speedMultiplier += Time.deltaTime * speedLibrary.fruitAccelerationMultiplier * Vector2.Distance(targetV, transform.position);
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
                speedMultiplier = speedLibrary.fruitStartSpeed;
             
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
            if (fruitScript.fruitType < 0 && fruitScript.activePowerUp && !activePowerUp && !fadeout && !newlyCreated)
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

    private IEnumerator WaitAndActivateFruit()
    {
        yield return new WaitForSeconds(0.26f);
        newlyCreated = false;
    }

    public void ActivatedByLightening(int type)
    {
        if (type == 0)
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color(255, 0, 0, 255);
        }
        else
        {
            outsideOfBoard = true;
            board.DestroyController(gameObject, false);
            // If type is bigger then -3 it means it is either vertical or horizontal harvester so it will randomizely spawn.
            if (type > -3)
            {
                board.CreatePowerUp(column, row, UnityEngine.Random.Range(-2, 0), false);
            }
            else
            {
                board.CreatePowerUp(column, row, type, false);
            }
            Destroy(gameObject);
        }
    }
}

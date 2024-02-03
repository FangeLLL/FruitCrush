using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using System.Drawing;

public class SwipeHint : MonoBehaviour
{
    private Board board;
    public Fruit fruit;
    public Fruit fruit2;
    public Fruit fruit3;
    public Fruit fruit4;
    public Fruit fruit5;
    public float hintThreshold = 3f; // Adjust this threshold as needed

    public bool continueIteration = true;
    public bool oneHintActive;
    [SerializeField] private bool showHint = false;
    [SerializeField] private bool isIterating = false;

    // Add a flag to track whether the coroutine has been started
    public bool hasCoroutineStarted = false;

    List<Vector2Int> possibleMoves = new List<Vector2Int>();
    private bool isMergeHorizontal;
    private int point;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        StartCoroutine(PowerUpsIteration());
    }

    private void Update()
    {
        /*if (board.hintBool && !board.exitUpdate)
        {
            if (isHintSearching)
            {
                if (!oneHintActive)
                {
                    Debug.Log("HINT GO");
                    StartCoroutine(PowerUpsIteration());
                    isHintSearching = false;
                }

            }
        }
        else
        {
            StopAllCoroutines();
            oneHintActive = false;
            Debug.Log("HINT STOPPED");
        }*/

        if(board.hintBool && !oneHintActive && showHint)
        {
            oneHintActive = true;
            StartCoroutine(WaitForHint());
           // hasCoroutineStarted = true;
        }

        if(board.hintBool && !oneHintActive && !showHint && !isIterating) 
        {
            StartCoroutine(PowerUpsIteration());
        }

    }

    private IEnumerator WaitForHint()
    {
        yield return new WaitForSeconds(3);
        continueIteration = true;
        StartCoroutine(PowerUpsIteration());

    }

    #region Iteration

    // THIS IS FOR POWER UPS HINT
    private IEnumerator PowerUpsIteration()
    {
        if (!isIterating)
        {
            isIterating = true;
            if (!continueIteration) continueIteration = true;
        }

        point = 0;

        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {
                    if (board.allFruits[i, j])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        //MERGE HORIZONTAL

                        /*            
                         *  -- 
                         *  
                         *  */

                        if (type < 0 && i + 1 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType < 0 &&
                                type + board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType < 0)
                            {
                                

                                if (Mathf.Abs(type + board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType) > Mathf.Abs(point))
                                {
                                    if (!showHint)
                                    {
                                        showHint = true;
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        isIterating = false;
                                        break;
                                    }
                                    else
                                    {
                                        isMergeHorizontal = true;
                                        point = type + board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType;
                                        fruit = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                        fruit2 = board.allFruits[i, j].GetComponent<Fruit>();
                                    }
                                    
                                    
                                }
                            }
                        }

                        // MERGE VERTICAL

                        /*            
                         *  |
                         *  |
                         *  
                         *  */

                        if (type < 0 && j + 1 < board.height)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType < 0 &&
                                type + board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType < 0)
                            {
                                

                                if (Mathf.Abs(type + board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType) > Mathf.Abs(point))
                                {
                                    if (!showHint)
                                    {
                                        showHint = true;
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        isIterating = false;
                                        break;
                                    }
                                    else
                                    {
                                        Debug.Log("TEST");
                                        isMergeHorizontal = false;
                                        point = type + board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType;
                                        fruit = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                        fruit2 = board.allFruits[i, j].GetComponent<Fruit>();
                                    }                                   
                                }
                            }
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);

        if (point < 0)
        {
            if (showHint && fruit && fruit2)
            {
                continueIteration = false;
                if (isMergeHorizontal)
                {
                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeRight, true);
                }
                else
                {
                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeUp, true);
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            //point = 0;
            isMergeHorizontal = false;
            if (continueIteration)
            {
                StartCoroutine(FiveMatchIteration());
            }
        }
        
        

    }

    private IEnumerator FiveMatchIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {                   
                    if (board.allFruits[i, j])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        if (j + 1 < board.height && board.allFruits[i, j + 1])
                        {
                            // FOR SWIPE DOWN

                            /*   X    
                             *   ||   
                             *   \/   
                             * -- --      
                             *     
                             *   
                             */
                            if (i + 4 < board.width && j + 1 < board.height)
                            {
                                if (board.allFruits[i + 1, j] && board.allFruits[i, j] &&
                                    type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                    board.allFruits[i + 2, j] &&
                                    board.allFruits[i + 3, j] &&
                                    type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType &&
                                    board.allFruits[i + 4, j] &&
                                    type == board.allFruits[i + 4, j].GetComponent<Fruit>().fruitType &&
                                    board.allFruits[i + 2, j + 1] &&
                                    type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (!showHint)
                                    {
                                        showHint = true;
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        isIterating = false;
                                        break;
                                    }

                                    else
                                    {
                                        fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                        fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                        fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                        fruit4 = board.allFruits[i + 4, j].GetComponent<Fruit>();
                                        fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                        fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                        fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                        fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                        fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);

                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        isIterating = false;
                                        break;
                                    }
                                    
                                }
                            }
                        }


                        // FOR SWIPE UP

                        /*   
                         *   
                         * -- --      
                         *   /\
                         *   ||
                         *   X
                         *   
                         */

                        if (i + 4 < board.width && j - 1 >= 0)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 4, j] &&
                                type == board.allFruits[i + 4, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j - 1] &&
                                type == board.allFruits[i + 2, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }

                                else
                                {
                                    fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 4, j].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*   
                         *   
                         *     |   
                         *     |
                         * X=> 
                         *     |
                         *     |
                         */

                        if (j + 4 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 4] &&
                                type == board.allFruits[i, j + 4].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("NO SHUFFLE");
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }

                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j + 4].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*   
                         *   
                         *     |   
                         *     |
                         *       <= X
                         *     |
                         *     |
                         */

                        if (j + 4 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 4] &&
                                type == board.allFruits[i, j + 4].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }

                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j + 4].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                               
                            }
                        }

                    }
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(LShapeIteration());
        }
    }



    // X FOR FRUIT
    private IEnumerator LShapeIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {
                    if (board.allFruits[i, j])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // FOR SWIPE DOWN
                        /*   X    
                         *   ||   
                         *   \/   
                         * --      
                         *   |    
                         *   | 
                         */

                        if (j + 3 < board.height && i - 2 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i, j + 2] &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 2, j + 2] &&
                                type == board.allFruits[i - 2, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType
                                )
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i - 2, j + 2].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT
                        /*       
                         *       
                         * --   <= X   
                         *   |    
                         *   | 
                         */

                        if (j + 2 < board.height && i - 2 >= 0 && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType && 
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 2, j + 2] &&
                                type == board.allFruits[i - 2, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType
                                )
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i - 2, j + 2].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE DOWN

                        /*   X    
                         *   ||   
                         *   \/   
                         *     __   
                         *   |    
                         *   | 
                         */

                        if (j + 3 < board.height && i + 2 < board.width)
                        {

                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i, j + 2] &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 2] &&
                                type == board.allFruits[i + 2, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType
                                )
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 2, j + 2].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT
                        /*       
                         *       
                         *  X =>   --     
                         *        |    
                         *        | 
                         */

                        if (j + 2 < board.height && i + 2 < board.width && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i, j + 2] &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 2] &&
                                type == board.allFruits[i + 2, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType
                                )
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 2, j + 2].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*     | 
                         *     |
                         *   __  
                         *      
                         *     /\ 
                         *     ||
                         *     X
                         */

                        if (j + 3 < board.height && i - 2 >= 0)
                        {
                            if (board.allFruits[i, j + 1] && board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 2, j + 1] &&
                                type == board.allFruits[i - 2, j + 1].GetComponent<Fruit>().fruitType
                                )
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i - 2, j + 1].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*     | 
                         *     |
                         *   __  <= X
                         *      
                         */

                        if (j + 2 < board.height && i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 2] &&
                                type == board.allFruits[i + 2, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 2, j + 2].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*     | 
                         *     |
                         *      --  
                         *      
                         *     /\ 
                         *     ||
                         *     X
                         */

                        if (j + 3 < board.height && i + 2 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*        | 
                         *        |
                         *    X=>  __  
                         *      
                         */

                        if (j + 2 < board.height && i + 3 < board.width && i - 1 >= 0)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i + 3, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(TShapeIteration());
        }
    }

    private IEnumerator TShapeIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {
                    if (j + 1 < board.height && board.allFruits[i, j] && board.allFruits[i, j + 1])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // FOR SWIPE RIGHT
                        /*      
                         *       |  
                         * X =>   --   
                         *       |
                         */

                        if (j + 2 < board.height)
                        {
                            if (i - 1 >= 0 && i + 2 < board.width && board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT
                        /*      
                         *       |  
                         *    --   <= X
                         *       |
                         */

                        if (j + 2 < board.height)
                        {
                            if (i + 1 < board.width && i - 2 >= 0 && board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 2, j + 1] &&
                                type == board.allFruits[i - 2, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i - 2, j + 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE DOWN
                        /*      
                         *       X
                         *       ||
                         *       \/
                         *      -  -
                         *       |
                         *       |
                         */

                        if (j + 3 < board.height)
                        {
                            if (i - 1 >= 0 && i + 1 < board.width && board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeDown, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeDown, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeDown, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeDown, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP
                        /*      
                         *      
                         *       |
                         *       |
                         *      - -
                         *       /\
                         *       ||
                         *       X
                         *       
                         */

                        if (j + 3 < board.height)
                        {
                            if (i - 1 >= 0 && i + 1 < board.width && board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);

                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(FourMatchIteration());
        }
    }

    private IEnumerator FourMatchIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {
                    if (board.allFruits[i, j])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // FOR SWIPE DOWN

                        /*   X    
                         *   ||   
                         *   \/   
                         * --  -      
                         *     
                         *   
                         */

                        if (j + 1 < board.height && i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*      
                         *      
                         *      
                         * --  -      
                         *   /\
                         *   ||
                         *   X
                         *   
                         */

                        if (j - 1 >= 0 && i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j - 1] &&
                                type == board.allFruits[i + 2, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                    fruit5 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE DOWN

                        /*    
                         *    X
                         *    ||
                         *    \/      
                         *   -  --      
                         *   
                         *   
                         */

                        if (j + 1 < board.height && i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*    
                         *          
                         *   -  --      
                         *    /\
                         *    ||
                         *    X
                         *   
                         */

                        if (j - 1 >= 0 && i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j - 1] &&
                                type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *      |     
                         *      |
                         * X => 
                         *      |
                         *    
                         *   
                         */

                        if (j + 3 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*    
                         *          
                         *     |     
                         *     |
                         *       <= X
                         *     |
                         *    
                         *   
                         */

                        if (j + 3 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *      |     
                         * X =>      
                         *      |
                         *      |
                         *    
                         *   
                         */

                        if (j + 3 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*    
                         *          
                         *      |     
                         *        <= X
                         *      |
                         *      |
                         *    
                         *   
                         */

                        if (j + 3 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(ColumnSquareIteration());
        }
    }

    private IEnumerator ColumnSquareIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {
                    if (j + 1 < board.height && board.allFruits[i, j] && board.allFruits[i, j + 1])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // UPPER BOUNDS
                        if (type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                        {
                            if (i + 1 < board.width && i - 1 >= 0)
                            {
                                if (i - 1 >= 0 && board.allFruits[i - 1, j + 1] && type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i - 2 >= 0 && board.allFruits[i - 2, j] && type == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //  Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                            fruit = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //   Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                                            fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }

                    if (j - 1 >= 0 && board.allFruits[i, j] && board.allFruits[i, j - 1])
                    {
                        int type2 = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        if (type2 == board.allFruits[i, j - 1].GetComponent<Fruit>().fruitType)
                        {
                            if (i + 1 < board.width && i - 1 >= 0)
                            {
                                if (i - 1 >= 0 && board.allFruits[i - 1, j - 1] && type2 == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i - 2 >= 0 && board.allFruits[i - 2, j] && type2 == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
                                            fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type2 == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type2 == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //   Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                            fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(RowSquareIteration());
        }
    }

    private IEnumerator RowSquareIteration()
    {
        // Loop through the entire board, starting from the bottom-right corner
        for (int j = board.height - 1; j >= 0; j--) // ROW (start from the last row)
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int i = board.width - 1; i >= 0; i--) // COLUMN (start from the last column)
                {
                    if (j + 1 < board.height && board.allFruits[i, j] && board.allFruits[i, j + 1])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // UPPER BOUNDS
                        if (type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                        {
                            if (i + 1 < board.width && i - 1 >= 0)
                            {
                                if (i - 1 >= 0 && board.allFruits[i - 1, j + 1] && type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i - 2 >= 0 && board.allFruits[i - 2, j] && type == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        

                                    }
                                    if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                            fruit = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                                            fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }

                    if (j - 1 >= 0 && board.allFruits[i, j] && board.allFruits[i, j - 1])
                    {
                        int type2 = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        if (type2 == board.allFruits[i, j - 1].GetComponent<Fruit>().fruitType)
                        {
                            if (i + 1 < board.width && i - 1 >= 0)
                            {
                                if (i - 1 >= 0 && board.allFruits[i - 1, j - 1] && type2 == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i - 2 >= 0 && board.allFruits[i - 2, j] && type2 == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
                                            fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type2 == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type2 == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                            fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                    if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        if (!showHint)
                                        {
                                            showHint = true;
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        else
                                        {
                                            //     Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                            fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                            fruit2 = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                            fruit3 = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                            fruit4 = board.allFruits[i, j].GetComponent<Fruit>();

                                            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, true);
                                            yield return new WaitForSeconds(0.1f);
                                            continueIteration = false;
                                            isIterating = false;
                                            break;
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(ThreeMatchIteration());
        }
    }

    private IEnumerator ThreeMatchIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if (!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {
                    if (board.allFruits[i, j])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // FOR SWIPE DOWN

                        /*   X    
                         *   ||   
                         *   \/   
                         *  -  -      
                         *     
                         *   
                         */

                        if (j + 1 < board.height && i + 2 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*      
                         *      
                         *      
                         *  -  -      
                         *   /\
                         *   ||
                         *   X
                         *   
                         */

                        if (j - 1 >= 0 && i + 2 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j - 1] &&
                                type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE DOWN

                        /*    
                         *    X
                         *    ||
                         *    \/      
                         *      --      
                         *   
                         *   
                         */

                        if (j - 1 >= 0 && i + 2 < board.width)
                        {
                            if (board.allFruits[i, j - 1] &&
                                board.allFruits[i + 1, j - 1] &&
                                type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j - 1] &&
                                type == board.allFruits[i + 2, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*    
                         *          
                         *      --      
                         *    /\
                         *    ||
                         *    X
                         *   
                         */

                        if (j + 1 < board.height && i + 2 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *  X =>  --      
                         *  
                         */

                        if (i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 3, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE DOWN

                        /*    
                         *     X
                         *     ||
                         *     \/        
                         *   --        
                         */

                        if (i + 2 < board.width && j + 1 < board.height)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*                  
                         *   -- 
                         *     X
                         *     /\
                         *     ||
                         *   
                         */

                        if (i + 2 < board.width && j - 1 >= 0)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 2, j - 1] &&
                                type == board.allFruits[i + 2, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*    
                         *     
                         *     
                         *             
                         *   -- <= X
                         *      
                         *   
                         */

                        if (i + 3 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *      |         
                         * X => 
                         *      |
                         *    
                         *   
                         */

                        if (j + 2 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*    
                         *          
                         *     |     
                         *       <= X
                         *     |
                         *    
                         *   
                         */

                        if (j + 2 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *           
                         * X =>      
                         *      |
                         *      |
                         *    
                         *   
                         */

                        if (j + 2 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*    
                         *          
                         *           
                         *        <= X
                         *      |
                         *      |
                         *    
                         *   
                         */

                        if (j + 2 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE DOWN

                        /*    
                         *      X
                         *      
                         *      ||
                         *      \/   
                         *      
                         *      |
                         *      |
                         *    
                         *   
                         */

                        if (j + 3 < board.height)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *           
                         *       |
                         *       |
                         *  X =>
                         *    
                         *   
                         */

                        if (j + 2 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 2] &&
                                type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE LEFT

                        /*    
                         *          
                         *           
                         *       |
                         *       |
                         *         <= X
                         *    
                         *   
                         */

                        if (j + 2 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i - 1, j] &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 2] &&
                                type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                    fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                        // FOR SWIPE UP

                        /*     
                         *      
                         *      |
                         *      |
                         *    
                         *      /\
                         *      ||
                         *      X
                         *   
                         */

                        if (j + 3 < board.height)
                        {
                            if (board.allFruits[i, j + 1] &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 3] &&
                                type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType)
                            {
                                if (!showHint)
                                {
                                    showHint = true;
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                else
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit2 = board.allFruits[i, j + 2].GetComponent<Fruit>();
                                    fruit3 = board.allFruits[i, j + 3].GetComponent<Fruit>();

                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, true);
                                    fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    isIterating = false;
                                    break;
                                }
                                
                            }
                        }

                    }
                }
            }
        }
        if(continueIteration)
        {
            StartCoroutine(board.Shuffle());
        }
    }





    #endregion

   

    public void StopHintCoroutines()
    {
        showHint = false;
        isIterating = false;

        if (fruit)
        {
            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, false);
            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, false);
            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, false);
            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, false);
            fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeFlash, false);
        }

        if(fruit2)
        {
            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeDown, false);
            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeUp, false);
            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeLeft, false);
            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeRight, false);
            fruit2.GetComponentInChildren<Animator>().SetBool(fruit2.swipeFlash, false);
        }

        if (fruit3)
        {
            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeDown, false);
            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeUp, false);
            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeLeft, false);
            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeRight, false);
            fruit3.GetComponentInChildren<Animator>().SetBool(fruit3.swipeFlash, false);
        }

        if (fruit4)
        {
            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeDown, false);
            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeUp, false);
            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeLeft, false);
            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeRight, false);
            fruit4.GetComponentInChildren<Animator>().SetBool(fruit4.swipeFlash, false);
        }

        if (fruit5)
        {
            fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeDown, false);
            fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeUp, false);
            fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeLeft, false);
            fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeRight, false);
            fruit5.GetComponentInChildren<Animator>().SetBool(fruit5.swipeFlash, false);
        }
      

        StopAllCoroutines();
        continueIteration = false;
        oneHintActive = false;
        //board.isRunning = false;
    }


    /*void Hint()
    {
        if(isHintSearching)
        {
            columnFailed = false;
            rowFailed = false;
            columnSquareFailed = false;

            ColumnIteration();

            if (columnFailed)
            {
                RowIteration();
            }
            else
                return;

            if (rowFailed)
            {
                ColumnSquareIteration();
            }
            else
                return;

            if (columnSquareFailed)
            {
                RowSquareIteration();
            }
            else
                return;

        }
    }*/
}                          

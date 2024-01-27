using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class SwipeHint : MonoBehaviour
{
    private Board board;
    public Fruit fruit;
    public Fruit fruit2;
    public float hintThreshold = 3f; // Adjust this threshold as needed

    public bool isHintSearching;
    public bool continueIteration = true;
    public bool oneHintActive;

    List<Vector2Int> possibleMoves = new List<Vector2Int>();

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }

    private void Update()
    {
        if (board.hintBool && !board.exitUpdate)
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
        }

    }

    #region Iteration

    // THIS IS FOR POWER UPS HINT
    private IEnumerator PowerUpsIteration()
    {
        oneHintActive = true;
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
                        if (type < 0 && board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType < 0)
                        {
                            if (j + 1 < board.width && j - 1 >= 0)
                            {
                                Debug.Log("POWER UP HINT");
                                if (i - 1 >= 0 && board.allFruits[i - 1, j] && type == -3 && type == board.allFruits[i - 1, j].GetComponent<Fruit>().fruitType)
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    //fruit2 = board.allFruits[i - 1, j].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    //fruit2.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }

                                if (i + 1 >= 0 && board.allFruits[i + 1, j] && type == -3 && type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType)
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    //fruit2 = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    //fruit2.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }

                                if ((i - 1 >= 0 && board.allFruits[i - 1, j] && type < 0 && board.allFruits[i - 1, j].GetComponent<Fruit>().fruitType < 0))
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }

                                if ((i + 1 >= 0 && board.allFruits[i + 1, j] && type < 0 && board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType < 0))
                                {
                                    fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }

                        }
                    }

                    if (j - 1 >= 0 && board.allFruits[i, j] && board.allFruits[i, j - 1])
                    {

                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(FiveMatchIteration());
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
                        Debug.Log(j);
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

                            if (i + 4 < board.width)
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
                                    fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
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

                        if (i + 4 < board.width)
                        {
                            Debug.Log("5 MATCH");
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
                                Debug.Log("5 MATCH GO");
                                fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                type == board.allFruits[i + 2, j +1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 2] &&
                                type == board.allFruits[i + 2, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                            if (board.allFruits[i, j+1] &&
                                board.allFruits[i, j+2] &&
                                type == board.allFruits[i, j+2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j+3] &&
                                type == board.allFruits[i, j+3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j + 1] &&
                                type == board.allFruits[i +1, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
                            }
                        }



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
                                fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                board.allFruits[i, j +2] &&
                                type == board.allFruits[i, j+2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j+3] &&
                                type == board.allFruits[i, j+3].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j + 1] &&
                                type == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                        //  Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        //    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                        fruit = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //   Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                                        fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
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
                                        //     Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        //     Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
                                        fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type2 == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type2 == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //   Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                        fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
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
                                        //     Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;

                                    }
                                    if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        //     Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                        fruit = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //     Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                                        fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
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
                                        //    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        //     Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
                                        fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                }
                                if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type2 == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    if (i + 2 < board.width && board.allFruits[i + 2, j] && type2 == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        //     Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                        fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
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
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 2, j + 1] &&
                                type == board.allFruits[i + 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 2, j - 1] &&
                                type == board.allFruits[i + 2, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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

                        if (j + 1 < board.height && i + 2 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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

                        if (j - 1 >= 0 && i + 2 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j - 1] &&
                                type == board.allFruits[i, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i, j - 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
                            }
                        }

                        // FOR SWIPE RIGHT

                        /*    
                         *          
                         *  X =>  --      
                         *  
                         */

                        if (i + 2 < board.width && i - 1 >= 0)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j] &&
                                type == board.allFruits[i - 1, j].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i-1, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 2, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 2, j - 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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

                        if (i + 2 < board.width)
                        {
                            if (board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 2, j] &&
                                board.allFruits[i + 3, j] &&
                                type == board.allFruits[i + 3, j].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i + 3, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i - 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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

                        if (j + 2 < board.height && i - 1 >= 0)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i - 1, j] &&
                                type == board.allFruits[i - 1, j].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i - 1, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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

                        if (j + 2 < board.height && i + 1 < board.width)
                        {
                            if (board.allFruits[i, j + 1] &&
                                type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i, j + 2] &&
                                type == board.allFruits[i, j + 2].GetComponent<Fruit>().fruitType &&
                                board.allFruits[i + 1, j] &&
                                type == board.allFruits[i + 1, j].GetComponent<Fruit>().fruitType)
                            {
                                fruit = board.allFruits[i + 1, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
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
                                fruit = board.allFruits[i, j].GetComponent<Fruit>();
                                fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                yield return new WaitForSeconds(0.1f);
                                continueIteration = false;
                                StopCoroutines();
                                break;
                            }
                        }

                    }
                }
            }
        }
        StopCoroutines();
    }





    #endregion

    public void StopCoroutines()
    {
        StopAllCoroutines();
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

using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class SwipeHint : MonoBehaviour
{
    private Board board;
    public Fruit fruit;
    public float hintThreshold = 3f; // Adjust this threshold as needed

    public bool isHintSearching;
    public bool continueIteration = true;

    List<Vector2Int> possibleMoves = new List<Vector2Int>();

    private void Start()
    {
        board = FindObjectOfType<Board>();       
    }

    private void Update()
    {
        if (isHintSearching)
        {
            StartCoroutine(ColumnSquareIteration());
            isHintSearching = false;
        }
    }

    #region Iteration

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
                                        Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
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
                                        Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
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
                                        Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
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
                                        Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
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
                                        Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;

                                    }
                                    if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
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
                                        Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
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
                                        Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i - 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
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
                                        Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                        fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                        fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                        yield return new WaitForSeconds(0.1f);
                                        continueIteration = false;
                                        StopCoroutines();
                                        break;
                                    }
                                    if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                    {
                                        Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
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
            StartCoroutine(ColumnIteration());
        }
    }

    private IEnumerator ColumnIteration()
    {
        // Loop through the entire board

        for (int i = 0; i < board.width; i++) // COLUMN
        {
            if(!continueIteration)
            {
                break;
            }
            else
            {
                for (int j = 0; j < board.height; j++) // ROW
                {

                    // WE CHECK IF THE GRID ABOVE DOES EXIST
                    if (j + 1 < board.height && board.allFruits[i, j] && board.allFruits[i, j + 1])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // WE CHECK THE ONE ABOVE THE OBJECT WE ARE REPEATING
                        if (type == board.allFruits[i, j + 1].GetComponent<Fruit>().fruitType)
                        {
                            if (j + 2 < board.height)
                            {
                                if (i - 1 >= 0 && board.allFruits[i - 1, j + 2] && type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType && board.allFruits[i, j + 2])
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 2));
                                    fruit = board.allFruits[i - 1, j + 2].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                                else if (i + 1 < board.width && board.allFruits[i + 1, j + 2] && type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType && board.allFruits[i, j + 2])
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 2));
                                    fruit = board.allFruits[i + 1, j + 2].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }

                            if (j + 3 < board.height)
                            {
                                if (board.allFruits[i, j + 3] && type == board.allFruits[i, j + 3].GetComponent<Fruit>().fruitType && board.allFruits[i, j + 2])
                                {
                                    Debug.Log("Possible Move - Column: " + (i) + ", Row: " + (j + 3));
                                    fruit = board.allFruits[i, j + 3].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }

                            if (j - 2 >= 0)
                            {
                                if (board.allFruits[i, j - 2] && type == board.allFruits[i, j - 2].GetComponent<Fruit>().fruitType && board.allFruits[i, j - 1])
                                {
                                    Debug.Log("Possible Move - Column: " + (i) + ", Row: " + (j - 2));
                                    fruit = board.allFruits[i, j - 2].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }


                            if (j - 1 >= 0)
                            {
                                if (i - 1 >= 0 && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i, j - 1])
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                    fruit = board.allFruits[i - 1, j - 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                                else if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i, j - 1])
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                                    fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }
                            j++;

                        }
                    }
                }
            }

        }
        yield return new WaitForSeconds(0.1f);
        if (continueIteration)
        {
            StartCoroutine(RowIteration());
        }
    }

    private IEnumerator RowIteration()
    {
        // Loop through the entire board, starting from the top-right corner
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
                    // WE CHECK IF THE GRID TO THE LEFT DOES EXIST
                    if (i - 1 >= 0 && board.allFruits[i, j] && board.allFruits[i - 1, j])
                    {
                        int type = board.allFruits[i, j].GetComponent<Fruit>().fruitType;

                        // WE CHECK THE ONE TO THE LEFT OF THE OBJECT WE ARE REPEATING
                        if (type == board.allFruits[i - 1, j].GetComponent<Fruit>().fruitType)
                        {
                            if (i - 2 >= 0)
                            {
                                if (j - 1 >= 0 && board.allFruits[i - 2, j - 1] && type == board.allFruits[i - 2, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 2, j])
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j - 1));
                                    fruit = board.allFruits[i - 2, j - 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                                else if (j + 1 < board.height && board.allFruits[i - 2, j + 1] && type == board.allFruits[i - 2, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i - 2, j])
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j + 1));
                                    fruit = board.allFruits[i - 2, j + 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }


                            if (i - 3 >= 0)
                            {
                                if (board.allFruits[i - 3, j] && type == board.allFruits[i - 3, j].GetComponent<Fruit>().fruitType && board.allFruits[i - 2, j])
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 3) + ", Row: " + (j));
                                    fruit = board.allFruits[i - 3, j].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeRight, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }

                            if (i + 2 < board.width)
                            {
                                if (board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                    fruit = board.allFruits[i + 2, j].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeLeft, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }



                            if (i + 1 < board.width)
                            {
                                if (j - 1 >= 0 && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                                    fruit = board.allFruits[i + 1, j - 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeUp, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                                else if (j + 1 < board.height && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType && board.allFruits[i + 1, j])
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                    fruit = board.allFruits[i + 1, j + 1].GetComponent<Fruit>();
                                    fruit.GetComponentInChildren<Animator>().SetBool(fruit.swipeDown, true);
                                    yield return new WaitForSeconds(0.1f);
                                    continueIteration = false;
                                    StopCoroutines();
                                    break;
                                }
                            }
                            i--;
                        }
                    }
                }
            }

        }
        yield return new WaitForSeconds(0.1f);
        StopCoroutines();
    }

    

    

    #endregion

    private void StopCoroutines()
    {
        StopAllCoroutines();
        board.isRunning = false;
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

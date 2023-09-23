using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SwipeHint : MonoBehaviour
{
    private Board board;
    private Fruit fruit;
    private float hintTimer = 0f;
    public float hintThreshold = 3f; // Adjust this threshold as needed



    List<Vector2Int> possibleMoves = new List<Vector2Int>();

    private void Start()
    {
        board = FindObjectOfType<Board>();
        hintTimer = 0f; // Initialize the hint timer
    }

    private void Update()
    {
       
        // Update the hint timer
        hintTimer += Time.deltaTime;

        // Check if the player is idle and the hint timer reached the threshold
        if (hintTimer >= hintThreshold)
        {
            // Player is idle for too long, find and show a hint

            //RowIteration();
            //ColumnIteration();
            //ColumnSquareIteration();
            RowSquareIteration();

            hintTimer = 0f; // Reset the hint timer
            
        }
    }

    #region Iteration

    private void ColumnIteration()
    {
        // Loop through the entire board

        for (int i = 0; i < board.width; i++) // COLUMN
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
                            if (i - 1 >= 0 && board.allFruits[i - 1, j + 2] && type == board.allFruits[i - 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 2));
                            }
                            else if (i + 1 < board.width && board.allFruits[i + 1, j + 2] && type == board.allFruits[i + 1, j + 2].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 2));
                            }
                        }
                        if (j - 1 >= 0)
                        {
                            if (i - 1 >= 0 && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                            }
                            else if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                            }
                        }
                        j++;

                    }
                }
            }
        }
    }

    private void RowIteration()
    {
        // Loop through the entire board, starting from the top-right corner
        for (int j = board.height - 1; j >= 0; j--) // ROW (start from the last row)
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
                            if (j - 1 >= 0 && board.allFruits[i - 2, j - 1] && type == board.allFruits[i - 2, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j - 1));
                            }
                            else if (j + 1 < board.height && board.allFruits[i - 2, j + 1] && type == board.allFruits[i - 2, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j + 1));
                            }
                        }
                        if (i + 1 < board.width)
                        {
                            if (j - 1 >= 0 && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
                            }
                            else if (j + 1 < board.height && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                            }
                        }
                        i--;
                    }
                }
            }
        }
    }

    private void ColumnSquareIteration()
    {
        for (int i = 0; i < board.width; i++) // COLUMN
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
                                if (i - 2 >= 0 && board.allFruits[i - 2, j] && type == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i -2) + ", Row: " + (j));
                                }
                                if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                }
                            }
                            if(i + 1 < board.width && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (i + 2 < board.width && board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                }
                                if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
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
                                if (i - 2 >= 0 && board.allFruits[i - 2, j] && type2 == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                }
                                if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
                                }
                            }
                            if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type2 == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (i + 2 < board.width && board.allFruits[i + 2, j] && type2 == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                }
                                if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void RowSquareIteration()
    {
        // Loop through the entire board, starting from the bottom-right corner
        for (int j = board.height - 1; j >= 0; j--) // ROW (start from the last row)
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
                                if (i - 2 >= 0 && board.allFruits[i - 2, j] && type == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                }
                                if ((i - 1 >= 0) && (j - 1 >= 0) && board.allFruits[i - 1, j - 1] && type == board.allFruits[i - 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j - 1));
                                }
                            }
                            if (i + 1 < board.width && board.allFruits[i + 1, j + 1] && type == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                            {
                                if (i + 2 < board.width && board.allFruits[i + 2, j] && type == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                }
                                if ((i + 1 < board.width) && (j - 1 >= 0) && board.allFruits[i + 1, j - 1] && type == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j - 1));
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
                                if (i - 2 >= 0 && board.allFruits[i - 2, j] && type2 == board.allFruits[i - 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 2) + ", Row: " + (j));
                                }
                                if ((i - 1 >= 0) && (j + 1 < board.height) && board.allFruits[i - 1, j + 1] && type2 == board.allFruits[i - 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i - 1) + ", Row: " + (j + 1));
                                }
                            }
                            if (i + 1 < board.width && board.allFruits[i + 1, j - 1] && type2 == board.allFruits[i + 1, j - 1].GetComponent<Fruit>().fruitType)
                            {
                                if (i + 2 < board.width && board.allFruits[i + 2, j] && type2 == board.allFruits[i + 2, j].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 2) + ", Row: " + (j));
                                }
                                if ((i + 1 < board.width) && (j + 1 < board.height) && board.allFruits[i + 1, j + 1] && type2 == board.allFruits[i + 1, j + 1].GetComponent<Fruit>().fruitType)
                                {
                                    Debug.Log("Possible Move - Column: " + (i + 1) + ", Row: " + (j + 1));
                                }
                            }
                        }
                    }
                }
            }
        }
    }




    #endregion

}

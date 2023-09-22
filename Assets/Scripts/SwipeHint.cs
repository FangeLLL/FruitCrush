using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeHint : MonoBehaviour
{
    private Board board;
    private float idleTime = 3f;
    private bool isIdle = false;

    void Awake()
    {
        board = FindObjectOfType<Board>();
        StartCoroutine(CheckForIdle());
    }
    private  IEnumerator CheckForIdle()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            if (!isIdle)
            {
                idleTime -= 1f;
                if (idleTime <= 0f)
                {
                    // PLAYER IS IDLE FOR TOO LONG, FIND A HINT
                    isIdle = true;  
                    FindAndShowHint();
                }
            }
        }
    }

    private void FindAndShowHint()
    {
        isIdle = true;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // LOOP THROUGH THE ENTIRE BOARD
        for (int row = 0; row < board.height; row++)
        {
            for(int column = 0; column < board.width; column++)
            {
                int fruitType = board.allFruits[column, row]?.GetComponent<Fruit>().fruitType ?? -1;

                // SKIP EMPTY CELLS OR IF THE FRUIT IS IN THE LAST ROW OR LAST COLUMN
                if (fruitType == -1 || (row == board.height - 1 && column == board.width - 1))
                    continue;

                // CHECK RIGHT (IF NOT IN LAST COLUMN)
                if (column < board.width - 1)
                {
                    int rightFruitType = board.allFruits[column + 1, row]?.GetComponent<Fruit>()?.fruitType ?? -1;
                    if(rightFruitType==fruitType)
                    {
                        possibleMoves.Add(new Vector2Int(column, row));
                        possibleMoves.Add(new Vector2Int(column + 1, row));
                        possibleMoves.Add(new Vector2Int(column + 2, row));
                        possibleMoves.Add(new Vector2Int(column + 3, row));
                        break;
                    }
                }

                // CHECK UP (IF NOT IN FIRST ROW)
                if (row > 0)
                {
                    int upFruitType = board.allFruits[column, row - 1]?.GetComponent<Fruit>()?.fruitType ?? -1;
                    if(upFruitType == fruitType)
                    {
                        possibleMoves.Add(new Vector2Int(column, row));
                        possibleMoves.Add(new Vector2Int(column, row - 1));
                        possibleMoves.Add(new Vector2Int(column, row - 2));
                        possibleMoves.Add(new Vector2Int(column, row - 3));
                        break;
                    }
                }

            }
        }

        // IF POSSIBLE MOVES ARE FOUND, SHOW A HINT (E.G., HIGHLIGHT THE MATCHED FRUITS)
        if(possibleMoves.Count > 0)
        {
            foreach(Vector2Int move in possibleMoves)
            {
                Debug.Log("Possible Move - Row: " + move.y + ", Column: " + move.x);
            }
        }
    }
}

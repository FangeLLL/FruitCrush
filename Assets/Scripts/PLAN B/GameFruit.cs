using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFruit : MonoBehaviour
{
    public int x;
    public int y;

    public int X { get { return x; } set { if (isMoveable()) x = value; } }
    public int Y { get { return y; } set { if (isMoveable()) x = value; } }

    private GameGrid.fruitType type;
    public GameGrid.fruitType Type { get { return type; } }

    private GameGrid gameGrid;

    public GameGrid gridRef { get { return gameGrid; } }

    private MoveableFruit moveableFruit;
    public MoveableFruit MoveableFruit { get { return moveableFruit; } }


    void Awake()
    {
        moveableFruit = GetComponent<MoveableFruit>();
    }

    void Update()
    {
        
    }

    public void Init(int _x, int _y, GameGrid _gameGrid, GameGrid.fruitType _type)
    {
        x = _x;
        y = _y;
        gameGrid = _gameGrid;
        type = _type;       
    }

    public bool isMoveable()
    {
        return moveableFruit != null;
    }
}

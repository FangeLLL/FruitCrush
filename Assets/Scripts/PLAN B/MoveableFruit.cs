using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableFruit : MonoBehaviour
{
    private GameFruit fruit;

    void Awake()
    {
        fruit = GetComponent<GameFruit>();
    }

    void Update()
    {
        
    }

    public void Move(int newX, int newY)
    {
        fruit.X = newX;
        fruit.Y = newY;
        fruit.transform.localPosition = fruit.gridRef.GetWorldPosition(newX, newY);
    }
}

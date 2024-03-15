using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBombMerge : MonoBehaviour
{
    public SpriteRenderer object1;
    public SpriteRenderer object2;

    public void Object1InFront()
    {
        object1.sortingOrder = 4;
        object2.sortingOrder = 3;
    }
    public void Object2InFront()
    {
        object1.sortingOrder = 3;
        object2.sortingOrder = 4;
    }
}

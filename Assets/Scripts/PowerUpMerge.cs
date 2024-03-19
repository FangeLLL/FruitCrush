using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpMerge : MonoBehaviour
{
    private SpriteRenderer object1, object2;

    void Start()
    {
        object1 = transform.GetChild(0).GetComponent<SpriteRenderer>();
        object2 = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

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

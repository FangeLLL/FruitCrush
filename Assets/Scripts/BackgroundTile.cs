using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    [SerializeField]
    public GameObject[] fruits;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        
    }

    private void Initialize()
    {
        int fruitToUse = Random.Range(0, fruits.Length);
        GameObject fruit = Instantiate(fruits[fruitToUse], transform.position, Quaternion.identity);
        fruit.transform.parent = this.transform;
        fruit.name = this.gameObject.name;
    }
}

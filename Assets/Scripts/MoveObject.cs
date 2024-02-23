using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveObject : MonoBehaviour
{
    private Vector2 tempPosition;

    public string colorType;

    public bool fadeout = false;

    public Vector2 targetV;

    public float speedMultiplier = 6f;

    public bool moveToward = false;

    public GameObject attachedPowerUp = null;

    public string damageID;

    void Awake()
    {
        targetV.x = transform.position.x;
        targetV.y = transform.position.y;

    }

    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor")
        {

            if (moveToward)
            {            
                transform.position = Vector2.MoveTowards(transform.position, targetV, speedMultiplier * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, targetV, speedMultiplier * Time.deltaTime);

            }

      
        }
    }

}

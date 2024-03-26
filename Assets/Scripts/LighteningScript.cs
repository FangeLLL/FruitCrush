using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LighteningScript : MonoBehaviour
{
    private ObjectSpeedAndTimeWaitingLibrary speedLibrary = new ObjectSpeedAndTimeWaitingLibrary();

    [HideInInspector]
    public Vector2 targetV;

    private float speedMultiplier;

    [HideInInspector]
    public string id = null;
    [HideInInspector]
    public int attachedPowerUpType=0;

    void Awake()
    {
        speedMultiplier = speedLibrary.lighteningSpeed;
        targetV.x = transform.position.x;
        targetV.y = transform.position.y;

    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor")
        {

            transform.position = Vector2.MoveTowards(transform.position, targetV, Time.deltaTime * speedMultiplier);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fruit fruitScript;
        if (fruitScript = collision.GetComponent<Fruit>())
        {
            if(fruitScript.selectedID == id)
            {
                fruitScript.ActivatedByLightening(attachedPowerUpType);
            }
        }
    }

}

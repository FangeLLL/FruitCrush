using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorBackgroundTile : MonoBehaviour
{
    // Start is called before the first frame update
    private LevelManager levelManager;
    public int row, column;
    public GameObject[] obstacles= new GameObject[3];
   // IndexLibrary indexLibrary = new IndexLibrary();

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                levelManager.ReplaceObject(column, row);
               
            }
        }
    }
    /*
    public IEnumerator RearrangeTileType()
    {
        bool[] obstaclesBool= new bool[obstacles.Length];
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < obstacles.Length; i++)
        {
          
            if (obstacles[i])
            {
                obstaclesBool[i] = true;
            }
            else
            {
                obstaclesBool[i] = false;
            }
        }
    }
    */
}

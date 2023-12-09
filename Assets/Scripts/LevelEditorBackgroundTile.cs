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
    public bool active=true;
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

        if (Input.GetMouseButtonDown(1))
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse position is within the bounds of this fruit
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                levelManager.DeactivateTile(column, row);

            }
        }
    }
    
}

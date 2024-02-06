using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObstacleSpecs")]
public class ObstacleSpecs : ScriptableObject
{


    /*
    
    Note: Id - Name of obstacle - Index place of obstacle

    Obstacle Ids:

    0 - Strawbale - 0 
    1 - Wheatfarm - 1
    2 - Strawbale Strong (Has two health) - 0
    3 - Apple Tree - 0
     
     */


    // indexOfLayer variable represent, this prefab preffered place of index value in obstacles variable.
    public int indexOfLayer;
    public bool boxObstacle;
    public bool indestructible;
    public bool isCollectible;
    public string obstacleHitSound;
    public bool powerUpNeed;

    public int id;
    public int taskID;

    // In every damage taken how much task proggress will add.
    public int[] amountOfCollect;

    public Sprite[] sprites;
    // public GameObject[] combinedObstacles;
    public bool is4TimesBigger;

    // It means when obstacle hits ground level its done. 
    public bool isDownward;

    // It means it has fruit script then when creating obstacle assigning unique id to prevent match. 
    public bool isMovable;
}

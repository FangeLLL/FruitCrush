using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObstacleSpecs")]
public class ObstacleSpecs : ScriptableObject
{


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

    // If obstacle is consecutive.
    public bool isConsecutive;

    // If obstacle spread wheathFarm 4x4 of plane
    public bool spreadWheatfarm;

    public string colorType;

    // If obstacle creatable like fruits for example coin.
    public bool creatableOnPlay;

}

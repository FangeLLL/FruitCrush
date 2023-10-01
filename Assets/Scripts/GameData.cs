using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public int width;
    public int height;

    // THE VALUES DEFINED IN THIS CONSTRUCTOR WILL BE THE DEFAULT VALUES
    // THE GAME STARTS WITH WHEN THERE IS NO DATA TO LOAD

    public GameData()
    {
        this.width = 0;
        this.height = 0;
    }
}

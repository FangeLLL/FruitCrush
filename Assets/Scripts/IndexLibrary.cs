using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexLibrary
{
    /*
    bool[] obstacleBools= new bool[2];
    public int TileType(bool[] obstacles)
    {
        
         
        Index:

        0 - Strawbale
        1 - Wheat Farm

        Type:

        0 - None
        1 - Strawbale
        2 - Wheat Farm
        3 - Strawbale and Wheat Farm
          
         

        if (obstacles[0] && obstacles[1])
        {
            return 3;
        }
        else
        {
            if (obstacles[0])
            {
                return 1;
            }else if (obstacles[1])
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
          
    }

    public bool[] GetBoolObstacles(int tileType) {
        if (tileType == 1)
        {
            obstacleBools[0] = true;
            obstacleBools[1] = false;
        }

        if (tileType == 2)
        {
            obstacleBools[0] = false;
            obstacleBools[1] = true;
        }

        if (tileType == 3)
        {
            obstacleBools[0] = true;
            obstacleBools[1] = true;

        }
        return obstacleBools; 
    }
    */
    /// <summary>
    ///  Converting one dimensional array (json saved data) to two dimensional array. 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="sourceArray"></param>
    /// <returns></returns>
    public int[,] Convert2DTo3D(int width,int height,int[] sourceArray)
    {
        int[,] arrangedArray = new int[width,height];
        for (int i = 0; height > i; i++)
        {
            for (int j = 0; width > j; j++)
            {
                arrangedArray[i, j] = sourceArray[(width * i) + j];

            }
        }
        return arrangedArray;
    } 

}

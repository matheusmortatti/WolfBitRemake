using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GenerateWorld : MonoBehaviour {

    public Text gridText;
    public int gridSizeX = 10, gridSizeY = 10, areaSize = 3, density = 2;

    private int seed = System.DateTime.Now.Millisecond;

    private enum RegionType {Forest, Village, Cemitery};

    // Use this for initialization
    void Start () {
        Random.InitState(seed);
        print(GenerateGrid());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    // Generate world grid
    RegionType[,] GenerateGrid()
    {
        RegionType[,] grid = new RegionType[gridSizeX, gridSizeY];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                    grid[i, j] = RegionType.Forest;
            }
        }

        for (int i = 0; i < grid.GetLength(0); i+=areaSize)
        {
            for (int j = 0; j < grid.GetLength(1); j+=areaSize)
            {
                for(int k = 0; k < density; k++)
                {
                    int rangeX = i + areaSize >= gridSizeX ? i - (gridSizeX - 1) : areaSize;
                    int rangeY = j + areaSize >= gridSizeY ? j - (gridSizeY - 1) : areaSize;
                    grid[i + Random.Range(0, rangeX), j + Random.Range(0, rangeY)] = (RegionType)choose((int)RegionType.Village, (int)RegionType.Cemitery);
                }
            }
        }



        //// Loop through the number of paths
        //for (int i = 0; i < areaSize; i++)
        //{

        //    // Choose path direction
        //    int dirX = choose(-1, 1), dirY = choose(-1, 1);

        //    if (Random.Range(0.0f, 1.0f) > 0.5)
        //        dirX = 0;
        //    else
        //        dirY = 0;

        //    // Generate Path
        //    int x = Random.Range(0, gridSizeX), y = Random.Range(0, gridSizeY);

        //    while (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        //    {
        //        grid[x, y] = RegionType.Forest;

        //        // The path can go in the original direction or choose to go sideways
        //        if (Random.Range(0.0f, 1.0f) > 0.5)
        //        {
        //            x += dirX;
        //            y += dirY;
        //        }
        //        else
        //        {
        //            x += dirX == 0 ? choose(-1, 1) : 0;
        //            y += dirY == 0 ? choose(-1, 1) : 0;
        //        }

        //    }

        //}

        return grid;
    }

    void print(RegionType[,] array)
    {
        gridText.text = "";
        for(int i = 0; i < array.GetLength(0); i++)
        {
            for(int j = 0; j < array.GetLength(1); j++)
            {
                int k = (int)array[i, j];
                gridText.text += k.ToString() + " ";
            }
            gridText.text += "\r\n";
        }
    }

    int choose(params int[] list)
    {
        return list[Random.Range(0, list.Length)];
    }
}
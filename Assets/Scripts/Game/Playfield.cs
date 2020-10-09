using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Playfield : MonoBehaviour
{

    public static int w = 8;
    public static int h = 21;
    public static int d = 8;
    public static Transform[,,] grid = new Transform[w, h, d];

    public static Vector3 roundVec3(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }

    public static bool insideBorder(Vector3 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < w && (int)pos.z >= 0 && (int)pos.z < d && (int)pos.y >= 0); 
    }
   
    public static void deletePlane(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            //Destroy(grid[x, y, 0].gameObject);
            grid[x, y, 0].GetComponent<BoxController>().DespawnBlock();
            grid[x, y, 0] = null;
            //Destroy(grid[x, y, d-1].gameObject);
            grid[x, y, d - 1].GetComponent<BoxController>().DespawnBlock();
            grid[x, y, d-1] = null;
        }
        System.Console.WriteLine();
        for (int z = 1; z < d-1; z++)
        {
            //Destroy(grid[0, y, z].gameObject);
            grid[0, y, z].GetComponent<BoxController>().DespawnBlock();
            grid[0, y, z] = null;
            //Destroy(grid[w-1, y, z].gameObject);
            grid[w - 1, y, z].GetComponent<BoxController>().DespawnBlock();
            grid[w-1, y, z] = null;
        }
    }

    public static void decreasePlane(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y, 0] != null)
            {
                //Move one towards bottom
                grid[x, y - 1, 0] = grid[x, y, 0];
                grid[x, y, 0] = null;
                //Update Block position
                grid[x, y - 1, 0].position += new Vector3(0, -1, 0);
            }
            if (grid[x, y, d-1] != null)
            {
                //Move one towards bottom
                grid[x, y - 1, d-1] = grid[x, y, d-1];
                grid[x, y, d-1] = null;
                //Update Block position
                grid[x, y - 1, d-1].position += new Vector3(0, -1, 0);
            }
        }
        for (int z = 1; z < d-1; ++z)
        {
            if (grid[0, y, z] != null)
            {                
                //Move one towards bottom
                grid[0, y - 1, z] = grid[0, y, z];
                grid[0, y, z] = null;
                //Update Block position
                grid[0, y - 1, z].position += new Vector3(0, -1, 0);
            }
            if (grid[w-1, y, z] != null)
            {
                //Move one towards bottom
                grid[w-1, y - 1, z] = grid[w-1, y, z];
                grid[w-1, y, z] = null;
                //Update Block position
                grid[w-1, y - 1, z].position += new Vector3(0, -1, 0);
            }
        }
    }

    public static void decreasePlanesAbove(int y)
    {
        for (int i = y; i < h; ++i)
        {
            decreasePlane(i);
        }
    }

    public static bool isPlaneFull(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y, 0] == null) return false;
            if (grid[x, y, d-1] == null) return false;
        }
        for (int z = 1; z < d-1; ++z)
        {
            if (grid[0, y, z] == null) return false;
            if (grid[w-1, y, z] == null) return false;
        }
        return true;
    }

    public static void deletePlanesCoroutine(FichaController context)
    {
        context.StartCoroutine(deleteFullPlanes(context));
        
    }

    public static IEnumerator deleteFullPlanes(FichaController context)
    {
        bool firstPlaneLocated = false;
        int firstPlane = 0;
        int numPlanes = 0;

        for (int y = 0; y < h; ++y)
        {
            if (isPlaneFull(y))
            {
                if (!firstPlaneLocated)
                {
                    firstPlane = y;
                    firstPlaneLocated = true;
                    numPlanes++;
                }
                else
                {
                    numPlanes++;
                }
                deletePlane(y);
            }
        }
        FindObjectOfType<ScoreController>().addScore(10 * ScoreController.level);

        if (firstPlaneLocated)            
        {
            FindObjectOfType<AudioManager>().Play("line");
            yield return new WaitForSeconds(1.2f);
            //Debug.Log("Planes: +" + numPlanes);
            FindObjectOfType<ScoreController>().addScore(numPlanes == 4 ? 800 * ScoreController.level : numPlanes * 100 * ScoreController.level);
            FindObjectOfType<ScoreController>().addLines(4*numPlanes);
            FindObjectOfType<ScoreController>().updateLvl();
            FindObjectOfType<CameraController>().Stomp();
            FindObjectOfType<AudioManager>().Play("stomp");
            for (int y = firstPlane; numPlanes > 0; ++y)
            {
                decreasePlanesAbove(y + 1);
                --y;
                --numPlanes;
            }
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.2f);
        FindObjectOfType<Spawner>().spawnNext(context.tetraToSpawnFace());       
    }



}

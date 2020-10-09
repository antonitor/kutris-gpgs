using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int SpawnHeight;    
    public GameObject[] groups;
    public enum Face { FRONT, LEFT, BACK, RIGHT }

    GameObject nextTetromino;
    GameObject currentTetromino;
    NextShape nextShape;
    AudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        nextShape = FindObjectOfType<NextShape>();        

        int rand = Random.Range(0, groups.Length);
        nextTetromino = groups[rand];
        nextShape.UpdateShape(rand);

        spawnNext(Face.FRONT);

        audioManager.Play("theme");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnGhost()
    {

    }


    public void spawnNext(Face face)
    {
        int i = Random.Range(0, groups.Length);
        FichaController ficha;
        switch (face)
        {
            case Face.FRONT:
                ficha = Instantiate(getTetromino(), new Vector3(3, SpawnHeight, 7), Quaternion.identity).GetComponent<FichaController>();
                ficha.SetOrientation(FichaController.Orientation.HO_FRONT);
                FindObjectOfType<CameraController>().FollowFicha(ficha);
                break;
            case Face.LEFT:
                ficha = Instantiate(getTetromino(), new Vector3(7, SpawnHeight,3), Quaternion.identity).GetComponent<FichaController>();
                ficha.SetOrientation(FichaController.Orientation.HO_LEFT);
                FindObjectOfType<CameraController>().FollowFicha(ficha);
                break;
            case Face.BACK:
                ficha = Instantiate(getTetromino(), new Vector3(3, SpawnHeight, 0), Quaternion.identity).GetComponent<FichaController>();
                ficha.SetOrientation(FichaController.Orientation.HO_BACK);
                FindObjectOfType<CameraController>().FollowFicha(ficha);
                break;
            case Face.RIGHT:
                ficha = Instantiate(getTetromino(), new Vector3(0, SpawnHeight, 3), Quaternion.identity).GetComponent<FichaController>();
                ficha.SetOrientation(FichaController.Orientation.HO_RIGHT);
                FindObjectOfType<CameraController>().FollowFicha(ficha);
                break;
        }
        
    }

    public void setFace(Face face)
    {
        switch (face)
        {
            case Face.FRONT:
                transform.position = new Vector3(4, SpawnHeight, 7);
                break;
            case Face.LEFT:
                transform.position = new Vector3(7, SpawnHeight, 4);
                break;
            case Face.BACK:
                transform.position = new Vector3(4, SpawnHeight, 0);
                break;
            case Face.RIGHT:
                transform.position = new Vector3(4, SpawnHeight, 0);
                break;
        }
    }

    GameObject getTetromino()
    {
        currentTetromino = nextTetromino;
        int i = Random.Range(0, groups.Length);
        nextTetromino = groups[i];
        nextShape.UpdateShape(i);
        return currentTetromino;
    }

}

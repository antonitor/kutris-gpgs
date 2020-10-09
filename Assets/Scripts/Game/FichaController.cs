using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FichaController : MonoBehaviour
{
    //                                                                      T   I   J   L   S   Z   O                                                            
    public int HRoff;       //Cubes at the right of the central cube.       1   2   1   1   1   1   1    
    public int HLoff;       //Cubes at the left of the central cube.        1   1   1   1   1   1   0
    public int VUoff;       //Cubes on top of the central cube.             1   0   1   0   0   0   1
    public int VDoff;       //Cubes below the central cube.                 0   0   0   1   1   1   0

    public int ground = 7;  //Number of Ground tiles starting from 0

    public Transform ghostPrefab;

    Transform ghost;
    Joystick joy;
    Joybutton joybutton;
    AudioManager audioManager;
    FaceArrowLeft leftArrow;
    FaceArrowRight rightArrow;

    [HideInInspector]
    public float spawn_timer = 0;
    [HideInInspector]
    public bool landed = false;
    float move_timer;
    float flip_timer;
    float fall_timer;
    float last_fall = 0;
    float x, y, z; //ORIENTATION ANGLES



    void Start()
    {
        joy = FindObjectOfType<FloatingJoystick>();
        joybutton = FindObjectOfType<Joybutton>();
        audioManager = FindObjectOfType<AudioManager>();
        leftArrow = FindObjectOfType<FaceArrowLeft>();
        rightArrow = FindObjectOfType<FaceArrowRight>();

        if (!isValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
            FindObjectOfType<GameOver>().endGame();
        }
        x = 0;

        ghost = Instantiate(ghostPrefab, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);

    }

    // Update is called once per frame
    void Update()
    {
        move_timer += Time.deltaTime;
        flip_timer += Time.deltaTime;
        fall_timer += Time.deltaTime;
        Vector3 roundPos = Playfield.roundVec3(transform.position);

        //DESCENSO
        if (((joy.Vertical < -0.8f || Input.GetKey(KeyCode.DownArrow)) && fall_timer >= 0.05f - (ScoreController.level / 90)) || Time.time - last_fall >= 1 - (ScoreController.level / 12.5f))
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                //SHAKE CAMERA
                //landed = true;
                FindObjectOfType<CameraController>().Shake();

                //Play landing sound
                audioManager.Play("land");

                DespawnGhost();

                // Clear filled horizontal lines
                Playfield.deletePlanesCoroutine(this);

                // Spawn next Group
                //FindObjectOfType<Spawner>().spawnNext(tetraToSpawnFace());

                // Disable script
                enabled = false;
            }
            fall_timer = 0;
            last_fall = Time.time;
        }


        //DESPLAZAR DERECHA E IZQUIERDA
        if (move_timer >= 0.23f - (ScoreController.level / 75))
        {
            //IZQUIERDA
            if (joy.Horizontal < -0.5f || Input.GetKey(KeyCode.LeftArrow))
            {
                switch (GetOrientation())
                {
                    case Orientation.HO_FRONT:
                        if (roundPos.x < ground - HLoff)
                        {
                            transform.Translate(new Vector3(1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - HLoff)
                        {
                            transform.Translate(new Vector3(HLoff, 0, -HRoff), Space.World);
                            SetOrientation(Orientation.HO_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HLoff, 0, HRoff), Space.World);
                                SetOrientation(Orientation.HO_FRONT);
                            }
                        }
                        break;
                    case Orientation.HO_LEFT:
                        if (roundPos.z > HLoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1f), Space.World);
                        }
                        else if (roundPos.z == HLoff)
                        {
                            transform.Translate(new Vector3(-HRoff, 0, -HLoff), Space.World);
                            SetOrientation(Orientation.HO_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HRoff, 0, HLoff), Space.World);
                                SetOrientation(Orientation.HO_LEFT);
                            }
                        }
                        break;
                    case Orientation.HO_BACK:
                        if (roundPos.x > HLoff)
                        {
                            transform.Translate(new Vector3(-1f, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == HLoff)
                        {
                            transform.Translate(new Vector3(-HLoff, 0, HRoff), Space.World);
                            SetOrientation(Orientation.HO_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HLoff, 0, -HRoff), Space.World);
                                SetOrientation(Orientation.HO_BACK);
                            }
                        }
                        break;
                    case Orientation.HO_RIGHT:
                        if (roundPos.z < ground - HLoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1f), Space.World);
                        }
                        else if (roundPos.z == ground - HLoff)
                        {
                            transform.Translate(new Vector3(HRoff, 0, HLoff), Space.World);
                            SetOrientation(Orientation.HO_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HRoff, 0, -HLoff), Space.World);
                                SetOrientation(Orientation.HO_RIGHT);
                            }
                        }
                        break;
                    case Orientation.V_FRONT:
                        if (roundPos.x < ground - VDoff)
                        {
                            transform.Translate(new Vector3(1f, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - VDoff)
                        {
                            transform.Translate(new Vector3(VDoff, 0, -VUoff), Space.World);
                            SetOrientation(Orientation.V_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VDoff, 0, VUoff), Space.World);
                                SetOrientation(Orientation.V_FRONT);
                            }
                        }
                        break;
                    case Orientation.V_LEFT:
                        if (roundPos.z > VDoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1f), Space.World);
                        }
                        else if (roundPos.z == VDoff)
                        {
                            transform.Translate(new Vector3(-VUoff, 0, -VDoff), Space.World);
                            SetOrientation(Orientation.V_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VUoff, 0, VDoff), Space.World);
                                SetOrientation(Orientation.V_LEFT);
                            }
                        }
                        break;
                    case Orientation.V_BACK:
                        if (roundPos.x > VDoff)
                        {
                            transform.Translate(new Vector3(-1f, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == VDoff)
                        {
                            transform.Translate(new Vector3(-VDoff, 0, VUoff), Space.World);
                            SetOrientation(Orientation.V_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VDoff, 0, -VUoff), Space.World);
                                SetOrientation(Orientation.V_BACK);
                            }
                        }
                        break;
                    case Orientation.V_RIGHT:
                        if (roundPos.z < ground - VDoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1f), Space.World);
                        }
                        else if (roundPos.z == ground - VDoff)
                        {
                            transform.Translate(new Vector3(VUoff, 0, VDoff), Space.World);
                            SetOrientation(Orientation.V_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VUoff, 0, -VDoff), Space.World);
                                SetOrientation(Orientation.V_RIGHT);
                            }
                        }
                        break;
                    case Orientation.H_FRONT:
                        if (roundPos.x < ground - HRoff)
                        {
                            transform.Translate(new Vector3(1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - HRoff)
                        {
                            transform.Translate(new Vector3(HRoff, 0, -HLoff), Space.World);
                            SetOrientation(Orientation.H_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HRoff, 0, HLoff), Space.World);
                                SetOrientation(Orientation.H_FRONT);
                            }
                        }
                        break;
                    case Orientation.H_LEFT:
                        if (roundPos.z > HRoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1f), Space.World);
                        }
                        else if (roundPos.z == HRoff)
                        {
                            transform.Translate(new Vector3(-HLoff, 0, -HRoff), Space.World);
                            SetOrientation(Orientation.H_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HLoff, 0, HRoff), Space.World);
                                SetOrientation(Orientation.H_LEFT);
                            }
                        }
                        break;
                    case Orientation.H_BACK:
                        if (roundPos.x > HRoff)
                        {
                            transform.Translate(new Vector3(-1f, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == HRoff)
                        {
                            transform.Translate(new Vector3(-HRoff, 0, HLoff), Space.World);
                            SetOrientation(Orientation.H_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HRoff, 0, -HLoff), Space.World);
                                SetOrientation(Orientation.H_BACK);
                            }
                        }
                        break;
                    case Orientation.H_RIGHT:
                        if (roundPos.z < ground - HRoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1f), Space.World);
                        }
                        else if (roundPos.z == ground - HRoff)
                        {
                            transform.Translate(new Vector3(HLoff, 0, HRoff), Space.World);
                            SetOrientation(Orientation.H_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HLoff, 0, -HRoff), Space.World);
                                SetOrientation(Orientation.H_RIGHT);
                            }
                        }
                        break;
                    case Orientation.VRT_FRONT:
                        if (roundPos.x < ground - VUoff)
                        {
                            transform.Translate(new Vector3(1f, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - VUoff)
                        {
                            transform.Translate(new Vector3(VUoff, 0, -VDoff), Space.World);
                            SetOrientation(Orientation.VRT_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VUoff, 0, VDoff), Space.World);
                                SetOrientation(Orientation.VRT_FRONT);
                            }
                        }
                        break;
                    case Orientation.VRT_LEFT:
                        if (roundPos.z > VUoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1f), Space.World);
                        }
                        else if (roundPos.z == VUoff)
                        {
                            transform.Translate(new Vector3(-VDoff, 0, -VUoff), Space.World);
                            SetOrientation(Orientation.VRT_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VDoff, 0, VUoff), Space.World);
                                SetOrientation(Orientation.VRT_LEFT);
                            }
                        }
                        break;
                    case Orientation.VRT_BACK:
                        if (roundPos.x > VUoff)
                        {
                            transform.Translate(new Vector3(-1f, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == VUoff)
                        {
                            transform.Translate(new Vector3(-VUoff, 0, VDoff), Space.World);
                            SetOrientation(Orientation.VRT_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VUoff, 0, -VDoff), Space.World);
                                SetOrientation(Orientation.VRT_BACK);
                            }
                        }
                        break;
                    case Orientation.VRT_RIGHT:
                        if (roundPos.z < ground - VUoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1f), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1f), Space.World);
                        }
                        else if (roundPos.z == ground - VUoff)
                        {
                            transform.Translate(new Vector3(VDoff, 0, VUoff), Space.World);
                            SetOrientation(Orientation.VRT_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VDoff, 0, -VUoff), Space.World);
                                SetOrientation(Orientation.VRT_RIGHT);
                            }
                        }
                        break;
                }
                move_timer = 0;
            }

            //DERECHA
            if (joy.Horizontal > 0.5f || Input.GetKey(KeyCode.RightArrow))
            {
                switch (GetOrientation())
                {
                    case Orientation.HO_FRONT:
                        if (roundPos.x > HRoff)
                        {
                            transform.Translate(new Vector3(-1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == HRoff)
                        {
                            transform.Translate(new Vector3(-HRoff, 0, -HLoff), Space.World);
                            SetOrientation(Orientation.HO_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HRoff, 0, HLoff), Space.World);
                                SetOrientation(Orientation.HO_FRONT);
                            }
                        }
                        break;
                    case Orientation.HO_RIGHT:
                        if (roundPos.z > HRoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1), Space.World);
                        }
                        else if (roundPos.z == HRoff)
                        {
                            transform.Translate(new Vector3(HLoff, 0, -HRoff), Space.World);
                            SetOrientation(Orientation.HO_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HLoff, 0, HRoff), Space.World);
                                SetOrientation(Orientation.HO_RIGHT);
                            }
                        }
                        break;
                    case Orientation.HO_BACK:
                        if (roundPos.x < ground - HRoff)
                        {
                            transform.Translate(new Vector3(1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - HRoff)
                        {
                            transform.Translate(new Vector3(HRoff, 0, HLoff), Space.World);
                            SetOrientation(Orientation.HO_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HRoff, 0, -HLoff), Space.World);
                                SetOrientation(Orientation.HO_BACK);
                            }
                        }
                        break;
                    case Orientation.HO_LEFT:
                        if (roundPos.z < ground - HRoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1), Space.World);
                        }
                        else if (roundPos.z == ground - HRoff)
                        {
                            transform.Translate(new Vector3(-HLoff, 0, HRoff), Space.World);
                            SetOrientation(Orientation.HO_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HLoff, 0, -HRoff), Space.World);
                                SetOrientation(Orientation.HO_LEFT);
                            }
                        }
                        break;
                    case Orientation.V_FRONT:
                        if (roundPos.x > VUoff)
                        {
                            transform.Translate(new Vector3(-1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == VUoff)
                        {
                            transform.Translate(new Vector3(-VUoff, 0, -VDoff), Space.World);
                            SetOrientation(Orientation.V_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VUoff, 0, VDoff), Space.World);
                                SetOrientation(Orientation.V_FRONT);
                            }
                        }
                        break;
                    case Orientation.V_RIGHT:
                        if (roundPos.z > VUoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1), Space.World);
                        }
                        else if (roundPos.z == VUoff)
                        {
                            transform.Translate(new Vector3(VDoff, 0, -VUoff), Space.World);
                            SetOrientation(Orientation.V_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VDoff, 0, VUoff), Space.World);
                                SetOrientation(Orientation.V_RIGHT);
                            }
                        }
                        break;
                    case Orientation.V_BACK:
                        if (roundPos.x < ground - VUoff)
                        {
                            transform.Translate(new Vector3(1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - VUoff)
                        {
                            transform.Translate(new Vector3(VUoff, 0, VDoff), Space.World);
                            SetOrientation(Orientation.V_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VUoff, 0, -VDoff), Space.World);
                                SetOrientation(Orientation.V_BACK);
                            }
                        }
                        break;
                    case Orientation.V_LEFT:
                        if (roundPos.z < ground - VUoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1), Space.World);
                        }
                        else if (roundPos.z == ground - VUoff)
                        {
                            transform.Translate(new Vector3(-VDoff, 0, VUoff), Space.World);
                            SetOrientation(Orientation.V_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VDoff, 0, -VUoff), Space.World);
                                SetOrientation(Orientation.V_LEFT);
                            }
                        }
                        break;
                    case Orientation.H_FRONT:
                        if (roundPos.x > HLoff)
                        {
                            transform.Translate(new Vector3(-1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1f, 0, 0), Space.World);
                        }
                        else if (roundPos.x == HLoff)
                        {
                            transform.Translate(new Vector3(-HLoff, 0, -HRoff), Space.World);
                            SetOrientation(Orientation.H_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HLoff, 0, HRoff), Space.World);
                                SetOrientation(Orientation.H_FRONT);
                            }
                        }
                        break;
                    case Orientation.H_RIGHT:
                        if (roundPos.z > HLoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1), Space.World);
                        }
                        else if (roundPos.z == HLoff)
                        {
                            transform.Translate(new Vector3(HRoff, 0, -HLoff), Space.World);
                            SetOrientation(Orientation.H_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HRoff, 0, HLoff), Space.World);
                                SetOrientation(Orientation.H_RIGHT);
                            }
                        }
                        break;
                    case Orientation.H_BACK:
                        if (roundPos.x < ground - HLoff)
                        {
                            transform.Translate(new Vector3(1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - HLoff)
                        {
                            transform.Translate(new Vector3(HLoff, 0, HRoff), Space.World);
                            SetOrientation(Orientation.H_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-HLoff, 0, -HRoff), Space.World);
                                SetOrientation(Orientation.H_BACK);
                            }
                        }
                        break;
                    case Orientation.H_LEFT:
                        if (roundPos.z < ground - HLoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1), Space.World);
                        }
                        else if (roundPos.z == ground - HLoff)
                        {
                            transform.Translate(new Vector3(-HRoff, 0, HLoff), Space.World);
                            SetOrientation(Orientation.H_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(HRoff, 0, -HLoff), Space.World);
                                SetOrientation(Orientation.H_LEFT);
                            }
                        }
                        break;
                    case Orientation.VRT_FRONT:
                        if (roundPos.x > VDoff)
                        {
                            transform.Translate(new Vector3(-1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == VDoff)
                        {
                            transform.Translate(new Vector3(-VDoff, 0, -VUoff), Space.World);
                            SetOrientation(Orientation.VRT_RIGHT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VDoff, 0, VUoff), Space.World);
                                SetOrientation(Orientation.VRT_FRONT);
                            }
                        }
                        break;
                    case Orientation.VRT_RIGHT:
                        if (roundPos.z > VDoff)
                        {
                            transform.Translate(new Vector3(0, 0, -1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, 1), Space.World);
                        }
                        else if (roundPos.z == VDoff)
                        {
                            transform.Translate(new Vector3(VUoff, 0, -VDoff), Space.World);
                            SetOrientation(Orientation.VRT_BACK);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VUoff, 0, VDoff), Space.World);
                                SetOrientation(Orientation.VRT_RIGHT);
                            }
                        }
                        break;
                    case Orientation.VRT_BACK:
                        if (roundPos.x < ground - VDoff)
                        {
                            transform.Translate(new Vector3(1, 0, 0), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(-1, 0, 0), Space.World);
                        }
                        else if (roundPos.x == ground - VDoff)
                        {
                            transform.Translate(new Vector3(VDoff, 0, VUoff), Space.World);
                            SetOrientation(Orientation.VRT_LEFT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(-VDoff, 0, -VUoff), Space.World);
                                SetOrientation(Orientation.VRT_BACK);
                            }
                        }
                        break;
                    case Orientation.VRT_LEFT:
                        if (roundPos.z < ground - VDoff)
                        {
                            transform.Translate(new Vector3(0, 0, 1), Space.World);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else transform.Translate(new Vector3(0, 0, -1), Space.World);
                        }
                        else if (roundPos.z == ground - VDoff)
                        {
                            transform.Translate(new Vector3(-VUoff, 0, VDoff), Space.World);
                            SetOrientation(Orientation.VRT_FRONT);
                            if (isValidGridPos())
                            {
                                updateGrid();
                                audioManager.Play("move");
                            }
                            else
                            {
                                transform.Translate(new Vector3(VUoff, 0, -VDoff), Space.World);
                                SetOrientation(Orientation.VRT_LEFT);
                            }
                        }
                        break;
                }
                move_timer = 0;
            }
        }


        //CAMBIO RAPIDO DE CARA
        //IZQUIERDA
        if ((Input.GetKeyDown(KeyCode.A) || leftArrow.pressed) && move_timer > 0.23f)
        {
            Vector3 originalPossition = transform.position;
            switch (GetOrientation())
            {
                case Orientation.HO_FRONT:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.HO_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_FRONT);
                    }
                    break;
                case Orientation.HO_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.HO_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_LEFT);
                    }
                    break;
                case Orientation.HO_BACK:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.HO_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_BACK);
                    }
                    break;
                case Orientation.HO_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.HO_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_RIGHT);
                    }
                    break;
                case Orientation.V_FRONT:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.V_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_FRONT);
                    }
                    break;
                case Orientation.V_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.V_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_LEFT);
                    }
                    break;
                case Orientation.V_BACK:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.V_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_BACK);
                    }
                    break;
                case Orientation.V_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.V_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_RIGHT);
                    }
                    break;
                case Orientation.H_FRONT:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.H_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_FRONT);
                    }
                    break;
                case Orientation.H_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.H_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_LEFT);
                    }
                    break;
                case Orientation.H_BACK:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.H_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_BACK);
                    }
                    break;
                case Orientation.H_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.H_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_RIGHT);
                    }
                    break;
                case Orientation.VRT_FRONT:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.VRT_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_FRONT);
                    }
                    break;
                case Orientation.VRT_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.VRT_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_LEFT);
                    }
                    break;
                case Orientation.VRT_BACK:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.VRT_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_BACK);
                    }
                    break;
                case Orientation.VRT_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.VRT_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_RIGHT);
                    }
                    break;
            }
            move_timer = 0;
        }

        //CAMBIO RAPIDO DE CARA
        //DERECHA
        if ((Input.GetKeyDown(KeyCode.D) || rightArrow.pressed) && move_timer > 0.23f)
        {
            Vector3 originalPossition = transform.position;
            switch (GetOrientation())
            {
                case Orientation.HO_FRONT:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.HO_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_FRONT);
                    }
                    break;
                case Orientation.HO_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.HO_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_RIGHT);
                    }
                    break;
                case Orientation.HO_BACK:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.HO_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_BACK);
                    }
                    break;
                case Orientation.HO_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.HO_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.HO_LEFT);
                    }
                    break;
                case Orientation.V_FRONT:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.V_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_FRONT);
                    }
                    break;
                case Orientation.V_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.V_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_RIGHT);
                    }
                    break;
                case Orientation.V_BACK:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.V_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_BACK);
                    }
                    break;
                case Orientation.V_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.V_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.V_LEFT);
                    }
                    break;
                case Orientation.H_FRONT:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.H_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_FRONT);
                    }
                    break;
                case Orientation.H_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.H_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_RIGHT);
                    }
                    break;
                case Orientation.H_BACK:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.H_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_BACK);
                    }
                    break;
                case Orientation.H_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.H_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.H_LEFT);
                    }
                    break;
                case Orientation.VRT_FRONT:
                    transform.position = new Vector3(0, originalPossition.y, 4);
                    SetOrientation(Orientation.VRT_RIGHT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_FRONT);
                    }
                    break;
                case Orientation.VRT_RIGHT:
                    transform.position = new Vector3(4, originalPossition.y, 0);
                    SetOrientation(Orientation.VRT_BACK);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_RIGHT);
                    }
                    break;
                case Orientation.VRT_BACK:
                    transform.position = new Vector3(7, originalPossition.y, 4);
                    SetOrientation(Orientation.VRT_LEFT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_BACK);
                    }
                    break;
                case Orientation.VRT_LEFT:
                    transform.position = new Vector3(4, originalPossition.y, 7);
                    SetOrientation(Orientation.VRT_FRONT);
                    if (isValidGridPos())
                    {
                        last_fall = Time.time;
                        updateGrid();
                        audioManager.Play("move");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        transform.position = originalPossition;
                        SetOrientation(Orientation.VRT_LEFT);
                    }
                    break;
            }
            move_timer = 0;            
        }



        //FLIP - GIRAR PIEZA
        if ((joybutton.pressed || Input.GetKey(KeyCode.UpArrow) || joy.Vertical > 0.9f) && flip_timer >= 0.26f - (ScoreController.level / 100))
        {
            switch (GetOrientation())
            {
                case Orientation.HO_FRONT:
                    SetOrientation(Orientation.V_FRONT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.HO_FRONT);
                    }
                    break;
                case Orientation.HO_LEFT:
                    SetOrientation(Orientation.V_LEFT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.HO_LEFT);
                    }
                    break;
                case Orientation.HO_BACK:
                    SetOrientation(Orientation.V_BACK);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.HO_BACK);
                    }
                    break;
                case Orientation.HO_RIGHT:
                    SetOrientation(Orientation.V_RIGHT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.HO_RIGHT);
                    }
                    break;
                case Orientation.VRT_FRONT:
                    SetOrientation(Orientation.HO_FRONT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.VRT_FRONT);
                    }
                    break;
                case Orientation.VRT_LEFT:
                    SetOrientation(Orientation.HO_LEFT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.VRT_LEFT);
                    }
                    break;
                case Orientation.VRT_BACK:
                    SetOrientation(Orientation.HO_BACK);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.VRT_BACK);
                    }
                    break;
                case Orientation.VRT_RIGHT:
                    SetOrientation(Orientation.HO_RIGHT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.VRT_RIGHT);
                    }
                    break;
                case Orientation.H_FRONT:
                    SetOrientation(Orientation.VRT_FRONT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.H_FRONT);
                    }
                    break;
                case Orientation.H_LEFT:
                    SetOrientation(Orientation.VRT_LEFT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.H_LEFT);
                    }
                    break;
                case Orientation.H_BACK:
                    SetOrientation(Orientation.VRT_BACK);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.H_BACK);
                    }
                    break;
                case Orientation.H_RIGHT:
                    SetOrientation(Orientation.VRT_RIGHT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.H_RIGHT);
                    }
                    break;
                case Orientation.V_FRONT:
                    SetOrientation(Orientation.H_FRONT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.V_FRONT);
                    }
                    break;
                case Orientation.V_LEFT:
                    SetOrientation(Orientation.H_LEFT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.V_LEFT);
                    }
                    break;
                case Orientation.V_BACK:
                    SetOrientation(Orientation.H_BACK);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.V_BACK);
                    }
                    break;
                case Orientation.V_RIGHT:
                    SetOrientation(Orientation.H_RIGHT);
                    if (isValidGridPos())
                    {
                        updateGrid();
                        audioManager.Play("flip");
                    }
                    else
                    {
                        audioManager.Play("nono");
                        SetOrientation(Orientation.V_RIGHT);

                    }
                    break;
            }
            flip_timer = 0;
        }

        UpdateGhost();
    }

    public enum Orientation
    {
        HO_FRONT, HO_LEFT, HO_BACK, HO_RIGHT, VRT_FRONT, VRT_LEFT, VRT_BACK, VRT_RIGHT,
        H_FRONT, H_LEFT, H_BACK, H_RIGHT, V_FRONT, V_LEFT, V_BACK, V_RIGHT
    }

    Orientation GetOrientation()
    {
        if (y == 0 && z == 0) return Orientation.HO_FRONT;
        else if (y == 90 && z == 0) return Orientation.HO_LEFT;
        else if (y == 180 && z == 0) return Orientation.HO_BACK;
        else if (y == 270 && z == 0) return Orientation.HO_RIGHT;
        else if (y == 0 && z == 270) return Orientation.VRT_FRONT;
        else if (y == 90 && z == 270) return Orientation.VRT_LEFT;
        else if (y == 180 && z == 270) return Orientation.VRT_BACK;
        else if (y == 270 && z == 270) return Orientation.VRT_RIGHT;

        else if (y == 0 && z == 180) return Orientation.H_FRONT;
        else if (y == 90 && z == 180) return Orientation.H_LEFT;
        else if (y == 180 && z == 180) return Orientation.H_BACK;
        else if (y == 270 && z == 180) return Orientation.H_RIGHT;
        else if (y == 0 && z == 90) return Orientation.V_FRONT;
        else if (y == 90 && z == 90) return Orientation.V_LEFT;
        else if (y == 180 && z == 90) return Orientation.V_BACK;
        else if (y == 270 && z == 90) return Orientation.V_RIGHT;
        else throw new Exception("------- WRONG ROTATION ------( " + x + "," + y + "," + z + ")");
    }

    public void SetOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.HO_FRONT:
                y = 0f; z = 0f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.HO_LEFT:
                y = 90f; z = 0f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.HO_BACK:
                y = 180f; z = 0f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.HO_RIGHT:
                y = 270f; z = 0f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.VRT_FRONT:
                y = 0f; z = 270f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.VRT_LEFT:
                y = 90; z = 270f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.VRT_BACK:
                y = 180f; z = 270f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.VRT_RIGHT:
                y = 270f; z = 270f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.H_FRONT:
                y = 0f; z = 180f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.H_LEFT:
                y = 90f; z = 180f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.H_BACK:
                y = 180f; z = 180f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.H_RIGHT:
                y = 270f; z = 180f;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.V_FRONT:
                y = 0f; z = 90F;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.V_LEFT:
                y = 90f; z = 90F;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.V_BACK:
                y = 180f; z = 90F;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
            case Orientation.V_RIGHT:
                y = 270f; z = 90F;
                transform.rotation = Quaternion.Euler(x, y, z);
                break;
        }
    }

    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector3 v = Playfield.roundVec3(child.position);
            //Debug.Log(GetOrientation().ToString() + v + " GRID INSIDE BORDERS? " + Playfield.insideBorder(v));
            // Not inside Border?
            if (!Playfield.insideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Playfield.grid[(int)v.x, (int)v.y, (int)v.z] != null &&
                Playfield.grid[(int)v.x, (int)v.y, (int)v.z].parent != transform)
                return false;
        }
        return true;
    }

    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < Playfield.h; ++y)
            for (int x = 0; x < Playfield.w; ++x)
                for (int z = 0; z < Playfield.d; ++z)
                    if (Playfield.grid[x, y, z] != null)
                        if (Playfield.grid[x, y, z].parent == transform)
                            Playfield.grid[x, y, z] = null;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector3 v = Playfield.roundVec3(child.position);
            Playfield.grid[(int)v.x, (int)v.y, (int)v.z] = child;
        }
    }

    public Spawner.Face tetraToSpawnFace()
    {
        switch (GetOrientation())
        {
            case Orientation.HO_FRONT:
                return Spawner.Face.FRONT;
            case Orientation.VRT_FRONT:
                return Spawner.Face.FRONT;
            case Orientation.H_FRONT:
                return Spawner.Face.FRONT;
            case Orientation.V_FRONT:
                return Spawner.Face.FRONT;
            case Orientation.HO_LEFT:
                return Spawner.Face.LEFT;
            case Orientation.VRT_LEFT:
                return Spawner.Face.LEFT;
            case Orientation.H_LEFT:
                return Spawner.Face.LEFT;
            case Orientation.V_LEFT:
                return Spawner.Face.LEFT;
            case Orientation.HO_BACK:
                return Spawner.Face.BACK;
            case Orientation.VRT_BACK:
                return Spawner.Face.BACK;
            case Orientation.H_BACK:
                return Spawner.Face.BACK;
            case Orientation.V_BACK:
                return Spawner.Face.BACK;
            case Orientation.HO_RIGHT:
                return Spawner.Face.RIGHT;
            case Orientation.VRT_RIGHT:
                return Spawner.Face.RIGHT;
            case Orientation.H_RIGHT:
                return Spawner.Face.RIGHT;
            case Orientation.V_RIGHT:
                return Spawner.Face.RIGHT;
            default: return Spawner.Face.FRONT;
        }
    }

    void UpdateGhost()
    {
        if (ghost != null)
        {
            ghost.rotation = transform.rotation;
            
            ghost.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            for (int gy = (int)ghost.position.y; gy >= 0; gy--)
            {
                ghost.transform.Translate(new Vector3(0, -1, 0), Space.World);
                if (!isValidGhostPos())
                {
                    ghost.transform.Translate(new Vector3(0, 1, 0), Space.World);
                    break;
                }
            }
        }
    }    

    bool isValidGhostPos()
    {
        foreach (Transform child in ghost.transform)
        {
            Vector3 v = Playfield.roundVec3(child.position);
            // Not inside Border?
            if (!Playfield.insideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Playfield.grid[(int)v.x, (int)v.y, (int)v.z] != null &&
                Playfield.grid[(int)v.x, (int)v.y, (int)v.z].parent != ghost.transform &&
                Playfield.grid[(int)v.x, (int)v.y, (int)v.z].parent != transform)
                return false;
        }
        return true;
    }

    void DespawnGhost()
    {
        while (ghost.transform.childCount > 0)
        {
            Transform child = ghost.transform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }
        Destroy(ghost.gameObject);
    }
}
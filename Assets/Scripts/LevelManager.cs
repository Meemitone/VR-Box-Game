using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class LevelManager : MonoBehaviour
{
    [Header("Buttons")]
    public bool moveThemUp = false;
    public bool moveThemIn = false;
    
    [Header("Settings")]
    public bool StartAtSpawn;
    public Vector3 moveInSpawn = new Vector3(0, -100, 0);
    public Vector3 onyanSpawn = new Vector3(0, 3, 0);
    public float cubeMoveNum;
    public float inLerpFloat = 0.01f;
    public float inLerpTolorance = 0.0001f;
    public float upSpeedCap = 5f;
    public float moveUpAcceleration = 0.01f;
    public float moveUpTime = 10;
    public float upCountSpeed = 0.2f;
    public float inCountSpeed = 0.2f;
    
    [Header("Arrays")]
    public GameObject[] cubes;
    public Vector3[] cubesSavedPos;
    public bool[] cubesActive;
    public bool[] cubesMoveUp;
    public bool[] cubesMoveIn;
    public float[] cubesMoveSpeed;

    private GameObject player;
    public int targetSceneIndex;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cubes = GameObject.FindGameObjectsWithTag("Cube");
        cubesSavedPos = new Vector3[cubes.Length];
        cubesActive = new bool[cubes.Length];
        cubesMoveUp = new bool[cubes.Length];
        cubesMoveIn = new bool[cubes.Length];
        cubesMoveSpeed = new float[cubes.Length]; //sets the array lengths

        for (int I = 0; I < cubes.Length; I++)
        {
            cubesActive[I] = true;
            cubesMoveUp[I] = false;
            cubesMoveIn[I] = false;
            cubesMoveSpeed[I] = 0f;
        }
        
        // initialises the arrays
        ArrayShuffle(cubes); // shuffles the list of cubes, so that they move randomly
        
        for (int I = 0; I < cubes.Length; I++)
        {
            cubesSavedPos[I] = cubes[I].transform.position; // saves the position of all of the cubes in the level
        }

        player.transform.position = onyanSpawn; // moves the onyan to the player's head

        if (StartAtSpawn)
        {
            for (int I = 0; I < cubes.Length; I++)
            {
                cubes[I].transform.position = moveInSpawn; // moves them all to the spawn point
                cubes[I].SetActive(false);
                cubesActive[I] = false;
            }
        }

        upSpeedCap = moveUpAcceleration * moveUpTime * 100;
    }
    void Update()
    {
        if(moveThemIn && !moveThemUp) MoveEmIn();
        if(moveThemUp && !moveThemIn) MoveEmUp();
    }
    void MoveEmIn()
    {
        if (cubeMoveNum <= cubes.Length - 1)
        {
            cubeMoveNum += inCountSpeed;
        }
        for (int I = 0; I < cubeMoveNum; I++)
        {
            cubesMoveIn[I] = true;
        } // increases the number of cubes that are moving
        
        for (int I = 0; I < cubes.Length; I++)
        {
            if (cubesMoveIn[I])
            {
                cubes[I].SetActive(true); // spawns the cubes
                cubesActive[I] = true;
                
                cubes[I].gameObject.transform.position = Vector3.Lerp(cubes[I].gameObject.transform.position, cubesSavedPos[I], inLerpFloat); // lerps from spawn to saved pos

                if (Vector3.Distance(cubes[I].transform.position, cubesSavedPos[I]) <= inLerpTolorance)
                {
                    cubes[I].transform.position = cubesSavedPos[I]; // if they are close enough, snap to saved pos
                }
            }
        }

        if (cubes[cubes.Length - 1].transform.position == cubesSavedPos[cubes.Length - 1]) // stops doing the thing and resets the number if the last one is in place
        {
            for (int I = 0; I < cubes.Length; I++) cubes[I].transform.position = cubesSavedPos[I];
            cubeMoveNum = 0;
            player.GetComponent<PlayerMovement>().Cease(player.GetComponent<PlayerMovement>().prog.resetFace, player.GetComponent<PlayerMovement>().prog.resetDir); // Jump from the player's head to the spawn cube
            moveThemIn = false;
        }
    }
    void MoveEmUp() // ----------------------------------------------------------------end level animation-------------------------------------------
    {

        if (cubeMoveNum <= cubes.Length - 1)
        {
            cubeMoveNum += upCountSpeed;
        }
        for (int I = 0; I < cubeMoveNum; I++)
        {
            cubesMoveUp[I] = true;
        } // increases the number of cubes that are moving
        
        
        for (int I = 0; I < cubes.Length; I++)
        {
            if (cubesMoveUp[I])
            {
                if (cubesMoveSpeed[I] >= upSpeedCap)
                {
                    cubesActive[I] = false;
                    cubes[I].gameObject.SetActive(false); // if the cube is high enough (measured by speed) then stop moving it and turn it off
                }
                else
                {
                    cubesMoveSpeed[I] += moveUpAcceleration;
                    cubes[I].transform.position += new Vector3(0, cubesMoveSpeed[I], 0); // else accelerate it upwards
                }
            }
        } // moves the cubes up

        if (cubesMoveSpeed[cubes.Length - 1] >= upSpeedCap) // if the last one is in place, stop doing the thing
        {
            cubesActive[cubes.Length - 1] = false;
            cubes[cubes.Length - 1].gameObject.SetActive(false);
            cubeMoveNum = 0;
            moveThemUp = false; // finishes moving them up
            SceneManager.LoadScene(targetSceneIndex); // loads the saved scene name
        }
    } // ----------------------------------------------------------------end level animation-------------------------------------------
    void ArrayShuffle(GameObject[] array)
    {
        GameObject tempGO;

        for (int I = 0; I < array.Length; I++)
        {
            Random r = new Random();
            int rnd = r.Next(I,array.Length);
            
            tempGO = array[rnd]; // picks a random element in the array, makes a temporary copy
            array[rnd] = array[I]; // sets that random one to this one
            array[I] = tempGO; // sets this one to that random one, we have effectively swapped array[I] with array[rnd]

        }
    }

    public void WinLevel(int LVL)
    {
        targetSceneIndex = LVL;
        moveThemUp = true;
        player.SetActive(false); // turns off onyan-san
    }
    public void loadScene(int index)
    {
        SceneManager.LoadScene(index);
        // scene 0 will be the main menu, scene 1 will be the victory screen, scene 2 will be tutorial, scenes 3+ will be levels 1+
    }
}

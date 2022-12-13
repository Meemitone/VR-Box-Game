using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaceScript : MonoBehaviour
{
    public FaceType faceT;
    [HideInInspector]
    public FaceScript faceNorth = null, faceWest = null, faceEast = null, faceSouth = null; //front is shorter than foreward
    private int CubeLayer = 1 << 6;//this is the layer that cubes are on, the faces occupy layer 7
    [SerializeField] private FaceCheckerScript north, south, east, west; //these are the face checkers
    public CubeScript.dirs sourcedir;

    public CubeScript.dirs nDir, sDir, eDir, wDir;
    public CubeScript myCube;

    public enum FaceType
    {
        DEFAULT, //No trouble here, moveable terrain nothing more, nothing less
        BLOCK, //No entry
        KILL, //Entry Causes Reset
        SPRING, //Use will put you over a pit, not a block (easier to code)
        CHECK, //checkpoint
        WIN, // win
    }

    public void doAwake()
    {
        //check if this face is inside a cube, if so, then it can't be traversed 
        Collider[] checklist = Physics.OverlapSphere(transform.position, 0.001f, CubeLayer, QueryTriggerInteraction.Collide);
        if (checklist.Length > 1)
            Debug.Log("Face hit multiple cubes", transform.parent.gameObject);
        if (checklist.Length > 0)
            gameObject.SetActive(false);//face hit at least 1 cube so disable it (and therefore it's childs)
        if (faceT == FaceType.BLOCK)
        { //neccesary? all this really does is make it unleavable
            gameObject.SetActive(false);
        }
        myCube = transform.parent.gameObject.GetComponent<CubeScript>();
        north.doAwake();
        south.doAwake();
        east.doAwake();
        west.doAwake();
    }

    public CubeScript.dirs GetMoveDir(CubeScript.dirs facing)
    {
        FaceScript target = GetFaceInDir(facing);
        if (target.sourcedir == sourcedir)
        {//this doesn't work with the idea of rotated cubes
            return facing;
        }
        if (target.sourcedir == facing)
        {
            //wrap around
            return myCube.OppositeDir(sourcedir);
        }

        {
            //climbing scenario
            return sourcedir;
        }
        //I overestimated how difficult this was to find
    }

    private void Start()
    {
        //Awake is called when the scene loads, start is called before the first frame if the object is enabled
        //I need to assign the facechecker directions after this gets it's direction from the cube script, so I do that now that awake has happened
        switch (sourcedir)
        {
            //this might need updating when rotation happens
            case CubeScript.dirs.BACK:
                north.direction = CubeScript.dirs.UP;
                south.direction = CubeScript.dirs.DOWN;
                east.direction = CubeScript.dirs.RIGHT;
                west.direction = CubeScript.dirs.LEFT; 
                break;
            case CubeScript.dirs.UP:
                north.direction = CubeScript.dirs.FOREWARD;
                south.direction = CubeScript.dirs.BACK;
                east.direction = CubeScript.dirs.RIGHT;
                west.direction = CubeScript.dirs.LEFT;
                break;
            case CubeScript.dirs.DOWN:
                north.direction = CubeScript.dirs.FOREWARD;
                south.direction = CubeScript.dirs.BACK;
                east.direction = CubeScript.dirs.LEFT;
                west.direction = CubeScript.dirs.RIGHT;
                break;
            case CubeScript.dirs.FOREWARD:
                north.direction = CubeScript.dirs.DOWN;
                south.direction = CubeScript.dirs.UP;
                east.direction = CubeScript.dirs.RIGHT;
                west.direction = CubeScript.dirs.LEFT;
                break;
            case CubeScript.dirs.LEFT:
                north.direction = CubeScript.dirs.FOREWARD;
                south.direction = CubeScript.dirs.BACK;
                east.direction = CubeScript.dirs.UP;
                west.direction = CubeScript.dirs.DOWN;
                break;
            case CubeScript.dirs.RIGHT:
                north.direction = CubeScript.dirs.FOREWARD;
                south.direction = CubeScript.dirs.BACK;
                east.direction = CubeScript.dirs.DOWN;
                west.direction = CubeScript.dirs.UP;
                break;
            default:
                Debug.Log("Face failed to initialize", this);
                break;
        }//this switch statement assigns all the directions to the childed checkers
        //get face links, east is commented, the rest are the same but replace east with new direction
        {
            //east block
            {
                if (east.cube != null)
                {
                    if (east.cube.transform.rotation == myCube.transform.rotation)
                    {//if the cubes have the same rotation, get the face in this direction the old fashioned way
                        faceEast = east.cube.GetExtWrapFace(sourcedir, east.direction);
                    }
                    else
                    {//if there's a rotation involved between the cubes then the target face should (SHOULD) be between the checker and this face (looks like "_|.") so a raycast from the checker to this should (SHOULD) find that sandwiched face
                        RaycastHit hit;
                        if(Physics.Raycast(east.transform.position, transform.position-east.transform.position, out hit ,(transform.position - east.transform.position).magnitude, LayerMask.GetMask("Faces"), QueryTriggerInteraction.Collide))
                        {
                            faceEast = hit.collider.gameObject.GetComponent<FaceScript>();
                        }
                        else
                        {
                            Debug.Log("East Raycast missed", this);
                        }
                    }
                }
                else if (east.face != null)
                {//if the checker found a face, easy money
                    faceEast = east.face;
                }
                else
                {//if the checker found nothing, then heading in the checkers direction wraps you around the cube
                    faceEast = myCube.GetIntWrapperFace(sourcedir, east.direction);
                }
                eDir = east.direction;
            }
            //west block
            {
                if (west.cube != null)
                {
                    if (west.cube.transform.rotation == myCube.transform.rotation)
                    {
                        faceWest = west.cube.GetExtWrapFace(sourcedir, west.direction);
                    }
                    else
                    { 
                        RaycastHit hit;
                        if (Physics.Raycast(west.transform.position, transform.position - west.transform.position, out hit, (transform.position - west.transform.position).magnitude, LayerMask.GetMask("Faces"), QueryTriggerInteraction.Collide))
                        {
                            faceWest = hit.collider.gameObject.GetComponent<FaceScript>();
                        }
                        else
                        {
                            Debug.Log("West Raycast missed", this);
                        }
                    }
                }
                else if (west.face != null)
                {
                    faceWest = west.face;
                }
                else
                {
                    faceWest = myCube.GetIntWrapperFace(sourcedir, west.direction);
                }
                wDir = west.direction;
            }
            //north block
            {
                if (north.cube != null)
                {
                    if (north.cube.transform.rotation == myCube.transform.rotation)
                    {
                        faceNorth = north.cube.GetExtWrapFace(sourcedir, north.direction);
                    }
                    else
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(north.transform.position, transform.position - north.transform.position, out hit, (transform.position - north.transform.position).magnitude, LayerMask.GetMask("Faces"), QueryTriggerInteraction.Collide))
                        {
                            faceNorth = hit.collider.gameObject.GetComponent<FaceScript>();
                        }
                        else
                        {
                            Debug.Log("North Raycast missed", this);
                        }
                    }
                }
                else if (north.face != null)
                {
                    faceNorth = north.face;
                }
                else
                {
                    faceNorth = myCube.GetIntWrapperFace(sourcedir, north.direction);
                }
                nDir = north.direction;
            }
            //south block
            {
                if (south.cube != null)
                {
                    if (south.cube.transform.rotation == myCube.transform.rotation)
                    {
                        faceSouth = south.cube.GetExtWrapFace(sourcedir, south.direction);
                    }
                    else
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(south.transform.position, transform.position - south.transform.position, out hit, (transform.position - south.transform.position).magnitude, LayerMask.GetMask("Faces"), QueryTriggerInteraction.Collide))
                        {
                            faceSouth = hit.collider.gameObject.GetComponent<FaceScript>();
                        }
                        else
                        {
                            Debug.Log("South Raycast missed", this);
                        }
                    }
                }
                else if (south.face != null)
                {
                    faceSouth = south.face;
                }
                else
                {
                    faceSouth = myCube.GetIntWrapperFace(sourcedir, south.direction);
                }
                sDir = south.direction;
            }
        }
        //bracketed to make collapsable in editor
    }
    public FaceScript GetFaceInDir(CubeScript.dirs facing)
    {
        if (facing == nDir)//if facing north
        {
            if (faceNorth.faceT == FaceType.BLOCK)//if the face in the north direction is a block
                return null; //tell the source that the move is invalid
            return faceNorth;//else give north face
        }
        if (facing == sDir)
        {
            if (faceSouth.faceT == FaceType.BLOCK)
                return null;
            return faceSouth;
        }
        if (facing == eDir)
        {
            if (faceEast.faceT == FaceType.BLOCK)
                return null;
            return faceEast;
        }
        if (facing == wDir)
        {
            if (faceWest.faceT == FaceType.BLOCK)
                return null;
            return faceWest;
        }
        Debug.Log("GetFaceInDir recieved invalid dir (Up or down while on the up face for e.g.)", this);
        return null;
    }

    public bool Enter(PlayerMovement player)
    {
        //called when the player enters this face
        //returns true if the player needs to run another resolve and be stopped after
        //returns false if the player need not be interrupted
        switch (faceT)//depending on this faces type
        {
            case FaceType.CHECK://trigger checkpoint interaction
                if (player.prog.resetFace != this)
                {
                    player.prog.resetFace = this;
                    if (sourcedir != CubeScript.dirs.FOREWARD && sourcedir != CubeScript.dirs.BACK)
                    {
                        player.prog.resetDir = CubeScript.dirs.FOREWARD;
                        player.prog.RunStop();
                    }
                    else
                    {
                        player.prog.resetDir = CubeScript.dirs.UP;
                        player.prog.RunStop();
                    }
                    return true;
                }
                break;
            case FaceType.KILL://kill the player
                player.Cease(player.prog.resetFace, player.prog.resetDir);
                return false;//return false because Cease initializes a terminating resolve
            case FaceType.WIN://win the level
                LevelManager LVL = FindObjectOfType<LevelManager>();
                LVL.WinLevel(0);
                return false;//player norts self as per LVL.Win
            default://enter did nothing
                return false;
        }
        return false;//this is to avoid a "not all paths return a value" warning, it's actually unreachable
    }
}

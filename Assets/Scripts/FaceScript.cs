using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScript : MonoBehaviour
{
    public FaceType faceT;
    [HideInInspector]
    public FaceScript faceNorth = null, faceWest = null, faceEast = null, faceSouth = null; //front is shorter than foreward
    private int CubeLayer = 1 << 6;//this is the layer that cubes are on, the faces occupy layer 7
    [SerializeField] private FaceCheckerScript north, south, east, west; //these are the face checkers
    public CubeScript.dirs sourcedir;
    private CubeScript.dirs checkDir;

    public CubeScript.dirs nDir, sDir, eDir, wDir;
    public CubeScript myCube;

    public enum FaceType
    {
        DEFAULT, //No trouble here, moveable terrain nothing more, nothing less
        BLOCK, //No entry
        KILL, //Entry Causes Reset
        SPRING, //Use will put you over a pit, not a block (easier to code)
        CHECK, //checkpoint
    }

    public void doAwake()
    {
        //check if this face is inside a cube, if so, then it can't be traversed 
        Collider[] checklist = Physics.OverlapSphere(transform.position, 0.01f, CubeLayer, QueryTriggerInteraction.Collide);
        if (checklist.Length > 1)
            Debug.Log("Face hit multiple cubes", transform.parent.gameObject);
        if (checklist.Length > 0)
            gameObject.SetActive(false);//face hit at least 1 cube so disable it (and therefore it's childs
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
        {
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
        //I need to assign the facechecker directions after this gets it's direction from the cube script, so I do that now
        switch (sourcedir)
        {
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
        //get face links
        {
            //east block
            {
                if (east.cube != null)
                {
                    faceEast = east.cube.GetExtWrapFace(sourcedir, east.direction);
                }
                else if (east.face != null)
                {
                    faceEast = east.face;
                }
                else
                {
                    faceEast = myCube.GetIntWrapperFace(sourcedir, east.direction);
                }
                eDir = east.direction;
            }
            //west block
            {
                if (west.cube != null)
                {
                    faceWest = west.cube.GetExtWrapFace(sourcedir, west.direction);
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
                    faceNorth = north.cube.GetExtWrapFace(sourcedir, north.direction);
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
                    faceSouth = south.cube.GetExtWrapFace(sourcedir, south.direction);
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
        if (facing == nDir)
        {
            if (faceNorth.faceT == FaceType.BLOCK)
                return null;
            return faceNorth;
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
        return null;
    }

    public bool Enter(PlayerMovement player)
    {
        switch(faceT)
        {
            case FaceType.CHECK:
                if (player.prog.resetFace != this)
                {
                    if (sourcedir != CubeScript.dirs.FOREWARD && sourcedir != CubeScript.dirs.BACK)
                        player.Cease(this, CubeScript.dirs.FOREWARD);
                    else
                        player.Cease(this, CubeScript.dirs.UP);
                    player.prog.resetFace = this;
                    return true;
                }
                break;
            default:
                return false;
        }
        return false;
    }
}

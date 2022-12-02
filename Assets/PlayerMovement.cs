using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CubeScript.dirs facing;//which way the player is facing
    public FaceScript standing;//the face the player is on
    public CubeScript.dirs targetDir;//which way the player is going to face next update
    private FaceScript targetFace;//the face the player will be on next update
    public int numberOfSteps = 1; //number of frames it takes to complete movement, 1 is instant framerate is 1 second
    [SerializeField] private float moveHeight;
    private PlayerProgrammer prog;
    // Start is called before the first frame update

    public void MovePlayer()
    {
        if (standing.GetFaceInDir(facing) == null)
        {
            prog.proceed = true;
            return;
        }
        else
        {
            targetFace = standing.GetFaceInDir(facing);
            targetDir = standing.GetMoveDir(facing);
            StartCoroutine(Resolve());
        }
    }

    public void TurnPlayerLeft()//counter clockwise
    {
        targetFace = standing;
        targetDir = getLeftTurn();
        StartCoroutine(Resolve());
    }

    

    public void TurnPlayerRight()//clockwise
    {
        targetFace = standing;
        targetDir = standing.myCube.OppositeDir(getLeftTurn());
        StartCoroutine(Resolve());
    }

    public void ActivateSpace()
    {
        prog.proceed = true;
        Debug.Log("activate space needs to be written");
    }

    IEnumerator Resolve()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetFace.transform.position;
        Quaternion currentRot = transform.rotation;
        Quaternion targetRot = GetTargetRot();
        int tempSteps = numberOfSteps;
        for(int i = 0; i < tempSteps; i++)
        {
            float div = 1f / ((float)tempSteps);
            transform.position = Vector3.Lerp(currentPos,targetPos,div);
            //this is the part where the arc needs defining
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, div);
            yield return null;
        }
        prog.proceed = true;
    }

    private Quaternion GetTargetRot()
    {
        if (facing == targetDir)
        {
            return transform.rotation;
        }
        else
        {
            return Quaternion.LookRotation(dirToVect(targetDir),dirToVect(targetFace.sourcedir));
        }
    }

    public Vector3 dirToVect(CubeScript.dirs dir)
    {
        switch(dir)
        {
            case CubeScript.dirs.UP:
                return Vector3.up;
            case CubeScript.dirs.DOWN:
                return Vector3.down;
            case CubeScript.dirs.RIGHT:
                return Vector3.right;
            case CubeScript.dirs.LEFT:
                return Vector3.left;
            case CubeScript.dirs.FOREWARD:
                return Vector3.forward;
            case CubeScript.dirs.BACK:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }
    private CubeScript.dirs getLeftTurn()
    {
        switch (standing.sourcedir)
        {
            case CubeScript.dirs.UP:
                switch (facing)
                {
                    case CubeScript.dirs.FOREWARD:
                        return CubeScript.dirs.LEFT;
                    case CubeScript.dirs.LEFT:
                        return CubeScript.dirs.BACK;
                    case CubeScript.dirs.BACK:
                        return CubeScript.dirs.RIGHT;
                    case CubeScript.dirs.RIGHT:
                        return CubeScript.dirs.FOREWARD;
                    default:
                        Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                        return CubeScript.dirs.NULL;
                }
            case CubeScript.dirs.DOWN:
                switch (facing)
                {
                    case CubeScript.dirs.FOREWARD:
                        return CubeScript.dirs.RIGHT;
                    case CubeScript.dirs.LEFT:
                        return CubeScript.dirs.FOREWARD;
                    case CubeScript.dirs.BACK:
                        return CubeScript.dirs.LEFT;
                    case CubeScript.dirs.RIGHT:
                        return CubeScript.dirs.BACK;
                    default:
                        Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                        return CubeScript.dirs.NULL;
                }
            case CubeScript.dirs.LEFT:
                switch (facing)
                {
                    case CubeScript.dirs.FOREWARD:
                        return CubeScript.dirs.DOWN;
                    case CubeScript.dirs.UP:
                        return CubeScript.dirs.FOREWARD;
                    case CubeScript.dirs.BACK:
                        return CubeScript.dirs.UP;
                    case CubeScript.dirs.DOWN:
                        return CubeScript.dirs.BACK;
                    default:
                        Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                        return CubeScript.dirs.NULL;
                }
            case CubeScript.dirs.RIGHT:
                switch (facing)
                {
                    case CubeScript.dirs.FOREWARD:
                        return CubeScript.dirs.UP;
                    case CubeScript.dirs.UP:
                        return CubeScript.dirs.BACK;
                    case CubeScript.dirs.BACK:
                        return CubeScript.dirs.DOWN;
                    case CubeScript.dirs.DOWN:
                        return CubeScript.dirs.FOREWARD;
                    default:
                        Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                        return CubeScript.dirs.NULL;
                }
            case CubeScript.dirs.FOREWARD:
                switch (facing)
                {
                    case CubeScript.dirs.UP:
                        return CubeScript.dirs.RIGHT;
                    case CubeScript.dirs.RIGHT:
                        return CubeScript.dirs.DOWN;
                    case CubeScript.dirs.DOWN:
                        return CubeScript.dirs.LEFT;
                    case CubeScript.dirs.LEFT:
                        return CubeScript.dirs.UP;
                    default:
                        Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                        return CubeScript.dirs.NULL;
                }
            case CubeScript.dirs.BACK:
                switch (facing)
                {
                    case CubeScript.dirs.UP:
                        return CubeScript.dirs.LEFT;
                    case CubeScript.dirs.LEFT:
                        return CubeScript.dirs.DOWN;
                    case CubeScript.dirs.DOWN:
                        return CubeScript.dirs.RIGHT;
                    case CubeScript.dirs.RIGHT:
                        return CubeScript.dirs.UP;
                    default:
                        Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                        return CubeScript.dirs.NULL;
                }
            default:
                Debug.Log("Turn Failure, Facing matches face orient or its inverse");
                return CubeScript.dirs.NULL;
        }
    }
}

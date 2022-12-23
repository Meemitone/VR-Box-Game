using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Changables")]
    public bool playtest = false;
    public bool changeColour = false;
    public CubeScript.dirs facing;//which way the player is facing
    public FaceScript standing;//the face the player is on
    
    [Header("\nDont fuck with these my guy")]
    public CubeScript.dirs targetDir;//which way the player is going to face next update
    private FaceScript targetFace;//the face the player will be on next update
    public int numberOfSteps = 1; //number of frames it takes to complete movement, 1 is instant framerate is 1 second
    public float stepTime = 0.5f;//amount of time to make a step
    [SerializeField] private float moveHeight;
    private float RMH;
    public PlayerProgrammer prog;
    public Animator anim;
    public ColourManager Cmanager;

    public GameObject VRCamera;
    public GameObject WASDCamera;
    


    private void Awake()
    {
        prog = FindObjectOfType<PlayerProgrammer>();
        Cmanager = FindObjectOfType<ColourManager>();
        RMH = moveHeight;
        if (playtest) 
        {
            prog.proceed = true; // sets the proceed variable to true to avoid lengthy setups
            WASDCamera.SetActive(true);
            VRCamera.SetActive(false);
        }
    }

    private void Update()
    {
        if(playtest && prog.proceed)
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                prog.proceed = false;
                MovePlayer();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                prog.proceed = false;
                TurnPlayerLeft();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                prog.proceed = false;
                TurnPlayerRight();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                prog.proceed = false;
                ActivateSpace();
            }
        }
        
        if (changeColour)
        {
            Cmanager.doColour = true; // sets the colour blind setting if this bool changes
            changeColour = false;
        }
    }

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
            StartCoroutine(Resolve(true));
        }
    }
    public void MoveUntilUnable()
    {
        StartCoroutine(MoveUntilStopped());
    }

    public void TurnPlayerLeft()//counter clockwise
    {
        targetFace = standing;
        targetDir = getLeftTurn();
        StartCoroutine(Resolve(true));
    }

    public void TurnPlayerRight()//clockwise
    {
        targetFace = standing;
        targetDir = standing.myCube.OppositeDir(getLeftTurn());
        StartCoroutine(Resolve(true));
    }

    public void ActivateSpace()
    {

        switch(standing.faceT)
        {
            case FaceScript.FaceType.SPRING: //move 2 spaces at once
                CubeScript.dirs tempdir = facing;
                FaceScript tempface = standing;
                if (tempface.GetFaceInDir(tempdir) == null)
                {
                    prog.proceed = true;//can't jump over a block, therefore, can't jump
                    return;
                }
                else
                {
                    tempface = tempface.GetFaceInDir(tempdir);
                    tempdir = tempface.GetMoveDir(tempdir); //we have the next face, now we face the nexter face
                    if (tempface.GetFaceInDir(tempdir) == null)
                    {
                        prog.proceed = true;//if the nexter face is a block, can't jump
                        return;
                    }
                    else
                    {
                        targetFace = tempface.GetFaceInDir(tempdir);
                        targetDir = tempface.GetMoveDir(tempdir);
                        moveHeight *= 2; //increase the hop height so that the bound is smoother
                        StartCoroutine(Resolve(true));
                    }
                }
                break;
            case FaceScript.FaceType.LOADLEVEL:
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                levelManager.GoToLevel(standing.loadLevelIndex);
                
                break;
            default:
                prog.proceed = true;
                Debug.Log("ActivateSpace fell out of it's switch (is the space interactable and is the code for it written?)");
                break;
        }
    }

    public void Cease(FaceScript resetFace, CubeScript.dirs resetDir)
    {
        StopAllCoroutines();
        targetFace = resetFace;
        targetDir = resetDir;
        StartCoroutine(Resolve(false));
    }

    IEnumerator MoveUntilStopped()
    {
        while (standing.GetFaceInDir(facing) != null) 
        {
            {
                targetFace = standing.GetFaceInDir(facing);
                targetDir = standing.GetMoveDir(facing);
                yield return Resolve(false);
            }
        }
        prog.proceed = true;
        yield return null;
    }

    IEnumerator Resolve(bool maintain)
    {
        if(targetFace.faceT != FaceScript.FaceType.BLOCK)
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = targetFace.transform.position;
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = GetTargetRot();

            /* if (targetRot == currentRot)
             {
                 targetRot = Quaternion.LookRotation(targetFace.myCube.gameObject.transform.right, Vector3.up);
             }
             /*
             {
                 int tempSteps = numberOfSteps;
                 for (int i = 0; i < tempSteps; i++)
                 {
                     float div = i / ((float)tempSteps);
                     float tempHeight = moveHeight * Mathf.Sin(i / ((float)tempSteps) * Mathf.PI);
                     transform.position = Vector3.Lerp(currentPos, targetPos, div);
                     Vector3 heightadjust = transform.up * tempHeight;
                     transform.localPosition += heightadjust;
                     transform.rotation = Quaternion.Slerp(currentRot, targetRot, div);
                     yield return null;
                 }
             }
             */
            float tempTime = stepTime;
            float currentTime = 0;

            anim.SetBool("Jump", true);
            if (anim.GetBool("Inverse")) { anim.SetBool("Inverse", false); }
            else
            { anim.SetBool("Inverse", true); }

            while (currentTime < tempTime)
            {
                currentTime += Time.deltaTime;
                float div = currentTime / tempTime;
                float tempHeight = moveHeight * Mathf.Sin((div) * Mathf.PI);
                transform.position = Vector3.Lerp(currentPos, targetPos, div);
                transform.localPosition += transform.up * tempHeight;
                transform.rotation = Quaternion.Slerp(currentRot, targetRot, div);

                yield return null;
            }

            anim.SetBool("Jump", false);

            transform.position = targetPos;
            transform.rotation = targetRot;
            standing = targetFace;
            facing = targetDir;
            moveHeight = RMH;
            if (targetFace.Enter(this))
            {
                StartCoroutine(Resolve(false));
                yield break;
            }
        }
        prog.proceed = maintain || playtest;
        yield break;
    }

    private Quaternion GetTargetRot()
    {
        /*if (facing == targetDir)
        {
            return transform.rotation;
        }
        else*/
        {
            return Quaternion.LookRotation(targetFace.myCube.dirToVect(targetDir), targetFace.myCube.dirToVect(targetFace.sourcedir));
            //above gets the rotation based on how the cube is rotated, below uses the global rotatespace. Above might still fail if the cube is rotated a full 90 degrees below is original
            //return Quaternion.LookRotation(dirToVect(targetDir),dirToVect(targetFace.sourcedir));
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

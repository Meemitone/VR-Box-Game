using UnityEngine;

public class CubeScript : MonoBehaviour
{
    //This is a reference script
    // Start is called before the first frame update
    public FaceScript up, down, left, right, foreward, back; //in Global direction terms
    public enum dirs //short for directions not directories
    {
        //null is the default value, if something isn't set up right we'll know
        NULL, UP, DOWN, LEFT, RIGHT, FOREWARD, BACK
    }

    private void Awake()
    {
        //here and in FaceScript.Start() I gotta make the knowledge known from vectors to apply rotations

        up.sourcedir = dirs.UP;
        down.sourcedir = dirs.DOWN;
        left.sourcedir = dirs.LEFT;
        right.sourcedir = dirs.RIGHT;
        foreward.sourcedir = dirs.FOREWARD;
        back.sourcedir = dirs.BACK;

        up.doAwake();
        down.doAwake();
        left.doAwake();
        right.doAwake();
        foreward.doAwake();
        back.doAwake();
    }

    public FaceScript GetIntWrapperFace(dirs source, dirs rection)//get the wraparound face of this cube to this cube
    {
        if(source == rection || source == OppositeDir(rection))
        {
            return null;
        }
        switch (rection)
        {
            case dirs.UP:
                return up;
            case dirs.DOWN:
                return down;
            case dirs.LEFT:
                return left;
            case dirs.RIGHT:
                return right;
            case dirs.BACK:
                return back;
            case dirs.FOREWARD:
                return foreward;
            default:
                return null;
        }
    }

    public FaceScript GetExtWrapFace(dirs source, dirs rection)
    {
        return GetIntWrapperFace(source, OppositeDir(rection));
    }

    public dirs OppositeDir(dirs flip)
    {
        switch(flip)
        {
            case dirs.UP:
                return dirs.DOWN;
            case dirs.DOWN:
                return dirs.UP;
            case dirs.LEFT:
                return dirs.RIGHT;
            case dirs.RIGHT:
                return dirs.LEFT;
            case dirs.BACK:
                return dirs.FOREWARD;
            case dirs.FOREWARD:
                return dirs.BACK;
            default:
                return dirs.NULL;
        }
    }
}

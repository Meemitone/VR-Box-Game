using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgrammer : MonoBehaviour
{
    public bool proceed = false;
    private PlayerMovement player;
    private FaceScript resetFace;
    private CubeScript.dirs resetDir;
    CodeSegment listFirst;
    CodeSegment currentCode;
    CodeSegment listLast;

    public GameObject codeMove, codeLeft, codeRight, codeUse;
    public enum codes
    {
        STEP,
        LEFT, 
        RIGHT, 
        USE, 
        END
    }

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        resetFace = player.standing;
        resetDir = player.facing;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(proceed)
        {

        }
    }

    public void AddLink(codes code)
    {
        GameObject newCode = null;
        switch(code)
        {
            case codes.STEP:
                newCode = Instantiate(codeMove);
                break;
            case codes.LEFT:
                newCode = Instantiate(codeLeft);
                break;
            case codes.RIGHT:
                newCode = Instantiate(codeRight);
                break;
            case codes.USE:
                newCode = Instantiate(codeUse);
                break;
            default:
                break;
        }

        if (newCode == null)
            return;

        {
            //Make the new code link up
            if (listFirst == null)
            {
                listFirst = newCode.GetComponent<CodeSegment>();
                listLast = listFirst;
            }
            else
            {
                listLast.nextCode = newCode.GetComponent<CodeSegment>();
                listLast = listLast.nextCode;
            }
        }

        {
            //Kate add in transform update
        }
    }

    public void RunStop()
    {
        if (listFirst == null)
            return;

        if (!proceed)
            proceed = true;

        proceed = false;

        player.Cease(resetFace, resetDir);
    }
}

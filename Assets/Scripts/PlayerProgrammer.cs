using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgrammer : MonoBehaviour
{
    public bool proceed = false;
    private PlayerMovement player;
    public FaceScript resetFace;
    public CubeScript.dirs resetDir;
    private CodeSegment listFirst;
    private CodeSegment currentCode;
    private CodeSegment listLast;
    public GameObject indexMarker;
    public GameObject UIMenu;
    public bool allowCode = true;
    public int currentIndex = 0; //0 is before the first segment (this is where the type line is)

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
        indexMarker.transform.SetParent(UIMenu.transform);
        UIUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (proceed && !player.playtest)
        {
            proceed = false;
            if (currentCode == null)
            {
                currentCode = listFirst;
            }
            if (currentCode != null)
            {
                switch (currentCode.code)
                {
                    case codes.STEP:
                        player.MovePlayer();
                        break;
                    case codes.LEFT:
                        player.TurnPlayerLeft();
                        break;
                    case codes.RIGHT:
                        player.TurnPlayerRight();
                        break;
                    case codes.USE:
                        player.ActivateSpace();
                        break;
                    default:
                        break;
                }
            }
            else
                return;
            currentCode = currentCode.Next;
        }
    }

    public void AddLink(int codenum)
    {
        if (!allowCode)
            return;

        GameObject newCode = null;
        codes code = (codes)codenum;
        switch (code)
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
                listLast.Next = newCode.GetComponent<CodeSegment>();
                newCode.GetComponent<CodeSegment>().Prev = listLast;
                listLast = listLast.Next;
            }
        }

        newCode.transform.SetParent(UIMenu.transform);

        UIUpdate();
    }

    public void Clear()
    {
        CodeSegment clearcurr = listFirst;
        while (clearcurr != null)
        {
            CodeSegment tempNext = clearcurr.Next;
            Destroy(clearcurr.gameObject);
            clearcurr = tempNext;
        }
        listFirst = null;
        listLast = null;
        UIUpdate();
    }

    public void Remove()
    {
        int index = currentIndex;
        //remove a specific command by index
        if (index <= 0 || index > Count())
        {
            currentIndex = 0;//we shouldn't get here but just in case it gets out of the range somehow
            return;
        }
        CodeSegment holder = listFirst;
        for (int i = 1; i < index; i++)
        {
            holder = holder.Next;
        }

        CodeSegment tempNext = holder.Next;
        CodeSegment tempPrev = holder.Prev;
        tempNext.Prev = tempPrev;
        tempPrev.Next = tempNext;
        if (index == 1)
        {
            listFirst = tempNext;
        }
        if (index == Count())
        {
            listLast = tempPrev;
        }

        Destroy(holder.gameObject);

        UIUpdate();
    }

    public int Count()
    {
        CodeSegment holder = listFirst;
        int count = 0;
        while (holder != null)
        {
            count++;
            holder = holder.Next;
        }
        return count;
    }

    public void IndexUp()
    {
        if (currentIndex < Count())
        {
            currentIndex++;
            UIUpdate();
        }
    }

    public void IndexDown()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UIUpdate();
        }
    }

    public void UIUpdate()
    {
        //rearrange the CodeSegment Objects to form the layout of the UI, along with inserting the text editor flashing | thing at currentIndex
    }

    public void RunStop()
    {
        if (listFirst == null)
            return;

        if (allowCode)
        {
            proceed = true;
            allowCode = false;
            return;
        }

        proceed = false;

        player.Cease(resetFace, resetDir);
        allowCode = true;
    }
}

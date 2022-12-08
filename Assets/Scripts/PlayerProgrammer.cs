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

    public Vector3 indexZero;
    public Vector3 codeZero;
    public Vector3 rightShift;
    public Vector3 downShift;

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
        newCode.transform.rotation = UIMenu.transform.rotation;
        {
            //Make the new code link up
            if (listFirst == null)
            {
                listFirst = newCode.GetComponent<CodeSegment>();
                listLast = listFirst;
            }
            else
            {
                CodeSegment indexed = getByIndex(currentIndex);
                if (indexed.Next == null)
                {
                    listLast = newCode.GetComponent<CodeSegment>();
                    indexed.Next = newCode.GetComponent<CodeSegment>();
                    indexed.Next.Prev = indexed;
                }
                else
                {
                    newCode.GetComponent<CodeSegment>().Next = indexed.Next;
                    newCode.GetComponent<CodeSegment>().Prev = indexed;
                    indexed.Next.Prev = newCode.GetComponent<CodeSegment>();
                    indexed.Next = newCode.GetComponent<CodeSegment>();
                }
                /*
                listLast.Next = newCode.GetComponent<CodeSegment>();
                newCode.GetComponent<CodeSegment>().Prev = listLast;
                listLast = listLast.Next;
                */
            }
        }

        newCode.transform.SetParent(UIMenu.transform);
        currentIndex++;
        UIUpdate();
    }

    public void Clear()
    {
        if (!allowCode)
            return;
        CodeSegment clearcurr = listFirst;
        while (clearcurr != null)
        {
            CodeSegment tempNext = clearcurr.Next;
            Destroy(clearcurr.gameObject);
            clearcurr = tempNext;
        }
        listFirst = null;
        listLast = null;
        currentIndex = 0;
        UIUpdate();
    }

    public void Remove()
    {
        if (!allowCode)
            return;
        int index = currentIndex;
        //remove a specific command by index
        if (index <= 0 || index > Count())
        {
            currentIndex = 0;//we shouldn't get here but just in case it gets out of the range somehow
            return;
        }
        CodeSegment holder = listFirst;
        //Debug.Log("index: " + index);
        for (int i = 1; i < index; i++)
        {
            holder = holder.Next;
            //Debug.Log("i: " + i);
        }

        CodeSegment tempNext = holder.Next;
        CodeSegment tempPrev = holder.Prev;
        if(tempNext!=null)
        tempNext.Prev = tempPrev;
        if (tempPrev != null)
            tempPrev.Next = tempNext;
        if (index == 1)
        {
            listFirst = tempNext;
        }
        if (index == Count())
        {
            listLast = tempPrev;
        }
        //Debug.Log("About to Delete");
        Destroy(holder.gameObject);
        currentIndex--;
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
        indexMarker.transform.localPosition = indexZero;
        int right = 0;
        int down = 0;
        if(currentIndex>0)
        {
            right = currentIndex % 8;
            if (right == 0)
                right = 8;
            down = currentIndex / 8;
            if (right == 8)
                down--;
        }
        indexMarker.transform.localPosition += right * rightShift + down * downShift;

        right = 0;
        down = 0;
        CodeSegment T = listFirst;
        while(T!=null)
        {
            T.gameObject.transform.localPosition = codeZero + right * rightShift + down * downShift;
            right++;
            if(right % 8 == 0)
            {
                right -= 8;
                down++;
            }
            T = T.Next;
        }
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
    
    private CodeSegment getByIndex(int index)
    {
        if(index == 0)
        return null;
        CodeSegment holder = listFirst;
        //Debug.Log("index: " + index);
        for (int i = 1; i < index; i++)
        {
            if(holder.Next!=null)
            holder = holder.Next;
            //Debug.Log("i: " + i);
        }
        return holder;
    }
}

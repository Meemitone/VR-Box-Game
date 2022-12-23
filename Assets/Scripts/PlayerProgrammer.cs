using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgrammer : MonoBehaviour
{
    [Tooltip("Hit this for playtest wasd controls, also hit playtest in playerMovement")]
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

    public GameObject codeMove, codeLeft, codeRight, codeUse, codeSprint;
    public enum codes
    {
        STEP,
        LEFT,
        RIGHT,
        USE,
        END,
        SPRINT //go until unable
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
        if (proceed && !player.playtest && player.gameObject.activeSelf)
        {
            proceed = false;
            if (currentCode == null)
            {
                listLast.highlight.SetActive(false);
                currentCode = listFirst;
            }
            if (currentCode != null)
            {
                switch (currentCode.code)
                {
                    case codes.STEP://0
                        player.MovePlayer();
                        break;
                    case codes.LEFT://1
                        player.TurnPlayerLeft();
                        break;
                    case codes.RIGHT://2
                        player.TurnPlayerRight();
                        break;
                    case codes.USE://3
                        player.ActivateSpace();
                        break;
                    case codes.SPRINT://5
                        player.MoveUntilUnable();
                        break;
                    default:
                        break;
                }
            }
            else
                return;
            if(currentCode.Prev!=null)
            currentCode.Prev.highlight.SetActive(false);
            currentCode.highlight.SetActive(true);
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
            case codes.SPRINT:
                newCode = Instantiate(codeSprint);
                break;
            default:
                break;
        }

        if (newCode == null)
            return;
        newCode.transform.rotation = UIMenu.transform.rotation;//the rotation of the code matches the menu
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
                if(indexed == null)
                {//index was 0 (insert at start of code)
                    if(listFirst!=null)//if there's code already
                    {
                        listFirst.Prev = newCode.GetComponent<CodeSegment>();//the fist piece of code needs to point backwards to this new one
                    }
                    newCode.GetComponent<CodeSegment>().Next = listFirst;//new code needs to say the next code is the one that's currently first
                    listFirst = newCode.GetComponent<CodeSegment>();//new code is now the first in the list
                }
                else if (indexed.Next == null)//indexed wasn't null, so we are in the list somewhere, if next is null, this is the end of the list
                {
                    listLast = newCode.GetComponent<CodeSegment>();//new code goes on the end
                    indexed.Next = newCode.GetComponent<CodeSegment>();//previous end points to this one as it's next
                    indexed.Next.Prev = indexed;//the backwards pointer of the one added as the next in line after the one we found, is the one we found (or "newCode.GetComponent<CodeSegment>().Prev = indexed;")
                }
                else//not the first in the list and not the last either
                {
                    newCode.GetComponent<CodeSegment>().Next = indexed.Next;//newcode is being inserted between 2 existing codes, it's next is the second one (the one after the indexed one)
                    newCode.GetComponent<CodeSegment>().Prev = indexed;//it's previous is the first of the 2, the indexed one
                    indexed.Next.Prev = newCode.GetComponent<CodeSegment>();//the second one needs to point back to the new code
                    indexed.Next = newCode.GetComponent<CodeSegment>();//the first needs to point forward to the new code
                }
                /*
                listLast.Next = newCode.GetComponent<CodeSegment>();
                newCode.GetComponent<CodeSegment>().Prev = listLast;
                listLast = listLast.Next;
                */
            }
        }

        newCode.transform.SetParent(UIMenu.transform);//add the new code to the menu for UIUpdate to handle
        currentIndex++;//the location of the index is now after the newly minted code
        UIUpdate();//rearrange the segments
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
        if (listFirst == null)//if there's no code, do nothing
            return;

        if (allowCode)
        {
            proceed = true;
            allowCode = false;
            currentCode = listFirst;
            currentCode.highlight.SetActive(true);
            return;
        }

        proceed = false;
        if (currentCode != null)
        {
            currentCode.highlight.SetActive(false);
            if (currentCode.Prev != null)
                currentCode.Prev.highlight.SetActive(false);
        }
        if (listLast != null)
            listLast.highlight.SetActive(false);
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

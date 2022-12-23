using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeSegment : MonoBehaviour
{
    public CodeSegment Next;
    public CodeSegment Prev;
    public PlayerProgrammer.codes code;
    public GameObject highlight;
}

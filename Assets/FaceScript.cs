using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScript : MonoBehaviour
{
    [HideInInspector] public Vector3 inUp, inFor, inLef, inRig, inBac;
    [HideInInspector] public FaceScript faceNorth = null, faceWest = null, faceEast = null, faceSouth = null; //front is shorter than foreward
    private int CubeLayer = 6;//this is the layer that cubes are on, the faces occupy layer 7
    [SerializeField] private FaceCheckerScript north, south, east, west; //these are the face checkers

    void Awake()
    {
        //check if this face is inside a cube, if so, then it can't be traversed 
        Collider[] checklist = Physics.OverlapSphere(transform.position, 0.01f, CubeLayer, QueryTriggerInteraction.Collide);
        if (checklist.Length > 1)
            Debug.Log("Face hit multiple cubes", transform.parent.gameObject);
        if (checklist.Length > 0)
            gameObject.SetActive(false);
        


        inUp = transform.up;
        inFor = transform.forward;
        inBac = -transform.forward;
        inRig = transform.right;
        inLef = -transform.right;
    }

    private void Start()
    {
        //Awake is called when the scene loads, start is called before the first frame if the object is enabled
    }

    

    
}

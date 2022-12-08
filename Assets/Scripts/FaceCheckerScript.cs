using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCheckerScript : MonoBehaviour
{
    public CubeScript.dirs direction;//direction of face to here relative to cube
    public Collider[] cubeOverlaps, faceOverlaps;
    public CubeScript cube = null; 
    public FaceScript face = null;
    public LayerMask cubeMask;
    public LayerMask faceMask;
    public void doAwake()
    {
        cubeMask = LayerMask.GetMask("Cubes");
        faceMask = LayerMask.GetMask("Faces");
        cubeOverlaps = Physics.OverlapSphere(transform.position, 0.001f, cubeMask, QueryTriggerInteraction.Collide);
        faceOverlaps = Physics.OverlapSphere(transform.position, 0.001f, faceMask, QueryTriggerInteraction.Collide);
        if (cubeOverlaps.Length > 0)
        {
            if (cubeOverlaps.Length > 1)
            {
                Debug.Log("Multiple cubes detected by one facecheck", gameObject);
            }
            cube = cubeOverlaps[0].GetComponent<CubeScript>();
        }
        if (faceOverlaps.Length > 0)
        {
            if (faceOverlaps.Length > 1)
            {
                Debug.Log("Multiple faces detected by one facecheck", gameObject);
            }
            face = faceOverlaps[0].GetComponent<FaceScript>();
        }
    }
}

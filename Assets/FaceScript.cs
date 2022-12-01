using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScript : MonoBehaviour
{
    [HideInInspector] public Vector3 inUp, inFor, inLef, inRig, inBac;
    [HideInInspector] public GameObject faceFor, faceLef, faceRight, faceDown;
    // Start is called before the first frame update
    void Awake()
    {
        inUp = transform.up;
        inFor = transform.forward;
        inBac = -transform.forward;
        inRig = transform.right;
        inLef = -transform.right;
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Cube"))
        {
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCheckerScript : MonoBehaviour
{
    private FaceScript par;//FaceScript of this' parent
    private FaceScript target;//the face we find
    private Vector3 direction;//direction of face to here

    private void Awake()
    {
        par = transform.parent.gameObject.GetComponent<FaceScript>();
        direction = transform.position - par.gameObject.transform.position; 
        //calculate which face I'm touching in the start because reasons
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

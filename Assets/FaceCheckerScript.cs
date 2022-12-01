using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCheckerScript : MonoBehaviour
{
    private FaceScript par;//FaceScript of this' parent
    private Vector3 direction;//direction of face to here

    private void Awake()
    {
        par = transform.parent.gameObject.GetComponent<FaceScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

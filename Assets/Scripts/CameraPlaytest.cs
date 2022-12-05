using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CameraPlaytest : MonoBehaviour
{
    public bool playTest = true;
    public float camSpeed = 1;
    public GameObject Cam;
    

    // Update is called once per frame
    void Update()
    {
        if(playTest){
            Cursor.lockState = CursorLockMode.Locked;
            gameObject.transform.Rotate(0,Input.GetAxis("Mouse X") * camSpeed,0);
            Cam.transform.Rotate(Input.GetAxis("Mouse Y") * -camSpeed,0,0);
        }
    }
}

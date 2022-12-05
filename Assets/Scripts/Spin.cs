using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void 
        Update()
    {
        if(Input.GetAxis("Vertical") > 0.1)
        {
            gameObject.transform.Rotate(0.5f,0.3f,0);
        }
        if(Input.GetAxis("Vertical") < -0.1)
        {
            gameObject.transform.Rotate(-0.4f,0.5f,-0.7f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexReaders : MonoBehaviour
{
    public int indexNum = 0;
    public PlayerProgrammer proggers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Detected");
        if(other.gameObject.CompareTag("Finger"))
        {
            proggers.SetIndex(indexNum);
        }
    }
}

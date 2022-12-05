using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysButton : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f; //percentage press to press
    [SerializeField] private float deadZone = 0.025f;

    private bool pressed;
    private Vector3 startPos;
    private ConfigurableJoint joiner;

    public UnityEvent onPressed, onReleased;


    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        joiner = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!pressed && GetVal() + threshold>=1)
        {
            Pressed();
        }
        if(pressed && GetVal() - threshold <=0)
        {
            Release();
        }
    }

    private float GetVal()
    {
        float val = Vector3.Distance(startPos, transform.localPosition) / joiner.linearLimit.limit;

        if(Mathf.Abs(val)< deadZone)
        {
            val = 0;
        }
        return Mathf.Clamp(val, -1, 1);
    }

    private void Pressed()
    {
        pressed = true;
        onPressed.Invoke();
    }

    private void Release()
    {
        pressed = false;
        onReleased.Invoke();
    }
}
